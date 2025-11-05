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

public abstract class Job : MonoBehaviour
{
    [SerializeField, Range(0, 2)] int m_skillIndex;

    private Action[] actions;

    protected JobType m_jobType;

    public void Initialize(JobType jobType)
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
    }

    //各ジョブ固有のパッシブスキルは派生先で実装する
    protected abstract void Passive1();
                                      
    protected abstract void Passive2();
                                      
    protected abstract void Passive3();
}