using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Test_Inventory : MonoBehaviour
{
	// シングルトン（インベントリをどこからでも呼び出せるようにする）
	public static Test_Inventory instance;
	private Test_InventoryUi m_inventoryUi;

	// アイテムの最大数
	private const int MaxItemNum = 25;

	// アイテムリスト
	//[SerializeField] PlayerStatus m_playerStatus;   // アイテムを保存する(scriptableObject)
	public List<Test_Item> items = new List<Test_Item>();

	// アイテム獲得音
	[SerializeField] AudioSource m_se;

	private void Awake()
	{
		if (!instance)
		{
			instance = this;
		}

		// 最初に現状の持ち物を埋める
		//items = m_playerStatus.itemList;
	}

	// Start is called before the first frame update
	void Start()
    {
		m_inventoryUi = GetComponent<Test_InventoryUi>();
	}

	// アイテムを追加
	public bool AddItem(Test_Item item)
	{
		// 現在のアイテム数を確認して、空きがあったら収納
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

	// アイテムを削除
	public void RemoveItem(Test_Item item)
	{
		items.Remove(item);
		m_inventoryUi.UpdateUi();
	}
}
