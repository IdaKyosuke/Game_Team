using Photon.Pun;
using UnityEngine;

public class PlayerAnime : MonoBehaviourPunCallbacks
{
    [SerializeField] Weapon_Collider m_collider; //UŒ‚—p‚Ì“–‚½‚è”»’è

	private bool m_isAttack = false;

	public void OnAttackInit()
	{
		//UŒ‚ŠJn
		//m_isAttack = true;
	}

    public void OnAttack1()
    {
		transform.root.GetComponent<PhotonView>().RPC("AttackAnime", RpcTarget.All);
    }

    public void OnAttack1End()
    {
		transform.root.GetComponent<PhotonView>().RPC("AttackAnimeEnd", RpcTarget.All);
	}

	public void Attack()
	{
		// UŒ‚’†‚Í–³‹
		if (m_isAttack) return;
		m_collider.StartAttack();
		m_isAttack = true;
	}

	public void AttackEnd()
	{
		// UŒ‚’†ˆÈŠO‚Í–³‹
		if (!m_isAttack) return;
		m_collider.EndAttack();
		m_isAttack = false;
	}

	public bool IsAttack()
	{
		return m_isAttack;
	}
}