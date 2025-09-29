using System.Collections;
using UnityEngine;

public enum StatusType
{
    Id,
    Hp,
    Mp,
    PhysicalPower,
    MagicPower,
    PhysicalDefense,
    MagicDefense,
    MoveSpeed,
    AttackSpeed,
    OpenSpeed,

    Length,
}

[CreateAssetMenu(fileName = "New Equipment", menuName = "ScriptableObject/EquipmentData")]
public class Equipment : ScriptableObject
{
    public int id;
    public int hp;
    public int mp;
    public int physicalPower;
    public int magicPower;
    public int physicalDefense;
    public int magicDefense;
    public int moveSpeed;
    public int openSpeed;
    public int attackSpeed;

    public int GetInfo(StatusType type)
    {
        switch (type)
        { 
            case StatusType.Id:
                return m_id;

            case StatusType.Hp: 
                return m_hp;

            case StatusType.Mp: 
                return m_mp;

            case StatusType.PhysicalPower: 
                return m_physicalPower;

            case StatusType.MagicPower: 
                return m_magicPower;

            case StatusType.PhysicalDefense: 
                return m_physicalDefense;

            case StatusType.MagicDefense: 
                return m_magicDefense;

            case StatusType.MoveSpeed: 
                return m_moveSpeed;

            case StatusType.AttackSpeed: 
                return m_attackSpeed;

            case StatusType.OpenSpeed: 
                return m_openSpeed;

            default: return 0;
        }  
    }
}
