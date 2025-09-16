using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StashUi_Box : MonoBehaviour
{
	// スロットを格納するパネル
	[SerializeField] Transform itemBox; // アイテムボックス用のスロット

	private Slot_Stash[] slots;

	// Start is called before the first frame update
	void Start()
	{
		slots = itemBox.GetComponentsInChildren<Slot_Stash>();
	}

	public void UpdateUi()
	{
		Debug.Log("Uiを更新中・・・");

		for (int i = 0; i < slots.Length; i++)
		{
			if (i < Stash_Box.instance.items.Count)
			{
				slots[i].AddItem(Stash_Box.instance.items[i]);
			}
			else
			{
				slots[i].ClearItem();
			}
		}
	}
}
