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

public class Job : MonoBehaviour
{
    [SerializeField] JobType m_jobType;
    [SerializeField] int m_skillIndex;

    private Action[] actions;

    private void Start()
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
    }

    virtual protected void Passive1() { Debug.Log("Passive 1"); }

    virtual protected void Passive2() { Debug.Log("Passive 2"); }

    virtual protected void Passive3() { Debug.Log("Passive 3"); }
}