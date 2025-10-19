using Photon.Pun;
using UnityEngine;

public class FPSMove : MonoBehaviourPunCallbacks
{
	[SerializeField] private Transform body;
	[SerializeField] private float sensX = 5f;
	[SerializeField] private float sensY = 5f;
	private float rotationY;
	CharacterController con;

	float playerSpeed;

	Rigidbody m_rigidbody;
	float inputHorizontal;
	float inputVertical;
	void Start()
	{
		con = GetComponent<CharacterController>();
		m_rigidbody = GetComponent<Rigidbody>();
		playerSpeed = 5.0f;
	}

	private void Update()
	{
		if (photonView.IsMine)
		{
			Look();

			inputHorizontal = Input.GetAxisRaw("Horizontal");
			inputVertical = Input.GetAxisRaw("Vertical");
		}
	}

	private void FixedUpdate()
	{
		if (photonView.IsMine) Walk();
	}

	// マウス視点移動
	private void Look()
	{
		Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X") * sensX,
			Input.GetAxis("Mouse Y") * sensY);

		//rotationX -= mouseInput.y;
		rotationY += mouseInput.x;
		rotationY %= 360; // 絶対値が大きくなりすぎないように

		// 上下の視点移動量をClamp
		//rotationX = Mathf.Clamp(rotationX, -90, 90);

		// 頭、体の向きの適用
		//head.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
		body.transform.localRotation = Quaternion.Euler(0, rotationY, 0);
	}

	// 歩き
	private void Walk()
	{
		playerSpeed = Input.GetKey(KeyCode.LeftShift) ? 10 : 5;

		// カメラの方向から、X-Z平面の単位ベクトルを取得
		Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

		// 方向キーの入力値とカメラの向きから、移動方向を決定
		Vector3 moveForward = cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;

		// 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
		con.Move(moveForward * playerSpeed* Time.deltaTime + new Vector3(0, m_rigidbody.velocity.y, 0));

		// キャラクターの向きを進行方向に
		if (moveForward != Vector3.zero)
		{
			transform.rotation = Quaternion.LookRotation(moveForward);
		}
	}
}