using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
[DefaultExecutionOrder(-50)]
public class StashManager : MonoBehaviourPunCallbacks
{
	// スタッシュ用サイズ
	private int m_stashWidth = 5;
	private int m_stashHeight = 10;

	// インベントリ用サイズ
	private int m_inventoryWidth = 5;
	private int m_inventoryHeight = 5;

	// ロビーのスタッシュ用サイズ
	[SerializeField] Info_InventorySize m_lobbyInfo;

	[SerializeField] GameObject m_stashGridParent;  // スタッシュのマス目の親オブジェクト
	[SerializeField] GameObject m_inventoryGridParent;	// インベントリのマス目の親オブジェクト

	private Grid[,] m_stashGridList;
	private Grid[,] m_inventoryGridList;

	// アイテムを一旦除ける用の場所
	[SerializeField] Transform m_moveItemTransform;
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
    [SerializeField] List<ItemList> m_itemList = new List<ItemList>();
    // 自分以外のアイテムリスト
    private List<ItemList> m_otherItemList = new List<ItemList>();
	// 売却予定のアイテムリスト
	private List<ItemList> m_sellItemList = new List<ItemList>();
	// トレーダーの販売アイテムリスト
	private List<ItemList> m_traderItemList = new List<ItemList>();
	// ロビーのスタッシュのアイテムリスト
	private List<ItemList> m_stashItemList = new List<ItemList>();

	// UIを表示するときの座標用
	[SerializeField] Transform m_inventoryPos;
	[SerializeField] Transform m_stashPos;

	// インベントリ関連の情報
	[SerializeField] Info_InventorySize m_infoStash;
	[SerializeField] Info_InventorySize m_infoInventory;

	// 自分の親を保管
	[SerializeField] GameObject m_parent;

	// プレイヤーの所持金やアイテムの情報
	[SerializeField] Info_Money m_infoMoney;
	[SerializeField] Info_CurrentItem m_infoItem;

	// 現在の状態が購入予定か（true : 購入, false : 売却）
	private bool m_isBuyMode;

	// 交渉予定の金額
	private int m_priceNegotiation;

	// ショップ用のマネージャーかどうか
	[SerializeField] bool m_isShop = false;

	// 購入時にアイテムの情報を表示するようのUI
	[SerializeField] Image m_buyItemIcon;
	[SerializeField] TextMeshProUGUI m_price;
	[SerializeField] TextMeshProUGUI m_itemName;
	// 売却金額を表示するテキスト
	[SerializeField] TextMeshProUGUI m_sellValue;

	// 購入モードで使用するUI
	[SerializeField] GameObject m_forBuyModeUI;
	// 売却モードで使用するUI
	[SerializeField] GameObject m_forSellModeUI;

	// 購入予定のアイテムのアイコン
	private Color m_color;
	// 購入予定のアイテムのオブジェクト
	private ItemList m_buyItem;
	// ショップのボタン関係を管理しているオブジェクト
	[SerializeField] ShopInfoList m_shopButtonInfo;
	private GameObject m_buyTrader = null;  // 購入しようとしているトレーダー

	// セーブデータ管理用
	private SaveData m_saveInstance = null;

	[SerializeField] bool m_isPlayer = true;
	private bool m_isScavenger = false;
	private bool m_isInventoryOpen = false;
	[SerializeField] TextMeshProUGUI m_text;

	private bool m_isStashOpen = false;
	private bool m_isLobby = false;

	private PlayerController m_playerCon = null;
	private bool m_isDeath = false;

	public bool IsLobby => m_isLobby;

	// Start is called before the first frame update
	void Start()
    {
		if (m_isPlayer)
		{
			// インベントリのサイズを取得
			m_inventoryWidth = m_infoInventory.GetSize.x;
			m_inventoryHeight = m_infoInventory.GetSize.y;
			CreateInventory(GridType.Inventory);
		}

		m_saveInstance = SaveData.Instance;

		Load(GridType.Inventory);
		// ダンジョンに入るタイミングでインベントリのセーブを削除
		if(!SceneManager.GetSceneByName("LobbyScene").isLoaded)
		{ 
			m_saveInstance.DeleteInventory();
			m_playerCon = transform.root.GetComponent<PlayerController>();
		}
		// ロビーに帰ってきたタイミングでスタッシュをコピー
		if (SceneManager.GetSceneByName("LobbyScene").isLoaded)
		{
			m_stashWidth = m_infoStash.GetSize.x;
			m_stashHeight = m_infoStash.GetSize.y;
			Load(GridType.Stash);
			m_isLobby = true;
		}

		m_isBuyMode = false;
		m_isShop = false;
		m_isInventoryOpen = false;
	}

    // Update is called once per frame
    void Update()
    {
		if (photonView)
		{
			if (!photonView.IsMine || !m_isPlayer)
			{
				return;
			}
		}

		if (m_playerCon != null)
		{
			if(!m_isDeath)
			{
				if (m_playerCon.IsDeath)
				{
					m_isDeath = true;
				}
			}
			else
			{
				return;
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			// 装備枠が選択されたときは無視
			if (m_checkType == GridType.Equipment)
			{
				return;
			}

			SetItem();
			// 未選択状態に戻す
			m_checkType = GridType.Empty;
		}
	}

	public void MoveItem()
	{
		// 装備枠が選択されたときは無視
		if (m_checkType == GridType.Equipment)
		{
			return;
		}
		SetItem();
		// 未選択状態に戻す
		m_checkType = GridType.Empty;
	}

	// シーフのパッシブスキル専用
	public void AddKey(ItemList item)
	{
		// プレハブを取得
		Loader.LoadGameObjectAsync(item.ItemData.objectName).Completed += op =>
		{
			GameObject g = Instantiate(op.Result, m_moveItemTransform);
			g.GetComponent<Item_Object>().SetBaseInfo();

			Debug.Log("add key");
			// アイテムリストのインデックス番号を処理した順に書き変える
			//g.GetComponent<Item_Object>().ChangeIndex(m_itemList.Count);

			// 自身の情報をオブジェクトに持たせる
			g.GetComponent<Item_Object>().ItemData = item.ItemData;

			// アイテムをインベントリに並べる
			CheckGrid(GridType.Stash, g, false, true, true, true);
		};
	}

	public void Save()
	{
		// 現在のアイテムをセーブ
		m_saveInstance.SaveInventory(m_itemList);
	}

	public void Load(GridType type)
	{
		// 現在のアイテムを全て削除
		ResetItemList(type);

		if(type == GridType.Inventory)
		{
			// セーブしたアイテムをロード
			m_itemList = new List<ItemList>(m_saveInstance.ReloadInventory());

			int count = 0;
			// リストをUIに反映
			foreach (ItemList item in m_itemList)
			{
				CreateItem(count, item, GridType.Inventory);
				count++;
			}
		}
		else if (type == GridType.Stash)
		{
			m_stashItemList = new List<ItemList>(m_saveInstance.ReloadStash());
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

						if(item.GetComponent<Item_Object>().GetEquipValue())
						{
							// 装備されていたら装備状態を解除する
							item.GetComponent<Item_Object>().SetEquipValue(
								false, 
								item.GetComponent<Item_Object>().GetGridType() == GridType.Inventory
								);
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
									if (m_isShop)
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
			if (isQuickMove) 
			{
				SearchOtherType(ref list, ref height, ref width); 
			}
			else
			{
				SearchEqualType(ref list, ref height, ref width); 
			}
		}

		// 枠外にはみ出すときはそもそも確認しない
		if (startGrid.x + (size.x - 1) >= width) return false;
		if (startGrid.y + (size.y - 1) >= height) return false;

		// 中身を確認
		for (int i = startGrid.y; i < startGrid.y + size.y; i++)
		{
			for(int j = startGrid.x; j < startGrid.x + size.x; j++)
			{
				if (list[j, i].GetInfo())
				{
					// 中身があるときはfalse
					Debug.Log("埋まっているマス[x,y] = [" + j + "," + i + "]");
					return false;
				}
			}
		}

		Debug.Log("アイテムのサイズ[x, y] = [" + size.x + "," + size.y + "]");
		// スペースが空いているときは中身が入っていることにする
		for (int i = startGrid.y; i < startGrid.y + size.y; i++)
		{
			for (int j = startGrid.x; j < startGrid.x + size.x; j++)
			{
				list[j, i].SetInfo(true);
				Debug.Log("[" + j + ", " + i + "]");
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
		for (int i = basePos.y; i < basePos.y + size.y; i++)
		{
			for (int j = basePos.x; j < basePos.x + size.x; j++)
			{
				list[j, i].SetInfo(info);
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
		// 何も漁っていない時は無視する
		if (!m_isLobby && !m_isScavenger) return false;

		// 購入モードでは無視
		if (!isTest && m_isBuyMode && m_isShop)
		{
			// アイテムが入るスペースがないので元の位置に戻す
			item.GetComponent<Item_Object>().PointerUp(false);
			return false;
		}

		return CheckGrid(type, item, isEquip, isAdd, isTest);
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

    // アイテム欄を作成する
    public void CreateStashUi(Info_InventorySize info, List<ItemList> itemList)
	{
		if (m_isInventoryOpen)
		{
			Debug.Log("already open stash ui");
            return;
		}

		if(m_stashUi != null)
		{
			Destroy(m_stashUi.gameObject);
		}

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

		m_stashUi.GetComponent<Inventory_Parent>().SetStashManager(this);

		// アイテムリストをコピー
		m_otherItemList = new List<ItemList>(itemList);

		// インベントリの情報を取得
		m_stashHeight = info.GetSize.y;
        m_stashWidth = info.GetSize.x;

		// インベントリの枠の親オブジェクトを取得
        m_stashGridParent = m_stashUi.GetComponent<Inventory_Parent>().GetContent;
		
		// スタッシュを作成
		CreateInventory(GridType.Stash);

		int count = 0;
		// リストをUIに反映
		foreach (ItemList item in m_otherItemList)
		{
			CreateItem(count, item);
			count++;
		}
		ManageUiActiveInfo();
	}

	private void CreateItem(int count, ItemList item, GridType type = GridType.Stash)
	{
		Loader.LoadGameObjectAsync(item.ItemData.objectName).Completed += op =>
		{
			// リストからオブジェクトを生成
			GameObject obj = Instantiate(
				op.Result,
				m_moveItemTransform
				);
			// アイテムの情報を設定
			obj.GetComponent<Item_Object>().ItemData = item.ItemData;

			obj.GetComponent<Item_Object>().SetBaseInfo();     
			
			// 自分が入っている枠のタイプを設定
			obj.GetComponent<Item_Object>().SetType(type);

			if (item.IsEquip())
			{
				if(type == GridType.Inventory)
				{	
					// 装備されていたアイテム
					m_inventoryUi.GetComponent<Inventory_Parent>().GetEquipments.GetComponent<EquipmentManager>().QuickEquip(obj, true);
				}
				else
				{
					// 装備されていたアイテム
					m_stashUi.GetComponent<Inventory_Parent>().GetEquipments.GetComponent<EquipmentManager>().QuickEquip(obj, true);
				}
			}
			else
			{
				// マス目を記憶
				obj.GetComponent<Item_Object>().SetGridIndex(item.GetGridIndex());
				// 普通のアイテムのマス目を埋める
				MoveItem(
					obj,
					item.GetGridIndex(),
					obj.GetComponent<Item_Object>().GetSize(),
					true,
					type
					);

				// UIを移動
				if (type == GridType.Inventory)
				{
					obj.GetComponent<Item_Object>().PointerUp(
						true,
						m_inventoryGridList[item.GetGridIndex().x, item.GetGridIndex().y].GetTransform()
						);
				}
				else
				{
					obj.GetComponent<Item_Object>().PointerUp(
						true,
						m_stashGridList[item.GetGridIndex().x, item.GetGridIndex().y].GetTransform()
						);
				}
			}

			// リストのアクティブなオブジェクトを保存
			item.SetActiveObject(obj);

			item.ChangeIndex(count);
		};
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
				//list = new Grid[m_inventoryHeight, m_inventoryWidth];
				m_inventoryGridList = list;
				SetInventorySize(ref height, ref width);
				parent = m_inventoryGridParent;
                break;

			case GridType.Stash:
				// スタッシュ用マス目の配列を作成
				list = new Grid[m_stashWidth, m_stashHeight];
				//list = new Grid[m_stashHeight, m_stashWidth];
				m_stashGridList = list;
				SetStashSize(ref height, ref width);
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
	public bool ManageUiActiveInfo()
	{
		if (m_stashUiParent.activeSelf)
		{
            //カーソルの操作を固定する
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

			// 閉じるときにスタッシュの変更を相手に渡す
			m_parent.GetComponent<StashController>().ReturnItemList(m_otherItemList);
            m_stashUiParent.SetActive(false);
			m_otherItemList = null;
			IsScavenger(false);
			if (m_stashUi)
			{
				Destroy(m_stashUi.gameObject);
			}
			m_isInventoryOpen = false;

			return true;
        }
        else
		{
			//カーソルの操作を可能にする
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            m_stashUiParent.SetActive(true);
			m_isInventoryOpen = true;

			return false;
        }
    }

	public void IsScavenger(bool value)
	{
		m_isScavenger = value;
	}

	public bool GetScavengerFlg()
	{
		return m_isScavenger;
	}

	// インベントリを開いているかを取得
	public bool IsOpenInventory()
	{
		return m_isInventoryOpen;
	}

	// アイテムを一旦除ける用の場所を返す
	public Transform GetMoveItemTransform()
	{
		return m_moveItemTransform;
	}

	// 装備枠のアイテムを設定する
	public void SetEquipment(GameObject item, bool value, bool isMine = false)
	{
		GridType lastType = item.GetComponent<Item_Object>().GetGridType();

		// 枠を超えた移動に関する情報の変更
		if (lastType != m_checkType)
		{
			// インベントリ or スタッシュ => 装備枠
			if(m_checkType == GridType.Equipment)
			{
				if(isMine)
				{
					item.GetComponent<Item_Object>().SetType(GridType.Inventory);
				}
				else
				{
					item.GetComponent<Item_Object>().SetType(GridType.Stash);
				}
			}
			else
			{
				item.GetComponent<Item_Object>().SetType(m_checkType);
			}
		}

		// 誰の持ち物か判断する
		List<ItemList> list = new List<ItemList>();
		// 装備する時
		if(value)
		{
			if (isMine)
			{
				// 自分の装備枠に入ってきたとき
				if (lastType == GridType.Stash)
				{
					// スタッシュ => 自身の装備枠
					// リストの入れ替え
					RemoveItemList(item, m_otherItemList);
					AddItemList(item, m_itemList);
				}
				else
				{
					// インベントリ => 自身の装備枠
				}
				list = m_itemList;
			}
			else
			{
				// 相手の装備枠に入れた時
				if (item.GetComponent<Item_Object>().GetGridType() == GridType.Inventory)
				{
					// インベントリ => 相手の装備枠
					RemoveItemList(item, m_itemList);
					AddItemList(item, m_otherItemList);
				}
				else
				{
					// 相手のインベントリ => 相手の装備枠
				}
				list = m_otherItemList;
			}
		}
		// 装備を外すとき
		else
		{
			if(isMine)
			{
				if(item.GetComponent<Item_Object>().GetGridType() == GridType.Stash)
				{
					// 自身の装備枠 => スタッシュ
					RemoveItemList(item, m_itemList);
					AddItemList(item, m_otherItemList);
					list = m_otherItemList;
				}
				else
				{
					// 自身の装備枠 => インベントリ
					list = m_itemList;
				}
			}
			else
			{
				if (item.GetComponent<Item_Object>().GetGridType() == GridType.Stash)
				{
					// 相手の装備枠 => インベントリ
					RemoveItemList(item, m_otherItemList);
					AddItemList(item, m_itemList);
					list = m_itemList;
				}
				else
				{
					// 相手の装備枠 => スタッシュ
					list = m_otherItemList;
				}
			}
		}

		// リストに装備状況を保存
		list[item.GetComponent<Item_Object>().GetIndex()].SetEquipInfo(value);
	}

	// ---- アイテムのリスト関連 ----
	// 自分のアイテムリストに追加する
	private void AddMyList(GameObject item)
	{
		if (m_isShop)
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
		else 
		{
			RemoveItemList(item, m_otherItemList);
			AddItemList(item, m_itemList);
		}
	}

	// 相手のアイテムリストに追加する
	private void AddOtherList(GameObject item)
	{
		if (m_isShop)
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
		else
		{
			RemoveItemList(item, m_itemList);
			AddItemList(item, m_otherItemList);
		}
	}

	// 指定したタイプのマス目を探索してリストを管理する
	private bool CheckGrid(
		GridType type,      // アイテムのGridType
		GameObject item,    // item自身
		bool isEquip,       // 装備されているか,
		bool isAdd = false, // リスト間での移動ではなく追加か
		bool isTest = false,// デバッグ用アイテムか
		bool isNormal = true// ショップ用の動きをしないか
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

		Debug.Log("[height,width] = [" + height + "," + width + "]");

		// リストを回す
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (CheckSpace(new Vector2Int(j, i), size, isEquip, true))
				{
					// 移動先の子オブジェクトに設定する
					if (m_isBuyMode)
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
						item.GetComponent<Item_Object>().SetEquipValue(
							false,
							item.GetComponent<Item_Object>().GetGridType() == GridType.Inventory
							);
					}
					else
					{
						// 現在の枠のgridtypeを保管
						GridType m = (GridType)((int)m_checkType + 1 > 1 ? 0 : 1);
						item.GetComponent<Item_Object>().SetType(m);

						// --- アイテムリストの管理 ---
						if (m_checkType == GridType.Inventory)
						{
							if (isTest) AddItemList(item, m_otherItemList, isNormal);
							else AddOtherList(item);
						}
						else
						{
							if (isTest) AddItemList(item, m_itemList, isNormal);
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

	// リストに追加する
	private void AddItemList(GameObject item, List<ItemList> list, bool isNormal = true)
	{
		ItemList info = ScriptableObject.CreateInstance<ItemList>();
		// マス目座標を保存
		info.SetGridIndex(item.GetComponent<Item_Object>().GetGridIndex());
		Debug.Log(item.GetComponent<Item_Object>().ItemData.objectName);

		// アクティブなオブジェクトを変更
		info.SetActiveObject(item);
		// リストにオブジェクトのExcel情報を追加
		info.ItemData = item.GetComponent<Item_Object>().ItemData;

		if(isNormal)
		{
			// リストのインデックスを保存
			info.ChangeIndex(list.Count);
			// アイテムリストに保存
			list.Add(info);
		}
		else
		{
			info.ChangeIndex(item.GetComponent<Item_Object>().GetIndex());
			list[item.GetComponent<Item_Object>().GetIndex()] = info;
		}

		// ショップの売却金額の合計を更新
		if (m_isShop && !m_isBuyMode)
		{
			CalcSoldValue();
		}
	}

	// リストから除外する(リストを指定する)
	private void RemoveItemList(GameObject item, List<ItemList> list, bool isDelete = false)
	{
		list.RemoveAt(item.GetComponent<Item_Object>().GetIndex());

		// オブジェクトを削除する
		if (isDelete)
		{
			item.GetComponent<Item_Object>().DeleteObject();
		}

		for (int i = 0; i < list.Count; i++)
		{
			list[i].ChangeIndex(i);
		}

		// ショップの売却金額の合計を更新
		if (m_isShop && !m_isBuyMode)
		{
			CalcSoldValue();
		}
	}

	// 自分のインベントリからアイテムから削除する
	public void RemoveInventory(ItemList item)
	{
		RemoveItemList(item.GetActiveObject(), m_itemList, true);
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
	// -------------------------------

	// ----- ボタンの処理 ------
	// 販売アイテムUIを作成
	public void SetShopItemUI(Info_InventorySize info, List<ItemList> itemList, bool isSet, GameObject trader)
	{
		// 購入モード以外では無視
		if (!m_isBuyMode) return;
		// 今取引しているトレーダーを再選択したときは無視
		if (m_buyTrader == trader) return;
		else
		{
			if(m_buyTrader)
			{
				m_buyTrader.GetComponent<SelectShopButton>().RefreshShopList(m_otherItemList);
			}
		}

		// 現在取引しているトレーダーを保持
		m_buyTrader = trader;

		ResetBuyItemInfo();
		// 表示しているUIを削除する
		CreateNewShop();
		// 商品リストをリセット
		m_otherItemList.Clear();
		// 商品リストの長さを渡されたリストの長さに変更
		SetListLength(ref m_otherItemList, itemList.Count);

		// 商品リストのコピー
		m_traderItemList = itemList;

		GetPrefabForShop(itemList);
	}

	// ショップ用のプレハブ取得関数
	//private async void GetPrefabForShop(int index, ItemList item)
	private async void GetPrefabForShop(List<ItemList> list)
	{
		int index = 0;
		foreach(var item in list)
		{
			GameObject g = Instantiate(await LoadAsync(item.ItemData.objectName), m_moveItemTransform);
			g.GetComponent<Item_Object>().SetBaseInfo();

			// アイテムリストのインデックス番号を処理した順に書き変える
			g.GetComponent<Item_Object>().ChangeIndex(index);

			// 自身の情報をオブジェクトに持たせる
			g.GetComponent<Item_Object>().ItemData = item.ItemData;

			// アイテムをスタッシュに並べる
			CheckGrid(GridType.Inventory, g, false, true, true, false);

			Debug.Log("index : " + index);
			index++;
		}
	}

	// ---- テスト用 ----
	private Task<GameObject> LoadAsync(string name)
	{
		var tcs = new TaskCompletionSource<GameObject>();

		var handle = Loader.LoadGameObjectAsync(name);
		handle.Completed += op =>
		{
			tcs.SetResult(op.Result);
		};

		return tcs.Task;
	}

	// 購入前にアイテムの情報を表示する
	public void SetBuyItemInfo(Sprite icon, string name, int price, int index)
	{
		// 最初に選ばれたアイテムの時だけ走る
		if (m_color.a == 0)
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
		if (m_infoMoney.GetCurrentMoney() < m_buyItem.GetActiveObject().GetComponent<Item_Object>().GetValue())
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
		if (m_buyItem)
		{
			if (m_isBuyMode)
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
		if(!m_isShop) return false;
		return m_isBuyMode;
	}
	// ---------------------------------

	public void ResetItemList(GridType type)
	{
		List<ItemList> list = new List<ItemList>();
		switch(type)
		{
			case GridType.Inventory:
				list = m_itemList;
				break;

			case GridType.Stash:
				list = m_stashItemList;
				break;
		}

		foreach(var item in list)
		{
			item.GetActiveObject().GetComponent<Item_Object>().ReadyMove();
			item.DeleteActiveObject();
		}

		list.Clear();
	}
	// ----------------------------------

	// ----- ショップ画面作成の共通の処理 -----
	private void CreateNewShop()
	{
		if(m_stashUi)
		{
			// 表示しているUIを削除する
			Destroy(m_stashUi.gameObject);
		}
		// 新しくUIを表示する
		m_stashUi = Instantiate(m_stashUiPrefab, m_stashPos);
		// インベントリの枠の親オブジェクトを取得
		m_stashGridParent = m_stashUi.GetComponent<Inventory_Parent>().GetContent;
		// インベントリの枠に自分を渡す
		m_stashUi.GetComponent<Inventory_Parent>().SetStashManager(this);
		// 内部的な配列を作成
		CreateInventory(GridType.Stash);
	}

	// ----- ボタンの処理 ------
	public void Deal()
	{
		if (m_isBuyMode)
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
			// 売却予定のアイテムを削除
			int add = 0;
			foreach (ItemList item in m_sellItemList)
			{
				// 売却したアイテムが入っていたマスを空ける
				Vector2Int basePos = item.GetGridIndex();
				Vector2Int size = item.GetActiveObject().GetComponent<Item_Object>().GetSize();
				for (int i = basePos.x; i < basePos.x + size.x; i++)
				{
					for (int j = basePos.y; j < basePos.y + size.y; j++)
					{
						m_stashGridList[i, j].SetInfo(false);
					}
				}
				// 売却用アイテム(半額)
				if(item.ItemData.equipmentType == 5)
				{
					// 売値を加算
					add += item.GetActiveObject().GetComponent<Item_Object>().GetValue() / 2;
				}
				// 売却用以外のアイテム(1/3)
				else
				{
					// 売値を加算
					add += item.GetActiveObject().GetComponent<Item_Object>().GetValue() / 3;
				}
				// インスタンス化されたオブジェクトを削除する
				item.DeleteActiveObject();
			}
			// 全ての売却アイテムのオブジェクトを処理し終わったら売却用リストをリセット
			m_sellItemList.Clear();
			// 売却した分の金額を追加する
			m_infoMoney.AddMoney(add);

			m_text.SetText("Money : " + m_infoMoney.GetCurrentMoney().ToString());

			CalcSoldValue();
		}
	}

	// 売却用アイテムの総額を計算する
	private void CalcSoldValue()
	{
		int value = 0;

		// 売却予定のアイテムを削除
		foreach (ItemList item in m_sellItemList)
		{
			// 売却用アイテム(半額)
			if (item.ItemData.equipmentType == 5)
			{
				// 売値を加算
				value += item.GetActiveObject().GetComponent<Item_Object>().GetValue() / 2;
			}
			// 売却用以外のアイテム(1/3)
			else
			{
				// 売値を加算
				value += item.GetActiveObject().GetComponent<Item_Object>().GetValue() / 3;
			}
		}

		m_sellValue.SetText(value.ToString());
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

            if (CheckGrid(item.GetComponent<Item_Object>().GetGridType(), item, false))
			{
				// 当たり判定を復活させる
				item.GetComponent<Item_Object>().ResetHitCol();
				// お金を消費
				m_infoMoney.UseMoney(m_buyItem.GetActiveObject().GetComponent<Item_Object>().GetValue());
				// 購入予定のアイテムをリセットする
				ResetBuyItemInfo();
			}

			m_text.SetText("Money : " + m_infoMoney.GetCurrentMoney().ToString());
		}
		else if (m_infoMoney.GetCurrentMoney() < m_buyItem.GetActiveObject().GetComponent<Item_Object>().GetValue())
		{
			// 所持金額が足りない場合

		}
	}

	// 購入モードに切り替える
	public void ChangeBuyMode()
	{
		// すでに購入モードの時は無視する
		if (m_isBuyMode) return;

		// UIの管理
		if(!m_forBuyModeUI.activeSelf)
		{
			m_forBuyModeUI.SetActive(true);
		}
		if(m_forSellModeUI.activeSelf)
		{
			m_forSellModeUI.SetActive(false);
		}

		m_buyTrader = null;

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

		// UIの管理
		if (!m_forSellModeUI.activeSelf)
		{
			m_forSellModeUI.SetActive(true);
		}
		if (m_forBuyModeUI.activeSelf)
		{
			m_forBuyModeUI.SetActive(false);
		}

		// 売却ボタンが隠れていた時用
		m_shopButtonInfo.ShowDealButton();

		// 売却モードに切り替える
		m_isBuyMode = false;

		// 購入予定のアイテムが選択されている時は元に戻す
		ResetBuyItemInfo();     
		
		// 直前まで取引していたトレーダーの情報を返す
		m_buyTrader.GetComponent<SelectShopButton>().RefreshShopList(m_otherItemList);

		// 内部的なリストをリセット
		m_sellItemList.Clear();

		// 処理を統合するために参照渡し
		m_otherItemList = m_sellItemList;

		CalcSoldValue();

		CreateNewShop();
	}
	// ---------------------------------

	// ショップ画面 => ロビー画面に遷移した時
	public void ResetShop()
	{
		if(m_isBuyMode)
		{
			// 購入予定のアイテムが選択されている時は元に戻す
			ResetBuyItemInfo();
		}
		else
		{
			// 売却用アイテムリストが空じゃないとき
			if (m_sellItemList.Count != 0)
			{
				List<ItemList> list = new List<ItemList>(m_sellItemList);
				foreach (ItemList item in list)
				{
					// アイテムをインベントリに返す
					QuickMoveItem(GridType.Stash, item.GetActiveObject(), false, true, false);
				}
				m_sellItemList.Clear();
			}
		}
		// 購入モードフラグを折る（折らないとショートカットがバグる）
		m_isBuyMode = false;
		m_isShop = false;
		m_isInventoryOpen = false;
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
	// ---------------------------------------------

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

	// listの長さを指定した長さに変更する
	public static void SetListLength<T>(ref List<T> list, int length)
	{
		for(int i = 0; i < length; i++)
		{
			list.Add(default(T));
		}
	}

	// ショップ画面を開く際に準備をする
	public void ReadyShop()
	{
		m_isShop = true;
		m_stashWidth = m_infoStash.GetSize.x;
		m_stashHeight = m_infoStash.GetSize.y;
		m_color = m_buyItemIcon.GetComponent<Image>().color;
		CreateInventory(GridType.Stash);
		m_text.SetText("Money : " + m_infoMoney.GetCurrentMoney().ToString());
	}

	// ロビーのスタッシュ画面を作成する
	public void CreateLobbyStash()
	{
		CreateStashUi(m_lobbyInfo, m_stashItemList);
		m_isStashOpen = true;
	}

	// ショップを開く
	public void StartShopMode()
	{
		// ショップの挙動に変更する
		m_isShop = true;
		// 購入モードで開始する
		ChangeBuyMode();
	}

	// スタッシュ <=> インベントリのやり取りを保存する
	public void SaveStash()
	{
		if(m_isStashOpen)
		{
			m_stashItemList = new List<ItemList>(m_otherItemList);

			// スタッシュの内容をセーブする
			m_saveInstance.SaveStash(m_stashItemList);

			m_isStashOpen = false;
            m_isInventoryOpen = false;

            // 表示しているUIを削除する
            Destroy(m_stashUi.gameObject);
		}
	}

	// ゲームの終了
	void OnApplicationQuit()
	{
		// アイテムの所持状況を保持
		m_saveInstance.SaveInventory(m_itemList);
		m_saveInstance.SaveStash(m_stashItemList);
	}
}
