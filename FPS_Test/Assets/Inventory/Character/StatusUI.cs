using TMPro;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] m_statusText;
    
    //private PlayerStatus m_playerStatus;
    [SerializeField] PlayerStatus m_playerStatus;

    private void LateUpdate()
    {
        SetupText();
    }

    private void SetupText()
    {
        //ステータスの更新
        m_statusText[0].text = m_playerStatus.Total.hp.ToString();
        m_statusText[1].text = m_playerStatus.Total.mp.ToString();
        m_statusText[2].text = m_playerStatus.Total.physicalPower.ToString();
        m_statusText[3].text = m_playerStatus.Total.magicPower.ToString();
        m_statusText[4].text = m_playerStatus.Total.physicalDefense.ToString();
        m_statusText[5].text = m_playerStatus.Total.magicDefense.ToString();
        m_statusText[6].text = m_playerStatus.Total.attackSpeed.ToString();
        m_statusText[7].text = m_playerStatus.Total.moveSpeed.ToString();
        m_statusText[8].text = m_playerStatus.Total.openSpeed.ToString();
        m_statusText[9].text = m_playerStatus.Total.requiredExp.ToString();
    }
}
