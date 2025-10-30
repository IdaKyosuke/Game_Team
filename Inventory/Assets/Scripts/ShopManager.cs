using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Progress;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class ShopManager : MonoBehaviour
{
	// スタッシュ用サイズ
	[SerializeField] int m_stashWidth = 5;
	[SerializeField] int m_stashHeight = 10;

	// インベントリ用サイズ
	[SerializeField] int m_inventoryWidth = 5;
	[SerializeField] int m_inventoryHeight = 5;

	[SerializeField] GameObject m_stashGridParent;  // スタッシュのマス目の親オブジェクト
	[SerializeField] GameObject m_inventoryGridParent;  // インベントリのマス目の親オブジェクト

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

	// アイテムリスト
	[SerializeField] List<ItemList> m_itemListMine = new List<ItemList>();

	// アイテムのエクセルデータ
	[SerializeField] ExcelData m_data;
	private int m_id;

	private bool m_isScavenger = false;

	// 自分の親を保管
	[SerializeField] GameObject m_parent;

	// プレイヤーの所持金やアイテムの情報
	[SerializeField] Info_Money m_infoMoney;
	[SerializeField] Info_CurrentItem m_infoItem;

	// 現在の状態が購入予定か（true : 購入, false : 売却）
	private bool m_isBuyMode = true;

	// 交渉予定の金額
	private int m_priceNegotiation;

	// ---- テスト用 ----
	[SerializeField] Info_InventorySize m_infoStash;
	[SerializeField] Info_InventorySize info2;
	// デバッグ用アイテムリスト
	[SerializeField] List<GameObject> items = new List<GameObject>();





	[SerializeField] TextMeshProUGUI m_text;


	// Start is called before the first frame update
	void Start()
	{
		// アイテム移動用のオブジェクト
		m_moveItemTransform = GameObject.FindWithTag("moveItemTransform").transform;

		CreateInventory(GridType.Inventory);
		CreateInventory(GridType.Stash);

		m_text.SetText("Money : " + m_infoMoney.GetCurrentMoney().ToString());
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			SetItem();
			// 未選択状態に戻す
			m_checkType = GridType.Empty;
		}
		

		// デバッグ用
		if(Input.GetKeyDown("1"))
		{
			AddItemInventory();
		}


		m_text.SetText("Money : " + m_infoMoney.GetCurrentMoney().ToString());
	}

	// アイテムを空き枠にセットする
	public void SetItem()
	{
		// 移動中のアイテムがないときは無視
		if (m_moveItemTransform.childCount <= 0) return;

		// アイテム移動用のオブジェクトの中身を確認
		GameObject item = m_moveItemTransform.GetChild(0).gameObject;

		// 購入モードで相手のインベントリにアイテムを入れようとした時は元の場所に戻す
		if(m_isBuyMode && m_checkType == GridType.Stash) item.GetComponent<Item_Object>().PointerUp(false);

		Grid[,] list = null;
		int height = 0;
		int width = 0;

		SearchEqualType(list, ref height, ref width);

		//switch (m_checkType)
		//{
		//	case GridType.Stash:
		//		list = m_stashGridList;
		//		height = m_stashHeight;
		//		width = m_stashWidth;
		//		break;

		//	case GridType.Inventory:
		//		list = m_inventoryGridList;
		//		height = m_inventoryHeight;
		//		width = m_inventoryWidth;
		//		break;

		//	// GridType.Empty
		//	case GridType.Empty:
		//		// マス目を選択していないとき
		//		item.GetComponent<Item_Object>().PointerUp(false);
		//		return;
		//}

		if(m_checkType == GridType.Empty)
		{
			// マス目を選択していないとき
			item.GetComponent<Item_Object>().PointerUp(false);
			return;
		}

		// リストを回す
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				// カーソルの重なっているマスが空の時
				if (list[j, i].OnPointer() && !list[j, i].GetInfo())
				{
					if (CheckSpace(
						new Vector2Int(j, i),
						item.GetComponent<Item_Object>().GetSize(),
						item.GetComponent<Item_Object>().GetEquipValue(),
						false
						))
					{
						// 移動先の子オブジェクトに設定する
						item.GetComponent<Item_Object>().PointerUp(
							true,
							list[j, i].GetTransform()
							);
						// 基点のインデックスを保持
						item.GetComponent<Item_Object>().SetGridIndex(new Vector2Int(j, i));
						// 現在のgridtypeを保管
						item.GetComponent<Item_Object>().SetType(m_checkType);
						// 装備の状態を変更
						item.GetComponent<Item_Object>().SetEquipValue(false);

						// --- アイテムリストの管理 ---
						if (m_checkType == GridType.Inventory)
						{
							AddMyList(item);
						}
						else
						{
							AddOtherList(item);
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
		if (isEquip && isQuickMove)
		{
			list = m_inventoryGridList;
			height = m_inventoryHeight;
			width = m_inventoryWidth;
		}
		else
		{
			if (isQuickMove)
			{
				//switch (m_checkType)
				//{
				//	case GridType.Inventory:
				//		list = m_stashGridList;
				//		height = m_stashHeight;
				//		width = m_stashWidth;
				//		break;

				//	case GridType.Stash:
				//		list = m_inventoryGridList;
				//		height = m_inventoryHeight;
				//		width = m_inventoryWidth;
				//		break;
				//}
				SearchOtherType(list, ref height, ref width);
			}
			else
			{
				//switch (m_checkType)
				//{
				//	case GridType.Stash:
				//		list = m_stashGridList;
				//		height = m_stashHeight;
				//		width = m_stashWidth;
				//		break;

				//	case GridType.Inventory:
				//		list = m_inventoryGridList;
				//		height = m_inventoryHeight;
				//		width = m_inventoryWidth;
				//		break;
				//}

				SearchEqualType(list, ref height, ref width);
			}
		}

		// 枠外にはみ出すときはそもそも確認しない
		if (startGrid.x + (size.x - 1) >= width) return false;
		if (startGrid.y + (size.y - 1) >= height) return false;

		// 中身を確認
		for (int i = startGrid.x; i < startGrid.x + size.x; i++)
		{
			for (int j = startGrid.y; j < startGrid.y + size.y; j++)
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
		List<ItemList> items = new List<ItemList>();

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
		if (changeList)
		{
			if (info)
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
	// インベントリ ⇔ 売却スペース（空き枠を探して自動で入れ替える）
	public bool QuickMoveItem(
		GridType type, 
		GameObject item, 
		bool isEquip,
		bool isAdd = false, 
		bool isTest = false
		)
	{
		Vector2Int size = item.GetComponent<Item_Object>().GetSize();

		// アイテムの入っているマス目のタイプに応じて探索する枠を変える
		m_checkType = type;

		// 全マス探索
		Grid[,] list = null;
		int height = 0;
		int width = 0;

		if (!isEquip)
		{
			// 装備されていない => アイテムの現在の枠タイプと違うタイプの枠を探索
			//switch (m_checkType)
			//{
			//	case GridType.Inventory:
			//		list = m_stashGridList;
			//		height = m_stashHeight;
			//		width = m_stashWidth;
			//		break;

			//	case GridType.Stash:
			//		list = m_inventoryGridList;
			//		height = m_inventoryHeight;
			//		width = m_inventoryWidth;
			//		break;
			//}
			SearchOtherType(list, ref height, ref width);
		}
		else
		{
			// 装備されている => インベントリに入れる
			list = m_inventoryGridList;
			height = m_inventoryHeight;
			width = m_inventoryWidth;
		}
		// リストを回す
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (CheckSpace(new Vector2Int(j, i), size, isEquip, true))
				{
					// 移動先の子オブジェクトに設定する
					item.GetComponent<Item_Object>().PointerUp(
						true,
						list[j, i].GetTransform()
						);
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
							if (isTest)
							{
								AddItemList(item, m_otherItemList);
							}
							else
							{
								AddOtherList(item);
							}
						}
						else
						{
							if (isTest)
							{
								AddItemList(item, m_itemList);
							}
							else
							{
								AddMyList(item);
							}
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
		m_stashHeight = info.GetSize.x;
		m_stashWidth = info.GetSize.y;
		// インベントリの枠の親オブジェクトを取得
		m_stashGridParent = m_stashUi.GetComponent<Inventory_Parent>().GetContent;

		// スタッシュを作成
		CreateInventory(GridType.Stash);
		// リストをUIに反映
		foreach (var item in m_otherItemList)
		{
			GameObject prefab = item.GetComponent<ItemList>().GetPrefab();
			// リストからオブジェクトを生成
			GameObject obj = Instantiate(
				prefab,
				m_moveItemTransform
				);

			if (item.GetComponent<ItemList>().IsEquip())
			{
				// 装備されていたアイテム
				m_stashUi.GetComponent<Inventory_Parent>().GetEquipments.GetComponent<Player_Equipment>().QuickEquip(obj);
			}
			else
			{
				// 普通のアイテムのマス目を埋める
				MoveItem(
					obj,
					item.GetComponent<Item_Object>().GetGridIndex(),
					obj.GetComponent<Item_Object>().GetSize(),
					true,
					GridType.Stash
					);
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
				height = m_inventoryHeight;
				width = m_inventoryWidth;
				parent = m_inventoryGridParent;
				break;

			case GridType.Stash:
				// スタッシュ用マス目の配列を作成
				list = new Grid[m_stashWidth, m_stashHeight];
				m_stashGridList = list;
				height = m_stashHeight;
				width = m_stashWidth;
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
		info.ChangeIndex(list.Count - 1);
		// アイテムリストに保存
		list.Add(info);
	}

	// 自分のアイテムリストに追加する
	private void AddMyList(GameObject item)
	{
		if (!m_isBuyMode)
		{
			AddItemList(item, m_itemList);
			RemoveItemList(item, m_sellItemList);
		}
		else
		{
			AddItemList(item, m_itemList);
			RemoveItemList(item, m_otherItemList);
		}
	}

	// 相手のアイテムリストに追加する
	private void AddOtherList(GameObject item)
	{
		if (!m_isBuyMode)
		{
			AddItemList(item, m_sellItemList);
			RemoveItemList(item, m_itemList);
		}
		else
		{
			AddItemList(item, m_otherItemList);
			RemoveItemList(item, m_itemList);
		}
	}

	// リストから除外する
	public void RemoveItemList(GameObject item, List<ItemList> list)
	{
		list.Remove(list[item.GetComponent<Item_Object>().GetIndex()]);
	}

	// インベントリが開いているかを取得
	public bool IsOpen()
	{
		return m_stashUiParent.activeSelf;
	}

	// 売却用ボタンの処理
	public void SellItem()
	{
		if(!m_isBuyMode && m_sellItemList.Count > 0)
		{
			int add = 0;
			
			// 売却予定のアイテムを削除
			foreach(ItemList item in m_sellItemList)
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
		}
	}

	// 購入用ボタンの処理
	public void BuyItem()
	{
		if (m_isBuyMode && m_infoMoney.GetCurrentMoney() >= m_priceNegotiation)
		{
			// お金を消費
			m_infoMoney.UseMoney(m_priceNegotiation);
		}
	}

	// 購入モードに切り替える
	public void ChangeBuyMode()
	{
		// すでに購入モードの時は無視する
		if (m_isBuyMode) return;

		// 購入モードに切り替える
		m_isBuyMode = true;

		if (m_sellItemList.Count != 0)
		{
			// 売却用アイテムリストが空じゃないとき
			foreach (ItemList item in m_sellItemList)
			{
				GameObject prefab = item.GetComponent<ItemList>().GetPrefab();
				// リストからオブジェクトを生成
				GameObject obj = Instantiate(
					prefab,
					m_moveItemTransform
					);

				// アイテムをインベントリに返す
				QuickMoveItem(GridType.Stash, obj, false, true, false);
			}

			m_sellItemList.Clear();
		}

		CreateNewShop();
	}

	// 売却モードに切り替える
	public void ChangeSellMode()
	{
		// すでに売却モードの時は無視する
		if (!m_isBuyMode) return;

		// 売却モードに切り替える
		m_isBuyMode = false;

		// 内部的な配列をリセット
		m_sellItemList.Clear();
		m_sellItemList = new List<ItemList>();

		CreateNewShop();
	}

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
	private void SearchEqualType(Grid[,] list, ref int height, ref int width)
	{
		switch (m_checkType)
		{
			case GridType.Inventory:
				list = m_inventoryGridList;
				height = m_inventoryHeight;
				width = m_inventoryWidth;
				break;

			case GridType.Stash:
				list = m_stashGridList;
				height = m_stashHeight;
				width = m_stashWidth;
				break;
		}
	}

	// 探索準備(選択されたGridType != 探索するGridType)
	private void SearchOtherType(Grid[,] list, ref int height, ref int width)
	{
		switch (m_checkType)
		{
			case GridType.Inventory:
				list = m_stashGridList;
				height = m_stashHeight;
				width = m_stashWidth;
				break;

			case GridType.Stash:
				list = m_inventoryGridList;
				height = m_inventoryHeight;
				width = m_inventoryWidth;
				break;
		}
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
