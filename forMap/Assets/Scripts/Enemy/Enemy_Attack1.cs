using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UŒ‚ƒpƒ^[ƒ“‚Ì”‚É‰‚¶‚Äˆ—‚ğ•Ï‚¦‚é
public class Enemy_Attack1 : MonoBehaviour
{
	[SerializeField] int m_attackNum = 0;   // UŒ‚ƒpƒ^[ƒ“‚Ì”
	Animator m_anim;

    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SelectAttack()
	{
		switch(m_attackNum)
		{
			case 1:
				OnePattern();
				break;

			case 2:
				TwoPattern();
				break;

			case 3:
				ThreePattern();
				break;
		}
	}

	// ‚Pí—Ş‚µ‚©‚È‚¢“G
	private void OnePattern()
	{
		m_anim.SetTrigger("attack");
	}

	// ‚Qí—Ş‚ ‚é“G
	private void TwoPattern()
	{
		int random = UnityEngine.Random.Range(0, 3);
		switch (random)
		{
			case 0:
				m_anim.SetTrigger("attack1");
				break;

			case 1:
				m_anim.SetTrigger("attack2");
				break;
		}
	}

	// ‚Rí—Ş‚ ‚é“G
	private void ThreePattern()
	{
		int random = UnityEngine.Random.Range(0, 3);
		switch (random)
		{
			case 0:
				m_anim.SetTrigger("attack1");
				break;

			case 1:
				m_anim.SetTrigger("attack2");
				break;

			case 2:
				m_anim.SetTrigger("attack3");
				break;
		}
	}

	// UŒ‚ƒpƒ^[ƒ“‚ğ•Ô‚·
	public int AttackNum()
	{
		return m_attackNum;
	}
}
