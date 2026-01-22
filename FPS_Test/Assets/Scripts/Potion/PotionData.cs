using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObject/Info PotionData")]
public class PotionData : ScriptableObject
{
    public int m_value; // Œø‰Ê’l
    public int m_count; // Œø‰Ê‰ñ”
    public float m_duration; // Œø‰ÊŠÔ
    public float m_interval; // Œø‰ÊŠÔŠu
    public string m_description; // à–¾•¶
}