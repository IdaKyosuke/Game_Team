using TMPro;
using UnityEngine;

public class Wizard : Job
{
    private const float Interval = 3;
    private const int RecoveryMP = 1;
    private const int UseMP = 5;

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
        if (m_status.CurrentMP >= m_status.Total.mp) return;
        
        //MP自動回復
        m_elapsedTime += Time.deltaTime;
        if (m_elapsedTime >= Interval)
        {
            m_status.CurrentMP += RecoveryMP;
            m_elapsedTime = 0;
        }
    }

    public override void Attack()
    {
        //魔法攻撃
        if (m_attackType == AttackType.Magical)
        {
            //MP不足なら攻撃しない
            if (m_status.CurrentMP <= UseMP) return;

            //弾の生成
            GameObject magic = Instantiate(m_magicBall, transform.position + transform.forward * 1.5f + m_offset, Quaternion.identity);
            magic.GetComponent<MagicAttack>().Init(transform.forward);
            magic.GetComponent<MagicAttack>().Parent = gameObject;

            //MP消費
            m_status.CurrentMP -= UseMP;

            return;
        }

        //物理攻撃
        m_weapon.GetComponent<Collider>().enabled = true;
    }

    public override void AttackEnd()
    {
        //物理攻撃の場合は攻撃コライダー無効化
        m_weapon.GetComponent<Collider>().enabled = false;
    }

    public override void Identity()
    {
        //攻撃方法の切り替え
        m_attackType = m_attackType == AttackType.Magical ? AttackType.Physical : AttackType.Magical;
    }

    protected override void Passive1()
    {
        m_condition.Grant = ConditionType.Burn;
        Debug.Log("状態異常付与 : 火傷");
    }

    protected override void Passive2()
    {
        m_condition.Grant = ConditionType.Frost;
        Debug.Log("状態異常付与 : 凍傷");
    }

    protected override void Passive3()
    {
        m_condition.Grant = ConditionType.Shock;
        Debug.Log("状態異常付与 : 感電");
    }
}