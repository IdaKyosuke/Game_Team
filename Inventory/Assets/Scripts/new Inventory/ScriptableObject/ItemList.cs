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
	public GameObject GetPrefab()
	{
		return m_prefab;
	}

	// プレハブを設定する
	public void SetPrefab(GameObject prefab)
	{
		m_prefab = prefab;
	}
}
