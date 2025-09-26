using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WeaponStatusType
{
	Hp,
	Atk,
	Def,
	Spd,

	Length,
}

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObject/Info Equipment")]

public class Info_Equipment : ScriptableObject
{
	public enum WeaponStatusType
	{
		Hp,
		Atk,
		Def,
		Spd,

		Length,
	}

	// 武器のステータス（仮）
	[SerializeField] int m_hp;
	[SerializeField] int m_atk;
	[SerializeField] int m_def;
	[SerializeField] int m_spd;

	public int GetInfo(WeaponStatusType type)
	{
		int n = 0;
		switch(type)
		{
			case WeaponStatusType.Hp:
				n = m_hp;
				break;

			case WeaponStatusType.Atk:
				n = m_atk;
				break;

			case WeaponStatusType.Def:
				n = m_def;
				break;

			case WeaponStatusType.Spd:
				n = m_spd;
				break;
		}

		return n;
	}
}
