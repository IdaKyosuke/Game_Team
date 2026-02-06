using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

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
	private int m_hp = 1000;
	private int m_atk;
	private int m_exp;

	// 攻撃を受けたときの血しぶき
	[SerializeField] GameObject m_blood;

	// 動きを止めるための判定をするコライダー
	[SerializeField] ForStop_Collider m_collider;

	// プレイヤーに近づいた時にその場で回転する速度
	[SerializeField] float m_rotSpeed = 3.0f;

	private bool m_isReady = false;

	public bool IsDeath => m_isDeath;

    // Start is called before the first frame update
    async void Start()
	{
        await UniTask.WaitUntil(() => FlgMan.Instance != null);
        // マップ生成が終わるまで待つ
        await UniTask.WaitUntil(() => FlgMan.Instance.IsCreatedMaze);

        m_agent = GetComponent<NavMeshAgent>();
		m_charaCon = GetComponent<CharacterController>();

		// 徘徊モード用にCharaconをアクティブ、Navmeshを非アクティブ
		m_charaCon.enabled = true;
		m_agent.enabled = false;

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
		m_isReady = true;
	}

	// Update is called once per frame
	void Update()
	{
		if (!m_isReady) return;
		if (!photonView.IsMine) return;

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
			Debug.Log("attack");
			photonView.RPC("AttackAnim", RpcTarget.All);
		}
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
		// navmeshが非アクティブの時
		if(!m_agent.enabled)
		{
			m_agent.enabled = true;
		}

		if (!m_collider.GetCheckFlg())
		{
			// プレイヤーに向かって移動
			m_agent.SetDestination(m_target.position);
			m_agent.isStopped = false;
		}
		else
		{
			m_agent.isStopped = true;

			Vector3 direction = m_target.position - transform.position;
			Quaternion rotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * m_rotSpeed);
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

	//  private void OnTriggerEnter(Collider other)
	//  {
	//      //プレイヤ－情報の取得
	//      PlayerStatus playerStatus;
	//      Job playerJob;
	//      Weapon_Collider weapon;

	//      // プレイヤーの物理攻撃を受けた
	//      if (!m_isDeath && !m_isGetHit && other.gameObject.CompareTag("weapon_player"))
	//      {
	//          playerStatus = other.transform.root.GetComponent<PlayerStatus>();
	//          playerJob = other.transform.root.GetComponent<Job>();
	//          weapon = other.GetComponent<Weapon_Collider>();
	//          Damage(playerStatus, playerJob, weapon, playerJob.AttackType);
	//      }

	//      // プレイヤーの魔法攻撃を受けた
	//      if (!m_isDeath && !m_isGetHit && other.gameObject.CompareTag("weapon_magic"))
	//      {
	//          playerStatus = other.GetComponent<MagicAttack>().Parent.GetComponent<PlayerStatus>();
	//          playerJob = other.GetComponent<MagicAttack>().Parent.GetComponent<Job>();
	//          weapon = other.GetComponent<Weapon_Collider>();
	//          Damage(playerStatus, playerJob, weapon, playerJob.AttackType);
	//      }
	//  }

	public void Damage(PlayerStatus playerStatus, Job playerJob, Weapon_Collider weapon)
	{
		//ダメ―ジ計算
		float damage = 0;
		switch (playerJob.AttackType)
		{
			case AttackType.Physical:
				damage += playerStatus.Total.physicalPower;
				break;

			case AttackType.Magical:
				damage += playerStatus.Total.magicPower;
				break;

			case AttackType.Cleric:
                //僧侶の攻撃は物理攻撃力の8倍ダメージ
                damage += (playerStatus.Total.physicalPower * 8);
				break;
		}

		//プレイヤーが魔法使いかつ物理攻撃の時はMPを2回復させる
		if (playerJob.JobType == JobType.Wizard)
		{
			if (playerJob.AttackType == AttackType.Physical)
			{
				playerStatus.MagicHeal(2);
			}
		}

		//被弾処理
		m_hp -= (int)damage;

		Debug.Log("エネミーに [ " + damage + " ] ダメージを与えた");

		// 攻撃を受けたフラグを立てる
		m_isGetHit = true;

		//死亡確認
		if (m_hp <= 0)
		{
			// 死亡状態にする
			photonView.RPC(nameof(SetDeath), RpcTarget.All, true);

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
		if (!m_isReady) return;
		if (!m_combat)
		{
			m_combat = true;
			// 移動をnavmeshに任せる
			m_charaCon.enabled = false;

			// 見つけたプレイヤーを追いかける
			m_player = player;

			Debug.Log("m_player[" + m_player.gameObject.name + "]");
		}

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
			Debug.Log("set m_targer");
		}
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

	// 追跡目標を見失った後に徘徊モードに戻す
	[PunRPC]
	public void ReWondering()
	{
		// 移動方法を切り替える
		if(!m_charaCon.enabled)
		{
			m_agent.enabled = false;
			m_charaCon.enabled = true;
		}

		// 戦闘用の情報をリセット
		m_combat = false;
		m_player = null;
	}

	[PunRPC]
	void SetDeath(bool isDeath)
	{
		m_isDeath = isDeath;
		GetComponent<Enemy_Animation>().IsDeath();
	}
}
