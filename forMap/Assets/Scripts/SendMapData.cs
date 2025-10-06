using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SendMapData : MonoBehaviour
{
	[SerializeField] List<Transform> m_enemyPos;			// “G
	[SerializeField] List<Transform> m_playerTreasurePos;	// •ó” 

	public List<Transform> GetEnemyPos()
	{
		return m_enemyPos;
	}

	public List<Transform> GetSpawnPos()
	{
		return m_playerTreasurePos;
	}
}
