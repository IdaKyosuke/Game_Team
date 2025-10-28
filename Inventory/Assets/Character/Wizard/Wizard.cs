using System;
using UnityEngine;

public class Wizard : Job
{
    private const float Interval = 3;

    private PlayerStatus m_status;
    private Condition m_condition;
    private float m_elapsedTime;

    private void Start()
    {
        m_status = GetComponent<PlayerStatus>();
        m_condition = GetComponent<Condition>();
        m_elapsedTime = 0;

        //パッシブスキルの初期化
        Initialize(JobType.Wizard);
    }

    private void Update()
    {
        //MPが最大値なら処理しない
        if (m_status.MP >= m_status.TotalStatus.mp) return;
        
        //MP自動回復
        m_elapsedTime += Time.deltaTime;
        if (m_elapsedTime >= Interval)
        {
            m_status.MP += 2;
            m_elapsedTime = 0;
        }
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