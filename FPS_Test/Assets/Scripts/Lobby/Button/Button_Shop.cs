using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Shop : Button_Function
{
	private GameObject m_shopUi;
	private GameObject m_buttonForShop;
	private ShopInfoList m_shopButtonInfo;
	private StashManager m_inventoryManager;

	private bool m_isPushed = false;

	public override void Initialize()
	{
		m_shopUi = GameObject.FindWithTag("shopUi");
		m_buttonForShop = GameObject.FindWithTag("buttonForShop");
		m_inventoryManager = GameObject.FindWithTag("inventoryManagerShop").GetComponent<StashManager>();

		m_shopButtonInfo = GameObject.FindWithTag("shopInfoList").GetComponent<ShopInfoList>();
		// トレーダーの初期設定
		m_shopButtonInfo.SetStart();
		m_buttonForShop.SetActive(false);

		m_shopUi.SetActive(false);
	}

	public override void PushThis()
	{
		if (m_isPushed) return;
		m_inventoryManager.StartShopMode();
		// 購入モードはとりあえずリストの最初のショップを表示する
		//m_shopButtonInfo.GetShopInfo(0).SetShopItem();
		m_buttonForShop.SetActive(true);
		m_shopUi.SetActive(true);

		m_isPushed = true;
	}

	public override void PushOther()
	{
		if (!m_isPushed) return;
		Debug.Log("other");

        // ロビー画面に遷移する時にショップ情報をリセットする
        m_inventoryManager.ResetShop();
		m_buttonForShop.SetActive(false);
        m_shopUi.SetActive(false);
		m_isPushed = false;
    }
}
