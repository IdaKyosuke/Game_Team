using UnityEngine;

public class StatusSupervision : MonoBehaviour
{
    [SerializeField] PlayerStatus m_playerStatus;
    [SerializeField] EquipmentStatus m_equipmentStatus;

    private StatusData.Parameters m_totalStatus;

    public StatusData.Parameters TotalStatus => m_totalStatus;

    private void Awake()
    {
        //プレイヤーステータスと装備ステータスの合計を計算
        m_totalStatus.hp = m_playerStatus.Value.hp + m_equipmentStatus.Value.hp;
        m_totalStatus.mp = m_playerStatus.Value.mp + m_equipmentStatus.Value.mp;
        m_totalStatus.physicalPower = m_playerStatus.Value.physicalPower + m_equipmentStatus.Value.physicalPower;
        m_totalStatus.magicPower = m_playerStatus.Value.magicPower + m_equipmentStatus.Value.magicPower;
        m_totalStatus.physicalDefense = m_playerStatus.Value.physicalDefense + m_equipmentStatus.Value.physicalDefense;
        m_totalStatus.magicDefense = m_playerStatus.Value.magicDefense + m_equipmentStatus.Value.magicDefense;
        m_totalStatus.moveSpeed = m_playerStatus.Value.moveSpeed + m_equipmentStatus.Value.moveSpeed;
        m_totalStatus.openSpeed = m_playerStatus.Value.openSpeed + m_equipmentStatus.Value.openSpeed;
    }
}