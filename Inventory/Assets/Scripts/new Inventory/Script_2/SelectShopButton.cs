using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectShopButton : MonoBehaviour
{
	[SerializeField] List<ItemList> m_shopList;
	[SerializeField] StashManager m_manager;
	[SerializeField] Info_InventorySize m_inventorySize;
	// アイテムの座標を設定しているか
	private bool m_isSet = false;

	private void Start()
	{
		if(!m_manager)
		{
			m_manager = GameObject.FindWithTag("stashManager").GetComponent<StashManager>();
		}
	}

	public void SetShopItem()
	{
		m_manager.SetShopItemUI(m_inventorySize, m_shopList, m_isSet);
	}

	// アイテムの座標を設定し終えた時に呼ぶ
	public void FinishSetItem()
	{
		m_isSet = true;
	}
}
