using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SendMapData : MonoBehaviour
{
	[SerializeField] List<Transform> m_enemyPortalPos;		// “G
	[SerializeField] List<Transform> m_playerTreasurePos;	// •ó” 

	public List<Transform> GetEnemyPortalPos()
	{
		return m_enemyPortalPos;
	}

	public List<Transform> GetSpawnPos()
	{
		return m_playerTreasurePos;
	}
}
