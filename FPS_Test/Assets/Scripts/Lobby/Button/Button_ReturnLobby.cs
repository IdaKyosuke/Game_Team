using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ReturnLobby : Button_Function
{

	public override void Initialize()
	{
		Debug.Log("Button_Battle initialize");
	}

	public override void PushThis()
	{
		/*
		m_startButton.SetActive(true);
		m_shopUi.SetActive(false);
		*/
	}

	public override void PushOther()
	{
		Debug.Log("Button_Battle pushOther");
	}
}
