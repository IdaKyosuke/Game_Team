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
			otherView.RPC("RequestDamageValue", otherView.Owner, photonView.ViewID);
		}
	}

	// (Weapon_Collider‚ÌRequestDamageValue)
	[PunRPC]
	void Damage(int power, int attackTypeNum, int ConditionTypeNum, int grantRate)
	{
		m_status.Damage(power, (AttackType)attackTypeNum, ConditionTypeNum, grantRate);
	}
}
