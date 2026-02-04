using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Potion : MonoBehaviour
{
    [SerializeField] protected PotionData m_data;

    protected PlayerStatus m_playerStatus;

    public PotionData Data => m_data;

    private void Start()
    {
        m_playerStatus = transform.root.GetComponent<PlayerStatus>();
    }

    public bool Use()
    {
        //ゲーム中のみ使用可能
        if(!SceneManager.GetSceneByName("GameScene").isLoaded) return false;

        //インベントリ内でのみ使用可能
        if (GetComponent<Item_Object>().GetGridType() != GridType.Inventory) return false;

        //ポーション効果開始
        StartCoroutine(UsePotion());

        return true;
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