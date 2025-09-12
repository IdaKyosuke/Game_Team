using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Rendering;

public class InventoryUi_City : MonoBehaviour
{
	// �X���b�g���i�[����p�l��
	[SerializeField] Transform inventoryPanel;	// �ʏ�̃C���x���g��
	[SerializeField] Transform shopInventoryPanel;  // ���p�p�C���x���g��
	[SerializeField] Transform stashInventory;	// �X�^�b�V���̃v���C���[�̎������g

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
		Debug.Log("Ui���X�V���E�E�E");

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
