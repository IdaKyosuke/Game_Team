using System.Net;
using UnityEngine;

public class Cleric : Job
{
    [SerializeField] Weapon_Collider m_attackCollier;

    private const int HealAmount = 5;           //回復量
    private const float HealInterval = 5.0f;    //回復間隔

    private PlayerStatus m_status;
    private int m_barrierPower;     //バリア量
    private float m_elapsedTime;    //回復用タイマー
    private bool m_canHeal;         //回復が可能かどうか   

    public int Barrier => m_barrierPower;

    private void Start()
    {
        //ステータスの取得
        m_status = GetComponent<PlayerStatus>();
        m_barrierPower = 0;

        //パッシブスキルの初期化
        Initialize(JobType.Cleric, AttackType.Physical);
    }

    private void Update()
    {
        //HP自動回復処理
        if (m_canHeal)
        {
            m_elapsedTime += Time.deltaTime;
            if (m_elapsedTime >= HealInterval)
            {
                Debug.Log("Cleric [ HP自動回復 ]");
                m_status.Heal(HealAmount);
                m_elapsedTime = 0f;
            }
        }
    }

    protected override void Identity()
    {
        //固有アクション
    }

    protected override void Passive1()
    {
        //エネミー特攻強化
        m_attackType = AttackType.Cleric;
    }

    protected override void Passive2()
    {
        //HP自動回復
        m_canHeal = true;
    }

    protected override void Passive3()
    {
        //バリア付与
        m_barrierPower = 300;
    }

    public int GetBarrierDamage(int damage)
    {
        if (m_barrierPower >= damage)
        {
            Debug.Log("Cleric [バリアで攻撃を無効化]");

            m_barrierPower -= damage;
            return 0;
        }
        else
        {
            int remainingDamage = damage - m_barrierPower;
            m_barrierPower = 0;
            Debug.Log("Cleric [バリアが破壊され、残りのダメージを受ける]");
            return remainingDamage;
        }
    }
}