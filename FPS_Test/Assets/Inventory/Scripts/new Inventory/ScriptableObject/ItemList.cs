using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemList", menuName = "ScriptableObject/ItemList")]
[Serializable]
public class ItemList : ScriptableObject
{
	// マス目の座標（左上）
	private Vector2Int m_gridIndex;
	// アイテムの情報のID
	public int m_id;
	// 確率
	public float m_attack;
	// プレハブ
	[SerializeField] GameObject m_prefab;
	private string m_prefabName;
	// 装備されているか
	private bool m_isEquip;
	// インスタンス化されているオブジェクト
	private GameObject m_activeObject;


	// マス目取得
	public Vector2Int GetGridIndex()
	{
		return m_gridIndex;
	}
	// マス目変更
	public void SetGridIndex(Vector2Int index)
	{
		m_gridIndex = index;
	}

	// プレハブを取得する
	public string GetPrefabName()
	{
		return m_prefabName;
	}

	// プレハブを設定する
	public void SetPrefab(GameObject prefab)
	{
		m_prefab = prefab;
		SetPrefabName(prefab.name);
	}

	public void SetPrefabName(string name)
	{
		m_prefabName = name;
	}

	// 装備状態の変更
	public void SetEquipInfo(bool value)
	{
		m_isEquip = value;
	}
	// 装備状態の取得
	public bool IsEquip()
	{
		return m_isEquip;
	}
	// アクティブなオブジェクトを保存
	public void SetActiveObject(GameObject obj)
	{
		m_activeObject = obj;
	}
	// アクティブなオブジェクトのインデックスを変更する
	public void ChangeIndex(int index)
	{
		m_activeObject.GetComponent<Item_Object>().ChangeIndex(index);
	}
	
}
