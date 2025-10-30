using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Collider : MonoBehaviour
{
	private Dictionary<int, bool> m_hitMasterInfo { get; } = new Dictionary<int, bool>();
	[SerializeField] GameObject m_hitEffect;
	private BoxCollider m_boxCollider;
	[SerializeField] GameObject m_parentPlayer;     // 自分の生成IDを保持
	private int m_parentID;

	private void Start()
	{
		// 自分の当たり判定を保持
		m_boxCollider = GetComponent<BoxCollider>();
		m_boxCollider.enabled = false;

		m_parentID = m_parentPlayer.GetInstanceID();
	}

	public void StartAttack()
	{
		// リストをリセット
		m_hitMasterInfo.Clear();
		m_boxCollider.enabled = true;
	}

	public void EndAttack()
	{
		m_boxCollider.enabled = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		// プレイヤーに当たったとき
		if(other.gameObject.CompareTag("Player"))
		{
			// 当たったオブジェクトのIDを貰ってくる
			int id = other.GetComponent<PlayerBone>().Parent.GetInstanceID();

            // 自分の親と当たったときは無視する
            if (id == m_parentID) return;

			// すでに当たったオブジェクトの時は無視する
			if (m_hitMasterInfo.ContainsKey(id)) return;

			// 初めて当たったときは相手のIDを登録
			m_hitMasterInfo[id] = true;

            //死体の場合は無視する
            if (other.GetComponent<PlayerBone>().IsDeath) return;

            //ダメージを与える
            if (other.TryGetComponent<PlayerStatus>(out var playerStatus))
			{
                playerStatus.Damage(
                m_parentPlayer.GetComponent<PlayerStatus>().TotalStatus.physicalPower,
                AttackType.Physical,
                m_parentPlayer.GetComponent<Condition>());

                // 当たった場所にエフェクトを表示
                Vector3 hitPos = other.ClosestPointOnBounds(GetComponent<BoxCollider>().bounds.center);
                Quaternion quaternion = Quaternion.identity;
                quaternion.x = hitPos.x - other.transform.position.x;
                quaternion.z = hitPos.z - other.transform.position.z;
                GameObject effect = Instantiate(m_hitEffect, hitPos, quaternion);
            }
		}
	}
}
