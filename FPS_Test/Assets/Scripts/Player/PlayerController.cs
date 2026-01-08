using Cysharp.Threading.Tasks;
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
	[SerializeField] PlayerAnime m_playerAnim;          // アニメーション管理用オブジェクト
	[SerializeField] GameObject m_spine;
	[SerializeField] Weapon_Collider m_weapon;

	private int m_playerId;
	private CharacterController m_characterController;  // CharacterController型の変数
	private PlayerStatus m_status;
	private StashController m_stashController;
	private Condition m_condition;
	private Job m_job;
	private Vector3 m_moveDirection;
	private float m_rotateX;
	private bool m_isDeath;
	private bool m_setPos;

	private GameManager m_gameManager = null;

	public bool IsDeath => m_isDeath;

	public Info_InventorySize InventortSize => m_inventortSize;

	public PlayerStatus Status => m_status;

    private void Awake()
    {
		m_setPos = false;
        m_characterController = GetComponent<CharacterController>();
        m_status = GetComponent<PlayerStatus>();
        m_stashController = GetComponent<StashController>();
        m_condition = GetComponent<Condition>();
        m_job = GetComponent<Job>();
        m_isDeath = false;
        m_characterController.enabled = false;

		m_gameManager = GameManager.Instance;
	}

    async void Start()
    {
        // マップ生成が終わるまで待つ
        await UniTask.WaitUntil(() => Create_Maze.IsMapReady);

        // マスターの持つリストを参照
        photonView.RPC(nameof(RequestPlayerSpawnPos), RpcTarget.MasterClient, photonView.ViewID);
    }

    // マスターの中で個々にポジションを送る
    [PunRPC]
	void RequestPlayerSpawnPos(int viewId)
	{
		Debug.Log("プレイヤーがポスを受け取る" + Create_Maze.GetPlayerSpawnPos());
		PhotonView.Find(viewId).RPC(nameof(SetPlayerPos), PhotonView.Find(viewId).Owner, Create_Maze.GetPlayerSpawnPos().position);
	}

	[PunRPC]
	void SetPlayerPos(Vector3 pos)
	{
		pos = new Vector3(0, 1, 0);
		transform.position = pos;
        m_characterController.enabled = true;
		m_setPos = true;
		
		//Debug.Log(PhotonNetwork.IsMasterClient + ":" + photonView.ViewID);
	}

	private void MiniMap()
	{
		int layer = transform.position.y < 10 ? transform.position.y < 5 ? 1 << 6 : 1 << 7 : 1 << 8;
		m_mapCamera.cullingMask = layer | (1 << 10);
	}

	void Update()
	{
		if (!m_setPos) return;
        if (!photonView.IsMine) return;
		if (m_isDeath) return;

        MiniMap();

        //ジャンプ
        if (m_characterController.isGrounded && Input.GetButton("Jump"))
        {
            m_moveDirection.y = m_jumpPower;
        }

        //攻撃
        if (Input.GetMouseButtonDown(0))
        {
            // 攻撃中は無視
            if (m_playerAnim.IsAttack) return;

			//職業別の攻撃処理
            m_job.Attack();

            //攻撃アニメーション
            m_animator[0].SetBool("attack", true);
            m_animator[1].SetBool("attack", true);
        }

		// デバッグ用
		if(Input.GetKeyDown("5"))
		{
			m_stashController.Save();
			m_gameManager.ReturnLobby();
		}
    }

    private void FixedUpdate()
	{
		if (!m_setPos) return;
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
        if (m_condition.Current == ConditionType.Shock || m_stashController.IsOpen)
        {
            m_moveDirection.x = 0;
            m_moveDirection.z = 0;
        }
        else
        {
            //移動速度の反映(20で割った値を使う)
            m_moveDirection.x = moveDirection.x * m_status.Total.moveSpeed / 20;
            m_moveDirection.z = moveDirection.z * m_status.Total.moveSpeed / 20;
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
		if (!m_setPos) return;

		//自身以外は移動不可
		if (!photonView.IsMine) return;

		// 死んでたら移動不可
		if (m_isDeath) return;

        //インベントリを開いているなら移動不可
        if (m_stashController.IsOpen) return;

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

	[PunRPC]
    public void OnDeathPlayer()
	{
		m_isDeath = true;
		m_animator[0].SetTrigger("Death");
        m_animator[1].SetTrigger("Death");
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
			(int)m_job.AttackType,
			(int)condition.Grant,
			condition.Rate);
	}

	// (PlayerControllerのRequestDamageValue)
	[PunRPC]
	void Damage(int power, int attackTypeNum, int ConditionTypeNum, int grantRate)
	{
        m_status.Damage(power, (AttackType)attackTypeNum, ConditionTypeNum, grantRate);
	}

	[PunRPC]
	void SetPlayerId(int playerId)
	{
		m_playerId = playerId;
	}

	public int GetPlayerId()
	{
		return m_playerId; 
	}
}