using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Create_Maze : MonoBehaviour
{
	[SerializeField] GameObject m_stairsMap;
	[SerializeField] GameObject m_enemy;
	[SerializeField] GameObject m_portal;
	[SerializeField] GameObject[] m_treasure = new GameObject[4];
	[SerializeField] int m_frameSize = 7;
	[SerializeField] int m_mapHeight = 3;

	[SerializeField] int m_playerSpawnPosAmount = 20;
	[SerializeField] int m_treasureAmount = 50;
	[SerializeField] int m_portalPosAmount = 5;
	[SerializeField] int m_enemyAmount = 200;

	[SerializeField] List<GameObject> m_mapPrefab;

	[SerializeField] GameObject m_wallOutSide;

	private int m_size = 42;

	private static List<Transform> m_playerSpawnPosList = new List<Transform>();

	[SerializeField] GameObject m_mapParent;			// 生成したマップのプレハブを入れる
	[SerializeField] Transform m_enemyParent;			// 生成した敵を入れる
	[SerializeField] Transform m_treasureBoxParent;     // 生成した宝箱を入れる
	[SerializeField] Transform m_portalParent;			// 生成した帰還用ポータルを入れる

	// Start is called before the first frame update
	void Start()
    {
		SetMap();
	}

	private void SetMap()
	{
		bool xCorner = false;
		List<SendMapData> mapdatas = new List<SendMapData>();
		for (int y  = 0; y < m_mapHeight; ++y)
		{
			for (int i = 0; i < m_frameSize; i++)
			{
				xCorner = i == 0 || i == m_frameSize - 1;
				for (int j = 0; j < m_frameSize; j++)
				{
					if (xCorner)
					{
						if (j == 0 || j == m_frameSize - 1)
						{
							if (y == 0)
							{
								mapdatas.Add(
									Instantiate(
										m_stairsMap, 
										new Vector3(m_size * i, y * 4.5f, m_size * j), 
										Quaternion.identity,
										m_mapParent.transform).GetComponent<SendMapData>()
										);
							}
							continue;
						}
					}
					GameObject map = Instantiate(
						m_mapPrefab[Random.Range(0, m_mapPrefab.Count)], 
						new Vector3(m_size * i, y * 4.5f, m_size * j), 
						Quaternion.identity, 
						m_mapParent.transform
						);
					// マップのレイヤーを分ける(仮置きだからマジックナンバー)
					GameObjectExtensions.SetLayerRecursively(map.transform, y + 6);
					mapdatas.Add(map.GetComponent<SendMapData>());
				}
			}
		}

		Instantiate(m_wallOutSide);

		SetPlayerTreasure(mapdatas);

		// 動的にnavMeshをbakeする
		m_mapParent.GetComponent<NavMeshSurface>().BuildNavMesh();

		SetEnemyReturn(mapdatas);
	}

	private void SetPlayerTreasure(List<SendMapData> mapData)
	{
		List<Transform> spawnPos = new List<Transform>();
		foreach (SendMapData data in mapData)
		{
			foreach (Transform t in data.GetSpawnPos())
			{
				// 宝箱とプレイヤーのスポーンポジションを全部入れる
				spawnPos.Add(t);
			}
		}

		for (int i = 0; i < m_playerSpawnPosAmount; ++i)
		{
			int index = Random.Range(0, spawnPos.Count);
			m_playerSpawnPosList.Add(spawnPos[index]);
			spawnPos.RemoveAt(index);
		}

		for (int i = 0; i < m_treasureAmount; ++i)
		{
			int treasureType =
				i < 20 ? 0 :
				i < 35 ? 1 :
				i < 45 ? 2 : 3;

			// プレイヤーは一度無視する
			int index = Random.Range(0, spawnPos.Count);
			Instantiate(m_treasure[treasureType], 
				spawnPos[index].position,
				spawnPos[index].rotation, 
				m_treasureBoxParent);
			spawnPos.RemoveAt(index);
		}
	}

	private void SetEnemyReturn(List<SendMapData> mapData)
	{
		List<Transform> spawnPos = new List<Transform>();
		foreach (SendMapData data in mapData)
		{
			foreach (Transform t in data.GetEnemyPortalPos())
			{
				// 敵と帰還場所のスポーンポジションを全部入れる
				spawnPos.Add(t);
			}
		}

		for (int i = 0; i < m_portalPosAmount; ++i)
		{
			int index = Random.Range(0, spawnPos.Count);
			Instantiate(m_portal, spawnPos[index].position, spawnPos[index].rotation, m_portalParent);
			spawnPos.RemoveAt(index);
		}

		for (int i = 0; i < m_enemyAmount; ++i)
		{
			int index = Random.Range(0, spawnPos.Count);
			Instantiate(m_enemy, spawnPos[index].position, spawnPos[index].rotation, m_enemyParent);
			spawnPos.RemoveAt(index);
		}
	}

	public static Transform GetPlayerSpawnPos()
	{
		Transform pos = m_playerSpawnPosList[0];
		m_playerSpawnPosList.RemoveAt(0);
		return pos;
	}
}
