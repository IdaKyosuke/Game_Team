using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class ExcelData : ScriptableObject
{
	public List<MapObjectEntity> common;
	public List<MapObjectEntity> rare;
	public List<MapObjectEntity> unique;
	public List<MapObjectEntity> legendary;
	public List<TreasureBoxEntity> treasureBox;
	public List<JobTextEntity> jobText;
}
