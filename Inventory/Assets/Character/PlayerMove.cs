using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private const float MouseSensitivity = 250.0f;

    [SerializeField] Animator m_animator;
    [SerializeField] float m_jumpPower;
    [SerializeField] float m_gravity;
    [SerializeField] Info_InventorySize m_inventortSize;
    [SerializeField] PlayerAnime m_playerAnim;      // アニメーション管理用オブジェクト
    [SerializeField] GameObject m_spine;
    [SerializeField] bool m_isLobby;

    private float m_rotateX;
    private bool m_isDeath;
    private Vector3 m_moveDirection;
    private CharacterController m_controller;

    private StashManager m_stashManager;
    private PlayerStatus m_playerStatus;
    private Condition m_condition;

    public bool IsDeath => m_isDeath;

    public Info_InventorySize InventortSize => m_inventortSize;

    // レイの当たった敵を保管する用
    private GameObject m_rayTarget;

    void Start()
    {
        m_stashManager = GetComponent<StashManager>();
        m_controller = GetComponent<CharacterController>();
        m_playerStatus = GetComponent<PlayerStatus>();
        m_condition = GetComponent<Condition>();
        m_isDeath = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //ジャンプ
        if (m_controller.isGrounded && Input.GetButton("Jump"))
        {
            m_moveDirection.y = m_jumpPower;
        }

        //攻撃
        if (Input.GetMouseButtonDown(0))
        {
            // 攻撃中は無視
            if (m_playerAnim.IsAttack()) return;

            //攻撃アニメーション
            m_animator.SetTrigger("Attack1");
        }

        //インベントリの操作
        if (Input.GetKeyDown("tab"))
		{
			m_stashManager.ManageUiActiveInfo();
		}

        //前方にRayを飛ばす
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hit))
        {
            //プレイヤー以外は無視
            if (!hit.transform.gameObject.CompareTag("Player")) return;

            //自身は無視
            if (hit.transform.root.gameObject == gameObject) return;

            //死体以外は無視
            if (!hit.transform.root.gameObject.GetComponent<PlayerMove>().IsDeath) return;

            // レイの当たった敵を保管
            m_rayTarget = hit.transform.gameObject;

            //Eキーが押されていなければ無視
            if (Input.GetKeyDown("e"))
            {
                Debug.Log("IsScavenger");

                m_stashManager.IsScavenger(true);

                //インベントリUIの表示
                m_stashManager.CreateStashUi(
                    m_rayTarget.transform.root.gameObject.GetComponent<Inventory_Info>().GetInfo(),
                    m_rayTarget.transform.root.gameObject.GetComponent<StashManager>().GetItemList()
                    );
            }
        }
    }

    void FixedUpdate()
    {
        bool isMove = false;

        //ロビー画面なら移動しない
        if(m_isLobby) return;

        //感電状態なら移動不可
        if (m_condition.Current != ConditionType.Shock)
        {
            //移動の入力
            Vector3 inputDiraction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            if (inputDiraction != Vector3.zero) isMove = true;

            //カメラの向きに合わせて移動方向を決定
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 moveDirection = (cameraForward * inputDiraction.z + Camera.main.transform.right * inputDiraction.x).normalized;

            //攻撃中は移動不可
            if (m_playerAnim.IsAttack())
            {
                m_moveDirection.x = 0;
                m_moveDirection.z = 0;
            }
            else
            {
                m_moveDirection.x = moveDirection.x * m_playerStatus.TotalStatus.moveSpeed;
                m_moveDirection.z = moveDirection.z * m_playerStatus.TotalStatus.moveSpeed;
            }
        }

        //自由落下
        m_moveDirection.y -= m_gravity * Time.deltaTime;

        //移動
        m_controller.Move(m_moveDirection * Time.deltaTime);

        //移動アニメーション
        m_animator.SetBool("Move", isMove);
    }

    private void LateUpdate()
    {
        //ロビー画面なら移動しない
        if (m_isLobby) return;

        // 視点移動
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        // 横回転
        transform.Rotate(Vector3.up * mouseX);

        // 腰の回転
        m_rotateX -= mouseY;
        m_rotateX = Mathf.Clamp(m_rotateX, -40.0f, 30.0f);
        m_spine.transform.localRotation = Quaternion.Euler(m_rotateX, 0, 0);
        Camera.main.transform.localRotation = Quaternion.Euler(m_rotateX, 0f, 0f);
    }

    public void OnDamage()
    {
        Debug.Log("Damage!!!!!!!");
    }

    public void OnDeath()
    {
        m_animator.SetTrigger("Death");
        m_isDeath = true;
    }

	// 変更後のアイテムリストを返す
	public void ReturnItemList(List<ItemList> items)
	{
		if (!m_rayTarget) return;

		List<ItemList> list = new List<ItemList>(items);
		m_rayTarget.transform.root.gameObject.GetComponent<StashManager>().CopyItemList(list);

		// ターゲットを空にする
		m_rayTarget = null;
	}
}