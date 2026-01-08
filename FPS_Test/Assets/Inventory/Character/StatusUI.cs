using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    [SerializeField] Slider[] m_slider;
    [SerializeField] TextMeshProUGUI[] m_sliderValue;
    [SerializeField] TextMeshProUGUI[] m_statusText;
    
    [SerializeField] PlayerStatus m_playerStatus;

    private void LateUpdate()
    {
        SetupText();
    }

    private void SetupText()
    {
        //ステータスの更新
        m_slider[0].maxValue = m_playerStatus.Total.hp;
        m_slider[0].value = m_playerStatus.CurrentHP;
        m_sliderValue[0].text = m_playerStatus.CurrentHP.ToString() + " / " + m_playerStatus.Total.hp.ToString();

        m_slider[1].maxValue = m_playerStatus.Total.mp;
        m_slider[1].value = m_playerStatus.CurrentMP;
        m_sliderValue[1].text = m_playerStatus.CurrentMP.ToString() + " / " + m_playerStatus.Total.mp.ToString();

        m_statusText[0].text = m_playerStatus.Total.physicalPower.ToString();
        m_statusText[1].text = m_playerStatus.Total.magicPower.ToString();
        m_statusText[2].text = m_playerStatus.Total.physicalDefense.ToString();
        m_statusText[3].text = m_playerStatus.Total.magicDefense.ToString();
        m_statusText[4].text = m_playerStatus.Total.attackSpeed.ToString();
        m_statusText[5].text = m_playerStatus.Total.moveSpeed.ToString();
        m_statusText[6].text = m_playerStatus.Total.openSpeed.ToString();
        m_statusText[7].text = m_playerStatus.Total.requiredExp.ToString();
    }
}