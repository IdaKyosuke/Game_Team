using UnityEngine;

public class Warrior : Job
{
    private PlayerStatus m_status;
    private PlayerParameter m_passiveStatus;
    private bool m_isOneLife = false;   //一度だけHP1で耐えるかどうか
    private bool m_isDamageCut = false; //ダメージカットが発動しているかどうか

    public bool IsDamageCut => m_isDamageCut;

    public bool IsOneLife
    { 
        get { return m_isOneLife; }
        set { m_isOneLife = value; }
    }

    private void Start()
    {
        //ステータスの取得
        m_status = GetComponent<PlayerStatus>();

        //パッシブスキルの初期化
        Initialize(JobType.Warrior, AttackType.Physical);
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

    public override void Identity()
    {
        //固有アクションなし
    }

    protected override void Passive1()
    {
        //全ステータス強化
        m_passiveStatus = new PlayerParameter(0)
        {
            hp = 200,
            mp = 0,
            physicalPower = 150,
            magicPower = 0,
            physicalDefense = 70,
            magicDefense = 70,
            moveSpeed = 5,
            openSpeed = 5
        };

        //ステータス反映
        m_status.PassiveStatus(m_passiveStatus);
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