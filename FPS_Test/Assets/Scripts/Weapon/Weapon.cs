using Photon.Pun;
using UnityEngine;

public class Weapon : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform m_muzzle;   // �e��
    [SerializeField] float m_rate;  // ���ˑҋ@����
	private float m_count;		// ���ˑҋ@���ԃJ�E���g�p
	private bool m_isShot = false;

    // Start is called before the first frame update
    void Start()
    {
		m_count = 0;
	}

    // Update is called once per frame
    void Update()
    {
		if (photonView.IsMine)
		{
			// ���N���b�N�Ŏˌ�
			if(!m_isShot && Input.GetMouseButton(0))
			{
				PhotonNetwork.Instantiate("Bullet", m_muzzle.position, m_muzzle.rotation);
				m_isShot = true;
			}

			if(m_isShot)
			{
				m_count += Time.deltaTime;
				if(m_count >= m_rate)
				{
					m_isShot = false;
					m_count = 0;
				}
			}
		}
    }
}
