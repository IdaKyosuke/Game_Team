using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stash_Test : MonoBehaviour
{
	[SerializeField] List<ItemList> m_items;

	public string ItemName(int index)
	{
		return m_items[index].GetPrefabName();
	}

	public List<ItemList> GetList()
	{
		return m_items;
	}

	public void LoadItemList(List<ItemList> list)
	{
		m_items = list;
	}
}
