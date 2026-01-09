using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Shop : Button_Function
{
	private GameObject m_shopUi;
	private ShopInfoList m_shopButtonInfo;

	public override void Initialize()
	{
		m_shopUi = GameObject.FindWithTag("shopUi");

		Debug.Log("shop start");

		if(m_shopUi.activeSelf)
		{
			m_shopUi.SetActive(false);
		}
	}

	public override void PushThis()
	{
		m_shopUi.SetActive(true);
		m_shopButtonInfo = GameObject.FindWithTag("shopInfoList").GetComponent<ShopInfoList>();
		// 購入モードはとりあえずリストの最初のショップを表示する
		m_shopButtonInfo.GetShopInfo(0).SetShopItem();
		Debug.Log("set");
	}

	public override void PushOther()
	{
		//m_startButton.SetActive(true);
	}
}
