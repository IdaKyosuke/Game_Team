using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Shop : Button_Function
{
	private GameObject m_shopUi;
	private GameObject m_startButton;

	public override void Initialize()
	{
		m_shopUi = GameObject.FindWithTag("shopUi");
		m_startButton = GameObject.FindWithTag("gameStartButton");

		Debug.Log("shop start");

		if(m_shopUi.activeSelf)
		{
			m_shopUi.SetActive(false);
		}
	}

	public override void PushThis()
	{
		m_shopUi.SetActive(true);
		m_startButton.SetActive(false);
	}

	public override void PushOther()
	{
		m_shopUi.SetActive(false);
		m_startButton.SetActive(true);
	}
}
