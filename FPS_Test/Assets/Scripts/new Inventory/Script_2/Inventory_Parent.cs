using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Parent : MonoBehaviour
{
    [SerializeField] GameObject m_content;
    [SerializeField] GameObject m_equipments;
	[SerializeField] StashManager stashManager;

    public GameObject GetContent => m_content;
    public GameObject GetEquipments => m_equipments;

	public void SetStashManager(StashManager manager)
	{
		stashManager = manager;
	}

	public StashManager GetStashManager() { return stashManager; }
}
