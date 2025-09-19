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
    private int m_count;
    private int m_interval;
    private int m_value;

    private void Awake()
    {
        m_condition = ConditionType.None;
        m_count = 0;
        m_interval = 0;
        m_value = 0;
    }

    public void ApplyCondition(ConditionType conditionType, GameObject other)
    {
        //状態異常のデータを取得
        m_condition = conditionType;
        m_count = m_conditionData.ConditionAbility[(int)m_condition].triggerCount;
        m_interval = m_conditionData.ConditionAbility[(int)m_condition].triggerInterval;
        m_value = m_conditionData.ConditionAbility[(int)m_condition].triggerValue;

        Debug.Log(other.GetComponent<PlayerStatus>().Health);

        switch (m_condition)
        { 
            case ConditionType.Burn:
                StartCoroutine(Burn(other));
                break;

            case ConditionType.Frost:
                StartCoroutine(Frost(other));
                break;

            case ConditionType.Poison:
                StartCoroutine(Poison(other));
                break;

            case ConditionType.Shock:
                StartCoroutine(Shock(other));
                break;

            case ConditionType.Regen:
                StartCoroutine(Regen(other));
                break;

            case ConditionType.None:
                //何もしない
                break;

            default:
                Debug.Log("存在しない状態異常 : Condition.cs");
                break;

        }    
    }

    private IEnumerator Burn(GameObject other)
    {
        //残りHPダメージに対する割合ダメージ
        for (int i = 0; i < m_count; ++i)
        {
            //一定時間待機
            yield return new WaitForSeconds(m_interval);

            //割合ダメージ
            int damage = other.GetComponent<PlayerStatus>().Health / m_value;
            other.GetComponent<PlayerStatus>().Damage(damage);

            Debug.Log("Burn : HP = " + other.GetComponent<PlayerStatus>().Health);
        }
    }

    private IEnumerator Frost(GameObject other)
    {
        //移動速度低下
        other.GetComponent<PlayerStatus>().Value.moveSpeed -= m_value;

        //一定時間待機
        yield return new WaitForSeconds(m_interval);

        //移動速度を元に戻す
        other.GetComponent<PlayerStatus>().Value.moveSpeed += m_value;
    }

    private IEnumerator Poison(GameObject other)
    {
        //一定時間ごとにダメージ
        for (int i = 0; i < m_count; ++i)
        {
            yield return new WaitForSeconds(m_interval);
            other.GetComponent<PlayerStatus>().Damage(m_value);

            Debug.Log("Poison : HP = " + other.GetComponent<PlayerStatus>().Health);
        }
    }

    private IEnumerator Shock(GameObject other)
    {
        //一定時間ごとにダメージ
        for (int i = 0; i < m_count; ++i)
        {
            yield return new WaitForSeconds(m_interval);
            other.GetComponent<PlayerStatus>().Damage(m_value);

            Debug.Log("Shock : HP = " + other.GetComponent<PlayerStatus>().Health);
        }
    }

    private IEnumerator Regen(GameObject other)
    {
        //一定時間ごとに回復
        for (int i = 0; i < m_count; ++i)
        {
            yield return new WaitForSeconds(m_interval);
            other.GetComponent<PlayerStatus>().Heal(m_value);

            Debug.Log("Regen : HP = " + other.GetComponent<PlayerStatus>().Health);
        }
    }
}