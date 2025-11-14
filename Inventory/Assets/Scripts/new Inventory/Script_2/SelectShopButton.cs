using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectShopButton : MonoBehaviour
{
	[SerializeField] List<ItemList> m_shopList;
	[SerializeField] StashManager m_manager;
	[SerializeField] Info_InventorySize m_inventorySize;

	private void Start()
	{
		if(!m_manager)
		{
			m_manager = GameObject.FindWithTag("stashManager").GetComponent<StashManager>();
		}
	}

	public void SetShopItem()
	{
		m_manager.SetShopItemUI(m_inventorySize, m_shopList);
	}
}
