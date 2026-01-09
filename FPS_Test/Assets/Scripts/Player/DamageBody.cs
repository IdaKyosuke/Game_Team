using Photon.Pun;
using System.Net.Sockets;
using UnityEngine;

public class DamageBody : MonoBehaviourPunCallbacks
{
	private void OnTriggerEnter(Collider other)
	{
		// •Ší‚Ìî•ñ‚ğæ“¾
		if (other.TryGetComponent(out Weapon_Collider weapon))
		{
			PhotonView otherView = weapon.Parent.GetComponent<PhotonView>();

			// ©•ª©g‚ÌŒ•‚Í”»’è‚ğæ‚ç‚È‚¢
			if (otherView == transform.root.GetComponent<PhotonView>()) return;
            otherView.RPC("RequestDamageValue", otherView.Owner, transform.root.GetComponent<PhotonView>().ViewID);
		}

		if (other.TryGetComponent(out Enemy_WeaponCollider enemy))
		{
			transform.root.GetComponent<PlayerStatus>().Damage(
				enemy.attackPower, (int)AttackType.Physical, (int)ConditionType.None, 0);
		}
	}
}