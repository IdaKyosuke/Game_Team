using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ショップの情報のリストを保持するクラス

public class ShopInfoList : MonoBehaviour
{
	[SerializeField] List<SelectShopButton> m_shopList;
	[SerializeField] GameObject m_dealButton;	// ショップの処理決定ボタン

	public SelectShopButton GetShopInfo(int index)
	{
		return m_shopList[index];
	}

	public void HideDealButton()
	{
		// ボタンがアクティブな時だけ走る
		if(m_dealButton.activeSelf)
		{
			m_dealButton.SetActive(false);
		}
	}

	public void ShowDealButton()
	{
		// ボタンが非アクティブな時だけ走る
		if (!m_dealButton.activeSelf)
		{
			m_dealButton.SetActive(true);
		}
	}

	public void SetStart()
	{
		foreach(var v in m_shopList)
		{
			v.MakeList();
		}
	}
}
