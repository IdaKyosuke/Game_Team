using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObject/Info ItemSize")]
public class Info_ItemSize : ScriptableObject
{
	[SerializeField] int m_width = 0;
	[SerializeField] int m_height = 0;

	public Vector2Int GetSize()
	{
		return new Vector2Int(m_width, m_height);
	}
}
