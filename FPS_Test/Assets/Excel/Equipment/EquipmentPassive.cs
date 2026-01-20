using System;

[Serializable]
public class EquipmentPassive
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

    public int condition;   //èÛë‘àŸèÌ


    public static EquipmentParameter operator +(EquipmentParameter a, EquipmentPassive b)
    {
        EquipmentParameter result = new EquipmentParameter();
        
        result.id = b.id;
        result.hp = a.hp + b.hp;
        result.mp = a.mp + b.mp;
        result.physicalPower = a.physicalPower + b.physicalPower;
        result.magicPower = a.magicPower + b.magicPower;
        result.physicalDefense = a.physicalDefense + b.physicalDefense;
        result.magicDefense = a.magicDefense + b.magicDefense;
        result.moveSpeed =  a.moveSpeed + b.moveSpeed;
        result.openSpeed = a.openSpeed + b.openSpeed;
        return result;
    }
}
