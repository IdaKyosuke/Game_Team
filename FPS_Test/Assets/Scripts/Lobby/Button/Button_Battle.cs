using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Battle : Button_Function
{
	public override void Initialize()
	{
		Debug.Log("Button_Battle initialize");
	}

	public override void PushThis()
	{
		Debug.Log("Button_Battle pushThis");
	}

	public override void PushOther()
	{
		Debug.Log("Button_Battle pushOther");
	}
}
