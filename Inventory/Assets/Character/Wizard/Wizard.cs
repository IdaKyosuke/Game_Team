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
    }

    private void Update()
    {
        //MP‚ªÅ‘å’l‚È‚çˆ—‚µ‚È‚¢
        if (m_status.MP >= m_status.TotalStatus.mp) return;
        
        //MPŽ©“®‰ñ•œ
        m_elapsedTime += Time.deltaTime;
        if (m_elapsedTime >= Interval)
        {
            m_status.MP += 2;
            m_elapsedTime = 0;
        }
    }

    protected override void Passive1()
    { 
        m_condition.Grant = ConditionType.Frost;
    }

    protected override void Passive2()
    {
        m_condition.Grant = ConditionType.Burn;
    }

    protected override void Passive3()
    {
        m_condition.Grant = ConditionType.Shock;
    }
}