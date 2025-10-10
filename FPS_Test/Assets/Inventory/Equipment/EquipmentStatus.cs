using UnityEngine;
using static Condition;

public class EquipmentStatus : MonoBehaviour
{
    [SerializeField] EquipmentParameter m_statusData;   //装備の基礎ステータス(ScriptableObject)
    [SerializeField] EquipmentData m_passiveSkillData;  //装備のパッシブスキル
    [SerializeField] Condition m_condition;             

    private Item_Object m_itemObject;                   //アイテムデータ
    private EquipmentParameter m_totalStatus;           //装備の総合ステータス

    public EquipmentParameter TotalStatus => m_totalStatus;

    private void Start()
    {
        m_itemObject = GetComponent<Item_Object>();

        //ランダムでパッシブスキルを設定
        int id = Random.Range(0, 2);
        if (id == 1)
        {
            id = Random.Range(0, m_passiveSkillData.EquipmentAbility.Count);
            m_statusData.id = id;
        }
		/*
        //武器なら状態異常付与のスキルを取得
        if (m_itemObject.GetWeaponType() == EquipmentType.Weapon)
        {
            m_condition.Grant = (ConditionType)m_passiveSkillData.EquipmentAbility[id].condition;
        }
		*/
        //装備の総合ステータスを計算
        m_totalStatus = m_statusData;
        m_totalStatus += m_passiveSkillData.EquipmentAbility[id];
    }
}