using System;
using System.Collections;
using UnityEngine;

public class Condition : MonoBehaviour
{
    public enum ConditionType
    {
        Burn,      //火傷
        Frost,     //凍傷
        Poison,    //猛毒
        Shock,     //感電
        Regen,     //再生
        None,      //通常
    }

    [SerializeField] ConditionData m_conditionData;

    private ConditionType m_condition;
    private PlayerStatus m_status;
    private int m_count;
    private int m_interval;
    private int m_value;

    private Action[] m_onConditions;

    public ConditionType CurrentCondition => m_condition;

    private void Awake()
    {
        m_status = GetComponent<PlayerStatus>();

        m_condition = ConditionType.None;
        m_count = 0;
        m_interval = 0;
        m_value = 0;

        //状態異常の処理を登録
        m_onConditions = new Action[(int)ConditionType.None]
        {
            () => StartCoroutine(Burn()),
            () => StartCoroutine(Frost()),
            () => StartCoroutine(Poison()),
            () => StartCoroutine(Shock()),
            () => StartCoroutine(Regen()),
        };
    }

    public void Init(ConditionType conditionType)
    {
        //状態異常のデータを取得
        m_condition = conditionType;
        m_count = m_conditionData.ConditionAbility[(int)m_condition].triggerCount;
        m_interval = m_conditionData.ConditionAbility[(int)m_condition].triggerInterval;
        m_value = m_conditionData.ConditionAbility[(int)m_condition].triggerValue;

        Debug.Log(GetComponent<PlayerStatus>().Health);

        //状態異常の処理
        m_onConditions[(int)m_condition]?.Invoke();
    }

    private IEnumerator Burn()
    {
        //残りHPダメージに対する割合ダメージ
        for (int i = 0; i < m_count; ++i)
        {
            //一定時間待機
            yield return new WaitForSeconds(m_interval);

            //割合ダメージ
            int damage = m_status.Health / m_value;
            m_status.Damage(damage);

            Debug.Log("Burn : HP = " + m_status.Health);
        }

        //状態異常を解除
        m_condition = ConditionType.None;
    }

    private IEnumerator Frost()
    {
        //移動速度低下
        m_status.Value.moveSpeed -= m_value;

        //一定時間待機
        yield return new WaitForSeconds(m_interval);

        //移動速度を元に戻す
        m_status.Value.moveSpeed += m_value;

        //状態異常を解除
        m_condition = ConditionType.None;
    }

    private IEnumerator Poison()
    {
        //一定時間ごとにダメージ
        for (int i = 0; i < m_count; ++i)
        {
            yield return new WaitForSeconds(m_interval);

            m_status.Damage(m_value);
            Debug.Log("Poison : HP = " + m_status.Health);
        }

        //状態異常を解除
        m_condition = ConditionType.None;
    }

    private IEnumerator Shock()
    {
        //ダメージを与えて一定時間移動不可

        //自身のレベルに応じた即時ダメージ
        m_status.Damage(m_value * m_status.Value.Level / 100);

        //一定時間待機
        yield return new WaitForSeconds(m_interval);

        //状態異常を解除
        m_condition = ConditionType.None;
    }

    private IEnumerator Regen()
    {
        //一定時間ごとに回復
        for (int i = 0; i < m_count; ++i)
        {
            yield return new WaitForSeconds(m_interval);
            
            m_status.Heal(m_value);
            Debug.Log("Regen : HP = " + m_status.Health);
        }

        //状態異常を解除
        m_condition = ConditionType.None;
    }
}