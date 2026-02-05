using Photon.Pun;
using TMPro;
using UnityEngine;

public class Wizard : Job
{
    private const float Interval = 3;
    private const int RecoveryMP = 2;
    private const int UseMP = 15;

    private Vector3 m_offset;
    private PlayerStatus m_status;
    private Condition m_condition;
    private float m_elapsedTime;

    private void Start()
    {
        m_status = GetComponent<PlayerStatus>();
        m_condition = GetComponent<Condition>();
        m_elapsedTime = 0;
        m_offset = new Vector3(0, 0.5f, 0);

        //パッシブスキルの初期化
        Initialize(JobType.Wizard, AttackType.Magical);
    }
    
    private void FixedUpdate()
    {
        //MPが最大値なら処理しない
        if (m_status.Current.mp >= m_status.Total.mp) return;
        
        //MP自動回復
        m_elapsedTime += Time.deltaTime;
        if (m_elapsedTime >= Interval)
        {
            m_status.Current.mp += RecoveryMP;
            m_elapsedTime = 0;
        }
    }

    public override void Attack()
    {
        //魔法攻撃
        if (m_attackType == AttackType.Magical)
        {
            //MP不足なら攻撃しない
            if (m_status.Current.mp <= UseMP) return;

            //弾の生成
            GameObject magic = PhotonNetwork.Instantiate(m_magicBall.name, transform.position + transform.forward * 2 + m_offset, Quaternion.identity);
            magic.GetComponent<MagicAttack>().Init(transform.forward, gameObject);

            //MP消費
            m_status.Current.mp -= UseMP;
            return;
        }

        //物理攻撃
        m_weapon.StartAttack();
    }

    public override void AttackEnd()
    {
        //物理攻撃の場合は攻撃コライダー無効化
        m_weapon.EndAttack();
    }

    public override void Identity()
    {
        //攻撃方法の切り替え
        m_attackType = m_attackType == AttackType.Magical ? AttackType.Physical : AttackType.Magical;
    }

    protected override void Passive1()
    {
        m_condition.Grant = ConditionType.Burn;
    }

    protected override void Passive2()
    {
        m_condition.Grant = ConditionType.Frost;
    }

    protected override void Passive3()
    {
        m_condition.Grant = ConditionType.Shock;
    }
}