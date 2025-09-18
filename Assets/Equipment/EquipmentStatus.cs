using UnityEngine;

public class EquipmentStatus : MonoBehaviour
{
    public enum EquipmentType
    {
        Weapon,
        Helmet,
        Armor,
        Gauntlet,
        Shoes,
    }

    [SerializeField] EquipmentType m_equipmentType;     //装備の種類
    [SerializeField] StatusData m_status;               //基礎ステータス
}
