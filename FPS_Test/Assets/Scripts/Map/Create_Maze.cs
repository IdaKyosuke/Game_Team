using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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
	private const int UniqueNum = 40;

	private const int m_playerSpawnPosAmount = 10;
	private const int m_treasureAmount = 50;
	private const int m_enemyAmount = 50;

	// 一回で出すポータルの数
	private const int m_oncePortalPosAmount = 5;

	private const int m_portalOffset = 2;

	// 帰還ポータルの生成位置
	private List<Transform> m_portalPosList;
	private float firstPortalTime = 4.0f;
	private float secondPortalTime = 10.0f;
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
		m_IsMapReady = true;
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

			// プレイヤーは一度無視する
			int index = Random.Range(0, spawnPos.Count);
			PhotonNetwork.InstantiateRoomObject(m_treasure[treasureType].name, 
				spawnPos[index].position,
				spawnPos[index].rotation);
			spawnPos.Remove(spawnPos[index]);
		}

		m_portalPosList = spawnPos;
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
		m_playerSpawnPosList.Remove(m_playerSpawnPosList[0]);
		return pos;
	}

	private void Update()
	{
		m_time += Time.deltaTime;

		// 最初のポータル出現
		if (!m_firstCreatePortal && m_time >= firstPortalTime)
		{
			CreatePortal();
			m_firstCreatePortal = true;
		}

		// 二回目のポータル出現
		if (!m_secondCreatePortal && m_time >= secondPortalTime)
		{
			CreatePortal();
			m_secondCreatePortal = true;
		}
	}

	private void CreatePortal()
	{
		// 帰還場所設定
		for (int i = 0; i < m_oncePortalPosAmount; ++i)
		{
			int index = Random.Range(0, m_portalPosList.Count);
			Vector3 pos = m_portalPosList[index].position;
			pos.y += m_portalOffset;
			PhotonNetwork.InstantiateRoomObject(m_portal.name, pos, m_portalPosList[index].rotation);
			m_portalPosList.Remove(m_portalPosList[index]);
		}
		m_portalText = PhotonNetwork.InstantiateRoomObject(m_portalTextPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);
		StartCoroutine(DestroyText());
	}

	IEnumerator DestroyText()
	{
		yield return new WaitForSeconds(2);
		PhotonNetwork.Destroy(m_portalText);
	}
}
