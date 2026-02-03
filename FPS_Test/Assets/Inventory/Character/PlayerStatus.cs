using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviourPunCallbacks
{
    [SerializeField] StatusData[] m_statusData;
    [SerializeField] List<GameObject> m_equipments;  //装備枠
    [SerializeField] UnityEvent m_onDamage;
    [SerializeField] UnityEvent m_onDeath;
	[SerializeField] Image m_damageEffect;

    private JobType m_jobType;
    private Condition m_condition;
    private PlayerParameter m_status;                   //自身の基礎ステータス
    private PlayerParameter m_currentStatus;            //実行時の変動ステータス
    private PlayerParameter m_passiveStatus;            //パッシブスキルによるステータス 
    private PlayerParameter m_totalStatus;              //合計ステータス
    private int m_level;

    public int Level => m_level;

    public int MaxLevel => m_statusData[(int)m_jobType].MaxLevel;

    public PlayerParameter Base => m_status;

    public PlayerParameter Current => m_currentStatus;

    public PlayerParameter Total => m_totalStatus;

    public List<GameObject> Equipments => m_equipments;

    private void Start()
    {
        //職業の取得
        m_jobType = GameManager.Instance.PlayerJobType;

        //レベル1のステータスを設定
        m_level = 1;

        //ステータスデータをクローンする
        m_status = m_statusData[(int)m_jobType].GetStatus(m_level).Clone();

        //パッシブステータスの初期化
        m_passiveStatus = new PlayerParameter(m_level);

        //実行時ステータスは基礎ステータスのクローンで初期化
        m_currentStatus = m_status.Clone(); 
        m_currentStatus += m_passiveStatus;
        m_currentStatus += EquipmentStatus();

        //状態の取得
        m_condition = GetComponent<Condition>();
    }
        
    private void Update()
    {
        // 数値をリセット
        m_totalStatus = new PlayerParameter(m_level);

        //パッシブスキルのステータスを加算
        m_totalStatus += m_passiveStatus;

        //装備ステータスを加算
        m_totalStatus += EquipmentStatus();

        //基礎ステータスを加算
        m_totalStatus += m_status;
    }

    private EquipmentParameter EquipmentStatus()
    {
        EquipmentParameter parameter = ScriptableObject.CreateInstance<EquipmentParameter>();

        // 装備枠分回す
        foreach (GameObject slot in m_equipments)
        {
            // 装備枠が空の場合は無視
            if (slot.transform.childCount == 0) continue;

            // 装備のステータスを加算
            Item_Object info = slot.transform.GetChild(0).GetComponent<Item_Object>();

            // まだ性能が未割当の時は無視
            if (!info.GetEquipmentInfo()) continue;

            // 装備のステータスを合計ステータスに加算
            parameter += info.GetEquipmentInfo();
        }

        return parameter;
    }

    public void AddExp(int exp)
    {
        //既にレベルマックスなら何もしない
        if (m_statusData[(int)m_jobType].MaxLevel <= m_level) return;

        //経験値の加算
        m_currentStatus.requiredExp += exp;

        //レベルアップ
        if (m_currentStatus.requiredExp < m_status.requiredExp) return;

        //レベルの加算
        m_level++;
        m_currentStatus.requiredExp = 0;

        //ステータスの設定
        m_status = m_statusData[(int)m_jobType].GetStatus(m_level).Clone();

        //体力と魔力を全回復
        m_currentStatus.hp = m_totalStatus.hp;
        m_currentStatus.mp = m_totalStatus.mp;
    }

    public void Heal(int value)
    {
        //回復
        m_currentStatus.hp += value;

        //上限値を超えないようにする
        if (m_currentStatus.hp >= m_totalStatus.hp) m_currentStatus.hp = m_totalStatus.hp;
    }

    public void MagicHeal(int value)
    {
        //回復
        m_currentStatus.mp += value;

        //上限値を超えないようにする
        if (m_currentStatus.mp >= m_totalStatus.mp) m_currentStatus.mp = m_totalStatus.mp;
    }

    public bool Damage(int power, AttackType attackType, int ConditionTypeNum, int grantRate)
    {
        //職業情報
        Warrior warrior;
        Cleric cleric;

        //既に死んでいるならダメージを与えない
        if (m_currentStatus.hp <= 0) return true;

        //ダメージ計算
        float damage = 0;
        switch (attackType)
        {
            case AttackType.Physical:
                damage = (power * 2) - (m_totalStatus.physicalDefense / 3);
                break;

            case AttackType.Magical:
                damage = (power * 2) - (m_totalStatus.magicDefense / 3);
                break;

            case AttackType.Cleric:
                damage = (power * 2) - (m_totalStatus.physicalDefense / 3);
                break;
        }

		Debug.Log(m_status.physicalDefense);

        //マイナスのダメージは与えない
        if (damage <= 0) return false;

        //戦士のダメージカットスキル確認
        if (TryGetComponent(out warrior))
        {
            if (warrior.IsDamageCut) damage *= 0.8f;
        }

        //僧侶のバリアスキル確認
        if (TryGetComponent(out cleric))
        {
            cleric.GetBarrierDamage((int)damage);
        }

        m_currentStatus.hp -= (int)damage;
        Debug.Log("プレイヤーが [" + (int)damage + "] ダメージ受けた!");

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
        if (m_currentStatus.hp <= 0)
        {
            //戦士の一度だけ耐えるスキル確認
            if (TryGetComponent(out warrior))
            {
                if (warrior.IsOneLife)
                {
                    m_currentStatus.hp = 1;
                    warrior.IsOneLife = false;
                    return false;
                }
            }

            //死亡通知
            m_currentStatus.hp = 0;
            photonView.RPC("OnDeathStash", RpcTarget.All);
            photonView.RPC("OnDeathPlayer", RpcTarget.All);
            m_onDeath?.Invoke();
			return true;
        }
        else
        {
            //被弾通知
            m_onDamage?.Invoke();
			return false;
        }
    }

    public void PenetrationDamage(int value)
    {
        //既に死んでいるならダメージを与えない
        if (m_currentStatus.hp <= 0) return;

        //マイナスのダメージは与えない
        if (value <= 0) return;

        //ダメージ
        m_currentStatus.hp -= value;

        //体力の確認
        if (m_currentStatus.hp <= 0)
        {
            m_currentStatus.hp = 0;

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

    public void PassiveStatus(PlayerParameter parameter)
    {
        //パッシブスキルでのステータス強化を更新
        m_passiveStatus = parameter;
        m_currentStatus += m_passiveStatus;
    }

    public void PassiveMoveSpeed(int value)
    { 
        m_passiveStatus.moveSpeed += value;
        m_currentStatus += m_passiveStatus;
    }
}