using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ItemValue")]
public class Info_ItemValue : ScriptableObject
{
	public int m_value;

	public int GetValue()
	{
		return m_value;
	}
}
