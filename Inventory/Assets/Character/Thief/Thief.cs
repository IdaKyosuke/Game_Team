using UnityEngine;

public class Thief : PlayerStatus
{
    private Condition m_condition;

    private void Start()
    {
        //デフォルトで毒を付与可能
        m_condition = GetComponent<Condition>();
        m_condition.Grant = ConditionType.Poison;
    }

    public void UniqueSkill()
    {
        Debug.Log("Thief Identity");

        if(Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("罠を設置");
        }
    }
}