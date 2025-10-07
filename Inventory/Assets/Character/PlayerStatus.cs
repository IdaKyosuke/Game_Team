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
    [SerializeField] List<GameObject> m_equipments;  //装備枠
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

        //合計ステータスの初期化
        m_totalStatus = new PlayerParameter(m_level);
    }

    private void Update()
    {
        // 数値をリセット
        m_totalStatus = new PlayerParameter(m_level);
        m_totalEquipmentStatus = new EquipmentParameter();

        // 装備枠分回す
        foreach (GameObject slot in m_equipments)
        {
            Debug.Log(m_equipments.Count);
            // 装備枠が空の場合0を加算していく
            if (slot.transform.childCount == 0) continue;

            // 装備のステータスを加算
            Item_Object info = slot.transform.GetChild(0).GetComponent<Item_Object>();
            m_totalEquipmentStatus += info.GetEquipmentInfo();
        }

        // 合計ステータスに装備のステータスを加算
        m_totalStatus += m_totalEquipmentStatus;

        //基礎ステータスを加算
        m_totalStatus += m_status;

        // デバッグ表示
        Debug.Log(
            $"hp:{m_totalStatus.hp}, " +
            $"mp:{m_totalStatus.mp}, " +
            $"physicalPower:{m_totalStatus.physicalPower}, " +
            $"magicPower:{m_totalStatus.magicPower}, " +
            $"physicalDefense:{m_totalStatus.physicalDefense}," +
            $"magicDefense:{m_totalStatus.magicDefense}, " +
            $"attackSpeed:{m_totalStatus.attackSpeed}, " +
            $"moveSpeed:{m_totalStatus.moveSpeed}," +
            $"openSpeed:{m_totalStatus.openSpeed}"
        );
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

    public void Damage(int power, AttackType attackType)
    {
        //既に死んでいるならダメージを与えない
        if (m_health <= 0) return;

        //防御力を考慮したダメージ計算
        int damage;
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

    // 装備枠を確認 => 空いていたら装備
    public void QuickEquip(GameObject item)
    {
        foreach (GameObject slot in m_equipments)
        {
            // 候補のタイプと一致する枠を見つけたら中を確認 => 空いていたら装備
            if (slot.GetComponent<GridIcon_Equipment>().GetEquipmentType() == item.GetComponent<Item_Object>().GetWeaponType())
            {
                // 装備枠を確認
                slot.GetComponent<GridIcon_Equipment>().QuickEquip(item);
            }
        }
    }

    //装備枠をリストに追加する
    public void SetSlot(GameObject slot)
    {
        m_equipments.Add(slot);
    }

}