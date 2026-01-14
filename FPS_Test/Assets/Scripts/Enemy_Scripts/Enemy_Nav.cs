using Cysharp.Threading.Tasks.Triggers;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Nav : MonoBehaviourPunCallbacks
{
	// NavMesh関連
	private NavMeshAgent m_agent;
	[SerializeField] GameObject m_player;
	[SerializeField] Transform m_target;

	// 通常の移動用
	private CharacterController m_charaCon;
	private Vector3 m_moveDir;
	[SerializeField] float m_maxMoveTime;   // 移動時間の最大値
	[SerializeField] float m_minMoveTime;   // 移動時間の最小値
	private float m_moveTime;
	private float m_countTime;
	private bool m_selected;    // 行動が選択されたか
	private bool m_isAttack;    // 攻撃が選択されたか
	private bool m_isDeath;     // 死亡したか
	private bool m_isHit;   // 攻撃がプレイヤーに当たったか
	private bool m_pastHit; // 今の攻撃でプレイヤーの体力を減らしたか（当たり判定を1回に抑える用）

	[SerializeField] float m_moveSpeed;

	private bool m_combat;  // 戦闘モードか
	private bool m_isGetHit;    // 攻撃を受けたか

	// 攻撃可能か判断する用のコライダー
	[SerializeField] GameObject m_checkAttackCol;

	// 自身のステータス
	private int m_hp = 100;
	private int m_atk;
	private int m_exp;

	// 攻撃を受けたときの血しぶき
	[SerializeField] GameObject m_blood;

	// 動きを止めるための判定をするコライダー
	[SerializeField] ForStop_Collider m_collider;

	// Start is called before the first frame update
	void Start()
	{
		m_agent = GetComponent<NavMeshAgent>();
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
		m_exp = 300;
	}

	// Update is called once per frame
	void Update()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		// 死亡したら行動しない
		if (m_isDeath) return;

		// 攻撃が命中した
		if (!m_pastHit && m_isHit)
		{
			m_pastHit = true;
		}

		if (m_combat)
		{
			Combat();
		}
		else
		{
			Wandering();
		}

		if (!m_isAttack && m_checkAttackCol.GetComponent<Collider_EnemyAttack>().CanAttack())
		{
			// 攻撃アニメーションを指定
			m_isAttack = true;
			GetComponent<Enemy_Animation>().AttackAnim();
		}

		Debug.Log("EnemyNav m_attack : " + m_isAttack);
	}

	// 徘徊モード
	private void Wandering()
	{
		if (!m_selected)
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
		if (!m_collider.GetCheckFlg())
		{
			// プレイヤーに向かって移動
			m_agent.SetDestination(m_target.position);
			m_agent.isStopped = false;
		}
		else
		{
			m_agent.isStopped = true;
			Debug.Log("stop");
		}
	}

	// 徘徊モード中、移動方向と時間を決める
	private void SelectMove()
	{
		// 移動方向
		int x = Random.Range(-1, 2);
		int z = Random.Range(-1, 2);
		m_moveDir = new Vector3(x, 0, z);

		// 移動時間
		m_moveTime = Random.Range(m_minMoveTime, m_maxMoveTime);
		m_selected = true;
	}

	// 実際に移動する
	private void Move()
	{
		m_countTime += Time.deltaTime;

		if (m_countTime >= m_moveTime)
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
        //プレイヤ－情報の取得
        PlayerStatus playerStatus;
        Job playerJob;
        Weapon_Collider weapon;

        // プレイヤーの物理攻撃を受けた
        if (!m_isDeath && !m_isGetHit && other.gameObject.CompareTag("weapon_player"))
        {
            playerStatus = other.transform.root.GetComponent<PlayerStatus>();
            playerJob = other.transform.root.GetComponent<Job>();
            weapon = other.GetComponent<Weapon_Collider>();
            Damage(playerStatus, playerJob, weapon, playerJob.AttackType);
        }

        // プレイヤーの魔法攻撃を受けた
        if (!m_isDeath && !m_isGetHit && other.gameObject.CompareTag("weapon_magic"))
        {
            playerStatus = other.GetComponent<MagicAttack>().Parent.GetComponent<PlayerStatus>();
            playerJob = other.GetComponent<MagicAttack>().Parent.GetComponent<Job>();
            weapon = other.GetComponent<Weapon_Collider>();
            Damage(playerStatus, playerJob, weapon, playerJob.AttackType);
        }
    }

    private void Damage(PlayerStatus playerStatus, Job playerJob, Weapon_Collider weapon, AttackType attackType)
    {
        //ダメ―ジ計算
        float damage = 0;
        switch (attackType)
        {
            case AttackType.Physical:
                damage += playerStatus.Total.physicalPower;
                break;

            case AttackType.Magical:
                damage += playerStatus.Total.magicPower;
                break;

            case AttackType.Cleric:
                damage += playerStatus.Total.physicalPower * 1.2f;
                break;
        }

        //プレイヤーが魔法使いかつ物理攻撃の時はMPを2回復させる
        if (playerJob.JobType == JobType.Wizard)
        {
            if (weapon.AttackType == AttackType.Physical)
            {
                playerStatus.MagicHeal(2);
            }
        }

        //被弾処理
        m_hp -= (int)damage;

        // 攻撃を受けたフラグを立てる
        m_isGetHit = true;

        //死亡確認
        if (m_hp <= 0)
        {
            // 死亡状態にする
            m_isDeath = true;

            // 死亡アニメーション
            GetComponent<Enemy_Animation>().IsDeath();

            // 自分の当たり判定を無くす
            GetComponent<CapsuleCollider>().enabled = false;

            // プレイヤーに経験値を加算する
            playerStatus.AddExp(m_exp);

            //プレイヤーの職業が僧侶か魔法使いの場合はMPを5回復させる
            if (playerJob.JobType == JobType.Cleric
            || playerJob.JobType == JobType.Wizard)
            {
                playerStatus.MagicHeal(5);
            }
        }
    }

    // プレイヤーを発見してモードが変わる
    public void InCombat(GameObject player)
	{
		m_combat = true;
		// 移動をnavmeshに任せる
		m_charaCon.enabled = false;

		// 見つけたプレイヤーを追いかける
		m_player = player;

		if (m_target)
		{
			if (Vector3.Distance(transform.position, m_target.transform.position) > Vector3.Distance(transform.position, player.transform.position))
			{
				// PlayerModel_TPS側のHipsが引っかかる
				m_target = m_player.transform;
			}
		}
		else
		{
			m_target = m_player.transform;
		}

		Debug.Log("start");
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
