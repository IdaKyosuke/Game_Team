using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Job : Button_Function
{
	public override void Initialize()
	{
		Debug.Log("Button_Job initialize");
	}

	public override void PushThis()
	{
		Debug.Log("Button_Job pushThis");
	}

	public override void PushOther()
	{
		Debug.Log("Button_Job pushOther");
	}
}
