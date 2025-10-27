
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Manager : MonoBehaviour
{
	[SerializeField] List<GameObject> m_enemyPrefabList = new List<GameObject>();   // 敵の生成候補リスト
	[SerializeField] Transform m_enemySpawnPosParent;	// 生成ポイントの親
	private List<Transform> m_spawnPosList = new List<Transform>(); // 生成ポイントのリスト
	[SerializeField] int m_enemyNum;    // 1度に生成される敵の数
	[SerializeField] float m_enemySpawnCoolTime;	// 新たに敵が生成されるまでの時間
	[SerializeField] List<GameObject> m_spawnedEnemyList = new List<GameObject>();	// 生成されている敵のリスト

	// Start is called before the first frame update
	void Start()
    {
		// 生成ポイントを取得
		foreach(Transform pos in m_enemySpawnPosParent)
		{
			m_spawnPosList.Add(pos);
		}

		// 最初の敵を生成
		SpawnEnemy();
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	private void SpawnEnemy()
	{
		// 最初にリストをコピー
		List<Transform> m_posList = new List<Transform>(m_spawnPosList);

		for (int i = 0; i < m_enemyNum; i++)
		{
			// 生成する敵を選択
			int enemy = UnityEngine.Random.Range(0, m_enemyPrefabList.Count);
			// 生成する場所を選択
			int pos = UnityEngine.Random.Range(0, m_posList.Count);

			// 生成された敵をリストで管理
			m_spawnedEnemyList.Add(Instantiate(m_enemyPrefabList[enemy], m_posList[pos].position, Quaternion.identity));

			// 1度生成した場所はリストから除外
			m_posList.Remove(m_posList[pos]);
		}
	}

	// プレイヤーの攻撃終了時に、敵のヒットフラグを折る
	public void ResetHitFlg()
	{
		foreach(GameObject enemy in m_spawnedEnemyList)
		{
			enemy.GetComponent<Enemy_Nav>().ResetHitFlg();
		}
	}

	// 階層が変わるたびに敵を削除する
	public void RemoveEnemy()
	{
		foreach(var enemy in m_spawnedEnemyList)
		{
			Destroy(enemy);
		}
	}
}
