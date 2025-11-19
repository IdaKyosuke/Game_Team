using TMPro;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] m_texts;
    [SerializeField] EquipmentStatus m_status;

    private void OnEnable()
    {
        m_texts[0].text = m_status.TotalStatus.hp.ToString();
        m_texts[1].text = m_status.TotalStatus.mp.ToString();
        m_texts[2].text = m_status.TotalStatus.physicalPower.ToString();
        m_texts[3].text = m_status.TotalStatus.magicPower.ToString();
        m_texts[4].text = m_status.TotalStatus.physicalDefense.ToString();
        m_texts[5].text = m_status.TotalStatus.magicDefense.ToString();
        m_texts[6].text = m_status.TotalStatus.attackSpeed.ToString();
        m_texts[7].text = m_status.TotalStatus.moveSpeed.ToString();
        m_texts[8].text = m_status.TotalStatus.openSpeed.ToString();
    }
}
