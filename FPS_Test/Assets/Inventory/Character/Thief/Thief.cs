using UnityEngine;

public class Thief : Job
{
    [SerializeField] GameObject m_trap;

    private PlayerStatus m_status;
    private Condition m_condition;

    private void Start()
    {
        //ステータスの取得
        m_status = GetComponent<PlayerStatus>();

        //デフォルトで毒を付与可能
        m_condition = GetComponent<Condition>();
        m_condition.Grant = ConditionType.Poison;

        //パッシブスキルの初期化
        Initialize(JobType.Thief, AttackType.Physical);
    }

    protected override void Identity()
    {
        //固有アクション
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //罠の設置
            Instantiate(m_trap, transform.position, Quaternion.identity);
        }
    }

    public override void Attack()
    {
        //攻撃処理
    }

    protected override void Passive1()
    {
        //移動速度UP
        m_status.PassiveStatus.moveSpeed += 3;
    }

    protected override void Passive2()
    {
        //開封速度UP
        m_status.PassiveStatus.openSpeed += 2;
    }

    protected override void Passive3()
    {
        //攻撃速度UP
        m_status.PassiveStatus.attackSpeed += 2;
    }
}