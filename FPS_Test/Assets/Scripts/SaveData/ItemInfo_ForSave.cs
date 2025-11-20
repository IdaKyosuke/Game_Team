using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemInfo_ForSave
{
	// マス目の座標（左上）
	public Vector2Int m_gridIndex;
	// アイテムの情報のID
	public int m_id;
	// 確率
	public float m_attack;
	// プレハブ
	[SerializeField] GameObject m_prefab;
	public string m_prefabName;
	// 装備されているか
	public bool m_isEquip;
}
