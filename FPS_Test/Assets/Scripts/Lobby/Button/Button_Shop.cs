using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Shop : Button_Function
{
	private GameObject m_shopUi;
	private GameObject m_buttonForShop;
	private ShopInfoList m_shopButtonInfo;
	private StashManager m_inventoryManager;

	public override void Initialize()
	{
		m_shopUi = GameObject.FindWithTag("shopUi");
		m_buttonForShop = GameObject.FindWithTag("buttonForShop");
		m_inventoryManager = GameObject.FindWithTag("inventoryManager").GetComponent<StashManager>();
		Debug.Log("buttonShop : " + m_inventoryManager);
		//if (m_shopUi.activeSelf)
		//{
		//	m_shopUi.SetActive(false);
		//}

		m_buttonForShop.SetActive(false);
	}

	public override void PushThis()
	{
		m_inventoryManager.StartShopMode();
		m_shopUi.SetActive(true);
		m_shopButtonInfo = GameObject.FindWithTag("shopInfoList").GetComponent<ShopInfoList>();
		// 購入モードはとりあえずリストの最初のショップを表示する
		m_shopButtonInfo.GetShopInfo(0).SetShopItem();
		m_buttonForShop.SetActive(true);
	}

	public override void PushOther()
	{
		m_buttonForShop.SetActive(false);
	}
}
