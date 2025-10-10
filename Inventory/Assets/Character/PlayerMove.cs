using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] Animator m_animator;
    [SerializeField] float m_jumpPower;
    [SerializeField] float m_gravity;
    [SerializeField] UnityEvent m_onUniqueSkill;
    [SerializeField] StashManager m_stashManager;
    [SerializeField] Info_InventorySize m_inventortSize;

    private Vector3 m_moveDirection;
    private CharacterController m_controller;

    private PlayerStatus m_playerStatus;
    private Condition m_condition;

    public Info_InventorySize InventortSize => m_inventortSize;

	// レイの当たった敵を保管する用
	private GameObject m_rayTarget;

    void Start()
    {
        m_stashManager = GetComponent<StashManager>();
        m_controller = GetComponent<CharacterController>();
        m_playerStatus = GetComponent<PlayerStatus>();
        m_condition = GetComponent<Condition>();
        m_moveDirection = Vector3.zero;
    }

    private void Update()
    {
        //前方にRayを飛ばす
        if (Physics.Raycast(transform.position, transform.forward, out var hit))
        {
            //プレイヤー以外は無視
            if (!hit.transform.gameObject.CompareTag("Player")) return;
			// レイの当たった敵を保管
			m_rayTarget = hit.transform.gameObject;

			//Debug.Log("Hit!!!!!!!!!!!!");
			//Eキーが押されていなければ無視
			if (Input.GetKeyDown("e"))
			{
				m_stashManager.IsScavenger(true);
				//インベントリUIの表示
				m_stashManager.CreateStashUi(
					m_rayTarget.GetComponent<Inventory_Info>().GetInfo(),
					m_rayTarget.GetComponent<StashManager>().GetItemList()
					);
			}
        }

        //攻撃
        if (Input.GetMouseButtonDown(0))
        {
            m_animator.SetBool("Attack", true);
        }

		if (Input.GetKeyDown("tab"))
		{
			m_stashManager.ManageUiActiveInfo();
		}

		if (Input.GetKeyDown("1"))
		{
			m_stashManager.AddItemInventory();
		}
		else if (Input.GetKeyDown("2"))
		{
			m_stashManager.AddItemStash();
		}
	}

    void FixedUpdate()
    {
        //移動したかどうか
        bool isMove = false;

        //自由落下
        m_moveDirection.y -= m_gravity * Time.deltaTime;

        //感電状態は移動不可
        if (m_condition.Current == ConditionType.Shock) return;

        //移動量の取得
        m_moveDirection = new Vector3(Input.GetAxis("Horizontal"), m_moveDirection.y, Input.GetAxis("Vertical"));
        if (m_controller.isGrounded)
        {
            if (Input.GetButton("Jump")) m_moveDirection.y = m_jumpPower;
        }

        //カメラの向きを考慮した移動量
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveVelocity = cameraForward * m_moveDirection.z + Camera.main.transform.right * m_moveDirection.x;
        moveVelocity = new Vector3(moveVelocity.x * m_playerStatus.Value.moveSpeed, m_moveDirection.y, moveVelocity.z * m_playerStatus.Value.moveSpeed);

        //移動
        m_controller.Move(moveVelocity * Time.deltaTime);

        //移動していれば回転させる
        Vector3 move = new Vector3(m_moveDirection.x, 0, m_moveDirection.z);
        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(move.normalized),
                0.2f
            );

            isMove = true;
        }

        //移動アニメーション
        m_animator.SetBool("Move", isMove);
    }

    public void OnDamage()
    {
        Debug.Log("Damage!!!!!!!");
    }

    public void OnDeath()
    {
        Debug.Log("Death!!!!!!!");
    }

	// 変更後のアイテムリストを返す
	public void ReturnItemList(List<ItemList> items)
	{
		if (!m_rayTarget) return;

		List<ItemList> list = new List<ItemList>(items);
		m_rayTarget.GetComponent<StashManager>().CopyItemList(list);
		// ターゲットを空にする
		m_rayTarget = null;
	}
}