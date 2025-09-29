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
                return id;

            case StatusType.Hp: 
                return hp;

            case StatusType.Mp: 
                return mp;

            case StatusType.PhysicalPower: 
                return physicalPower;

            case StatusType.MagicPower: 
                return magicPower;

            case StatusType.PhysicalDefense: 
                return physicalDefense;

            case StatusType.MagicDefense: 
                return magicDefense;

            case StatusType.MoveSpeed: 
                return moveSpeed;

            case StatusType.AttackSpeed: 
                return attackSpeed;

            case StatusType.OpenSpeed: 
                return openSpeed;

            default: return 0;
        }  
    }
}
