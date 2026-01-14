using UnityEngine;

public class Warrior : Job
{
    private PlayerStatus m_status;
    private bool m_isOneLife;   //一度だけHP1で耐えるかどうか
    private bool m_isDamageCut; //ダメージカットが発動しているかどうか

    public bool IsOneLife => m_isOneLife;

    public bool IsDamageCut => m_isDamageCut;

    private void Start()
    {
        //ステータスの取得
        m_status = GetComponent<PlayerStatus>();

        //パッシブスキルの初期化
        Initialize(JobType.Warrior, AttackType.Physical);

        m_isOneLife = false;
        m_isDamageCut = false;
    }

    public override void Attack()
    {
        //攻撃コライダー有効化
        m_weapon.enabled = true;
    }

    public override void AttackEnd()
    {
        //攻撃コライダー無効化
        m_weapon.enabled = false;
    }

    public override void Identity()
    {
        //固有アクションなし
    }

    protected override void Passive1()
    {
        //全ステータス強化
        m_status.PassiveStatus += new PlayerParameter(0)
        {
            hp = 200,
            mp = 0,
            physicalPower = 200,
            magicPower = 0,
            physicalDefense = 100,
            magicDefense = 100,
            attackSpeed = 0,
            moveSpeed = 4,
            openSpeed = 4
        };
    }

    protected override void Passive2()
    {
        //一度だけHP1で耐える
        m_isOneLife = true;
    }

    protected override void Passive3()
    {
        //ダメージカット
        m_isDamageCut = true;
    }
}