using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private List<GameObject> m_equipments;

    private void Start()
    {
        //プレイヤーの装備枠を取得
        m_equipments = transform.root.GetComponent<PlayerStatus>().Equipments;
    }

    public void QuickEquip(GameObject item, bool firstSetItemFlg = false)
    {
        foreach (GameObject slot in m_equipments)
        {
            // 候補のタイプと一致する枠を見つけたら中を確認 => 空いていたら装備
            if (slot.GetComponent<GridIcon_Equipment>().GetEquipmentType() == item.GetComponent<Item_Object>().GetWeaponType())
            {
                // 装備枠を確認
                slot.GetComponent<GridIcon_Equipment>().QuickEquip(item, !firstSetItemFlg);
            }
        }
    }

    //装備枠をリストに追加する
    public void SetSlot(GameObject slot)
    {
        m_equipments.Add(slot);
    }
}
