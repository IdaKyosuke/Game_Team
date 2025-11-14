using System;
using UnityEngine;

public enum PotionType
{ 
    Heal,
    Money,
    PowerUP,
    DefenseUP,

    Length,
}

public class ItemData : MonoBehaviour
{
    [SerializeField] PotionType m_type;    //種類
    [SerializeField] int m_value;          //効果値

    private Action[] m_actions;
    private PlayerStatus m_status;

    private void Start()
    {
        //親のステータスを参照
        m_status = transform.root.GetComponent<PlayerStatus>();

        //効果の登録
        m_actions = new Action[(int)PotionType.Length]
        {
            () => Heal(),
            () => Money(),
            () => PowerUP(),
            () => DefenseUP(),
        };
    }

    public void Use()
    {
        //アイテムの使用
        m_actions[(int)m_type]?.Invoke();
    }

    private void Heal()
    {
        Debug.Log("回復ポーション使用");

        //回復
        m_status.Heal(m_value);
    }

    private void Money()
    {
        Debug.Log("換金アイテム使用");
    }

    private void PowerUP()
    {
        Debug.Log("攻撃力UPポーション使用");
    }

    private void DefenseUP()
    {
        Debug.Log("防御力UPポーション使用");
    }
}