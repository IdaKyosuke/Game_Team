using UnityEngine;

public class Warrior : Job
{
    [SerializeField] Weapon_Collider m_attackCollier;
    [SerializeField] PlayerParameter m_parameter;
    private PlayerStatus m_status;

    private void Start()
    {
        //ステータスの取得
        m_status = GetComponent<PlayerStatus>();

        //パッシブスキルの初期化
        Initialize(JobType.Warrior, AttackType.Physical);
    }

    protected override void Identity()
    {
        //固有アクションなし
    }

    public override void Attack()
    {
        //攻撃処理
        Debug.Log("プレイヤーの攻撃");
        m_attackCollier.StartAttack();
    }

    public override void AttackEnd()
    {
        //攻撃終了処理
        Debug.Log("プレイヤーの攻撃終了");
        m_attackCollier.EndAttack();
    }

    protected override void Passive1()
    {
        //全ステータス強化
        m_status.PassiveStatus += m_parameter;
    }

    protected override void Passive2()
    {
        //攻撃速度UP
        m_status.PassiveStatus.attackSpeed += 3;
    }

    protected override void Passive3()
    {
        //ダメージカット
    }
}