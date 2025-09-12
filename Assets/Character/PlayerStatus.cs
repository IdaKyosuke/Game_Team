using UnityEngine;
using UnityEngine.Events;

public class PlayerStatus : MonoBehaviour
{
    enum Job
    {
        Warrior,
        Wizard,
        Cleric,
        Thief,
    }

    [SerializeField] Job m_job;
    [SerializeField] StatusData m_statusData;
    [SerializeField] UnityEvent m_onDamage;
    [SerializeField] UnityEvent m_onDeath;

    private StatusData.Parameters m_status;
    private int m_level;
    private int m_health;
    private int m_exp;

    private void Start()
    {
        //レベル1のステータスを設定
        m_level = 1;
        m_status = m_statusData.GetStatus(m_level);

        //体力
        m_health = m_status.hp;
    }

    private void LevelUp(int exp)
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
        int damage = (power * 2) - (m_status.defense / 3);

        //マイナスのダメージは与えない
        if (damage <= 0) return;

        //ダメージ
        m_health -= damage;

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
}