using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
	private CharacterController characterController;  // CharacterController型の変数
	private Vector3 moveVelocity;  // キャラクターコントローラーを動かすためのVector3型の変数
	[SerializeField] private Camera m_mapCamera;
	[SerializeField] private Transform verRot;  //縦の視点移動の変数(カメラに合わせる)
	[SerializeField] private Transform horRot;  //横の視点移動の変数(プレイヤーに合わせる)
	[SerializeField] private float moveSpeed;  //移動速度
	[SerializeField] private float sensX = 2f;
	[SerializeField] private float sensY = 2f;
	private float rotationY, rotationX;
	bool m_isGrounded;
	bool m_isDeath = false;

	[SerializeField] private float jumpPower;  //ジャンプ力

	private void Awake()
    {
		characterController = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
        characterController.enabled = false;
    }

    void Start()
	{
		// マスターの持つリストを参照
		photonView.RPC(nameof(RequestPlayerSpawnPos), RpcTarget.MasterClient, photonView.ViewID);
    }

	[PunRPC]
	void RequestPlayerSpawnPos(int viewId)
	{
		PhotonView.Find(viewId).RPC(nameof(SetPlayerPos), PhotonView.Find(viewId).Owner, Create_Maze.GetPlayerSpawnPos().position);
	}

	[PunRPC]
	void SetPlayerPos(Vector3 pos)
	{
		// transform.position = pos;
		transform.position = new Vector3(0, 1, 0);
        characterController.enabled = true;
	}

	void TreasureOpen()
	{
		RaycastHit hit;
		// レイを飛ばす
		if (Physics.Raycast(transform.position, transform.forward, out hit, 20))
		{
			if (Input.GetMouseButtonDown(0) && hit.transform.TryGetComponent(out TreasureBoxItem treasure))
			{
				treasure.GetItem();
				hit.transform.GetComponent<TreasureAnime>().Open();
			}
		}
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
		return characterController.isGrounded;
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

			m_isGrounded = CheckGrounded();

			TreasureOpen();

			Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X") * sensX,
				Input.GetAxis("Mouse Y") * sensY);

			rotationX -= mouseInput.y;
			rotationY += mouseInput.x;
			rotationY %= 360; // 絶対値が大きくなりすぎないように

			// 上下の視点移動量をClamp
			rotationX = Mathf.Clamp(rotationX, -90, 90);

			// 頭、体の向きの適用
			verRot.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
			horRot.transform.localRotation = Quaternion.Euler(0, rotationY, 0);

			//Wキーがおされたら
			if (Input.GetKey(KeyCode.W))
			{
				characterController.Move(this.gameObject.transform.forward * moveSpeed * Time.deltaTime);
			}
			//Sキーがおされたら
			if (Input.GetKey(KeyCode.S))
			{
				characterController.Move(this.gameObject.transform.forward * -1f * moveSpeed * Time.deltaTime);
			}
			//Aキーがおされたら
			if (Input.GetKey(KeyCode.A))
			{
				characterController.Move(this.gameObject.transform.right * -1 * moveSpeed * Time.deltaTime);
			}
			//Dキーがおされたら
			if (Input.GetKey(KeyCode.D))
			{
				characterController.Move(this.gameObject.transform.right * moveSpeed * Time.deltaTime);
			}

			// 接地しているとき
			if (m_isGrounded)
			{
				// ジャンプ
				if (Input.GetKeyDown(KeyCode.Space))
				{
					moveVelocity.y = jumpPower;
				}
			}
			// 空中にいる時
			else
			{
				// 重力をかける
				moveVelocity.y += Physics.gravity.y * Time.deltaTime;
			}

			// キャラクターを動かす
			characterController.Move(moveVelocity * Time.deltaTime);

			// 移動スピードをアニメーターに反映する
			//animator.SetFloat("MoveSpeed", new Vector3(moveVelocity.x, 0, moveVelocity.z).magnitude);
		}
	}

	public override void OnLeftRoom()
	{
		Debug.Log("LeftRoom");
		m_isDeath = true;
		base.OnLeftRoom();
	}
}