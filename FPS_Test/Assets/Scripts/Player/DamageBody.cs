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
			PhotonView otherView = other.transform.root.GetComponent<PhotonView>();
            otherView.RPC("RequestDamageValue", otherView.Owner, transform.root.GetComponent<PhotonView>().ViewID);
		}
	}
}