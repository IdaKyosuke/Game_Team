using UnityEngine;
using UnityEngine.Rendering;

public class EquipmentStatus : MonoBehaviour
{
    [SerializeField] EquipmentParameter m_statusData;   //装備の基礎ステータス(ScriptableObject)
    [SerializeField] EquipmentData m_passiveSkillData;  //装備のパッシブスキル

    private EquipmentParameter m_totalStatus;           //装備の総合ステータス

    public EquipmentParameter TotalStatus => m_totalStatus;

    public void SetPassve()
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
        //m_totalStatus += m_passiveSkillData.EquipmentAbility[id];
    }
}