using UnityEngine;

public class HealPotion : Potion
{
    protected override void PotionEffect()
    {
        Debug.Log("ポーションで体力を回復");
       m_playerStatus.Heal(m_data.value);
    }

    protected override void EffectExpires()
    {
        //回復ポーションには効果終了時の処理はない
    }
}