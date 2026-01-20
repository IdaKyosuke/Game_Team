using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Trader ItemList")]
public class Trader_ItemList : ScriptableObject
{
	[SerializeField] List<Trader_ItemInfo> m_shopList;

	public List<Trader_ItemInfo> GetList()
	{
		return m_shopList;
	}
}
