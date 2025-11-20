using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private const float MouseSensitivity = 140.0f;

    [SerializeField] Camera m_mainCamera;
	[SerializeField] Camera m_mapCamera;
    [SerializeField] Animator[] m_animator;
    [SerializeField] float m_jumpPower;
    [SerializeField] float m_gravity;
    [SerializeField] Info_InventorySize m_inventortSize;
    [SerializeField] PlayerAnime m_playerAnim;			// アニメーション管理用オブジェクト
    [SerializeField] GameObject m_spine;
	[SerializeField] Weapon_Collider m_weapon;

	private CharacterController m_characterController;  // CharacterController型の変数
    private PlayerStatus m_status;
    private StashController m_stashController;
    private Condition m_condition;
    private Vector3 m_moveDirection;
    private float m_rotateX;
    private bool m_isDeath;

    public bool IsDeath => m_isDeath;

    public Info_InventorySize InventortSize => m_inventortSize;

    public PlayerStatus Status => m_status;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_status = GetComponent<PlayerStatus>();
        m_stashController = GetComponent<StashController>();
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
        if (!photonView.IsMine) return;
		if (m_isDeath) return;

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
            //m_animator[0].SetTrigger("Attack1");
            //m_animator[1].SetTrigger("Attack1");
            m_animator[0].SetBool("attack", true);
            m_animator[1].SetBool("attack", true);
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
		if (m_isDeath) return;

        bool isMove = false;

        //移動の入力
        Vector3 inputDiraction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (inputDiraction != Vector3.zero) isMove = true;

        //カメラの向きに合わせて移動方向を決定
        Vector3 cameraForward = Vector3.Scale(m_mainCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveDirection = (cameraForward * inputDiraction.z + m_mainCamera.transform.right * inputDiraction.x).normalized;

        //移動不可
        if (m_condition.Current == ConditionType.Shock || m_playerAnim.IsAttack() || m_stashController.IsOpen)
        {
            m_moveDirection.x = 0;
            m_moveDirection.z = 0;
        }
        else
        {
            m_moveDirection.x = moveDirection.x * m_status.Total.moveSpeed;
            m_moveDirection.z = moveDirection.z * m_status.Total.moveSpeed;
        }

        //自由落下
        m_moveDirection.y -= m_gravity * Time.deltaTime;

        //移動
        m_characterController.Move(m_moveDirection * Time.deltaTime);

        //移動アニメーション
        m_animator[0].SetBool("Move", isMove);
        m_animator[1].SetBool("Move", isMove);
    }

    void LateUpdate()
	{
        //自身以外は移動不可
        if (!photonView.IsMine) return;

		// 死んでたら移動不可
		if (m_isDeath) return;

        //インベントリを開いているなら移動不可
        if (m_stashController.IsOpen) return;

        //攻撃中は視点移動不可
        if (m_playerAnim.IsAttack()) return;    

        // 視点移動
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        // 横回転
        transform.Rotate(Vector3.up * mouseX);

        // 腰の回転
        m_rotateX -= mouseY;
        m_rotateX = Mathf.Clamp(m_rotateX, -40.0f, 30.0f);
        m_spine.transform.localRotation = Quaternion.Euler(m_rotateX, 0, 0);
        m_mainCamera.transform.localRotation = Quaternion.Euler(m_rotateX, 0f, 0f);
    }

    public void OnDeath()
    {
        m_animator[0].SetTrigger("Death");
        m_animator[1].SetTrigger("Death");
    }

    public override void OnLeftRoom()
	{
		Debug.Log("LeftRoom");
		photonView.RPC(nameof(RequestOnDeathPlayer), photonView.Owner);
		base.OnLeftRoom();
	}

	[PunRPC]
	void RequestOnDeathPlayer()
	{
		Debug.Log("死んだ : " + photonView);
		m_isDeath = true;
		OnDeath();
	}

	[PunRPC]
	void AttackAnime()
	{
		if (!photonView.IsMine) Debug.Log("攻撃アニメーション");
		transform.GetChild(0).GetComponent<PlayerAnime>().Attack();
		transform.GetChild(1).GetComponent<PlayerAnime>().Attack();
	}

	[PunRPC]
	void AttackAnimeEnd()
	{
		transform.GetChild(0).GetComponent<PlayerAnime>().AttackEnd();
		transform.GetChild(1).GetComponent<PlayerAnime>().AttackEnd();
	}

	// (DamageBodyのOnTriggerEnter)
	[PunRPC]
	void RequestDamageValue(int viewId)
	{
		PhotonView view = PhotonView.Find(viewId);
		Condition condition = GetComponent<Condition>();
		int power = GetComponent<PlayerStatus>().Total.physicalPower;

        Debug.Log(view);

		view.RPC(nameof(Damage), view.Owner,
			power,
			(int)m_weapon.AttackType,
			(int)condition.Grant,
			condition.Rate);
	}

	// (PlayerControllerのRequestDamageValue)
	[PunRPC]
	void Damage(int power, int attackTypeNum, int ConditionTypeNum, int grantRate)
	{
        m_status.Damage(power, (AttackType)attackTypeNum, ConditionTypeNum, grantRate);
	}
}