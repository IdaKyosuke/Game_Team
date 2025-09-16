using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StashUi_Box : MonoBehaviour
{
	// �X���b�g���i�[����p�l��
	[SerializeField] Transform itemBox; // �A�C�e���{�b�N�X�p�̃X���b�g

	private Slot_Stash[] slots;

	// Start is called before the first frame update
	void Start()
	{
		slots = itemBox.GetComponentsInChildren<Slot_Stash>();
	}

	public void UpdateUi()
	{
		Debug.Log("Ui���X�V���E�E�E");

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
