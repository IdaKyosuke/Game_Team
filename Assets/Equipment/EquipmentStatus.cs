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

    [SerializeField] EquipmentType m_equipmentType;
    [SerializeField] EquipmentData m_statusData;
    [SerializeField] StatusData m_status;
    [SerializeField] int m_level;

    private StatusData.Parameters m_parameters;
    private int m_attackSpeed;  

    public StatusData.Parameters Value => m_parameters;

    private void Awake()
    {
        //装備のパッシブステータスをランダムに決定
        int id = Random.Range(0, 3);

        //装備のパッシブステータスを設定
        m_parameters = m_status.GetStatus(m_level);

        m_parameters.hp += m_statusData.EquipmentAbility[id].hp;
        m_parameters.mp += m_statusData.EquipmentAbility[id].mp;
        
        m_parameters.physicalPower += m_statusData.EquipmentAbility[id].physicalPower;
        m_parameters.magicPower += m_statusData.EquipmentAbility[id].magicPower;
        
        m_parameters.physicalDefense += m_statusData.EquipmentAbility[id].physicalDefense;
        m_parameters.magicDefense += m_statusData.EquipmentAbility[id].magicDefense;
        
        m_parameters.moveSpeed += m_statusData.EquipmentAbility[id].moveSpeed;
        m_parameters.openSpeed += m_statusData.EquipmentAbility[id].openSpeed;

        m_attackSpeed = m_statusData.EquipmentAbility[id].attackSpeed;
    }
}