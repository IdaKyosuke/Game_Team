using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SendMapData : MonoBehaviour
{
	[SerializeField] List<Transform> m_stairsPos;		// 階段
	[SerializeField] List<Transform> m_enemyPos;		// 敵
	[SerializeField] List<Transform> m_treasurePos;     // 宝箱
	[SerializeField] List<Transform> m_spawnPos;		// プレイヤー生成

    void Update()
    {
        
    }

	public List<Transform> SendStairsPos()
	{
		return m_stairsPos;
	}
}
