using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Rendering;

public class InventoryUi_City : MonoBehaviour
{
	// スロットを格納するパネル
	[SerializeField] Transform inventoryPanel;	// 通常のインベントリ
	[SerializeField] Transform shopInventoryPanel;  // 売却用インベントリ
	[SerializeField] Transform stashInventory;	// スタッシュのプレイヤーの持ち物枠

	private Slot_City[] slots;
	//private Slot_ForSell[] slots_forSell;
	//private Slot_Stash[] slots_stash;

	// Start is called before the first frame update
	void Start()
	{
		slots = inventoryPanel.GetComponentsInChildren<Slot_City>();
		//slots_forSell = shopInventoryPanel.GetComponentsInChildren<Slot_ForSell>();
		//slots_stash = stashInventory.GetComponentsInChildren<Slot_Stash>();

		UpdateUi();
	}

	public void UpdateUi()
	{
		Debug.Log("Uiを更新中・・・");

		for (int i = 0; i < slots.Length; i++)
		{
			if (i < Inventory_City.instance.items.Count)
			{
				slots[i].AddItem(Inventory_City.instance.items[i]);
				//slots_forSell[i].AddItem(Inventory_City.instance.items[i]);
				//slots_stash[i].AddItem(Inventory_City.instance.items[i]);
			}
			else
			{
				slots[i].ClearItem();
				//slots_forSell[i].ClearItem();
				//slots_stash[i].ClearItem();
			}
		}
	}
}
