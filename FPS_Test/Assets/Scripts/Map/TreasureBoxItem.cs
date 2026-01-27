using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Rarity
{
	Common,
	Rare,
	Unipue,
	Legendary,

	Length,
}

public class TreasureBoxItem : MonoBehaviourPunCallbacks
{
	[SerializeField] ExcelData m_excelData;
	[SerializeField] Rarity m_rarity;
	[SerializeField] Info_InventorySize m_inventorySize;

	private Animator m_anime;

	private List<MapObjectEntity> m_treasureList = new List<MapObjectEntity>();
	private List<ItemList> m_itemList = new List<ItemList>();
	private bool[,] m_isEquipped;

	// 宝箱の鍵が開いているかどうか
	private bool m_isLock;
	// 今誰かが宝箱を開けているかどうか
	private bool m_isNowOpen;
	// 宝箱が開けられたかどうか
	private bool m_isOpened;

	public bool IsLock => m_isLock;

	public bool IsNowOpen => m_isNowOpen;

	public bool IsOpened => m_isOpened;

	void Start()
	{
		m_isLock = (m_rarity == Rarity.Legendary);
		m_anime = GetComponent<Animator>();
		m_isNowOpen = false;
		m_isOpened = false;
		if (!photonView.IsMine) return;
		m_isEquipped = new bool[m_inventorySize.GetSize.x, m_inventorySize.GetSize.y];
		for (int i = 0; i < m_inventorySize.GetSize.y; i++)
		{
			for (int j = 0; j <  m_inventorySize.GetSize.x; j++)
			{
				m_isEquipped[j, i] = false;
			}
		}

		// 抽選会数を設定
		int selectAmount = Random.Range(
			m_excelData.treasureBox[(int)m_rarity].itemMin,
			m_excelData.treasureBox[(int)m_rarity].itemMax + 1);

		SelectTreasureItem(selectAmount);
	}

	public Info_InventorySize GetInfo()
	{
		return m_inventorySize;
	}

	[PunRPC]
	public void SetItemList(ItemList info)
	{
		m_itemList.Add(info);
	}

	public List<ItemList> GetItemList()
	{
		return m_itemList;
	}

	private void SelectTreasureItem(int selectAmount)
	{
		// 宝箱のレアリティの抽選の数値を取得
		int[] probability = new int[(int)Rarity.Length];
		probability[0] = m_excelData.treasureBox[(int)m_rarity].common;
		probability[1] = m_excelData.treasureBox[(int)m_rarity].rare;
		probability[2] = m_excelData.treasureBox[(int)m_rarity].unique;
		probability[3] = m_excelData.treasureBox[(int)m_rarity].legendary;

		// 宝箱のレアリティに応じて確定のレアリティのアイテムを一つ抽選
		photonView.RPC(nameof(SetItemList), RpcTarget.All, SetItemData(SelectObject(m_rarity)));

		// selectAmountの数だけ抽選する
		for (int i = 0; i < selectAmount; ++i)
		{
			Rarity treasureType = SelectRarity(probability);
			MapObjectEntity treasureItem =  SelectObject(treasureType);
			ItemList item = SetItemData(treasureItem);

			photonView.RPC(nameof(SetItemList), RpcTarget.All, item);
		}
	}

	private Rarity SelectRarity(int[] probability)
	{
		// レアリティの抽選
		int raritySelectNum = Random.Range(0, 101);
		int rarityNum = 0;

		for (int i = 0; i < (int)Rarity.Length; ++i)
		{
			raritySelectNum -= probability[i];
			if (raritySelectNum < 0)
			{
				rarityNum = i;
				break;
			}
		}
		return (Rarity)rarityNum;
	}

	private MapObjectEntity SelectObject(Rarity type)
	{
		// 今回の抽選されたレアリティに応じてアイテム情報を取得
		List<MapObjectEntity> objectData = m_excelData.common;
		switch (type)
		{
			case Rarity.Common:
				objectData = m_excelData.common;
				break;

			case Rarity.Rare:
				objectData = m_excelData.rare;
				break;

			case Rarity.Unipue:
				objectData = m_excelData.unique;
				break;

			case Rarity.Legendary:
				objectData = m_excelData.legendary;
				break;
		}

		int objectSelectNum = Random.Range(0, 101);
		int objectIndex = 0;
		// アイテムを抽選
		for (int j = 0; j < objectData.Count; ++j)
		{
			objectSelectNum -= objectData[j].probability;
			if (objectSelectNum <= 0)
			{
				objectIndex = j;
				break;
			}
		}
		return objectData[objectIndex];
	}

	private ItemList SetItemData(MapObjectEntity treasureItem)
	{
		ItemList info = ScriptableObject.CreateInstance<ItemList>();

		bool isSet = false;

		for (int i = 0; i < m_inventorySize.GetSize.y; ++i)
		{
			for (int j = 0; j < m_inventorySize.GetSize.x; ++j)
			{
				// マス目座標を保存
				if (CheckSpace(new Vector2Int(j, i), new Vector2Int(treasureItem.width, treasureItem.height)))
				{
					isSet = true;
					info.SetGridIndex(new Vector2Int(j, i));
					break;
				}
			}
			if (isSet) break;
		}
		
		info.ItemData = treasureItem;

		return info;
	}

	private bool CheckSpace(Vector2Int startGrid, Vector2Int size)
	{
		// 枠外にはみ出すときはそもそも確認しない
		if (startGrid.x + (size.x - 1) >= m_inventorySize.GetSize.x) return false;
		if (startGrid.y + (size.y - 1) >= m_inventorySize.GetSize.y) return false;

		// 中身を確認
		for (int i = startGrid.y; i < startGrid.y + size.y; i++)
		{
			for (int j = startGrid.x; j < startGrid.x + size.x; j++)
			{
				if (m_isEquipped[j, i])
				{
					// 中身があるときはfalse
					return false;
				}
			}
		}

		// スペースが空いているときは中身が入っていることにする
		for (int i = startGrid.y; i < startGrid.y + size.y; i++)
		{
			for (int j = startGrid.x; j < startGrid.x + size.x; j++)
			{
				m_isEquipped[j, i] = true;
			}
		}

		return true;
	}

	// stringによる参照のため必要な関数(StashControllerのUpdate)
	[PunRPC]
	void RequestTreasureData(int requestId)
	{
		PhotonView view = PhotonView.Find(requestId);

		GetComponent<TreasureBoxItem>().Open();
		Debug.Log("view.RPC s : " + view);
		view.RPC("ReceiveInventoryData", view.Owner, GetInfo(), GetItemList());
	}

	// stringによる参照のため必要な関数(StashControllerのReturnItemList)
	[PunRPC]
	void RequestCopyItemList(List<ItemList> list)
	{
		CopyItemList(list);
	}

	// スタコンのUpdate
	[PunRPC]
	void SetNowOpen(bool isOpen)
	{
		m_isNowOpen = isOpen;
	}

	[PunRPC]
	void SetLock(bool isLock)
	{
		m_isLock = isLock;
	}

	[PunRPC]
	void IsOpen()
	{
		m_isOpened = true;
	}

	private void CopyItemList(List<ItemList> list)
	{
		m_itemList = new List<ItemList>(list);
	}

	public void Open()
	{
		m_anime.SetBool("Open", true);
	}
}
