using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Button_Change : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_buttonText;
    [SerializeField] GameObject m_SkillSelectFrame;
	public void OnClick()
    {
        //スキル選択UIが非表示なら表示、表示なら非表示にする
        m_SkillSelectFrame.SetActive(!m_SkillSelectFrame.activeSelf);
    }

    private void Update()
    {
        //ボタンのテキストを切り替える
        if (m_SkillSelectFrame.activeSelf)
        {
            m_buttonText.text = "Job";
        }
        else
        {
            m_buttonText.text = "Skill";
        }
    }
}