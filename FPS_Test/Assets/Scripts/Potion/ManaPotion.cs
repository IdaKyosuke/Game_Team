using UnityEngine;

public class ManaPotion : Potion
{
    protected override void PotionEffect()
    {
        //MP回復
        m_playerStatus.MagicHeal(m_data.value);
    }

    protected override void EffectExpires()
    {
        //マナポーションには効果終了時の処理はない
    }
}