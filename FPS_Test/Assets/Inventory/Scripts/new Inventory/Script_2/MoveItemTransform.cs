using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveItemTransform : MonoBehaviour
{
	[SerializeField] GameObject m_stashManager;
	[SerializeField] GameObject m_equipmentManager;
	[SerializeField] Camera m_camera;

	public GameObject GetStashManager()
	{
		return m_stashManager;
	}

	public GameObject GetEquipmentManager()
	{
		return m_equipmentManager;
	}

	public Camera GetCamera()
	{
		return m_camera;
	}
}
