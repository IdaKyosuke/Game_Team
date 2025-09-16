using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stash_Box : MonoBehaviour
{
	// アイテムボックスのシングルトン
	public static Stash_Box instance;
	[SerializeField] PlayerStatus playerStatus;	// プレイヤーのscriptableObject

	// アイテムの最大数
	private const int MaxItemNum = 50;

	// アイテムボックスのアイテムリスト
	public List<Test_Item> items = new List<Test_Item>();

	private StashUi_Box m_inventoryUi;

	private void Awake()
	{
		if(!instance)
		{
			instance = this;
		}
		// 参照で受け取る
		items = playerStatus.stashList;
	}

	// Start is called before the first frame update
	void Start()
    {
		m_inventoryUi = GetComponent<StashUi_Box>();
		m_inventoryUi.UpdateUi();
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

	// アイテムボックスからアイテムを探す
	public bool SearchInventory(Test_Item item)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].Equals(item)) return true;
		}

		return false;
	}
}
