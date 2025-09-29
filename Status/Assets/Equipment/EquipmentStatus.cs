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

        Length,
    }

    [SerializeField] EquipmentType m_equipmentType;     //装備の種類
    [SerializeField] Equipment m_statusData;            //装備の基礎ステータス
    [SerializeField] EquipmentData m_passiveSkillData;  //装備のパッシブスキル
    [SerializeField] int m_level;

    private Equipment m_totalStatus;                    //装備の総合ステータス

    public Equipment TotalStatus => m_totalStatus;

    private void Awake()
    {
        //ランダムでパッシブスキルを設定
        int id = Random.Range(0, 2);
        if (id == 1)
        {
            id = Random.Range(0, m_passiveSkillData.EquipmentAbility.Count);
            m_statusData.id = id;
        }

        //装備の総合ステータスを計算
        m_totalStatus = m_statusData;
    }
}