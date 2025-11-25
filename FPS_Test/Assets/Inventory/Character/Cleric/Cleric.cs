using System.Net;
using UnityEngine;

public class Cleric : Job
{
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