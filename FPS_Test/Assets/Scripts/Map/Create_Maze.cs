using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Create_Maze : MonoBehaviourPunCallbacks
{
	[SerializeField] GameObject m_stairsMap;
	[SerializeField] GameObject m_enemy;
	[SerializeField] GameObject m_portal;
	[SerializeField] GameObject[] m_treasure = new GameObject[4];
	[SerializeField] int m_frameSize = 7;
	[SerializeField] int m_mapHeight = 3;

	private float m_time = 0;

	// 宝箱のそれぞれのレアリティの数
	private const int CommonNum = 20;
	private const int RareNum = 35;
	private const int UniqueNum = 45;

	private const int m_playerSpawnPosAmount = 10;
	private const int m_treasureAmount = 50;
	private const int m_enemyAmount = 0;

	// 一回で出すポータルの数
	private const int m_oncePortalPosAmount = 5;

	private const int m_portalOffset = 2;

	// 帰還ポータルの生成位置
	private List<Vector3> m_portalPosList = new List<Vector3>();
	private float firstPortalTime = 240.0f;
	private float secondPortalTime = 420.0f;
	// ポータルを生成したかどうか
	private bool m_firstCreatePortal = false;
	private bool m_secondCreatePortal = false;

	[SerializeField] GameObject m_portalTextPrefab;
	private GameObject m_portalText;

	[SerializeField] List<GameObject> m_mapPrefab;

	[SerializeField] GameObject m_wallOutSide;

	// 一区画のサイズ
	private const int m_size = 42;

	private static List<Transform> m_playerSpawnPosList = new List<Transform>();

    public static bool m_IsMapReady = false;

    public static bool IsMapReady => m_IsMapReady;

    [SerializeField] GameObject m_mapParent;			// 生成したマップのプレハブを入れる

	// Start is called before the first frame update
	void Awake()
    {
		if (PhotonNetwork.IsMasterClient) SetMap();
	}

	private void SetMap()
	{
        // マップ生成開始
        m_IsMapReady = false;

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
									PhotonNetwork.InstantiateRoomObject(
										m_stairsMap.name, 
										new Vector3(m_size * i, y * 4.5f, m_size * j), 
										Quaternion.identity).GetComponent<SendMapData>()
										);
							}
							continue;
						}
					}
					GameObject map = PhotonNetwork.InstantiateRoomObject(
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
		SetObjectSpawn(mapdatas);

		// 動的にnavMeshをbakeする
		m_mapParent.GetComponent<PhotonView>().RPC("SetBake", RpcTarget.All);

		PhotonNetwork.InstantiateRoomObject(m_wallOutSide.name, transform.position, Quaternion.identity);

		SetEnemySpawn(mapdatas);

		// マップ生成完了
		photonView.RPC(nameof(SetReady), RpcTarget.All);
    }

	[PunRPC]
	void SetReady()
	{
		m_IsMapReady=true;
	}

    [PunRPC]
	void RequestChangeLayer(int layerNum, int viewId)
	{
		Transform map = PhotonView.Find(viewId).transform;
		GameObjectExtensions.SetLayerRecursively(map, layerNum);
	}

	private void SetObjectSpawn(List<SendMapData> mapData)
	{
		List<Transform> spawnPos = new List<Transform>();
		foreach (SendMapData data in mapData)
		{
			foreach (Transform t in data.GetSpawnPos())
			{
				// 宝箱とプレイヤーと帰還場所のスポーンポジションを全部入れる
				spawnPos.Add(t);
			}
		}

		m_playerSpawnPosList.Clear();
		// プレイヤーのスポーンポジション設定
		for (int i = 0; i < m_playerSpawnPosAmount; ++i)
		{
			int index = Random.Range(0, spawnPos.Count);
			m_playerSpawnPosList.Add(spawnPos[index]);
			spawnPos.Remove(spawnPos[index]);
		}

		// 宝箱の場所設定
		for (int i = 0; i < m_treasureAmount; ++i)
		{
			int treasureType =
				i < CommonNum ? 0 :
				i < RareNum ? 1 :
				i < UniqueNum ? 2 : 3;

			int index = Random.Range(0, spawnPos.Count);
			PhotonNetwork.InstantiateRoomObject(m_treasure[treasureType].name, 
				spawnPos[index].position,
				spawnPos[index].rotation);
			spawnPos.Remove(spawnPos[index]);
		}

		foreach (Transform tr in spawnPos)
		{
			photonView.RPC(nameof(SetPortalPos), RpcTarget.All, tr.position);
		}
	}

	private void SetEnemySpawn(List<SendMapData> mapData)
	{
		List<Transform> spawnPos = new List<Transform>();
		foreach (SendMapData data in mapData)
		{
			foreach (Transform t in data.GetEnemyPortalPos())
			{
				// 敵のスポーンポジションを全部入れる
				spawnPos.Add(t);
			}
		}

		for (int i = 0; i < m_enemyAmount; ++i)
		{
			int index = Random.Range(0, spawnPos.Count);
			PhotonNetwork.InstantiateRoomObject(m_enemy.name, spawnPos[index].position, spawnPos[index].rotation);
			spawnPos.Remove(spawnPos[index]);
		}
	}

	public static Transform GetPlayerSpawnPos()
	{
		Transform pos = m_playerSpawnPosList[0];
		pos.position += pos.forward;
		m_playerSpawnPosList.Remove(m_playerSpawnPosList[0]);
		return pos;
	}

	private void Update()
	{
		m_time += Time.deltaTime;
		if (!PhotonNetwork.IsMasterClient) return;

		// 最初のポータル出現
		if (!m_firstCreatePortal && m_time >= firstPortalTime)
		{
			CreatePortal();
			photonView.RPC(nameof(SetFirstPortal), RpcTarget.All);
		}

		// 二回目のポータル出現
		if (!m_secondCreatePortal && m_time >= secondPortalTime)
		{
			CreatePortal();
			photonView.RPC(nameof(SetSecondPortal), RpcTarget.All);
		}
	}

	[PunRPC]
	void SetFirstPortal()
	{
		m_firstCreatePortal = true;
	}

	[PunRPC]
	void SetSecondPortal()
	{
		m_secondCreatePortal = true;
	}

	[PunRPC]
	void SetPortalPos(Vector3 pos)
	{
		m_portalPosList.Add(pos);
	}

	[PunRPC]
	void RemovePortalPos(Vector3 pos)
	{
		m_portalPosList.Remove(pos);
	}

	private void CreatePortal()
	{
		// 帰還場所設定
		for (int i = 0; i < m_oncePortalPosAmount; ++i)
		{
			int index = Random.Range(0, m_portalPosList.Count);
			Vector3 pos = m_portalPosList[index];
			pos.y += m_portalOffset;
			PhotonNetwork.InstantiateRoomObject(m_portal.name, pos, Quaternion.identity);
			photonView.RPC(nameof(RemovePortalPos), RpcTarget.All, m_portalPosList[index]);
		}
		m_portalText = PhotonNetwork.InstantiateRoomObject(m_portalTextPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);
	}
}
