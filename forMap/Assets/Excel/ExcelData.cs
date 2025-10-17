using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class ExcelData : ScriptableObject
{
	public List<ObjectEntity> common;
	public List<ObjectEntity> rare;
	public List<ObjectEntity> unique;
	public List<ObjectEntity> legendary;
	public List<TreasureBoxEntity> treasureBox;
}
