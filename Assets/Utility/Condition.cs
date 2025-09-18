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
    private float m_elapsedTime;

    public void ApplyCondition(ConditionType conditionType, GameObject other)
    {
        //状態異常のデータを取得
        m_condition = conditionType;
        m_count = m_conditionData.ConditionAbility[(int)m_condition].triggerCount;
        m_interval = m_conditionData.ConditionAbility[(int)m_condition].triggerInterval;
        m_value = m_conditionData.ConditionAbility[(int)m_condition].triggerValue;

        switch (m_condition)
        { 
            case ConditionType.Burn:
                Burn(other);
                break;

            case ConditionType.Frost:
                Frost(other);
                break;

            case ConditionType.Poison:
                Poison(other);
                break;

            case ConditionType.Shock:
                Shock(other);
                break;

            case ConditionType.Regen:
                Regen(other);
                break;

            default:
                Debug.Log("存在しない状態異常 : Condition.cs");
                break;

        }    
    }

    private void Burn(GameObject other)
    {
       
    }

    private void Frost(GameObject other)
    {
        
    }

    private void Poison(GameObject other)
    {
        
    }

    private void Shock(GameObject other)
    {
        
    }

    private void Regen(GameObject other)
    {
       
    }
}