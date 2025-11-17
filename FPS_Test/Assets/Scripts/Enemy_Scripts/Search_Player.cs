using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵がプレイヤーを発見するためのコライダー
public class Search_Player : MonoBehaviour
{
	[SerializeField] GameObject m_enemy;    // コライダーの親オブジェクト
	[SerializeField] float m_rayLength;	// レイの長さ
	private Vector3 m_rayDir;   // レイを飛ばす方向

	// 判定しないレイヤーマスク(EnemyBody)
	private int m_layerMask = 1 << 30 | 1 << 11;

	private void OnTriggerStay(Collider other)
	{
		if(other.gameObject.CompareTag("playerModel"))
		{
			if(CheckRay(other.gameObject))
			{
				// プレイヤーを認識
				m_enemy.GetComponent<Enemy_Nav>().InCombat(other.gameObject);
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
