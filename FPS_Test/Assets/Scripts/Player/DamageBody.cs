using Photon.Pun;
using UnityEngine;

public class DamageBody : MonoBehaviourPunCallbacks
{
	[SerializeField] PlayerStatus m_status;

	private void OnTriggerEnter(Collider other)
	{
		// •Ší‚Ìî•ñ‚ğæ“¾
		if (other.TryGetComponent(out Weapon_Collider weapon))
		{
			PhotonView otherView = other.GetComponent<Weapon_Collider>().Parent.GetComponent<PhotonView>();

			// ©•ª©g‚ÌŒ•‚Í”»’è‚ğæ‚ç‚È‚¢
			if (otherView == transform.root.GetComponent<PhotonView>()) return;
            otherView.RPC("RequestDamageValue", otherView.Owner, transform.root.GetComponent<PhotonView>().ViewID);
		}
	}
}