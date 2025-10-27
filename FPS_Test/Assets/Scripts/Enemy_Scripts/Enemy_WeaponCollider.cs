using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵の攻撃判定コライダーにつける
public class Enemy_WeaponCollider : MonoBehaviour
{
	[SerializeField] GameObject m_enemy;    // 武器の持ち主

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			m_enemy.GetComponent<Enemy_Nav>().GiveHit();
		}
	}
}
