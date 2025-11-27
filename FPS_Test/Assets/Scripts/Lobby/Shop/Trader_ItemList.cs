using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Trader ItemList")]
public class Trader_ItemList : ScriptableObject
{
	[SerializeField] List<GameObject> m_shopList;

	public List<GameObject> GetList()
	{
		return m_shopList;
	}
}
