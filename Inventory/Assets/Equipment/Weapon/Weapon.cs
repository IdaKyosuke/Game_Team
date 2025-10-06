using UnityEngine;

public enum AttackType
{
    Physical,
    Magical,
}

public class Weapon : EquipmentStatus
{
    [SerializeField] AttackType m_attackType;  
}