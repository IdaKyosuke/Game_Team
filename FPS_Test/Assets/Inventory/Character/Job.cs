using System;
using UnityEngine;

public enum JobType
{
    Warrior,
    Wizard,
    Cleric,
    Thief,

    Length,
}

public enum AttackType
{
    Physical,   //物理
    Magical,    //魔法
    Cleric,     //エネミー特攻
}

public abstract class Job : MonoBehaviour
{
    [SerializeField, Range(0, 2)] int m_skillIndex;

    private Action[] actions;

    protected JobType m_jobType;
    protected AttackType m_attackType;

    public AttackType AttackType => m_attackType;

    private void Update()
    {
        //固有アクション
        Identity();
    }

    public void Initialize(JobType jobType, AttackType attackType)
    {
        //パッシブスキルの登録
        actions = new Action[]
        {
            Passive1,
            Passive2,
            Passive3,
        };

        //パッシブスキルの発動
        actions[m_skillIndex]?.Invoke();

        //ジョブタイプの設定
        m_jobType = jobType;
        m_attackType = attackType;
    }

    //固有アクション
    protected abstract void Identity();

    //攻撃の処理
    public abstract void Attack();

    public abstract void AttackEnd();

    //各ジョブ固有のパッシブスキルは派生先で実装する
    protected abstract void Passive1();
                                      
    protected abstract void Passive2();
                                      
    protected abstract void Passive3();
}