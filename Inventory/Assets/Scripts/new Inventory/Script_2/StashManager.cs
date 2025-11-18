using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public struct Grid
{
	private GameObject m_grid;	// マス目のオブジェクト
	private bool m_info;		// 中身が埋まっているか

	private Grid(GameObject grid = null, bool info = false)
	{
		m_grid = grid;
		m_info = info;
	}

	// 枠の情報をセット
	public void SetGrid(GameObject grid)
	{
		m_grid = grid;
	}

	// 枠のTransformを取得
	public Transform GetTransform()
	{
		return m_grid.transform;
	}

	// ポインターが重なっているか
	public bool OnPointer()
	{
		return m_grid.GetComponent<GridIcon>().OnPointer();
	}

	// 中身があるかを取得
	public bool GetInfo()
	{
		return m_grid.GetComponent<GridIcon>().GetOnFillUi();
	}

	// 中身の状態を変える
	public void SetInfo(bool info)
	{
		m_grid.GetComponent<GridIcon>().SetUi(info);
	}

	// gridtypeを設定する
	public void SetGridType(GridType type)
	{
		m_grid.GetComponent<GridIcon>().SetType(type);
	}
}

public class StashManager : MonoBehaviour
{
	// スタッシュ用サイズ
	private int m_stashWidth = 5;
	private int m_stashHeight = 10;

	// インベントリ用サイズ
	private int m_inventoryWidth = 5;
	private int m_inventoryHeight = 5;

	[SerializeField] GameObject m_stashGridParent;  // スタッシュのマス目の親オブジェクト
	[SerializeField] GameObject m_inventoryGridParent;	// インベントリのマス目の親オブジェクト

	private Grid[,] m_stashGridList;
	private Grid[,] m_inventoryGridList;
	private Transform m_moveItemTransform;
	private GridType m_checkType = GridType.Empty;
	
	[SerializeField] GameObject m_stashUiParent;

	[SerializeField] GameObject m_stashUi;
	[SerializeField] GameObject m_inventoryUi;

	// プレハブ
	[SerializeField] GameObject m_stashUiPrefab;
	[SerializeField] GameObject m_inventoryUiPrefab;

    // インベントリの種類とサイズ情報
    [SerializeField] Info_InventorySize m_inventoryInfo;

    // アイテムリスト
    private List<ItemList> m_itemList = new List<ItemList>();
    // 自分以外のアイテムリスト
    private List<ItemList> m_otherItemList = new List<ItemList>();
	// 売却予定のアイテムリスト
	private List<ItemList> m_sellItemList = new List<ItemList>();

	// UIを表示するときの座標用
	[SerializeField] Transform m_inventoryPos;
	[SerializeField] Transform m_stashPos;

	// デバッグ用アイテムリスト
	[SerializeField] List<GameObject> items = new List<GameObject>();
	// 自分の親を保管
	[SerializeField] GameObject m_parent;

	// テスト用
	[SerializeField] bool m_isPlayer = true;
	private bool m_isScavenger = false;

	// アイテムのエクセルデータ
	[SerializeField] ExcelData m_data;
	private int m_id;

	// プレイヤーの所持金やアイテムの情報
	[SerializeField] Info_Money m_infoMoney;
	[SerializeField] Info_CurrentItem m_infoItem;

	// 現在の状態が購入予定か（true : 購入, false : 売却）
	private bool m_isBuyMode = true;

	// 交渉予定の金額
	private int m_priceNegotiation;

	// ショップ用のマネージャーかどうか
	[SerializeField] bool m_isShop = false;

	// 購入時にアイテムの情報を表示するようのUI
	[SerializeField] Image m_buyItemIcon;
	[SerializeField] TextMeshProUGUI m_price;
	[SerializeField] TextMeshProUGUI m_itemName;

	// 購入予定のアイテムのアイコン
	private Color m_color;
	// 購入予定のアイテムのオブジェクト
	private ItemList m_buyItem;
	// ショップのボタン関係を管理しているオブジェクト
	private ShopInfoList m_shopButtonInfo;

	// ---- テスト用 ----
	[SerializeField] Info_InventorySize m_infoStash;
	[SerializeField] Info_InventorySize m_infoInventory;

	[SerializeField] TextMeshProUGUI m_text;

	// Start is called before the first frame update
	void Start()
    {
		// アイテム移動用のオブジェクト
		m_moveItemTransform = GameObject.FindWithTag("moveItemTransform").transform;
		m_shopButtonInfo = GameObject.FindWithTag("shopInfoList").GetComponent<ShopInfoList>();

		if (m_isPlayer)
		{
			// インベントリのサイズを取得
			m_inventoryWidth = m_infoInventory.GetSize.x;
			m_inventoryHeight = m_infoInventory.GetSize.y;
			CreateInventory(GridType.Inventory);
		}

		if(m_isShop)
		{
			m_stashWidth = m_infoStash.GetSize.x;
			m_stashHeight = m_infoStash.GetSize.y;
			m_color = m_buyItemIcon.GetComponent<Image>().color;
			CreateInventory(GridType.Stash);
			m_text.SetText("Money : " + m_infoMoney.GetCurrentMoney().ToString());
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (!m_isPlayer) return;

		if (Input.GetMouseButtonUp(0))
		{
			SetItem();
			// 未選択状態に戻す
			m_checkType = GridType.Empty;
		}

		if(m_isShop)
		{
			// デバッグ用
			if (Input.GetKeyDown("1")) AddItemInventory();
		}
    }

	// アイテムを空き枠にセットする
	public void SetItem()
	{
		// 移動中のアイテムがないときは無視
		if (m_moveItemTransform.childCount <= 0) return;

		// アイテム移動用のオブジェクトの中身を確認
		GameObject item = m_moveItemTransform.GetChild(0).gameObject;

		// 購入モードで相手のインベントリにアイテムを入れようとした時は元の場所に戻す
		if (m_isShop && m_isBuyMode && m_checkType == GridType.Stash)
		{ 
			item.GetComponent<Item_Object>().PointerUp(false);
			return;
		}

		Grid[,] list = null;
		int height = 0;
		int width = 0;

		if (m_checkType == GridType.Empty)
		{
			// マス目を選択していないとき
			item.GetComponent<Item_Object>().PointerUp(false);
			return;
		}

		SearchEqualType(ref list, ref height, ref width);

		// リストを回す
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				// カーソルの重なっているマスが空の時
				if (list[j, i].OnPointer() && !list[j, i].GetInfo())
				{
					if (CheckSpace(new Vector2Int(j, i), item.GetComponent<Item_Object>().GetSize(), item.GetComponent<Item_Object>().GetEquipValue(), false))
					{
						// 移動先の子オブジェクトに設定する
						item.GetComponent<Item_Object>().PointerUp(true, list[j, i].GetTransform());
						// 基点のインデックスを保持
						item.GetComponent<Item_Object>().SetGridIndex(new Vector2Int(j, i));
						if (item.GetComponent<Item_Object>().GetEquipValue())
						{
							// 装備されていたら装備状態を解除する
							item.GetComponent<Item_Object>().SetEquipValue(false, item.GetComponent<Item_Object>().GetGridType() == GridType.Inventory);
						}
						else if (m_checkType != item.GetComponent<Item_Object>().GetGridType())
						{
							// アイテムが枠を超えて移動していた時
							// --- アイテムリストの管理 ---
							if (m_checkType == GridType.Inventory)
							{
								AddMyList(item);
							}
							else
							{
								AddOtherList(item);
							}
							// 現在のgridtypeを保管
							item.GetComponent<Item_Object>().SetType(m_checkType);
						}
						else
						{
							// 同じ枠内でアイテムが移動していた時
							switch (m_checkType)
							{
								case GridType.Stash:
									if(m_isShop)
									{
										// ショップ用の時
										m_sellItemList[item.GetComponent<Item_Object>().GetIndex()].SetGridIndex(item.GetComponent<Item_Object>().GetGridIndex());
									}
									else
									{
										// それ以外の時
										m_otherItemList[item.GetComponent<Item_Object>().GetIndex()].SetGridIndex(item.GetComponent<Item_Object>().GetGridIndex());
									}
									break;

								case GridType.Inventory:
									m_itemList[item.GetComponent<Item_Object>().GetIndex()].SetGridIndex(item.GetComponent<Item_Object>().GetGridIndex());
									break;
							}
						}
					}
					else
					{ 
						// 元の位置に戻す
						item.GetComponent<Item_Object>().PointerUp(false);
					}

					return;
				}
			}
		}
		// 選択マスが空いていないとき
		item.GetComponent<Item_Object>().PointerUp(false);
	}

	// アイテムが入るスペースを確認
	private bool CheckSpace(Vector2Int startGrid, Vector2Int size, bool isEquip, bool isQuickMove = false)
	{
		Grid[,] list = null;
		int height = 0;
		int width = 0;

		// 装備品をショートカットで外す場合はインベントリに入る
		if(isEquip && isQuickMove)
		{
			list = m_inventoryGridList;
			SetInventorySize(ref height, ref width);
		}
		else
		{
			if (isQuickMove) SearchOtherType(ref list, ref height, ref width);
			else SearchEqualType(ref list, ref height, ref width);
		}
		
		// 枠外にはみ出すときはそもそも確認しない
		if (startGrid.x + (size.x - 1) >= width) return false;
		if (startGrid.y + (size.y - 1) >= height) return false;

		// 中身を確認
		for (int i = startGrid.x; i < startGrid.x + size.x; i++)
		{
			for(int j = startGrid.y; j < startGrid.y + size.y; j++)
			{
				if (list[i, j].GetInfo())
				{
					// 中身があるときはfalse
					return false;
				}
			}
		}

		// スペースが空いているときは中身が入っていることにする
		for (int i = startGrid.x; i < startGrid.x + size.x; i++)
		{
			for (int j = startGrid.y; j < startGrid.y + size.y; j++)
			{
				list[i, j].SetInfo(true);
			}
		}

		return true;
	}

	// 指定したマスの状態を変更する
	public void MoveItem(
		GameObject item,
		Vector2Int basePos,
		Vector2Int size,
		bool info,
		GridType type,
		bool changeList = false
	)
	{
		// アドレスコピー
		Grid[,] list = null;
		List<ItemList> items = new List<ItemList> ();

		switch (type)
		{
			case GridType.Stash:
				list = m_stashGridList;
				items = m_otherItemList;
				break;

			case GridType.Inventory:
				list = m_inventoryGridList;
				items = m_itemList;
				break;
		}

		// スペースが空いているときは中身が入っていることにする
		for (int i = basePos.x; i < basePos.x + size.x; i++)
		{
			for (int j = basePos.y; j < basePos.y + size.y; j++)
			{
				list[i, j].SetInfo(info);
			}
		}

		// ショートカットで装備をした時にリストの内容を変更する
		if(changeList)
		{
			if(info) 
			{
				AddItemList(item, items);
			}
			else 
			{ 
				RemoveItemList(item, items);
			}
		}
	}

	// アイテムを置けるかを探す
	public void StartSet(GridType type)
	{
		m_checkType = type;
	}

	// インベントリのアイテムリストを返す
	public List<ItemList> GetItemList()
	{
		return m_itemList;
	}

	// --- ショートカット ---
	// インベントリ ⇔ スタッシュ（空き枠を探して自動で入れ替える）
	public bool QuickMoveItem(
		GridType type, 
		GameObject item, 
		bool isEquip, 
		bool isAdd = false, 
		bool isTest = false
	)
	{
		// 購入モードでは無視
		if (!isTest && m_isBuyMode)
		{
			// アイテムが入るスペースがないので元の位置に戻す
			item.GetComponent<Item_Object>().PointerUp(false);
			return false;
		}

		return CheckGrid(type, item, isEquip, isAdd, isTest);
    }

	// 指定したタイプのマス目を探索してリストを管理する
	
	private bool CheckGrid(
		GridType type,		// アイテムのGridType
		GameObject item,	// item自身
		bool isEquip,		// 装備されているか,
		bool isAdd = false, // 移動ではなく追加か
		bool isTest = false // デバッグ用アイテムか
	)
	{
		Vector2Int size = item.GetComponent<Item_Object>().GetSize();

		// アイテムの入っているマス目のタイプに応じて探索する枠を変える
		m_checkType = type;

		// 全マス探索
		Grid[,] list = null;
		int height = 0;
		int width = 0;

		// 装備されていない => アイテムの現在の枠タイプと違うタイプの枠を探索
		if (!isEquip) SearchOtherType(ref list, ref height, ref width);
		else
		{
			// 装備されている => インベントリに入れる
			list = m_inventoryGridList;
			SetInventorySize(ref height, ref width);
		}

		// リストを回す
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (CheckSpace(new Vector2Int(j, i), size, isEquip, true))
				{
					// 移動先の子オブジェクトに設定する
					if(m_isBuyMode)
					{
						item.GetComponent<Item_Object>().MoveItem(list[j, i].GetTransform());
					}
					else
					{
						item.GetComponent<Item_Object>().PointerUp(true, list[j, i].GetTransform());
					}
					// 基点のインデックスを保持
					item.GetComponent<Item_Object>().SetGridIndex(new Vector2Int(j, i));
					// 装備をショートカットで外すとき
					if (item.GetComponent<Item_Object>().GetEquipValue())
					{
						// 装備されていたら装備状態を解除する
						item.GetComponent<Item_Object>().SetEquipValue(false);
						// アイテムリストに追加する
						AddItemList(item, m_itemList);
						// gridTypeをインベントリに変更する
						item.GetComponent<Item_Object>().SetType(GridType.Inventory);
					}
					else
					{
						// 現在の枠のgridtypeを保管
						GridType m = (GridType)((int)m_checkType + 1 > 1 ? 0 : 1);
						item.GetComponent<Item_Object>().SetType(m);

						// --- アイテムリストの管理 ---
						if (m_checkType == GridType.Inventory)
						{
							if (isTest) AddItemList(item, m_otherItemList);
							else AddOtherList(item);
						}
						else
						{
							if (isTest) AddItemList(item, m_itemList);
							else AddMyList(item);
						}
					}
					return true;
				}
			}
		}

		if (isAdd)
		{
			item.GetComponent<Item_Object>().Remove();
		}
		else
		{
			// アイテムが入るスペースがないので元の位置に戻す
			item.GetComponent<Item_Object>().PointerUp(false);
		}

		return false;
	}

	// アイテムを外部から追加する
	public void AddItem(GridType type, GameObject item, bool isTest = false)
	{
		QuickMoveItem(type, item, false, true, isTest);
    }

	// アイテムリストをコピーする
	public void CopyItemList(List<ItemList> list)
	{
		m_itemList = new List<ItemList>(list);
	}

	// インベントリの種類とサイズの情報を渡す
	public Info_InventorySize GetInventoryInfo()
	{
		return m_inventoryInfo;
    }

    // アイテム欄を作成する
    public void CreateStashUi(Info_InventorySize info, List<ItemList> itemList)
	{
		// UIを表示
		switch (info.GetInventoryType)
		{
			case Info_InventorySize.InventoryType.Inventory:
				m_stashUi = Instantiate(m_inventoryUiPrefab, m_stashPos);
				break;

			case Info_InventorySize.InventoryType.Stash:
				m_stashUi = Instantiate(m_stashUiPrefab, m_stashPos);
				break;
		}

		// アイテムリストをコピー
		m_otherItemList = new List<ItemList>(itemList);
		// インベントリの情報を取得
		m_stashHeight = info.GetSize.y;
        m_stashWidth = info.GetSize.x;
		// インベントリの枠の親オブジェクトを取得
        m_stashGridParent = m_stashUi.GetComponent<Inventory_Parent>().GetContent;
		
		// スタッシュを作成
		CreateInventory(GridType.Stash);
		// リストをUIに反映
		foreach (var item in m_otherItemList)
		{
			GameObject prefab = item.GetPrefab();
			// リストからオブジェクトを生成
			GameObject obj = Instantiate(prefab, m_moveItemTransform);

			if (item.IsEquip())
			{
				// 装備されていたアイテム
				m_stashUi.GetComponent<Inventory_Parent>().GetEquipments.GetComponent<Player_Equipment>().QuickEquip(obj);
            }
			else
			{
				// 普通のアイテムのマス目を埋める
				MoveItem(obj, item.GetGridIndex(), obj.GetComponent<Item_Object>().GetSize(), true, GridType.Stash);
			}
		}
		OpenUi();
	}

	// 内部的なインベントリを作成する
	private void CreateInventory(GridType type)
	{
		Grid[,] list = null;
		int height = 0;
		int width = 0;
		GameObject parent = null;
        switch (type)
		{
			case GridType.Inventory:
				// インベントリ用マス目の配列を作成
				list = new Grid[m_inventoryWidth, m_inventoryHeight];
				m_inventoryGridList = list;
				SetInventorySize(ref height, ref width);
				parent = m_inventoryGridParent;
                break;

			case GridType.Stash:
				// スタッシュ用マス目の配列を作成
				list = new Grid[m_stashWidth, m_stashHeight];
				m_stashGridList = list;
                SetStashSize( ref height, ref width );
				parent = m_stashGridParent;
                break;
        }

        // スタッシュ用配列を作成
        // 配列とマス目の状態を合わせる
        int count = 0;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                // オブジェクトを追加
                GameObject g = parent.transform.GetChild(count).gameObject;
                list[j, i].SetGrid(g);
                count++;
                // 中身を空にする
                list[j, i].SetInfo(false);
                list[j, i].SetGridType(type);
            }
        }
    }

    // インベントリのUIを開閉(Tab用)
	public void ManageUiActiveInfo()
	{
		if (m_stashUiParent.activeSelf)
		{
			// 閉じるときにスタッシュの変更を相手に渡す
			m_parent.GetComponent<PlayerMove>().ReturnItemList(m_otherItemList);
            m_stashUiParent.SetActive(false);
			IsScavenger(false);
			m_otherItemList = null;
			if (m_stashUi)
			{
				Destroy(m_stashUi.gameObject);
			}
			m_stashGridList = null;
		}
		else
		{
			m_stashUiParent.SetActive(true);
        }
    }

	private void OpenUi()
	{
        m_stashUiParent.SetActive(true);
    }

	public void IsScavenger(bool value)
	{
		m_isScavenger = value;
	}

	// 自分のアイテムリストに追加する
	private void AddMyList(GameObject item)
	{
		if (!m_isBuyMode)
		{
			RemoveItemList(item, m_sellItemList);
			AddItemList(item, m_itemList);
		}
		else
		{
			RemoveItemList(item, m_otherItemList);
			AddItemList(item, m_itemList);
		}
	}

	// 相手のアイテムリストに追加する
	private void AddOtherList(GameObject item)
	{
		if (!m_isBuyMode)
		{
			RemoveItemList(item, m_itemList); 
			AddItemList(item, m_sellItemList);
		}
		else
		{
			RemoveItemList(item, m_itemList);
			AddItemList(item, m_otherItemList);
		}
	}

	// リストに追加する
	private void AddItemList(GameObject item, List<ItemList> list)
	{
		ItemList info = new ItemList();
		// マス目座標を保存
		info.SetGridIndex(item.GetComponent<Item_Object>().GetGridIndex());
		// プレハブを取得
		info.SetPrefab(item.GetComponent<Item_Object>().GetPrefab());
		// アクティブなオブジェクトを変更
		info.SetActiveObject(item);
		// リストのインデックスを保存
		info.ChangeIndex(list.Count);
		// アイテムリストに保存
		list.Add(info);
	}

	// リストから除外する(リストを指定する)
	private void RemoveItemList(GameObject item, List<ItemList> list)
	{
		list.RemoveAt(item.GetComponent<Item_Object>().GetIndex());
		for (int i = 0; i < list.Count; i++)
		{
			list[i].ChangeIndex(i);
		}
	}

	// リストから削除する(アイテムが今入っている枠のタイプを指定する)
	public void SelectRemoveItemList(GameObject item)
	{
		List<ItemList> list = new List<ItemList>();
		switch (item.GetComponent<Item_Object>().GetGridType())
		{
			case GridType.Inventory:
				list = m_itemList;
				break;

			case GridType.Stash:
				list = m_otherItemList;
				break;
		}

		RemoveItemList(item, list);
	}

	// インベントリが開いているかを取得
	public bool IsOpen()
	{
		return m_stashUiParent.activeSelf;
	}

	// ----- ボタンの処理 ------
	public void Deal()
	{
		if(m_isBuyMode)
		{
			BuyItem();
		}
		else
		{
			SellItem();
		}
	}
	// 売却用ボタン
	public void SellItem()
	{
		if (!m_isBuyMode && m_sellItemList.Count > 0)
		{
			int add = 0;

			// 売却予定のアイテムを削除
			foreach (ItemList item in m_sellItemList)
			{
				// 売却したアイテムが入っていたマスを空ける
				Vector2Int basePos = item.GetGridIndex();
				Vector2Int size = item.GetPrefab().GetComponent<Item_Object>().GetSize();
				for (int i = basePos.x; i < basePos.x + size.x; i++)
				{
					for (int j = basePos.y; j < basePos.y + size.y; j++)
					{
						m_stashGridList[i, j].SetInfo(false);
					}
				}
				// 売値を加算
				add += item.GetPrefab().GetComponent<Item_Object>().GetValue();
				// インスタンス化されたオブジェクトを削除する
				item.DeleteActiveObject();
			}
			// 全ての売却アイテムのオブジェクトを処理し終わったら売却用リストをリセット
			m_sellItemList.Clear();
			// 売却した分の金額を追加する
			m_infoMoney.AddMoney(add);

			m_text.SetText("Money : " + m_infoMoney.GetCurrentMoney().ToString());
		}
	}

	// 売却用アイテムの総額を計算する
	private int CalcSoldValue()
	{
		int add = 0;

		// 売却予定のアイテムを削除
		foreach (ItemList item in m_sellItemList)
		{
			// 売却したアイテムが入っていたマスを空ける
			Vector2Int basePos = item.GetGridIndex();
			Vector2Int size = item.GetPrefab().GetComponent<Item_Object>().GetSize();
			for (int i = basePos.x; i < basePos.x + size.x; i++)
			{
				for (int j = basePos.y; j < basePos.y + size.y; j++)
				{
					m_stashGridList[i, j].SetInfo(false);
				}
			}
			// 売値を加算
			add += item.GetPrefab().GetComponent<Item_Object>().GetValue();
		}

		return add;
	}

	// 購入用ボタン
	public void BuyItem()
	{
		// 購入予定アイテムを選択していない時は無視
		if (!m_buyItem) return;

		if (
			m_isBuyMode && 
			m_infoMoney.GetCurrentMoney() >= m_buyItem.GetActiveObject().GetComponent<Item_Object>().GetValue()
		)
		{
			// アイテムを移動する
			GameObject item = m_buyItem.GetActiveObject();

			if(CheckGrid(item.GetComponent<Item_Object>().GetGridType(), item, false))
			{
				// お金を消費
				m_infoMoney.UseMoney(m_buyItem.GetActiveObject().GetComponent<Item_Object>().GetValue());

				// 購入予定のアイテムをリセットする
				ResetBuyItemInfo();
			}

			m_text.SetText("Money : " + m_infoMoney.GetCurrentMoney().ToString());
		}
		else if(m_infoMoney.GetCurrentMoney() < m_buyItem.GetActiveObject().GetComponent<Item_Object>().GetValue())
		{
			// 所持金額が足りない場合

		}
	}

	// 購入モードに切り替える
	public void ChangeBuyMode()
	{
		// すでに購入モードの時は無視する
		if (m_isBuyMode) return;

		if (m_sellItemList.Count != 0)
		{
			List<ItemList> list = new List<ItemList>(m_sellItemList);
			// 売却用アイテムリストが空じゃないとき
			foreach (ItemList item in list)
			{
				// アイテムをインベントリに返す
				QuickMoveItem(GridType.Stash, item.GetActiveObject(), false, true, false);
			}
			m_sellItemList.Clear();
		}
		// 購入モードに切り替える
		m_isBuyMode = true;

		// 購入モードはとりあえずリストの最初のショップを表示する
		m_shopButtonInfo.GetShopInfo(0).SetShopItem();
	}

	// 売却モードに切り替える
	public void ChangeSellMode()
	{
		// すでに売却モードの時は無視する
		if (!m_isBuyMode) return;

		// 売却ボタンが隠れていた時用
		m_shopButtonInfo.ShowDealButton();

		// 売却モードに切り替える
		m_isBuyMode = false;

		// 購入予定のアイテムが選択されている時は元に戻す
		ResetBuyItemInfo();

		// 内部的なリストをリセット
		m_sellItemList.Clear();

		CreateNewShop();
	}
	// ---------------------------------

	// ショップ画面作成の共通の処理
	private void CreateNewShop()
	{
		// 表示しているUIを削除する
		Destroy(m_stashUi.gameObject);
		// 新しくUIを表示する
		m_stashUi = Instantiate(m_stashUiPrefab, m_stashPos);
		// インベントリの枠の親オブジェクトを取得
		m_stashGridParent = m_stashUi.GetComponent<Inventory_Parent>().GetContent;
		// 内部的な配列を作成
		CreateInventory(GridType.Stash);
	}

	// 探索準備(選択されたGridType == 探索するGridType)
	private void SearchEqualType(ref Grid[,] list, ref int height, ref int width)
	{
		switch (m_checkType)
		{
			case GridType.Inventory:
				list = m_inventoryGridList;
				SetInventorySize(ref height, ref width);
				break;

			case GridType.Stash:
				list = m_stashGridList;
				SetStashSize(ref height, ref width);
				break;
		}
	}
	// 探索準備(選択されたGridType != 探索するGridType)
	private void SearchOtherType(ref Grid[,] list, ref int height, ref int width)
	{
		switch (m_checkType)
		{
			case GridType.Inventory:
				list = m_stashGridList;
				SetStashSize(ref height, ref width);
				break;

			case GridType.Stash:
				list = m_inventoryGridList;
				SetInventorySize(ref height, ref width);
				break;
		}
	}

	// 配列の縦横を設定する
	private void SetStashSize(ref int height, ref int width)
	{
		height = m_stashHeight;
		width = m_stashWidth;
	}
	private void SetInventorySize(ref int height, ref int width)
	{
		height = m_inventoryHeight;
		width = m_inventoryWidth;
	}

	// 装備枠のアイテムを設定する
	public void SetEquipment(GameObject item, bool value, bool isMine = false)
	{
		// 誰の持ち物か判断する
		List<ItemList> list = new List<ItemList>();
		if (isMine)
		{
			list = m_itemList;
			if (item.GetComponent<Item_Object>().GetGridType() == GridType.Stash)
			{
				RemoveItemList(item, m_otherItemList);
				AddItemList(item, m_itemList);
				item.GetComponent<Item_Object>().SetType(GridType.Inventory);
			}
		}
		else
		{
			list = m_otherItemList;
			if (item.GetComponent<Item_Object>().GetGridType() == GridType.Inventory)
			{
				RemoveItemList(item, m_itemList);
				AddItemList(item, m_otherItemList);
				item.GetComponent<Item_Object>().SetType(GridType.Stash);
			}
		}

		// リストに装備状況を保存
		list[item.GetComponent<Item_Object>().GetIndex()].SetEquipInfo(value);
	}

	// 販売アイテムUIを作成
	public void SetShopItemUI(Info_InventorySize info, List<ItemList> itemList, bool isSet)
	{
		if (!m_isBuyMode) return;
		// 表示しているUIを削除する
		CreateNewShop();
		// 商品リストをリセット
		m_otherItemList.Clear();

		foreach (ItemList item in itemList)
		{
			GameObject g = Instantiate(item.GetPrefab(), m_moveItemTransform);
			// アイテムをスタッシュに並べる
			CheckGrid(GridType.Inventory, g, false, true, true);
		}

		ResetBuyItemInfo();
	}

	// 購入前にアイテムの情報を表示する
	public void SetBuyItemInfo(Sprite icon, string name, int price, int index)
	{
		// 最初に選ばれたアイテムの時だけ走る
		if(m_color.a == 0)
		{
			m_color.a = 255;
			m_buyItemIcon.color = m_color;
		}
		else
		{
			// それまで選択されていたアイテムを選択状態から外す
			m_buyItem.GetActiveObject().GetComponent<Item_Object>().RemoveSelected();
			// 解放したマス目を埋めなおす
			m_buyItem.GetActiveObject().GetComponent<Item_Object>().ResetItem();
		}
		// アイテムのオブジェクトを購入予定にする
		m_buyItem = m_otherItemList[index];
		m_buyItem.GetActiveObject().GetComponent<Item_Object>().PointerDownForShop();

		// アイテムの情報を表示する
		m_buyItemIcon.sprite = icon;
		m_itemName.SetText(name);
		m_price.SetText(price.ToString());
		if(m_infoMoney.GetCurrentMoney() < m_buyItem.GetActiveObject().GetComponent<Item_Object>().GetValue())
		{
			// 所持金額が足りていない場合
			m_price.color = Color.red;
			m_shopButtonInfo.HideDealButton();
		}
		else
		{
			// 足りている場合
			m_price.color = Color.white;
			m_shopButtonInfo.ShowDealButton();
		}
	}

	// トレーダーを入れ替えた時に購入予定のアイテムの情報をリセット
	private void ResetBuyItemInfo()
	{
		// アイテムアイコン用のUIを透明にする
		m_color.a = 0;
		m_buyItemIcon.color = m_color;
		m_buyItemIcon.sprite = null;
		// テキストをリセット
		m_itemName.SetText("");
		m_price.SetText("");
		if(m_buyItem)
		{
			if(m_isBuyMode)
			{
				// 解放したマス目を埋めなおす
				m_buyItem.GetActiveObject().GetComponent<Item_Object>().FillGrid();
			}
			else
			{
				// アイテムを元の位置に戻す
				m_buyItem.GetActiveObject().GetComponent<Item_Object>().ResetItem();
			}
			// アイテムをリセット
			m_buyItem = null;
		}
	}

	public bool IsBuyMode()
	{
		return m_isBuyMode;
	}

	// ----- デバッグ用関数 -----
	public void AddItemInventory()
	{
		m_id = Random.Range(0, items.Count);
		GameObject item = Instantiate(items[m_id], m_moveItemTransform);
		item.GetComponent<Item_Object>().ChangeParent(m_moveItemTransform);
		AddItem(GridType.Stash, item, true);
	}

	public void AddItemStash()
	{
		if (!m_stashUi) return;
		m_id = Random.Range(0, items.Count);
		GameObject item = Instantiate(items[m_id], m_moveItemTransform);
		item.GetComponent<Item_Object>().ChangeParent(m_moveItemTransform);
		AddItem(GridType.Inventory, item, true);
	}
	// ----------------------------
}