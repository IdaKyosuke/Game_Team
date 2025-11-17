using Photon.Pun;
using UnityEngine;

public class DamageBody : MonoBehaviourPunCallbacks
{
	[SerializeField] PlayerStatus m_status;

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("‰½‚©‚É“–‚½‚Á‚½");
		// •Ší‚Ìî•ñ‚ğæ“¾
		if (other.TryGetComponent(out Weapon_Collider weapon))
		{
			Debug.Log("Œ•‚É“–‚½‚Á‚½");
			PhotonView otherView = other.transform.root.GetComponent<PhotonView>();
			Debug.Log("Œ•‚ÌPhoton" + otherView);
			otherView.RPC("RequestDamageValue", otherView.Owner, transform.root.GetComponent<PhotonView>().ViewID);
		}
	}
}
