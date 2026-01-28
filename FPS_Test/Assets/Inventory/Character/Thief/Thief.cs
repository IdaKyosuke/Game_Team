using Photon.Pun;
using UnityEngine;

public class Thief : Job
{
    private const int PassiveMoveSpeedValue = 10;

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

    public override void Attack()
    {
        //攻撃コライダー有効化
        m_weapon.StartAttack();
    }

    public override void AttackEnd()
    {
        //攻撃コライダー無効化
        m_weapon.EndAttack();
    }

    public override void Identity()
    {
        //罠の設置
        Instantiate(m_trap, transform.position, Quaternion.identity);
    }

    protected override void Passive1()
    {
        //移動速度UP
        m_status.PassiveMoveSpeed(PassiveMoveSpeedValue);
    }

    protected override void Passive2()
    {
        //開封速度を半分に時短
        GetComponent<StashController>().ScavengerTime /= 2;
    }

    protected override void Passive3()
    {
        //鍵の取得
        Debug.Log("鍵を一つ入手した！");
		ItemList item = ScriptableObject.CreateInstance<ItemList>();
		item.ItemData = GetComponent<StashController>().GetExcel.unique[8];
		GetComponent<StashController>().GetManager().AddKey(item);
	}
}