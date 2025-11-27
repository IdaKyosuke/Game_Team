using UnityEngine;

public class Wizard : Job
{
    private const float Interval = 3;

    [SerializeField] GameObject m_magicBall;
    [SerializeField] GameObject m_camera;

    private Vector3 m_attackOffset;
    private PlayerStatus m_status;
    private Condition m_condition;
    private float m_elapsedTime;

    private void Start()
    {
        m_status = GetComponent<PlayerStatus>();
        m_condition = GetComponent<Condition>();
        m_elapsedTime = 0;
        m_attackOffset = new Vector3(0, 0.4f, 0);

        //パッシブスキルの初期化
        Initialize(JobType.Wizard, AttackType.Magical);
    }

    private void Update()
    {
        //MPが最大値なら処理しない
        if (m_status.Current.mp >= m_status.Total.mp) return;
        
        //MP自動回復
        m_elapsedTime += Time.deltaTime;
        if (m_elapsedTime >= Interval)
        {
            m_status.Current.mp += 2;
            m_elapsedTime = 0;
        }
    }

    protected override void Identity()
    {
        //固有アクション
    }

    public override void Attack()
    {
        //攻撃処理
        Debug.Log("Wizardの攻撃開始");

        //魔弾生成
        GameObject ball = Instantiate(m_magicBall, m_camera.transform.position, m_camera.transform.rotation);
        ball.GetComponent<MagicAttack>().Init(m_camera.transform.forward);
    }

    public override void AttackEnd()
    {
        //攻撃終了処理
        Debug.Log("Wizardの攻撃終了");
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