using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Rendering;

public class Test_InventoryUi : MonoBehaviour
{
	// �X���b�g���i�[����p�l��
	public Transform inventoryPanel;
	private Test_Slot[] slots;

	// Start is called before the first frame update
	void Start()
    {
		if(!inventoryPanel)
		{
			inventoryPanel = GameObject.FindWithTag("inventoryUI").GetComponent<Transform>().GetChild(1).transform;	
		}
		
		slots = inventoryPanel.GetComponentsInChildren<Test_Slot>();

		UpdateUi();
	}

    public void UpdateUi()
	{
		Debug.Log("Ui���X�V���E�E�E");

		for(int i = 0; i < slots.Length; i++)
		{
			if (i < Test_Inventory.instance.items.Count)
			{
				slots[i].AddItem(Test_Inventory.instance.items[i]);
			}
			else
			{
				slots[i].ClearItem();
			}
		}
	}
}
