using UnityEngine;

public class Warrior : Job
{
    [SerializeField] PlayerParameter m_parameter;
    private PlayerStatus m_status;

    private void Start()
    {
        //ステータスの取得
        m_status = GetComponent<PlayerStatus>();

        //パッシブスキルの初期化
        Initialize(JobType.Warrior);
    }

    private void Update()
    {

    }

    protected override void Passive1()
    {
        //全ステータス強化
        m_status.PassiveStatus += m_parameter;
    }

    protected override void Passive2()
    {
        //攻撃速度UP
        m_status.PassiveStatus.attackSpeed += 3;
    }

    protected override void Passive3()
    {
        //ダメージカット
    }
}