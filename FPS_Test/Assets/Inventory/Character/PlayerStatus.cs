using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatus : MonoBehaviourPunCallbacks
{
    [SerializeField] StatusData m_statusData;
    [SerializeField] List<GameObject> m_equipments;  //装備枠
    [SerializeField] UnityEvent m_onDamage;
    [SerializeField] UnityEvent m_onDeath;

    private Condition m_condition;
    private EquipmentParameter m_totalEquipmentStatus;  //装備のステータスの実数値(合計値)
    private PlayerParameter m_status;                   //自身の基礎ステータス
    private PlayerParameter m_currentStatus;            //実行時の変動ステータス
    private PlayerParameter m_passiveStatus;            //パッシブスキルによるステータス 
    private PlayerParameter m_totalStatus;              //合計ステータス
    private int m_hp;   //残り体力
    private int m_mp;   //残り魔力
    private int m_exp;  //現在の経験値
    private int m_level;

    public PlayerParameter Value => m_status;

    public PlayerParameter Current => m_currentStatus;

    public PlayerParameter Total => m_totalStatus;

    public List<GameObject> Equipments => m_equipments;

    public int Health => m_currentStatus.hp;

    public PlayerParameter PassiveStatus
    {
        get { return m_passiveStatus; }
        set { m_passiveStatus = value; }
    }

    public int CurrentHP
    {
        get { return m_hp; }
        set { m_hp = value; }
    }

    public int CurrentMP
    {
        get { return m_mp; }
        set { m_mp = value; }
    }

    public int CurrentExp
    {
        get { return m_exp; }
        set { m_exp = value; }
    }

    public int Level
    {
        get { return m_level; }
    }

    public int MaxLevel
    {
        get { return m_statusData.MaxLevel; }
    }

    private void Awake()
    {
        //レベル1のステータスを設定
        m_level = 1;
        m_status = m_statusData.GetStatus(m_level);

        //実行時ステータスの設定
        m_currentStatus = m_status;
        m_hp = m_currentStatus.hp;
        m_mp = m_currentStatus.mp;

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
            // 装備枠が空の場合は無視
            if (slot.transform.childCount == 0) continue;

            // 装備のステータスを加算
            Item_Object info = slot.transform.GetChild(0).GetComponent<Item_Object>();

			// まだ性能が未割当の時は無視
			if(!info.GetEquipmentInfo()) continue;

            // 装備のステータスを合計ステータスに加算
            m_totalEquipmentStatus += info.GetEquipmentInfo();
        }

        // 合計ステータスに装備の合計ステータスを加算
        m_totalStatus += m_totalEquipmentStatus;

        //パッシブスキルのステータスを加算
        m_totalStatus += m_passiveStatus;

        //基礎ステータスを加算
        m_totalStatus += m_status;

        //デバッグ用
        if (Input.GetKeyDown(KeyCode.O))
        {
            PenetrationDamage(53);
        }
    }

    public void AddExp(int exp)
    {
        //既にレベルマックスなら何もしない
        if (m_statusData.MaxLevel <= m_level) return;
        Debug.Log("現在のレベル[ " + m_level + " ]");
        Debug.Log("上限のレベル[ " + MaxLevel+ " ]");

        //経験値の加算
        m_exp += exp;

        //レベルアップ
        if (m_exp <= m_status.requiredExp) return;

        //レベルの加算
        m_level++;
        m_exp = 0;
        Debug.Log("レベルアップ");

        //ステータスの設定
        m_status = m_statusData.GetStatus(m_level);

        //体力と魔力を全回復
        m_hp = m_status.hp;
        m_mp = m_status.mp;
    }

    public void Heal(int value)
    {
        //回復
        m_hp += value;

        //上限値を超えないようにする
        if (m_hp >= m_totalStatus.hp) m_hp = m_totalStatus.hp;
    }

    public void MagicHeal(int value)
    {
        //回復
        m_mp += value;

        //上限値を超えないようにする
        if (m_mp >= m_totalStatus.mp) m_mp = m_totalStatus.mp;
    }

    public void Damage(int power, AttackType attackType, int ConditionTypeNum, int grantRate)
    {
        //職業情報
        Warrior warrior;
        Cleric cleric;

        //既に死んでいるならダメージを与えない
        if (m_hp <= 0) return;

        //ダメージ計算
        float damage = 0;
        switch (attackType)
        {
            case AttackType.Physical:
                damage = (power * 2) - (m_status.physicalDefense / 3);
                break;

            case AttackType.Magical:
                damage = (power * 2) - (m_status.magicDefense / 3);
                break;

            case AttackType.Cleric:
                damage = (power * 2) - (m_status.physicalDefense / 3);
                break;
        }

        //マイナスのダメージは与えない
        if (damage <= 0) return;

        //戦士のダメージカットスキル確認
        if (TryGetComponent(out warrior))
        {
            if (warrior.IsDamageCut)
            {
                damage *= 0.9f;
                return;
            }
        }

        //僧侶のバリアスキル確認
        if (TryGetComponent(out cleric))
        { 
            cleric.GetBarrierDamage((int)damage);
        }

        m_hp -= (int)damage;
        Debug.Log("Damage : " + damage);

		//状態異常付与の抽選
        ConditionType conditionType = (ConditionType)ConditionTypeNum;
        if (conditionType != ConditionType.None)
        {
            if (grantRate >= Random.Range(0, 100))
            {
                m_condition.Init(conditionType);
            }
        }

        //体力の確認
        if (m_hp <= 0)
        {
            //戦士の一度だけ耐えるスキル確認
            if (TryGetComponent(out warrior))
            {
                if (warrior.IsOneLife)
                {
                    m_hp = 1;
                    return;
                }
            }

            //死亡通知
            m_hp = 0;
            photonView.RPC("OnDeathPlayer", RpcTarget.All);
			photonView.RPC("OnDeathStash", RpcTarget.All);
			m_onDeath?.Invoke();
        }
        else
        {
            //被弾通知
            m_onDamage?.Invoke();
        }
    }

    public void PenetrationDamage(int value)
    {
        //既に死んでいるならダメージを与えない
        if (m_hp <= 0) return;

        //マイナスのダメージは与えない
        if (value <= 0) return;

        //ダメージ
        m_hp -= value;

        //体力の確認
        if (m_hp <= 0)
        {
            m_hp = 0;

            //死亡通知
            Debug.Log("死亡");
            photonView.RPC("OnDeathPlayer", RpcTarget.All);
			photonView.RPC("OnDeathStash", RpcTarget.All);
			m_onDeath?.Invoke();
        }
        else
        {
            //被弾通知
            m_onDamage?.Invoke();
        }
    }
}