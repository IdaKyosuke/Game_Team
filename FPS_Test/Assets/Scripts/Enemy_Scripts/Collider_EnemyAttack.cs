using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵が攻撃判定を取る用のコライダー
public class Collider_EnemyAttack : MonoBehaviour
{
	private bool m_canAttack;

    // Start is called before the first frame update
    void Start()
    {
		m_canAttack = false;
	}
	private void OnTriggerStay(Collider other)
	{
		// プレイヤーが攻撃範囲に入ったら
		if (other.gameObject.CompareTag("playerModel"))
		{
			m_canAttack = true;
			Debug.Log("attackReady");
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("playerModel"))
		{
			m_canAttack = false;
			Debug.Log("attackNotReady");
		}
	}

	public bool CanAttack()
	{
		return m_canAttack;
	}

	// 攻撃フラグを折る
	public void ResetFlag()
	{
		m_canAttack = false;
	}

}
