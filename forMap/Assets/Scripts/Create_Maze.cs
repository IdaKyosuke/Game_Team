using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Create_Maze : MonoBehaviour
{
	enum Direction
	{
		Up,
		Down,
		Left,
		Right,

		Length,
	}
	
	[SerializeField] GameObject m_stairsMap;
	[SerializeField] GameObject m_treasure;
	[SerializeField] int m_frameSize = 7;
	[SerializeField] int m_mapHeight = 3;
	private List<List<bool>> m_map = new List<List<bool>>();

	[SerializeField] List<GameObject> m_mapPrefab;

	[SerializeField] GameObject m_wallOutSide;

	private int m_size = 42;

	// Start is called before the first frame update
	void Start()
    {
		/*
		// 探索用マップを作成
		for(int i = 0; i < m_frameSize; i++)
		{
			m_map.Add(new List<bool>());

			for(int j = 0; j < m_frameSize; j++)
			{
				m_map[i].Add(false);
			}
		}

		// 偶数マスを選択（スタートする座標）
		int x = 0 + Random.Range(0, m_frameSize / 2) * 2;
		int y = 0 + Random.Range(0, m_frameSize / 2) * 2;

		// マップ作成
		MakeMap(x, y);

		// マップ配置
		SetPrefab();
		*/
		SetMap();
	}

	private void MakeMap(int x, int y)
	{
		// 道をtrueにする
		m_map[x][y] = true;

		// 右隣りが同じ色？
		if (x + 2 < m_frameSize && !m_map[x + 2][y])
		{
			m_map[x + 1][y] = true;
			MakeMap(x + 2, y);
		}
		// 左隣りが同じ色？
		if (x - 2 >= 0 && !m_map[x - 2][y])
		{
			m_map[x - 1][y] = true;
			MakeMap(x - 2, y);
		}
		// 下隣りが同じ色？
		if (y + 2 < m_frameSize && !m_map[x][y + 2])
		{
			m_map[x][y + 1] = true;
			MakeMap(x, y + 2);
		}
		// 上隣りが同じ色？
		if (y - 2 >= 0 && !m_map[x][y - 2])
		{
			m_map[x][y - 1] = true;
			MakeMap(x, y - 2);
		}
	}

	private void SetPrefab()
	{
		for(int i = 0; i < m_frameSize; i++)
		{
			for (int j = 0; j < m_frameSize; j++)
			{
				if (m_map[i][j])
				{
					Instantiate(m_mapPrefab[Random.Range(0, m_mapPrefab.Count)], new Vector3(m_size * i, 0, m_size * j), Quaternion.identity);
				}
			}
		}
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
								mapdatas.Add(Instantiate(m_stairsMap, new Vector3(m_size * i, y * 4.5f, m_size * j), Quaternion.identity).GetComponent<SendMapData>());
							}
							continue;
						}
					}
					mapdatas.Add(Instantiate(m_mapPrefab[Random.Range(0, m_mapPrefab.Count)], new Vector3(m_size * i, y * 4.5f, m_size * j), Quaternion.identity).GetComponent<SendMapData>());
				}
			}
		}

		Instantiate(m_wallOutSide);

		SetPlayerTreasure(mapdatas);
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

		for (int i = 0; i < 20; ++i)
		{
			// プレイヤーは一度無視する
			int index = Random.Range(0, spawnPos.Count);
			Instantiate(m_treasure, spawnPos[index].position, spawnPos[index].rotation);
			spawnPos.RemoveAt(index);
		}
	}

	/*
	private void CreateStairs(List<SendMapData> mapDatas)
	{
		int oneFloorSize = (int)Mathf.Pow(m_frameSize, 2);
		// 階層ごと
		for (int y = 0; y < m_mapHeight; ++y)
		{
			// その階層の階段の位置場所を決める引数リスト
			List<int> nums = new List<int>();
			for (int i = oneFloorSize * y; i < oneFloorSize * (y + 1); ++i)
			{
				nums.Add(i);
			}

			for (int i = 0; i < 3; ++i)
			{
				int index = nums[Random.Range(0, nums.Count)];
				Transform stairTransform = mapDatas[index].SendStairsPos()[0];
				Instantiate(m_stairs, stairTransform.position, stairTransform.rotation);
				nums.Remove(index);
			}

			for (int i = 0; i < nums.Count; ++i)
			{
				Transform ceilingTransform = mapDatas[nums[i]].SendStairsPos()[0];
				Instantiate(m_ceiling, ceilingTransform.position, ceilingTransform.rotation);
			}
		}
	}
	*/
}
