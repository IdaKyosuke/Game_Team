using Photon.Pun;
using UnityEngine;

public class PlayerAnime : MonoBehaviourPunCallbacks
{
    [SerializeField] Weapon_Collider m_collider; //攻撃用の当たり判定
	[SerializeField] Animator m_anim;

	private bool m_isAttack = false;

	public void OnAttackInit()
	{
		//攻撃開始
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
		// 攻撃中は無視
		if (m_isAttack) return;

		//コライダーが無ければ何もしない
		if(m_collider) m_collider.StartAttack();
        
		m_isAttack = true;
	}

	public void AttackEnd()
	{
		// 攻撃中以外は無視
		if (!m_isAttack) return;

		//コライダーが無ければ何にもしない
		if(m_collider) m_collider.EndAttack();

		m_isAttack = false;
	}

	public bool IsAttack()
	{
		return m_isAttack;
	}

	public void StartAttack()
	{
		m_anim.SetBool("attack", false);
	}
}