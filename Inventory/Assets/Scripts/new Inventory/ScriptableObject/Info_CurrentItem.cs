using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Info CurrentItem")]
public class Info_CurrentItem : ScriptableObject
{
	// 現在の所持アイテムと状況
	[SerializeField] List<ItemList> m_itemList = new List<ItemList>();

	// スタッシュに保存されているアイテムと情報
	[SerializeField] List<ItemList> m_stashItemList = new List<ItemList>();

	// シーンをまたぐ時に持ち越すアイテム一覧
	public void SetItemList(List<ItemList> list)
	{
		// 一旦中身をリセット
		m_itemList.Clear();
		m_itemList = new List<ItemList>(list);
	}

	// 保持しているリストを返す
	public List<ItemList> GetItemList()
	{
		return m_itemList;
	}

	// スタッシュ用のアイテムリストを返す
	public List<ItemList> GetStashItemList()
	{
		return m_stashItemList;
	}
}
