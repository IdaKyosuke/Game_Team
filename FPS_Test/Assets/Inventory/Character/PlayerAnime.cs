using Photon.Pun;
using UnityEngine;

public class PlayerAnime : MonoBehaviourPunCallbacks
{
	[SerializeField] Animator m_anim;

    private Job m_job;
    private bool m_isAttack = false;

	public bool IsAttack => m_isAttack;

    private void Start()
    {
        m_job  = transform.root.GetComponent<Job>();
    }

    public void OnAttackInit()
	{
		//攻撃開始
		m_isAttack = true;
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

		//ジョブごとの攻撃処理
		m_job.Attack();

        m_isAttack = true;
	}

	public void AttackEnd()
	{
		// 攻撃中以外は無視
		if (!m_isAttack) return;

        //ジョブごとの攻撃終了処理
        m_job.AttackEnd();

        m_isAttack = false;
	}

	public void StartAttack()
	{
		m_anim.SetBool("attack", false);
	}
}