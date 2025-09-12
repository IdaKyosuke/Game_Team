using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Test_Inventory : MonoBehaviour
{
	// �V���O���g���i�C���x���g�����ǂ�����ł��Ăяo����悤�ɂ���j
	public static Test_Inventory instance;
	private Test_InventoryUi m_inventoryUi;

	// �A�C�e���̍ő吔
	private const int MaxItemNum = 25;

	// �A�C�e�����X�g
	//[SerializeField] PlayerStatus m_playerStatus;   // �A�C�e����ۑ�����(scriptableObject)
	public List<Test_Item> items = new List<Test_Item>();

	// �A�C�e���l����
	[SerializeField] AudioSource m_se;

	private void Awake()
	{
		if (!instance)
		{
			instance = this;
		}

		// �ŏ��Ɍ���̎������𖄂߂�
		//items = m_playerStatus.itemList;
	}

	// Start is called before the first frame update
	void Start()
    {
		m_inventoryUi = GetComponent<Test_InventoryUi>();
	}

	// �A�C�e����ǉ�
	public bool AddItem(Test_Item item)
	{
		// ���݂̃A�C�e�������m�F���āA�󂫂�����������[
		if(items.Count < MaxItemNum)
		{
			items.Add(item);
			m_inventoryUi.UpdateUi();
			m_se.Play();
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
}
