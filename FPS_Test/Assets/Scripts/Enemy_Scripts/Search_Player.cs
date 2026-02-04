using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵がプレイヤーを発見するためのコライダー
public class Search_Player : MonoBehaviourPunCallbacks
{
	[SerializeField] GameObject m_enemy;    // コライダーの親オブジェクト
	[SerializeField] float m_rayLength;	// レイの長さ
	private Vector3 m_rayDir;   // レイを飛ばす方向

	// 判定しないレイヤーマスク(EnemyBody)
	private int m_layerMask = 1 << 30 | 1 << 11;

	// ターゲットを追跡中か
	private bool m_isCombat = false;
	// ターゲットを見失っている時間
	private float m_countTime = 0;
	// ターゲットをリセットするまでの時間
	[SerializeField] float m_resetTime = 3.0f;
	// 追跡しているターゲット
	private GameObject m_target = null;

	private void FixedUpdate()
	{
		if (!photonView.IsMine) return;
		if(m_isCombat)
		{
			m_countTime += Time.deltaTime;
			if(m_countTime >= m_resetTime)
			{
				m_countTime = 0;
				m_isCombat = false;
				// プレイヤーが敵の感知範囲の外に出た時追跡をやめる
				photonView.RPC("ReWondering", RpcTarget.All);
				m_target = null;
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if(other.gameObject.CompareTag("playerModel"))
		{
			if(CheckRay(other.gameObject))
			{
				// プレイヤーを認識
				m_enemy.GetComponent<Enemy_Nav>().InCombat(other.gameObject);
				m_isCombat = true;
				if(m_target == other.gameObject)
				{
					// 追跡していたターゲットを再発見した時
					m_countTime = 0;
				}
				else
				{	
					// 新しいターゲットを見つけた時、ターゲットを保存
					m_target = other.gameObject;
				}
			}
		}
	}

	// 実際にレイを飛ばす
	private bool CheckRay(GameObject player)
	{
		// 自分の親とプレイヤーのベクトル
		m_rayDir = player.transform.position - transform.root.position;
        // 自分の親からプレイヤーに対してレイを作成
        Ray ray = new Ray(transform.root.position + new Vector3(0, 0.5f, 0), m_rayDir);

		Debug.DrawRay(ray.origin, m_rayDir, Color.red, m_rayLength);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, m_rayLength, ~m_layerMask))
		{
			//Debug.Log("Rayのゲームオブジェクト[ " + hit.collider.gameObject + " ]");
			if (hit.collider.gameObject.CompareTag("playerModel"))
			{
				// 敵とプレイヤーの間に何も障害物が無い場合
				return true;
			}
		}
		return false;
	}
}
