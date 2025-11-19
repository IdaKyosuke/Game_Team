using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObject/Info EquipmentData")]
public class EquipmentParameter : ScriptableObject
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

    public static EquipmentParameter operator+ (EquipmentParameter a, EquipmentParameter b)
    {
        EquipmentParameter result = ScriptableObject.CreateInstance<EquipmentParameter>();
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

    public static EquipmentParameter operator- (EquipmentParameter a, EquipmentParameter b)
	{
		EquipmentParameter result = ScriptableObject.CreateInstance<EquipmentParameter>();
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

    static public PlayerParameter operator+ (PlayerParameter a, EquipmentParameter b)
    {
        PlayerParameter result = new PlayerParameter(a.Level);
        result.hp = a.hp + b.hp;
        result.mp = a.mp + b.mp;
        result.physicalPower = a.physicalPower + b.physicalPower;
        result.magicPower = a.magicPower + b.magicPower;
        result.physicalDefense = a.physicalDefense + b.physicalDefense;
        result.magicDefense = a.magicDefense + b.magicDefense;
        result.attackSpeed = a.attackSpeed + b.attackSpeed;
        result.moveSpeed = a.moveSpeed + b.moveSpeed;
        result.openSpeed = a.openSpeed + b.openSpeed;
        result.requiredExp = a.requiredExp; 
        return result;
    }

    public static PlayerParameter operator- (PlayerParameter a, EquipmentParameter b)
    {
        PlayerParameter result = new PlayerParameter(a.Level);
        result.hp = a.hp - b.hp;
        result.mp = a.mp - b.mp;
        result.physicalPower = a.physicalPower - b.physicalPower;
        result.magicPower = a.magicPower - b.magicPower;
        result.physicalDefense = a.physicalDefense - b.physicalDefense;
        result.magicDefense = a.magicDefense - b.magicDefense;
        result.attackSpeed = a.attackSpeed - b.attackSpeed;
        result.moveSpeed = a.moveSpeed - b.moveSpeed;
        result.openSpeed = a.openSpeed - b.openSpeed;
        result.requiredExp = a.requiredExp; 
        return result;
    }
}
