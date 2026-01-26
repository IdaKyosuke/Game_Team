using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillText_Manager : MonoBehaviour
{
	// ロビーのスキル関連
	[SerializeField] ExcelData m_excelData;
	[SerializeField] List<TextMeshProUGUI> m_textList;

	public void ChangeSkillText(int jobType)
	{
		m_textList[0].SetText(m_excelData.jobText[jobType].firstPassive);
		m_textList[1].SetText(m_excelData.jobText[jobType].secondPassive);
		m_textList[2].SetText(m_excelData.jobText[jobType].thirdPassive);
	}
}
