using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
	private CharacterController characterController;  // CharacterController型の変数
	private Vector3 moveVelocity;  // キャラクターコントローラーを動かすためのVector3型の変数
	[SerializeField] private Animator animator;
	[SerializeField] private Transform verRot;  //縦の視点移動の変数(カメラに合わせる)
	[SerializeField] private Transform horRot;  //横の視点移動の変数(プレイヤーに合わせる)
	[SerializeField] private float moveSpeed;  //移動速度
	[SerializeField] private float sensX = 2f;
	[SerializeField] private float sensY = 2f;
	private float rotationY, rotationX;
	bool m_isGrounded;

	[SerializeField] private float jumpPower;  //ジャンプ力

	void Start()
	{
		characterController = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void FixedUpdate()
	{
		m_isGrounded = CheckGrounded();
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

	void Update()
	{
		// ゲームの終了
		EndGame();

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

	//ゲーム終了
	private void EndGame()
	{
		//Escが押された時
		if (Input.GetKey(KeyCode.Escape))
		{

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
		}

	}
}