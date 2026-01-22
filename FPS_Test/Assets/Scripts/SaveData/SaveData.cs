using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{
	private static SaveData m_instance = null;
	[SerializeField] SaveData m_manager;

	public static SaveData Instance
	{
		get
		{
			if (m_instance == null)
			{
				// データをロードする
				Load();
			}
			return m_instance;
		}
	}

	// SaveData(TestData)をJsonに変換したテキスト(リロード時に何度も読み込まないように保持)
	[SerializeField] private static string m_jsonText = "";

	// ---- 保存されるデータ("public" か "SerializeField" をつける) ----
	public List<ItemInfo_ForSave> m_inventoryItem = new List<ItemInfo_ForSave>();	// インベントリ内のアイテム
	public List<ItemInfo_ForSave> m_stashItem = new List<ItemInfo_ForSave>();		// スタッシュ内のアイテム

	// ---- データを再読み込みする ----
	// インベントリ用
	public List<ItemList> ReloadInventory()
	{
		JsonUtility.FromJsonOverwrite(GetJson(), this);

		List <ItemList> lists = new List<ItemList>();

		foreach (var data in m_inventoryItem)
		{
			lists.Add(LoadData(data));
		}

		return lists;
	}
	// スタッシュ用
	public List<ItemList> ReloadStash()
	{
		JsonUtility.FromJsonOverwrite(GetJson(), this);

		List <ItemList> lists = new List<ItemList>();

		foreach (var data in m_stashItem)
		{
			lists.Add(LoadData(data));
		}

		return lists;
	}

	// データを読み込む
	private static void Load()
	{
		m_instance = JsonUtility.FromJson<SaveData>(GetJson());
	}

	// ---- 保存しているJsonを取得 ----
	private static string GetJson()
	{
		// すでにJsonを取得している時はそれを返す
		if (!string.IsNullOrEmpty(m_jsonText))
		{
			return m_jsonText;
		}

		// Jsonを保存している場所のパスを取得
		string path = GetFilePath();

		// Jsonが存在しているかを調べてから取得して変換(存在しない場合は新たなクラスを作成してJsonに変換)
		if (File.Exists(path))
		{
			m_jsonText = File.ReadAllText(path);
		}
		else
		{
			m_jsonText = JsonUtility.ToJson(new SaveData());
		}

		return m_jsonText;
	}

	// ---- データをJsonにして保存 ----
	// インベントリ用
	public void SaveInventory(List<ItemList> list)
	{
		m_inventoryItem.Clear();
		foreach (var item in list)
		{
			ItemInfo_ForSave save = new ItemInfo_ForSave();
			CreateSaveData(ref save, item);
			m_inventoryItem.Add(save);
		}

		m_jsonText = JsonUtility.ToJson(this);
		File.WriteAllText(GetFilePath(), m_jsonText);
	}
	// スタッシュ用
	public void SaveStash(List<ItemList> list)
	{
		m_stashItem.Clear();
		foreach (var item in list)
		{
			ItemInfo_ForSave save = new ItemInfo_ForSave();
			CreateSaveData(ref save, item);
			m_stashItem.Add(save);
		}

		m_jsonText = JsonUtility.ToJson(this);
		File.WriteAllText(GetFilePath(), m_jsonText);
	}

	// アイテムに関するセーブデータを作成する
	private ItemInfo_ForSave CreateSaveData(ref ItemInfo_ForSave save, ItemList item)
	{
		save.m_gridIndex = item.GetGridIndex();
		save.m_id = item.m_id;
		save.m_attack = item.m_attack;
		save.m_itemData = item.ItemData;
		save.m_isEquip = item.IsEquip();

		return save;
	}

	// アイテムのデータをロードする
	private ItemList LoadData(ItemInfo_ForSave data)
	{
		ItemList item = ScriptableObject.CreateInstance<ItemList>();
		item.SetGridIndex(data.m_gridIndex);
		item.m_id = data.m_id;
		item.m_attack = data.m_attack;
		item.ItemData = data.m_itemData;
		item.SetEquipInfo(data.m_isEquip);

		return item;
	}

	// ---- データを全て削除し、初期化 ----
	public void DeleteInventory()
	{
		m_inventoryItem.Clear();
		m_jsonText = JsonUtility.ToJson(new SaveData());
		File.WriteAllText(GetFilePath(), m_jsonText);
	}
	public void DeleteStash()
	{
		m_stashItem.Clear();
		m_jsonText = JsonUtility.ToJson(new SaveData());
		File.WriteAllText(GetFilePath(), m_jsonText);
	}

	// ---- 保存先のパスを取得 ----
	private static string GetFilePath()
	{
		string path = "SaveData_Item";

		// エディタ上ではAssetsと同じ階層
#if UNITY_EDITOR
		path += ".json";
#else
		path = Application.persistentDataPath + "/" + path;
#endif
		return path;
	}
}
