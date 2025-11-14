using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ショップの情報のリストを保持するクラス

public class ShopInfoList : MonoBehaviour
{
	[SerializeField] List<SelectShopButton> m_shopList;

	public SelectShopButton GetShopInfo(int index)
	{
		return m_shopList[index];
	}
}
