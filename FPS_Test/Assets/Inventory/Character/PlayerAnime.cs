using Photon.Pun;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class PlayerAnime : MonoBehaviourPunCallbacks
{
    private Animator m_anim;

	[SerializeField] List<GameObject> m_models = new List<GameObject>();
	[SerializeField] GameObject m_deathModel;

	public bool IsAttack => m_anim.GetBool("attack");

    private void Start()
    {
        m_anim = GetComponent<Animator>();
    }

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

	[PunRPC]
	public void ChangeModel()
	{
		m_deathModel.SetActive(true);
		// ¶‚«‚Ä‚¢‚éŠÔ‚Ìƒ‚ƒfƒ‹‚ğ”ñ•\¦‚É
		foreach (var m in m_models)
		{
			m.SetActive(false);
		}
	}
}