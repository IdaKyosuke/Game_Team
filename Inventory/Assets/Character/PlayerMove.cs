using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMove : MonoBehaviour
{
    private const float MouseSensitivity = 230.0f;

    [SerializeField] Animator m_animator;
    [SerializeField] float m_jumpPower;
    [SerializeField] float m_gravity;
    [SerializeField] UnityEvent m_onUniqueSkill;
    [SerializeField] StashManager m_stashManager;
    [SerializeField] Info_InventorySize m_inventortSize;
	[SerializeField] PlayerAnime m_playerAnim;      // アニメーション管理用オブジェクト
	[SerializeField] GameObject m_spine;

    private float xRotation;
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

        Cursor.lockState = CursorLockMode.Locked;
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
            // 攻撃中は無視
            if (m_playerAnim.IsAttack()) return;

            //ジャンプ中は無視
            if(!m_controller.isGrounded) return;

            m_animator.SetTrigger("Attack1");
        }

		if (Input.GetKeyDown("tab"))
		{
			m_stashManager.ManageUiActiveInfo();
		}

		// デバッグ用
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
        if (m_controller.isGrounded && Input.GetButton("Jump")) m_moveDirection.y = m_jumpPower;

        //カメラの向きを考慮した移動量
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveVelocity = cameraForward * m_moveDirection.z + Camera.main.transform.right * m_moveDirection.x;
        moveVelocity = new Vector3(moveVelocity.x * m_playerStatus.Value.moveSpeed, m_moveDirection.y, moveVelocity.z * m_playerStatus.Value.moveSpeed);

		// 攻撃中は移動できない
		if(!m_playerAnim.IsAttack())
		{
			//移動
			m_controller.Move(moveVelocity * Time.deltaTime);
            isMove = true;
		}

        //移動アニメーション
        m_animator.SetBool("Move", isMove);
	}

    private void LateUpdate()
    {
        //攻撃中は回転しない
        if (m_playerAnim.IsAttack()) return;

        // 視点移動
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        // 横回転
        transform.Rotate(Vector3.up * mouseX);

        // 腰の回転
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -40.0f, 30.0f);
        m_spine.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void OnDamage()
    {
        Debug.Log("Damage!!!!!!!");
    }

    public void OnDeath()
    {
        Debug.Log("Death!!!!!!!");
        m_animator.SetTrigger("Death");
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