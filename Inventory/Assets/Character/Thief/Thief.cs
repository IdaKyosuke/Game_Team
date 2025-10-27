using UnityEngine;

public class Thief : Job
{
    [SerializeField] GameObject m_trap;

    private Condition m_condition;

    private void Start()
    {
        //デフォルトで毒を付与可能
        m_condition = GetComponent<Condition>();
        m_condition.Grant = ConditionType.Poison;
    }

    private void Update()
    {
        //固有アクション
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //罠の設置
            Instantiate(m_trap, transform.position, Quaternion.identity);
        }
    }

    protected override void Passive1()
    {

    }

    protected override void Passive2()
    {

    }

    protected override void Passive3()
    {

    }
}