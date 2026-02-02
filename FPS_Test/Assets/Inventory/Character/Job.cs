using System;
using UnityEngine;

public enum JobType
{
    Warrior,
    Wizard,
    Cleric,
    Thief,
	None,

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
    private int m_skillIndex;

    private Action[] actions;

    protected Weapon_Collider m_weapon;
    protected GameObject m_magicBall;
    protected JobType m_jobType;
    protected AttackType m_attackType;

    public JobType JobType => m_jobType;

    public AttackType AttackType => m_attackType;

    private void Awake()
    {
        //武器オブジェクトの取得
        m_weapon = GetComponent<PlayerController>().Weapon;
        m_magicBall = GetComponent<PlayerController>().MagicBall;

        //選択されたスキルを取得
        m_skillIndex = GameManager.Instance.PlayerPassiveSkill;
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

        //UIの更新
        GetComponent<PlayerController>().ViewUI.SetIcon(m_attackType);
    }

    //各ジョブ固有のパッシブスキルは派生先で実装する
    protected abstract void Passive1();
                                      
    protected abstract void Passive2();
                                      
    protected abstract void Passive3();

    //固有アクション
    public abstract void Identity();

    //攻撃処理
    public abstract void Attack();

    public abstract void AttackEnd();
}