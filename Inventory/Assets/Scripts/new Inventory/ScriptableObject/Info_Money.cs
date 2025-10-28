using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Info Money")]
public class Info_Money : ScriptableObject
{
	[SerializeField] int m_haveMoney;

	// Š‹à‚Ì‰ÁZ
	public void AddMoney(int money)
	{
		m_haveMoney += money;
	}

	// Š‹à‚Ìæ“¾
	public int GetCurrentMoney()
	{
		return m_haveMoney;
	}

	// Š‹à‚Ìg—p
	public void UseMoney(int money)
	{
		m_haveMoney -= money;
	}
}
