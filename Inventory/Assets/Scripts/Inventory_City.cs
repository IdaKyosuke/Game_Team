using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory_City : MonoBehaviour
{
	// シングルトン（インベントリをどこからでも呼び出せるようにする）
	public static Inventory_City instance;
	private InventoryUi_City m_inventoryUi;

	// アイテムの最大数
	const int MaxItemNum = 25;

	// アイテムリスト
	//[SerializeField] PlayerStatus m_playerStatus;	// インベントリを内容を保存する用
	public List<Test_Item> items = new List<Test_Item>();

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}

		// 最初に現状の持ち物を埋める(参照渡し)
		//items = m_playerStatus.itemList;
	}

	// Start is called before the first frame update
	void Start()
	{
		m_inventoryUi = GetComponent<InventoryUi_City>();
		//m_inventoryUi.UpdateUi();
	}

	// アイテムを追加
	public bool AddItem(Test_Item item)
	{
		// 現在のアイテム数を確認して、空きがあったら収納
		if (items.Count < MaxItemNum)
		{
			items.Add(item);
			m_inventoryUi.UpdateUi();
			return true;
		}
		else
		{
			return false;
		}
	}

	// アイテムを削除
	public void RemoveItem(Test_Item item)
	{
		items.Remove(item);
		m_inventoryUi.UpdateUi();
	}

	// 今の手持ちからアイテムを探す
	public bool SearchInventory(Test_Item item)
	{
		for(int i = 0; i < items.Count; i++)
		{
			if (items[i].Equals(item)) return true;
		}

		return false;
	}
}
