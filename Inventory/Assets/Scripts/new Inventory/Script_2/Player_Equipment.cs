using System.Collections.Generic;
using UnityEngine;

public class Player_Equipment : MonoBehaviour
{
	// 装備枠のリスト
	[SerializeField] List<GameObject> m_equipments = new List<GameObject>();

    // プレイヤーのステータス
    [SerializeField] StatusData m_status;

	// 実数値のリスト
	private EquipmentParameter m_weaponStatus;

    // Start is called before the first frame update
    void Start()
    {
		// 数値をリセット
		m_weaponStatus = new EquipmentParameter();
    }

    // Update is called once per frame
    void Update()
    {
		// 数値をリセット
		m_weaponStatus = new EquipmentParameter();

        // 装備枠分回す
        foreach (GameObject slot in m_equipments)
		{
			// 装備枠が空の場合0を加算していく
			if (slot.transform.childCount == 0) continue;

            // 装備のステータスを加算
            Item_Object info = slot.transform.GetChild(0).GetComponent<Item_Object>();
			m_weaponStatus += info.GetEquipmentInfo();
        }

        // デバッグ表示
        Debug.Log(
            $"id:{m_weaponStatus.id}," +
			$"hp:{m_weaponStatus.hp}, " +
			$"mp:{m_weaponStatus.mp}, " +
            $"physicalPower:{m_weaponStatus.physicalPower}, " +
			$"magicPower:{m_weaponStatus.magicPower}, " +
            $"physicalDefense:{m_weaponStatus.physicalDefense}," +
			$"magicDefense:{m_weaponStatus.magicDefense}, " +
            $"attackSpeed:{m_weaponStatus.attackSpeed}, " +
			$"moveSpeed:{m_weaponStatus.moveSpeed}," +
			$"openSpeed:{m_weaponStatus.openSpeed}"
        );
    }

    // 装備枠を確認 => 空いていたら装備
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

	//装備枠をリストに追加する
	public void SetSlot(GameObject slot)
	{
		m_equipments.Add(slot);
	}
}
