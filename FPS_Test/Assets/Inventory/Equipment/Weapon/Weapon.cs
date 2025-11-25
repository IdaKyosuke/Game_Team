using UnityEngine;

public enum AttackType
{
    Physical,   //物理
    Magical,    //魔法
    Cleric,     //エネミー特攻
}

public class Weapon : EquipmentStatus
{
    [SerializeField] AttackType m_attackType;  
}