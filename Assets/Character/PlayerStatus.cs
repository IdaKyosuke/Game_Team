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
        //���x��1�̃X�e�[�^�X��ݒ�
        m_level = 1;
        m_status = m_statusData.GetStatus(m_level);

        //�̗�
        m_health = m_status.hp;
    }

    private void LevelUp(int exp)
    {
        //���Ƀ��x���}�b�N�X�Ȃ牽�����Ȃ�
        if (m_statusData.MaxLevel <= m_level) return;

        //�o���l�̉��Z
        m_exp += exp;

        //���x���A�b�v
        if (m_exp <= m_status.requiredExp) return;

        //���x���̉��Z
        m_level++;
        m_exp = 0;

        //�X�e�[�^�X�̐ݒ�
        m_status = m_statusData.GetStatus(m_level);
    }

    public void Damage(int power)
    {
        //���Ɏ���ł���Ȃ�_���[�W��^���Ȃ�
        if (m_health <= 0) return;

        //�_���[�W�v�Z
        int damage = (power * 2) - (m_status.defense / 3);

        //�}�C�i�X�̃_���[�W�͗^���Ȃ�
        if (damage <= 0) return;

        //�_���[�W
        m_health -= damage;

        //�̗͂̊m�F
        if (m_health <= 0)
        {
            //���S�ʒm
            m_onDeath?.Invoke();
        }
        else
        {
            //��e�ʒm
            m_onDamage?.Invoke();
        }
    }
}