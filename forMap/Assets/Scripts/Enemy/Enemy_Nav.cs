using UnityEngine;
using UnityEngine.AI;

public class Enemy_Nav : MonoBehaviour
{
	// NavMesh関連
	private NavMeshAgent m_agent;
	[SerializeField] GameObject m_player;
	[SerializeField] Transform m_target;

	// 通常の移動用
	private CharacterController m_charaCon;
	private Vector3 m_moveDir;
	[SerializeField] float m_maxMoveTime;	// 移動時間の最大値
	[SerializeField] float m_minMoveTime;	// 移動時間の最小値
	private float m_moveTime;
	private float m_countTime;
	private bool m_selected;    // 行動が選択されたか
	private bool m_isAttack;    // 攻撃が選択されたか
	private bool m_isDeath;     // 死亡したか
	private bool m_isHit;   // 攻撃がプレイヤーに当たったか
	private bool m_pastHit;	// 今の攻撃でプレイヤーの体力を減らしたか（当たり判定を1回に抑える用）

	[SerializeField] float m_moveSpeed;

	private bool m_combat;  // 戦闘モードか
	private bool m_isGetHit;	// 攻撃を受けたか

	// 攻撃可能か判断する用のコライダー
	[SerializeField] GameObject m_checkAttackCol;

	// 自身のステータス
	//[SerializeField] Enemy_Data m_data;
	private int m_hp;
	private int m_atk;
	private int m_exp;

	// プレイヤーのステータス
	[SerializeField] GameObject m_playerStatus;

	// 攻撃を受けたときの血しぶき
	[SerializeField] GameObject m_blood;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
		if(!m_player)
		{
			m_player = GameObject.FindWithTag("Player");
		}
		if(!m_playerStatus)
		{
			m_playerStatus = GameObject.FindWithTag("playerStatus");
		}
		m_target = m_player.transform;
		m_charaCon = GetComponent<CharacterController>();
		m_moveDir = Vector3.zero;
		m_combat = false;
		m_moveTime = 0;
		m_countTime = 0;
		m_selected = false;
		m_isAttack = false;
		m_isDeath = false;
		m_isGetHit = false;
		m_isHit = false;
		m_pastHit = m_isHit;
		//m_hp = m_data.hp;
		//m_atk = m_data.attack;
		//m_exp = m_data.exp;
	}

	// Update is called once per frame
	void Update()
    {
		// 死亡したら行動しない
		if(m_isDeath) return;

		// 攻撃が命中した
		if(!m_pastHit && m_isHit)
		{
			// プレイヤーの体力を減らして、攻撃を当てたフラグを立てる
			//m_playerStatus.GetComponent<Player_DungeonStatus>().GetHit(m_atk);
			m_pastHit = true;
			Debug.Log("hit");
		}

		// 攻撃中は移動しない
		if (m_isAttack) return;

		if (!m_isAttack && m_checkAttackCol.GetComponent<Collider_EnemyAttack>().CanAttack())
		{
			m_isAttack = true;
			// 攻撃アニメーションを指定
			GetComponent<Enemy_Animation>().AttackAnim();
		}

		if(m_combat)
		{
			Combat();
		}
		else
		{
			//Wandering();
		}
	}

	// 徘徊モード
	private void Wandering()
	{
		if(!m_selected)
		{
			SelectMove();
		}
		else
		{
			Move();
		}
	}

	// 戦闘モード
	private void Combat()
	{
		// プレイヤーに向かって移動
		m_agent.SetDestination(m_target.position);
	}

	// 徘徊モード中、移動方向と時間を決める
	private void SelectMove()
	{
		// 移動方向
		int x = UnityEngine.Random.Range(-1, 2);
		int z = UnityEngine.Random.Range(-1, 2);
		m_moveDir = new Vector3(x, 0, z);

		// 移動時間
		m_moveTime = UnityEngine.Random.Range(m_minMoveTime, m_maxMoveTime);
		m_selected = true;
	}

	// 実際に移動する
	private void Move()
	{
		m_countTime += Time.deltaTime;

		if(m_countTime >= m_moveTime)
		{
			// 移動時間を超えた
			m_selected = false;
			m_countTime = 0;
		}
		else
		{
			m_charaCon.Move(m_moveDir * m_moveSpeed * Time.deltaTime);
			if (m_moveDir != Vector3.zero)
			{
				transform.rotation = Quaternion.LookRotation(m_moveDir);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(!m_isDeath && !m_isGetHit && other.gameObject.CompareTag("weapon_player"))
		{
			// プレイヤーの武器で攻撃されたらダメージを受ける
			//m_hp -= m_playerStatus.GetComponent<Player_DungeonStatus>().GetAtk();
			// 出血エフェクトを出す
			Instantiate(m_blood, other.ClosestPointOnBounds(this.transform.position), Quaternion.identity);
			// 攻撃を受けたフラグを立てる
			m_isGetHit = true;
			if(m_hp <= 0)
			{
				// 死亡状態にする
				m_isDeath = true;
				// 死亡アニメーション
				GetComponent<Enemy_Animation>().IsDeath();
				// 自分の当たり判定を無くす
				GetComponent<CapsuleCollider>().enabled = false;
				// プレイヤーに経験値を加算する
				//m_playerStatus.GetComponent<Player_DungeonStatus>().AddExp(m_exp);
			}
		}
	}

	// プレイヤーを発見してモードが変わる
	public void InCombat()
	{
		m_combat = true;
		// 移動をnavmeshに任せる
		m_charaCon.enabled = false;
	}

	// 今のモードを取得
	public bool IsCombat()
	{
		return m_combat;
	}

	// 攻撃が終了した
	public void FinishAttack()
	{
		m_isAttack = false;
		// 攻撃に関するフラグを折る
		m_pastHit = false;
		m_isHit = false;
	}

	// 攻撃ヒットフラグを折る
	public void ResetHitFlg()
	{
		m_isGetHit = false;
	}

	// プレイヤーに攻撃が命中した
	public void GiveHit()
	{
		m_isHit = true;
	}
}
