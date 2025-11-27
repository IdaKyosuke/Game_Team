using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Button_Function
{
	public abstract void Initialize();

	// ボタンが押されたときの処理
	public abstract void PushThis();
	// 他のボタンが押されたときの処理
	public abstract void PushOther();
}
