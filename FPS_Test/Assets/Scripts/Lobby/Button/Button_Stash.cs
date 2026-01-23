using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Stash : Button_Function
{
	private StashManager m_inventoryManager;
	private GameObject m_shopUi = null;
	private bool m_isPushed = false;

    public override void Initialize()
	{
        m_inventoryManager = GameObject.FindWithTag("inventoryManagerShop").GetComponent<StashManager>();
        m_shopUi = GameObject.FindWithTag("shopUi");
    }

	public override void PushThis()
	{
		if (m_isPushed) return;
		m_inventoryManager.CreateLobbyStash();
        m_shopUi.SetActive(true);
        m_isPushed = true;
    }

	public override void PushOther()
	{
		if (!m_isPushed)
		{
            return;
		}
        // 他のボタンが押されたときに変更内容を保存する
        m_inventoryManager.SaveStash();
		m_shopUi.SetActive(false);
		m_isPushed = false;
    }
}
