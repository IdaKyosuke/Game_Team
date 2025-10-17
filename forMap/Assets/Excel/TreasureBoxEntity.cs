using System.Numerics;
using UnityEngine.Playables;
using UnityEngine.Rendering;

[System.Serializable]
public class TreasureBoxEntity
{
	public int id;                  // ID
	public string rarity;			// 表示名
	public int width;               // 横幅
	public int height;              // 縦幅
	public int itemMin;				// アイテムの抽選会数の最低値
	public int itemMax;				// アイテムの抽選会数の最大値
	public int common;			// commonの数
	public int rare;				// rareの数
	public int unique;            // uniqueの数
	public int legendary;			// legendaryの数
}