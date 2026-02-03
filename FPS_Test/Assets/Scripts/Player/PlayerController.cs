using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
	[SerializeField] PlayerAnime m_playerAnimFPS;          // アニメーション管理用オブジェクト
	[SerializeField] Weapon_Collider m_weapon;
	[SerializeField] GameObject m_magicBall;
	[SerializeField] PlayerViewUI m_playerViewUI;

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

	public Weapon_Collider Weapon => m_weapon;

	public GameObject MagicBall => m_magicBall;

	public PlayerViewUI ViewUI => m_playerViewUI;

    private void Awake()
    {
        m_setPos = false;
        m_characterController = GetComponent<CharacterController>();
        m_status = GetComponent<PlayerStatus>();
        m_stashController = GetComponent<StashController>();
        m_condition = GetComponent<Condition>();
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

		//職業の取得
		switch (GameManager.Instance.PlayerJobType)
		{ 
			case JobType.Warrior:
				m_job = gameObject.AddComponent<Warrior>();
				break;

			case JobType.Wizard:
				m_job = gameObject.AddComponent<Wizard>();
				break;

			case JobType.Cleric:
				m_job = gameObject.AddComponent<Cleric>();
				break;

			case JobType.Thief:
				m_job = gameObject.AddComponent<Thief>();
				break;
        }


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
		pos += new Vector3(0, 1, 0);
		transform.position = pos;
        m_characterController.enabled = true;
		m_setPos = true;
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
		if (m_stashController.NowScavenger) return;

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
            if (m_playerAnimFPS.IsAttack) return;

            //インベントリを開いているなら無視
			if (m_stashController.IsOpen) return;

            //攻撃アニメーション
            m_animator[0].SetBool("attack", true);
            m_animator[1].SetBool("attack", true);
        }

		//固有アクション
		if (Input.GetKeyDown(KeyCode.Q))
		{
            //攻撃中は無視
            if (m_playerAnimFPS.IsAttack) return;

            //職業別の固有アクション
            m_job.Identity();

			//プレイヤーUIに反映
			m_playerViewUI.SetIcon(m_job.AttackType);
        }

        // デバッグ用
        if (Input.GetKeyDown("5"))
		{
			m_stashController.Save();
			m_gameManager.ReturnLobby(photonView.IsMine);
		}
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Portal"))
		{
			PhotonView.Destroy(other);
			m_stashController.Save();
			m_gameManager.ReturnLobby(photonView.IsMine);
		}
	}

	private void FixedUpdate()
	{
		if (!m_setPos) return;
		if (!photonView.IsMine) return;
		if (m_isDeath) return;
        if (m_stashController.NowScavenger) return;

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

        //箱開け中なら移動不可
        if (m_stashController.NowScavenger) return;

		//モデルの位置を補正
		transform.GetChild(0).localPosition = new Vector3(0, -1, 0);
		transform.GetChild(1).localPosition = new Vector3(0, -1, 0);

        // 視点移動
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        // 横回転
        transform.Rotate(Vector3.up * mouseX);

        // 腰の回転
        m_rotateX -= mouseY;
        m_rotateX = Mathf.Clamp(m_rotateX, -40.0f, 30.0f);
        m_mainCamera.transform.localRotation = Quaternion.Euler(m_rotateX, 0f, 0f);
    }

	[PunRPC]
    public void OnDeathPlayer()
	{
		Debug.Log(photonView);
		if (m_isDeath) return;
		m_isDeath = true;
		m_animator[0].SetTrigger("Death");
        m_animator[1].SetTrigger("Death");

		m_stashController.DeleteInventory();
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
        if (m_status.Damage(power, (AttackType)attackTypeNum, ConditionTypeNum, grantRate))
		{
			m_gameManager.ReturnLobby(photonView.IsMine);
		}
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