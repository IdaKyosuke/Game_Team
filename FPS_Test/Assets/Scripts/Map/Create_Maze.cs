using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Create_Maze : MonoBehaviourPunCallbacks
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
	[SerializeField] int m_enemyAmount = 0;

	[SerializeField] List<GameObject> m_mapPrefab;

	[SerializeField] GameObject m_wallOutSide;

	private int m_size = 42;

	private static List<Transform> m_playerSpawnPosList = new List<Transform>();

	[SerializeField] GameObject m_mapParent;			// 生成したマップのプレハブを入れる

	// Start is called before the first frame update
	void Awake()
    {
		if (PhotonNetwork.IsMasterClient) SetMap();
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
									PhotonNetwork.Instantiate(
										m_stairsMap.name, 
										new Vector3(m_size * i, y * 4.5f, m_size * j), 
										Quaternion.identity).GetComponent<SendMapData>()
										);
							}
							continue;
						}
					}
					GameObject map = PhotonNetwork.Instantiate(
						m_mapPrefab[Random.Range(0, m_mapPrefab.Count)].name, 
						new Vector3(m_size * i, y * 4.5f, m_size * j), 
						Quaternion.identity
						);
					// マップのレイヤーを分ける(仮置きだからマジックナンバー)
					photonView.RPC(nameof(RequestChangeLayer), RpcTarget.AllBuffered, y + 6, map.GetComponent<PhotonView>().ViewID);
					mapdatas.Add(map.GetComponent<SendMapData>());
				}
			}
		}

		foreach (SendMapData map in mapdatas)
		{
			map.ReqestSetParent();
		}

		PhotonNetwork.Instantiate(m_wallOutSide.name, transform.position, Quaternion.identity);

		SetPlayerTreasure(mapdatas);

		// 動的にnavMeshをbakeする
		m_mapParent.GetComponent<NavMeshSurface>().BuildNavMesh();

		SetEnemyReturn(mapdatas);
	}

	[PunRPC]
	void RequestChangeLayer(int layerNum, int viewId)
	{
		Transform map = PhotonView.Find(viewId).transform;
		GameObjectExtensions.SetLayerRecursively(map, layerNum);
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
			spawnPos.Remove(spawnPos[index]);
		}

		for (int i = 0; i < m_treasureAmount; ++i)
		{
			int treasureType =
				i < 20 ? 0 :
				i < 35 ? 1 :
				i < 45 ? 2 : 3;

			// プレイヤーは一度無視する
			int index = Random.Range(0, spawnPos.Count);
			PhotonNetwork.Instantiate(m_treasure[treasureType].name, 
				spawnPos[index].position,
				spawnPos[index].rotation);
			spawnPos.Remove(spawnPos[index]);
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
			PhotonNetwork.Instantiate(m_portal.name, spawnPos[index].position, spawnPos[index].rotation);
			spawnPos.RemoveAt(index);
		}

		for (int i = 0; i < m_enemyAmount; ++i)
		{
			int index = Random.Range(0, spawnPos.Count);
			PhotonNetwork.Instantiate(m_enemy.name, spawnPos[index].position, spawnPos[index].rotation);
			spawnPos.RemoveAt(index);
		}
	}

	public static Transform GetPlayerSpawnPos()
	{
		Debug.Log(m_playerSpawnPosList.Count);
		Transform pos = m_playerSpawnPosList[0];
		m_playerSpawnPosList.Remove(m_playerSpawnPosList[0]);
		return pos;
	}
}
