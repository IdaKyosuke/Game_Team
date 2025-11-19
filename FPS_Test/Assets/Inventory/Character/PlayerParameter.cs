using System;
using UnityEngine;

[Serializable]
public class PlayerParameter
{
    [SerializeField] private int level;

    public int hp;
    public int mp;

    public int physicalPower;
    public int magicPower;

    public int physicalDefense;
    public int magicDefense;

    public int attackSpeed;
    public int moveSpeed;
    public int openSpeed;

    public int requiredExp;

    public int Level => level;

    public PlayerParameter(int level)
    {
        this.level = level;
    }

    static public PlayerParameter operator +(PlayerParameter a, PlayerParameter b)
    {
        PlayerParameter result = new PlayerParameter(a.level);
        result.hp = a.hp + b.hp;
        result.mp = a.mp + b.mp;
        result.physicalPower = a.physicalPower + b.physicalPower;
        result.magicPower = a.magicPower + b.magicPower;
        result.physicalDefense = a.physicalDefense + b.physicalDefense;
        result.magicDefense = a.magicDefense + b.magicDefense;
        result.attackSpeed = a.attackSpeed + b.attackSpeed;
        result.moveSpeed = a.moveSpeed + b.moveSpeed;
        result.openSpeed = a.openSpeed + b.openSpeed;
        result.requiredExp = a.requiredExp + b.requiredExp;
        return result;
    }

    static public PlayerParameter operator -(PlayerParameter a, PlayerParameter b)
    {
        PlayerParameter result = new PlayerParameter(a.level);
        result.hp = a.hp - b.hp;
        result.mp = a.mp - b.mp;
        result.physicalPower = a.physicalPower - b.physicalPower;
        result.magicPower = a.magicPower - b.magicPower;
        result.physicalDefense = a.physicalDefense - b.physicalDefense;
        result.magicDefense = a.magicDefense - b.magicDefense;
        result.attackSpeed = a.attackSpeed - b.attackSpeed;
        result.moveSpeed = a.moveSpeed - b.moveSpeed;
        result.openSpeed = a.openSpeed - b.openSpeed;
        result.requiredExp = a.requiredExp - b.requiredExp;
        return result;
    }

    public static EquipmentParameter operator +(EquipmentParameter a, PlayerParameter b)
    {
        EquipmentParameter result = new EquipmentParameter();
        result.id = a.id; 
        result.hp = a.hp + b.hp;
        result.mp = a.mp + b.mp;
        result.physicalPower = a.physicalPower + b.physicalPower;
        result.magicPower = a.magicPower + b.magicPower;
        result.physicalDefense = a.physicalDefense + b.physicalDefense;
        result.magicDefense = a.magicDefense + b.magicDefense;
        result.attackSpeed = a.attackSpeed + b.attackSpeed;
        result.moveSpeed = a.moveSpeed + b.moveSpeed;
        result.openSpeed = a.openSpeed + b.openSpeed;
        return result;
    }

    public static EquipmentParameter operator -(EquipmentParameter a, PlayerParameter b)
    {
        EquipmentParameter result = new EquipmentParameter();
        result.id = a.id; 
        result.hp = a.hp - b.hp;
        result.mp = a.mp - b.mp;
        result.physicalPower = a.physicalPower - b.physicalPower;
        result.magicPower = a.magicPower - b.magicPower;
        result.physicalDefense = a.physicalDefense - b.physicalDefense;
        result.magicDefense = a.magicDefense - b.magicDefense;
        result.attackSpeed = a.attackSpeed - b.attackSpeed;
        result.moveSpeed = a.moveSpeed - b.moveSpeed;
        result.openSpeed = a.openSpeed - b.openSpeed;
        return result;
    }
}
