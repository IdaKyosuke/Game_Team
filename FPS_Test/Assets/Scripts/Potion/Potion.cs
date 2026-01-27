using System.Collections;
using UnityEngine;

public abstract class Potion : MonoBehaviour
{
    [SerializeField] protected PotionData m_data;

    protected PlayerStatus m_playerStatus;

    public PotionData Data => m_data;

    private void Start()
    {
        m_playerStatus = transform.root.GetComponent<PlayerStatus>();
    }

    public void Use()
    {
        StartCoroutine(UsePotion());
    }

    //ポーションの効果は継承先で実装
    private IEnumerator UsePotion()
    {
        for (int i = 0; i < m_data.count; ++i)
        {
            //効果を発動
            PotionEffect();

            //次の効果発動まで待機
            yield return new WaitForSeconds(m_data.interval);
        }

        //効果終了時の処理
        EffectExpires();
    }

    protected abstract void PotionEffect();

    protected abstract void EffectExpires();
}