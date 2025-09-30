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
    [SerializeField] EquipmentParameter m_statusData;   //装備の基礎ステータス
    [SerializeField] EquipmentData m_passiveSkillData;  //装備のパッシブスキル

    private EquipmentParameter m_totalStatus;           //装備の総合ステータス

    public EquipmentParameter TotalStatus => m_totalStatus;

    private void Awake()
    {
        //ランダムでパッシブスキルを設定
        //int id = Random.Range(0, 2);
        //if (id == 1)
        //{
        //    id = Random.Range(0, m_passiveSkillData.EquipmentAbility.Count);
        //    m_statusData.id = id;
        //}

        //装備の総合ステータスを計算
        m_totalStatus = m_statusData;

        Debug.Log($"<color=cyan>Equip:{m_equipmentType}</color>\n" +
            $"HP:{m_totalStatus.hp} MP:{m_totalStatus.mp}\n" +
            $"物理攻撃力:{m_totalStatus.physicalPower} 魔法攻撃力:{m_totalStatus.magicPower}\n" +
            $"物理防御力:{m_totalStatus.physicalDefense} 魔法防御力:{m_totalStatus.magicDefense}\n" +
            $"移動速度:{m_totalStatus.moveSpeed} 開錠速度:{m_totalStatus.openSpeed} 攻撃速度:{m_totalStatus.attackSpeed}");


        //m_totalStatus += m_passiveSkillData.EquipmentAbility[id];
    }
}