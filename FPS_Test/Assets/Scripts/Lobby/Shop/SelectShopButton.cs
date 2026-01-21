using System.Collections.Generic;
using UnityEngine;

public class SelectShopButton : MonoBehaviour
{
	[SerializeField] Trader_ItemList m_objectList;				// ExcelのRarityとid情報が入っている
	private List<ItemList> m_shopList = new List<ItemList>();

	[SerializeField] StashManager m_manager;
	[SerializeField] Info_InventorySize m_inventorySize;

	[SerializeField] ExcelData m_excelData;

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
			ItemList item = ScriptableObject.CreateInstance<ItemList>();
			List<MapObjectEntity> objectData = m_excelData.common;
			Trader_ItemInfo info = m_objectList.GetList()[Random.Range(0, m_objectList.GetList().Count)];

			switch (info.m_rarity)
			{
				case Rarity.Common:
					objectData = m_excelData.common;
					break;

				case Rarity.Rare:
					objectData = m_excelData.rare;
					break;
			}

			foreach(var data in objectData)
			{
				if(data.id == info.m_id)
				{
					item.ItemData = data;

					break;
				}
			}
			m_shopList.Add(item);
		}
	}
}
