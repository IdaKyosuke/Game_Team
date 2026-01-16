using Photon.Pun;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class PlayerAnime : MonoBehaviourPunCallbacks
{
    [SerializeField] Animator m_anim;

    public bool IsAttack => m_anim.GetBool("attack");

    public void OnAttack1()
    {
        //E‹Æ•Ê‚ÌUŒ‚ˆ—
        transform.root.GetComponent<Job>().Attack();
    }

    public void OnAttack1End()
    {
        //E‹Æ•Ê‚ÌUŒ‚I—¹ˆ—
        transform.root.GetComponent<Job>().AttackEnd();
    }

    public void ResetBool()
    { 
        m_anim.SetBool("attack", false);
    }
}