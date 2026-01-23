using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObject/Info PotionData")]
public class PotionData : ScriptableObject
{
    public string potionName; // ポーション名
    public string description; // 説明文

    public int value; // 効果値
    public int count; // 効果回数
    public float duration; // 効果時間
    public float interval; // 効果間隔
}