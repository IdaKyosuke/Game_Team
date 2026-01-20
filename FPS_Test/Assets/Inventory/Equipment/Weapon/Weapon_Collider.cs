using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Collider : MonoBehaviourPunCallbacks
{
	[SerializeField] GameObject m_hitEffect;

	private Dictionary<int, bool> m_hitMasterInfo { get; } = new Dictionary<int, bool>();
	private Collider m_collider;
	private int m_parentID;

	private GameObject m_parent = null;

	public GameObject Parent
	{
		get { return m_parent; }
		set { m_parent = value; }
    }

    private void Start()
	{
        // 自分の当たり判定を保持
        m_collider = GetComponent<Collider>();
		if (gameObject.CompareTag("weapon_player"))
		{
			m_collider.enabled = false;
            m_parent = transform.root.gameObject;
        }

        m_parentID = m_parent.gameObject.GetInstanceID();
    }

    public void StartAttack()
	{
		// リストをリセット
		m_hitMasterInfo.Clear();
		m_collider.enabled = true;
	}

	public void EndAttack()
	{
		m_collider.enabled = false;
	}

	private void OnTriggerEnter(Collider other)
	{
        // プレイヤーに当たったとき
        if (other.gameObject.CompareTag("playerModel"))
		{
			//相手プレイヤーの親を取得
			GameObject otherPlayer = other.transform.root.gameObject;

			// 当たったオブジェクトのIDを貰ってくる
			int id = otherPlayer.GetInstanceID();

			// 自分の親と当たったときは無視する
			if (id == m_parentID) return;

			// すでに当たったオブジェクトの時は無視する
			if (m_hitMasterInfo.ContainsKey(id)) return;

			// 初めて当たったときは相手のIDを登録
			m_hitMasterInfo[id] = true;

			//死体の場合は無視する
			if (otherPlayer.GetComponent<PlayerController>().IsDeath) return;

			// 当たった場所にエフェクトを表示
			Vector3 hitPos = other.ClosestPointOnBounds(GetComponent<Collider>().bounds.center);
			Quaternion quaternion = Quaternion.identity;
			quaternion.x = hitPos.x - other.transform.position.x;
			quaternion.z = hitPos.z - other.transform.position.z;
		}

        // 敵に当たったとき
        if (other.TryGetComponent<Enemy_Nav>(out var enemyNav))
		{
            //敵オブジェクトを取得
            GameObject otherEnemy = enemyNav.gameObject;

            // 当たったオブジェクトのIDを貰ってくる
            int id = otherEnemy.GetInstanceID();

            // すでに当たったオブジェクトの時は無視する
            if (m_hitMasterInfo.ContainsKey(id)) return;

            // 初めて当たったときは相手のIDを登録
            m_hitMasterInfo[id] = true;

            //死体の場合は無視する
            if (enemyNav.IsDeath) return;

			//エネミーの被弾処理を呼び出す
			enemyNav.Damage(m_parent.GetComponent<PlayerStatus>(), m_parent.GetComponent<Job>(), this);
        }
    }
}