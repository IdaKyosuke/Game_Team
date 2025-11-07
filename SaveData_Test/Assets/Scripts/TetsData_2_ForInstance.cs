using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class TetsData_2_ForInstance : MonoBehaviour
{
	private TestData_2 m_instance;
	[SerializeField] List<TextMeshProUGUI> m_list;

	[SerializeField] string m_name;
	[SerializeField] int m_lv;
	[SerializeField] int m_money;

	private void Start()
	{
		m_instance = TestData_2.Instance;
	}

	private void Update()
	{
		m_list[0].text = m_name;
		m_list[1].text = m_lv.ToString();
		m_list[2].text = m_money.ToString();

		SetValue();
	}

	public void Save()
	{
		m_instance.Save();
	}

	public void Load()
	{
		m_instance.Reload();

		m_name = m_instance.m_name;
		m_lv = m_instance.m_lv;
		m_money = m_instance.m_money;
	}

	public void SetValue()
	{
		m_instance.m_name = m_name;
		m_instance.m_lv = m_lv;
		m_instance.m_money = m_money;
	}
}
