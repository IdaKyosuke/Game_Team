using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
	[SerializeField] float m_moveSpeed = 1.0f;
	private Rigidbody m_rb;
	[SerializeField] Camera m_playerCam;
	[SerializeField] int m_hp;
	[SerializeField] int m_damage;

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
		// 視点移動
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

		// キー移動
		float inputX = Input.GetAxis("Horizontal");
		float inputZ = Input.GetAxis("Vertical");

		// カメラの方向から、X-Z平面の単位ベクトルを取得
		Vector3 cameraForward = Vector3.Scale(m_playerCam.transform.forward, new Vector3(1, 0, 1)).normalized;

		// 方向キーの入力値とカメラの向きから、移動方向を決定
		Vector3 moveForward = cameraForward * inputZ + Camera.main.transform.right * inputX;

		// 移動
		m_rb.velocity = moveForward * m_moveSpeed;

		// 横回転
		transform.Rotate(0, mouseX, 0);
    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Bullet"))
		{
			m_hp -= m_damage;
			if(m_hp <= 0)
			{
				m_hp = 0;
			}
		}
	}
}
