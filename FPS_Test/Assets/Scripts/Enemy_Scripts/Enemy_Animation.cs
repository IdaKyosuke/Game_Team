using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Animation : MonoBehaviourPunCallbacks
{
	[SerializeField] int m_attackAnimNum;	// 攻撃アニメーションの数
	private Animator m_anim;
	private bool m_isAttack;    // 攻撃中か
	private bool m_startCoolTime;   // 攻撃後の硬直時間のカウントを開始するか
	[SerializeField] float m_coolTime;	// 攻撃後の硬直時間
	private float m_countTime;
	private Vector3 m_pastPos;  // 1フレーム前の座標
	private bool m_isDeath; // 現在の状態
	[SerializeField] GameObject m_weaponCol;	// 武器の当たり判定

    // Start is called before the first frame update
    void Start()
    {
		m_anim = GetComponent<Animator>();
		m_isAttack = false;
		m_startCoolTime = false;
		m_pastPos = transform.position;
		m_isDeath = false;
		m_weaponCol.SetActive(false);	// 最初は当たり判定を消す
	}

	private void Update()
	{
		if (m_isDeath) return;
		if (!photonView.IsMine) return;

		// 移動アニメーション
		WalkAnim();

		m_pastPos = transform.position;
	}

	private void FixedUpdate()
	{
		if (m_isDeath) return;
		if (!photonView.IsMine) return;

		// クールタイムのカウント
		if (m_startCoolTime)
		{
			m_countTime += Time.deltaTime;
			if(m_countTime >= m_coolTime)
			{
				Debug.Log("finish count");

				m_startCoolTime = false;
				m_countTime = 0;
                photonView.RPC("SetIsAttack", RpcTarget.All, false);
                // 攻撃フラグを折る
                GetComponent<Enemy_Nav>().FinishAttack();
			}
		}
	}

	[PunRPC]
	// 攻撃アニメーションを指定する
	public void AttackAnim()
	{
		// 攻撃中はモーションを再指定しない
		if (!m_isAttack)
		{
            photonView.RPC("SetIsAttack", RpcTarget.All, true);

			// 攻撃アニメーションを指定
			m_anim.SetTrigger("Attack");
		}
	}

	[PunRPC]
	void SetIsAttack(bool isAttack)
	{
		m_isAttack = isAttack;
	}

	private void WalkAnim()
	{
		// 移動中
		if(m_isAttack || m_pastPos == transform.position)
		{
			// 攻撃中
			m_anim.SetBool("walk", false);
		}
		else
		{
			m_anim.SetBool("walk", true);
		}
	}

	// 攻撃モーションの開始
	public void StartAttackAnim()
	{
		m_anim.SetBool("attack", false);
	}

	// 攻撃モーションの終了(アニメーション用)
	public void FinishAttackAnim()
	{
		// クールタイムカウントを開始する
		m_startCoolTime = true;
		// 攻撃の選択番号をリセット
		m_anim.SetBool("attack", false);
	}

	// 攻撃中か
	public bool IsAttack()
	{
		return m_isAttack;
	}

	// 死亡アニメーションの開始
	public void IsDeath()
	{
		m_anim.SetBool("death", true);
		m_isDeath = true;
	}

	// 死亡アニメーションのフラグを折る
	public void FinishDeath()
	{
		m_anim.SetBool("death", false);
    }

    // ----武器の当たり判定の管理----
    // 有効にする
    public void EnableCol()
	{
		m_weaponCol.SetActive(true);
	}
	// 無効にする
	public void DisableCol()
	{
		m_weaponCol.SetActive(false);
	}
}
