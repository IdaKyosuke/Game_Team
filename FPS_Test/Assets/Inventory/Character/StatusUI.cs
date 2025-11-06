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
        m_statusText[0].text = m_playerStatus.TotalStatus.hp.ToString();
        m_statusText[1].text = m_playerStatus.TotalStatus.mp.ToString();
        m_statusText[2].text = m_playerStatus.TotalStatus.physicalPower.ToString();
        m_statusText[3].text = m_playerStatus.TotalStatus.magicPower.ToString();
        m_statusText[4].text = m_playerStatus.TotalStatus.physicalDefense.ToString();
        m_statusText[5].text = m_playerStatus.TotalStatus.magicDefense.ToString();
        m_statusText[6].text = m_playerStatus.TotalStatus.attackSpeed.ToString();
        m_statusText[7].text = m_playerStatus.TotalStatus.moveSpeed.ToString();
        m_statusText[8].text = m_playerStatus.TotalStatus.openSpeed.ToString();
        m_statusText[9].text = m_playerStatus.TotalStatus.requiredExp.ToString();
    }
}
