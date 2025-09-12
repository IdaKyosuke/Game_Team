using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chest Items", menuName = "ScriptableObject/Chest ItemList")]
public class Chest_ItemList : ScriptableObject
{
	// どの箱用のテーブルか
	public new string name = "New Name";

	public List<GameObject> ItemList = new List<GameObject>();
}
