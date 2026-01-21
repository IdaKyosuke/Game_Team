using System.Collections;
using System.Net;
using UnityEngine;

public class Cleric : Job
{
    private const int ManaCost = 15;        //スキル使用時の消費MP
    private const int BuffValue = 100;      //スキル効果値
    private const int Duration = 20;        //スキル効果時間(秒)

    private const int HealAmount = 3;           //回復量
    private const float HealInterval = 5.0f;    //回復間隔
    private const int BarrierMaxValue = 300;    //バリア最大値

    private PlayerStatus m_status;
    private int m_barrierPower;     //バリア量
    private float m_elapsedTime;    //回復用タイマー
    private bool m_canHeal;         //回復が可能かどうか   
    private bool m_isBuffActive;     //バフが有効かどうか

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

    public override void Identity()
    {
        //既にバフが有効なら何もしない
        if (m_isBuffActive) return;
        m_isBuffActive = true;

        //MP不足なら何もしない
        if (m_status.Current.mp <= ManaCost) return;
        m_status.Current.mp -= ManaCost;

        //バフ付与
        StartCoroutine(BuffDuration());
    }

    private IEnumerator BuffDuration()
    {
        //MPを消費して強化
        m_status.Total.physicalDefense += BuffValue;
        m_status.Total.magicDefense += BuffValue;
        Debug.Log("一定時間防御力UP");

        //効果時間が終了するまで待機
        yield return new WaitForSeconds(Duration);

        //強化効果を解除
        m_status.Total.physicalDefense -= BuffValue;
        m_status.Total.magicDefense -= BuffValue;
        m_isBuffActive = false;
        Debug.Log("防御力UPの効果が切れた");
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
        m_barrierPower = BarrierMaxValue;
    }

    public override void Attack()
    {
        //攻撃コライダー有効化
        m_weapon.StartAttack();
    }

    public override void AttackEnd()
    {
        //攻撃コライダー無効化
        m_weapon.EndAttack();
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