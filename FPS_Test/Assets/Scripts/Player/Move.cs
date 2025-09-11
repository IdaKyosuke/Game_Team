using Photon.Pun;
using UnityEngine;

public class Move : MonoBehaviourPunCallbacks
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
		// ���g�����������I�u�W�F�N�g�����Ɉړ��������s��
		if (photonView.IsMine)
		{
			// ���_�ړ�
			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = Input.GetAxis("Mouse Y");

			// �L�[�ړ�
			float inputX = Input.GetAxis("Horizontal");
			float inputZ = Input.GetAxis("Vertical");

			// �J�����̕�������AX-Z���ʂ̒P�ʃx�N�g�����擾
			Vector3 cameraForward = Vector3.Scale(m_playerCam.transform.forward, new Vector3(1, 0, 1)).normalized;

			// �����L�[�̓��͒l�ƃJ�����̌�������A�ړ�����������
			Vector3 moveForward = cameraForward * inputZ + Camera.main.transform.right * inputX;

			// �ړ�
			m_rb.velocity = moveForward * m_moveSpeed;

			// ����]
			transform.Rotate(0, mouseX, 0);
		}
    }

	public void ChangeName(string name)
	{
		if (photonView.IsMine)
		{
			photonView.Owner.NickName = name;
		}
	}

    public string GetName()
    {
        if (photonView.IsMine)
        {
            return photonView.Owner.NickName;
        }
        else
        {
            return null;
        }
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
