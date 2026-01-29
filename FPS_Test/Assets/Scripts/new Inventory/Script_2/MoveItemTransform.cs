using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveItemTransform : MonoBehaviour
{
	[SerializeField] GameObject m_stashManager;
	[SerializeField] GameObject m_equipmentManager;

	public GameObject GetStashManager()
	{
		return m_stashManager;
	}

	public GameObject GetEquipmentManager()
	{
		return m_equipmentManager;
	}
}
