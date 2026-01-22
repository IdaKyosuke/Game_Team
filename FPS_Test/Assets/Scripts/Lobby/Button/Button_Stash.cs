using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Stash : Button_Function
{
	private StashManager m_inventoryManager;


	private GameObject m_stamane;

	public override void Initialize()
	{
		m_stamane = GameObject.FindWithTag("inventoryManagerShop");
		//m_inventoryManager = GameObject.FindWithTag("inventoryManager").GetComponent<StashManager>();
		m_inventoryManager = m_stamane.GetComponent<StashManager>();

		Debug.Log("buttonStash : " + m_stamane.name);
	}

	public override void PushThis()
	{
		m_inventoryManager.CreateLobbyStash();
	}

	public override void PushOther()
	{
		// 他のボタンが押されたときに変更内容を保存する
		m_inventoryManager.SaveStash();
	}
}
