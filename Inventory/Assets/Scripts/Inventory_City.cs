using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory_City : MonoBehaviour
{
	// �V���O���g���i�C���x���g�����ǂ�����ł��Ăяo����悤�ɂ���j
	public static Inventory_City instance;
	private InventoryUi_City m_inventoryUi;

	// �A�C�e���̍ő吔
	const int MaxItemNum = 25;

	// �A�C�e�����X�g
	//[SerializeField] PlayerStatus m_playerStatus;	// �C���x���g������e��ۑ�����p
	public List<Test_Item> items = new List<Test_Item>();

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}

		// �ŏ��Ɍ���̎������𖄂߂�(�Q�Ɠn��)
		//items = m_playerStatus.itemList;
	}

	// Start is called before the first frame update
	void Start()
	{
		m_inventoryUi = GetComponent<InventoryUi_City>();
		//m_inventoryUi.UpdateUi();
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

	// ���̎莝������A�C�e����T��
	public bool SearchInventory(Test_Item item)
	{
		for(int i = 0; i < items.Count; i++)
		{
			if (items[i].Equals(item)) return true;
		}

		return false;
	}
}
