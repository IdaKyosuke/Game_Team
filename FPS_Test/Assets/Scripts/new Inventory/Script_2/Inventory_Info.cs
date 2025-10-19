using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Info : MonoBehaviour
{
	[SerializeField] Info_InventorySize m_inventorySize;

	public Info_InventorySize GetInfo()
	{
		return m_inventorySize;
	}
}
