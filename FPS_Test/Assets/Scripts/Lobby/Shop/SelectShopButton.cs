using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SelectShopButton : MonoBehaviour
{
	[SerializeField] Trader_ItemList m_objectList;				// Item_Obejct型
	private List<ItemList> m_shopList = new List<ItemList>();	

	[SerializeField] StashManager m_manager;
	[SerializeField] Info_InventorySize m_inventorySize;
	// アイテムの座標を設定しているか
	private bool m_isSet = false;

	private void Start()
	{
		if(!m_manager)
		{
			m_manager = GameObject.FindWithTag("inventoryManager").GetComponent<StashManager>();
		}

		MakeList();
	}

	public void SetShopItem()
	{
		m_manager.SetShopItemUI(m_inventorySize, m_shopList, m_isSet, gameObject);
	}

	// アイテムの座標を設定し終えた時に呼ぶ
	public void FinishSetItem()
	{
		m_isSet = true;
	}

	// m_objectList(GameObject) => m_shopList(ItemList)
	private void MakeList()
	{
		foreach(GameObject obj in m_objectList.GetList())
		{
			ItemList item = ScriptableObject.CreateInstance<ItemList>();
			item.SetPrefabName(obj.name);
			m_shopList.Add(item);
		}
	}
}
