using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Setting : Button_Function
{
	public override void Initialize()
	{
		Debug.Log("button_setting initialize");
	}

	public override void PushThis()
	{
		Debug.Log("button_setting pushThis");
	}

	public override void PushOther()
	{
		Debug.Log("button_setting pushOther");
	}
}
