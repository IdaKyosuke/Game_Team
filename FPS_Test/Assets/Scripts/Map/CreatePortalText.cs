using Photon.Pun;
using UnityEngine;

public class CreatePortalText : MonoBehaviourPunCallbacks
{
	[SerializeField] float m_destroyTime;
	float m_time;

    // Update is called once per frame
    void Update()
    {
		m_time += Time.deltaTime;
		
		if (m_time >= m_destroyTime)
		{
			PhotonNetwork.Destroy(gameObject);
		}
    }
}
