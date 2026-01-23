using UnityEngine;

public class Button_ReturnLobby : Button_Function
{
	private StashManager m_stashManager;
	private GameObject m_shopUi;

	public override void Initialize()
	{
		m_shopUi = GameObject.FindWithTag("shopUi");
		m_stashManager = GameObject.FindWithTag("inventoryManagerShop").GetComponent<StashManager>();
	}

	public override void PushThis()
	{
		if(m_shopUi.activeSelf)
		{
			// ロビー画面に遷移する時にショップ情報をリセットする
			m_stashManager.ResetShop();
			m_shopUi.SetActive(false);
		}
	}

	public override void PushOther()
	{

	}
}