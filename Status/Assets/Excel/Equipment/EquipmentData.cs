using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
[CreateAssetMenu(menuName = "ScriptableObject/EquipmentData")]
public class EquipmentData : ScriptableObject
{
    public List<EquipmentParameter> EquipmentAbility;
}