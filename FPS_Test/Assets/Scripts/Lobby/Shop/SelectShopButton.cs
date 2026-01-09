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
		m_manager.SetShopItemUI(m_inventorySize, ref m_shopList, m_isSet, gameObject);
	}

	// アイテムの座標を設定し終えた時に呼ぶ
	public void FinishSetItem()
	{
		m_isSet = true;
	}

	// m_objectList(GameObject) => m_shopList(ItemList) 販売アイテムリストの作成
	private void MakeList()
	{
		// 販売アイテムの数
		int itemCount = Random.Range(5, 21);

		for(int i = 0; i < itemCount; i++)
		{
			// アイテムを候補からランダムに取得
			GameObject obj = m_objectList.GetList()[Random.Range(0, m_objectList.GetList().Count)];
			ItemList item = ScriptableObject.CreateInstance<ItemList>();
			item.SetPrefabName(obj.name);
			m_shopList.Add(item);
		}
		/*
		foreach(GameObject obj in m_objectList.GetList())
		{
			ItemList item = ScriptableObject.CreateInstance<ItemList>();
			item.SetPrefabName(obj.name);
			m_shopList.Add(item);
		}
		*/
	}

	// 販売アイテムリストの更新
	public void UpdataItemList(List<ItemList> itemList)
	{
		Debug.Log("コピー前" + m_shopList.Count);
		m_shopList = itemList;
		Debug.Log("コピー後" + m_shopList.Count);
	}
}
