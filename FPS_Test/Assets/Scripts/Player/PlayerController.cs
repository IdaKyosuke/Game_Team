using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private const float MouseSensitivity = 250.0f;

	[SerializeField] Camera m_mapCamera;
    [SerializeField] Animator m_animator;
    [SerializeField] float m_jumpPower;
    [SerializeField] float m_gravity;
    [SerializeField] Info_InventorySize m_inventortSize;
    [SerializeField] PlayerAnime m_playerAnim;			// アニメーション管理用オブジェクト
    [SerializeField] GameObject m_spine;

    private float m_rotateX;
    private bool m_isDeath;
    private Vector3 m_moveDirection;

    private GameObject m_rayTarget;						// レイの当たった敵を保管する用
	private CharacterController m_characterController;	// CharacterController型の変数
    private StashManager m_stashManager;
    private PlayerStatus m_playerStatus;
    private Condition m_condition;

    public bool IsDeath => m_isDeath;

    public Info_InventorySize InventortSize => m_inventortSize;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_stashManager = GetComponent<StashManager>();
        m_playerStatus = GetComponent<PlayerStatus>();
        m_condition = GetComponent<Condition>();
        m_isDeath = false;
        m_characterController.enabled = false;
    }

    void Start()
	{
		// マスターの持つリストを参照
		photonView.RPC(nameof(RequestPlayerSpawnPos), RpcTarget.MasterClient, photonView.ViewID);
    }

	// マスターの中で個々にポジションを送る
	[PunRPC]
	void RequestPlayerSpawnPos(int viewId)
	{
		PhotonView.Find(viewId).RPC(nameof(SetPlayerPos), PhotonView.Find(viewId).Owner, Create_Maze.GetPlayerSpawnPos().position);
	}

	[PunRPC]
	void SetPlayerPos(Vector3 pos)
	{
		pos = new Vector3(0, 1, 0);
		transform.position = pos;
        m_characterController.enabled = true;
		
		//Debug.Log(PhotonNetwork.IsMasterClient + ":" + photonView.ViewID);
	}

	private bool CheckGrounded()
	{
		/*
		// 放つ光線の初期位置と姿勢
		// 若干身体にめり込ませた位置から発射しないと正しく判定できない時がある
		var ray = new Ray(origin: transform.position + Vector3.up * rayOffset, direction: Vector3.down);

		// Raycastがhitするかどうかで判定
		return Physics.Raycast(ray, 2);
		*/
		return m_characterController.isGrounded;
	}

	private void MiniMap()
	{
		int layer = transform.position.y < 10 ? transform.position.y < 5 ? 1 << 6 : 1 << 7 : 1 << 8;
		m_mapCamera.cullingMask = layer | (1 << 10);
	}

	void Update()
	{
		if (m_isDeath) return;
        if (photonView.IsMine)
        {
            MiniMap();

            //ジャンプ
            if (CheckGrounded() && Input.GetButton("Jump"))
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

            //前方にRayを飛ばす
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hit))
            {
                //プレイヤー以外は無視
                if (!hit.transform.gameObject.CompareTag("Player")) return;

                //自身は無視
                if (hit.transform.root.gameObject == gameObject) return;

                //死体以外は無視
                if (!hit.transform.root.gameObject.GetComponent<PlayerController>().IsDeath) return;

                // レイの当たった敵を保管
                m_rayTarget = hit.transform.gameObject;

                // 移動スピードをアニメーターに反映する
                //animator.SetFloat("MoveSpeed", new Vector3(moveVelocity.x, 0, moveVelocity.z).magnitude);
            }
        }
	}

    private void FixedUpdate()
    {
        bool isMove = false;

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
        m_characterController.Move(m_moveDirection * Time.deltaTime);

        //移動アニメーション
        m_animator.SetBool("Move", isMove);
    }

    void LateUpdate()
	{
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

    public override void OnLeftRoom()
	{
		Debug.Log("LeftRoom");
		photonView.RPC(nameof(RequestOnDeath), photonView.Owner);
		base.OnLeftRoom();
	}

	[PunRPC]
	void RequestOnDeath()
	{
		m_isDeath = true;
	}
}