using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stash_Box : MonoBehaviour
{
	// �A�C�e���{�b�N�X�̃V���O���g��
	public static Stash_Box instance;
	[SerializeField] PlayerStatus playerStatus;	// �v���C���[��scriptableObject

	// �A�C�e���̍ő吔
	private const int MaxItemNum = 50;

	// �A�C�e���{�b�N�X�̃A�C�e�����X�g
	public List<Test_Item> items = new List<Test_Item>();

	private StashUi_Box m_inventoryUi;

	private void Awake()
	{
		if(!instance)
		{
			instance = this;
		}
		// �Q�ƂŎ󂯎��
		items = playerStatus.stashList;
	}

	// Start is called before the first frame update
	void Start()
    {
		m_inventoryUi = GetComponent<StashUi_Box>();
		m_inventoryUi.UpdateUi();
    }

	// �A�C�e����ǉ�
	public bool AddItem(Test_Item item)
	{
		// ���݂̃A�C�e�������m�F���āA�󂫂�����������[
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

	// �A�C�e�����폜
	public void RemoveItem(Test_Item item)
	{
		items.Remove(item);
		m_inventoryUi.UpdateUi();
	}

	// �A�C�e���{�b�N�X����A�C�e����T��
	public bool SearchInventory(Test_Item item)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].Equals(item)) return true;
		}

		return false;
	}
}
