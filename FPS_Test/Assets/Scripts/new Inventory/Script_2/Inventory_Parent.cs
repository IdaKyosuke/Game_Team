using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Parent : MonoBehaviour
{
    [SerializeField] GameObject m_content;
    [SerializeField] GameObject m_equipments;
	[SerializeField] StashManager m_stashManager;

    public GameObject GetContent => m_content;
    public GameObject GetEquipments => m_equipments;

	public void SetStashManager(StashManager manager)
	{
		m_stashManager = manager;
	}

	public StashManager GetStashManager() { return m_stashManager; }

	public void SetEquipmentManager(GameObject manager)
	{
		m_equipments = manager;
	}
}
