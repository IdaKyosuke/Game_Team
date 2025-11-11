using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] StatusData m_statusData;
    [SerializeField] List<GameObject> m_equipments;  //装備枠
    [SerializeField] UnityEvent m_onDamage;
    [SerializeField] UnityEvent m_onDeath;

    private Condition m_condition;
    private EquipmentParameter m_totalEquipmentStatus;  //装備のステータスの実数値(合計値)
    private PlayerParameter m_status;                   //自身の基礎ステータス
    private PlayerParameter m_passiveStatus;            //パッシブスキルによるステータス 
    private PlayerParameter m_totalStatus;              //合計ステータス
    private int m_level;
    private int m_health;
    private int m_mp;
    private int m_exp;

    public PlayerParameter Value => m_status;

    public PlayerParameter Total => m_totalStatus;

    public List<GameObject> Equipments => m_equipments;

    public int Health => m_health;

    public int MP
    { 
        get { return m_mp; }
        set { m_mp = value; }
    }

    public PlayerParameter PassiveStatus
    {
        get { return m_passiveStatus; }
        set { m_passiveStatus = value; }
    }

    private void Awake()
    {
        //レベル1のステータスを設定
        m_level = 1;
        m_status = m_statusData.GetStatus(m_level);

        //体力
        m_health = m_status.hp;

        //合計ステータスの初期化
        m_totalStatus = new PlayerParameter(m_level);

        //パッシブステータスの初期化
        m_passiveStatus = new PlayerParameter(m_level);

        //状態の取得
        m_condition = GetComponent<Condition>();
    }

    private void Update()
    {
        // 数値をリセット
        m_totalStatus = new PlayerParameter(m_level);
        m_totalEquipmentStatus =  ScriptableObject.CreateInstance<EquipmentParameter>();

        // 装備枠分回す
        foreach (GameObject slot in m_equipments)
        {
            // 装備枠が空の場合0を加算していく
            if (slot.transform.childCount == 0) continue;

            // 装備のステータスを加算
            Item_Object info = slot.transform.GetChild(0).GetComponent<Item_Object>();
            m_totalEquipmentStatus += info.GetEquipmentInfo();
        }

        // 合計ステータスに装備の合計ステータスを加算
        m_totalStatus += m_totalEquipmentStatus;

        //パッシブスキルのステータスを加算
        m_totalStatus += m_passiveStatus;

        //基礎ステータスを加算
        m_totalStatus += m_status;
    }

    public void LevelUp(int exp)
    {
        //既にレベルマックスなら何もしない
        if (m_statusData.MaxLevel <= m_level) return;

        //経験値の加算
        m_exp += exp;

        //レベルアップ
        if (m_exp <= m_status.requiredExp) return;

        //レベルの加算
        m_level++;
        m_exp = 0;

        //ステータスの設定
        m_status = m_statusData.GetStatus(m_level);
    }

    public void Damage(int power, AttackType attackType, Condition condition)
    {
        //既に死んでいるならダメージを与えない
        if (m_health <= 0) return;

        //ダメージ計算
        int damage = 0;
        switch (attackType)
        {
            case AttackType.Physical:
                damage = (power * 2) - (m_status.physicalDefense / 3);
                break;

            case AttackType.Magical:
                damage = (power * 2) - (m_status.magicDefense / 3);
                break;
        }

        //マイナスのダメージは与えない
        if (damage <= 0) return;

        //ダメージ
        m_health -= damage;
        Debug.Log("Damage : " + damage);

        //状態異常付与の抽選
        ConditionType conditionType = condition.Grant;
        if (conditionType != ConditionType.None)
        {
            if (condition.Rate(conditionType) >= Random.Range(0, 100))
            {
                m_condition.Init(conditionType);
            }
        }

        //体力の確認
        if (m_health <= 0)
        {
            //死亡通知
            m_onDeath?.Invoke();
        }
        else
        {
            //被弾通知
            m_onDamage?.Invoke();
        }
    }

    public void ConditionDamage(int value)
    {
        //既に死んでいるならダメージを与えない
        if (m_health <= 0) return;

        //マイナスのダメージは与えない
        if (value <= 0) return;

        //ダメージ
        m_health -= value;

        //体力の確認
        if (m_health <= 0)
        {
            //死亡通知
            m_onDeath?.Invoke();
        }
        else
        {
            //被弾通知
            m_onDamage?.Invoke();
        }
    }

    public void Heal(int value)
    {
        //回復
        m_health += value;
    }
}