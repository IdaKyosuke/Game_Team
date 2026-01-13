using Photon.Pun;
using UnityEngine;

public class PlayerAnime : MonoBehaviourPunCallbacks
{
    [SerializeField] Weapon_Collider m_attackCollier;
    [SerializeField] Animator m_anim;

	public bool IsAttack => m_anim.GetBool("attack");

    public void OnAttack1()
    { 
        m_attackCollier?.StartAttack();
    }

    public void OnAttack1End()
    { 
		m_attackCollier?.EndAttack();
    }

    public void ResetBool()
    { 
        m_anim.SetBool("attack", false);
    }
}