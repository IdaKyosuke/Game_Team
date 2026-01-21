using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Stash : Button_Function
{
	private GameObject m_buttonForShop;
	private StashManager m_inventoryManager;

	public override void Initialize()
	{
		m_buttonForShop = GameObject.FindWithTag("buttonForStash");
		m_buttonForShop.SetActive(false);
		m_inventoryManager = GameObject.FindWithTag("inventoryManager").GetComponent<StashManager>();

		Debug.Log("buttonStash : " + m_inventoryManager);
	}

	public override void PushThis()
	{
		m_inventoryManager.CreateLobbyStash();
		m_buttonForShop.SetActive(true);
	}

	public override void PushOther()
	{
		m_buttonForShop.SetActive(false);
		// 他のボタンが押されたときに変更内容を保存する
		m_inventoryManager.SaveStash();
	}
}
