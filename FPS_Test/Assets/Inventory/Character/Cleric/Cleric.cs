using System.Net;
using UnityEngine;

public class Cleric : Job
{
    [SerializeField] Weapon_Collider m_attackCollier;

    private void Start()
    {
        //パッシブスキルの初期化
        Initialize(JobType.Cleric, AttackType.Cleric);
    }

    protected override void Identity()
    {
        //固有アクション
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
        
    }

    protected override void Passive2()
    {
        
    }

    protected override void Passive3()
    {

    }
}