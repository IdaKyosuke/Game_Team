using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEnemyEquipments : MonoBehaviour
{
	// 装備枠のリスト
	[SerializeField] List<GameObject> m_equipments = new List<GameObject>();

	public void QuickEquip(GameObject item)
	{
		foreach (GameObject slot in m_equipments)
		{
			// 候補のタイプと一致する枠を見つけたら中を確認 => 空いていたら装備
			if (slot.GetComponent<GridIcon_Equipment>().GetEquipmentType() == item.GetComponent<Item_Object>().GetWeaponType())
			{
				// 装備枠を確認
				slot.GetComponent<GridIcon_Equipment>().QuickEquip(item);
			}
		}
	}
}
