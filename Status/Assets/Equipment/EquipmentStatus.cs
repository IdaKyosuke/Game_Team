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
    [SerializeField] EquipmentParameter m_statusData;   //装備の基礎ステータス
    [SerializeField] EquipmentData m_passiveSkillData;  //装備のパッシブスキル
    [SerializeField] int m_level;

    private EquipmentParameter m_totalStatus;           //装備の総合ステータス

    public EquipmentParameter TotalStatus => m_totalStatus;

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
        m_totalStatus.hp = m_statusData.hp + m_passiveSkillData.EquipmentAbility[id].hp;
        m_totalStatus.mp = m_statusData.mp + m_passiveSkillData.EquipmentAbility[id].mp;
        m_totalStatus.physicalPower = m_statusData.physicalPower + m_passiveSkillData.EquipmentAbility[id].physicalPower;
        m_totalStatus.magicPower = m_statusData.magicPower + m_passiveSkillData.EquipmentAbility[id].magicPower;
        m_totalStatus.physicalDefense = m_statusData.physicalDefense + m_passiveSkillData.EquipmentAbility[id].physicalDefense;
        m_totalStatus.magicDefense = m_statusData.magicDefense + m_passiveSkillData.EquipmentAbility[id].magicDefense;
        m_totalStatus.moveSpeed = m_statusData.moveSpeed + m_passiveSkillData.EquipmentAbility[id].moveSpeed;
        m_totalStatus.openSpeed = m_statusData.openSpeed + m_passiveSkillData.EquipmentAbility[id].openSpeed;
        m_totalStatus.attackSpeed = m_statusData.attackSpeed + m_passiveSkillData.EquipmentAbility[id].attackSpeed;
    }
}