using UnityEngine;

public class Weapon : EquipmentStatus
{
    public enum AttackType
    {
        Physical,
        Magical,
    }

    [SerializeField] AttackType m_attackType;  
}