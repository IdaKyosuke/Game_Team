using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatus : MonoBehaviour
{
    public enum JobType
    {
        Warrior,
        Wizard,
        Cleric,
        Thief,

        Length,
    }

    [SerializeField] JobType m_job;
    [SerializeField] StatusData m_statusData;
    [SerializeField] List<GameObject> m_equipment;  //装備枠
    [SerializeField] UnityEvent m_onDamage;
    [SerializeField] UnityEvent m_onDeath;

    private EquipmentParameter m_totalEquipmentStatus;  //装備のステータスの実数値(合計値)
    private PlayerParameter m_status;                   //自身の基礎ステータス
    private PlayerParameter m_totalStatus;              //合計ステータス
    private int m_level;
    private int m_health;
    private int m_exp;

    public PlayerParameter Value => m_status;

    public PlayerParameter TotalStatus => m_totalStatus;

    public int Health => m_health;

    private void Start()
    {
        //レベル1のステータスを設定
        m_level = 1;
        m_status = m_statusData.GetStatus(m_level);

        //体力
        m_health = m_status.hp;
    }

    private void Update()
    {
        // 数値をリセット
        m_totalStatus = new PlayerParameter(0);
        m_totalEquipmentStatus = new EquipmentParameter();

        // 装備枠分回す
        foreach (GameObject item in m_equipment)
        {
            // 装備枠が空の場合は次へ
            //if (item.transform.childCount == 0) continue;

            // 装備のステータスを加算
            // EquipmentStatus info = item.transform.GetChild(0).GetComponent<EquipmentStatus>();
            EquipmentStatus info = item.GetComponent<EquipmentStatus>();
            m_totalEquipmentStatus += info.TotalStatus;
        }

        //自身のステータスに装備のステータスを加算
        m_totalStatus = m_status + m_totalEquipmentStatus;
    }

    public virtual void Identity() {}

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

    public void Damage(int power)
    {
        //既に死んでいるならダメージを与えない
        if (m_health <= 0) return;

        //ダメージ計算
        //int damage = (power * 2) - (m_status.defense / 3);

        //マイナスのダメージは与えない
        if (power <= 0) return;

        //ダメージ
        m_health -= power;

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