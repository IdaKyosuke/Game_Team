using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item_Object : MonoBehaviour
{
	[SerializeField] Info_ItemSize m_info;
	private bool m_isDrag = false;		// カーソルを追従しているか
	private bool m_quickMove = false;	// ショートカット移動をしているか
	private bool m_quickEquip = false;	// 高速装備を行う

	private RectTransform rectTransform; // 移動したいオブジェクトのRectTransform
	private RectTransform parentRectTransform; // 移動したいオブジェクトの親(Panel)のRectTransform
	private Vector2 prevPos; //保存しておく初期position
	Transform iconParent;

	[SerializeField] Transform m_moveItemTransform;  // 移動時に格納されるオブジェクト
	[SerializeField] GameObject m_collider; // 当たり判定用の画像

	// 配列のマス目
	private Vector2Int m_pos;

	// マネージャー
	[SerializeField] GameObject m_inventoryManager;
	[SerializeField] EquipmentManager m_equipmentManager;

	// カメラ
	private Camera m_camera;

	private GridType m_gridType = GridType.Inventory;

	// 装備かどうか & 装備のタイプは
	[SerializeField] EquipmentType m_equipmentType = EquipmentType.None;
	private bool m_isEquip = false;     // 現在装備されているか

	// 装備の情報
	private EquipmentStatus m_equipmentInfo = null;

	// デバッグ用
	[SerializeField] GameObject m_mine;

	// テスト用（自分のリストのindex）
	private int m_index;

	// アイテムが購入予定に選択されているか
	private bool m_isSelected = false;

	// アイテムの情報
	[SerializeField] MapObjectEntity m_itemData;

	public MapObjectEntity ItemData
	{
		get { return m_itemData; }
		set { m_itemData = value; }
	}

	// Start is called before the first frame update
	void Start()
    {
		rectTransform = GetComponent<RectTransform>();
		m_equipmentInfo = GetComponent<EquipmentStatus>();

        parentRectTransform = rectTransform.parent as RectTransform;       
		if(!iconParent)
		{
			// 現時点の親を保存
			iconParent = transform.parent;
		}

		// スケールを1にする
		rectTransform.localScale = Vector3.one;
	}

    // Update is called once per frame
    void Update()
    {
		// ショートカット移動
		if (m_quickMove)
		{
			QuickMove();
		}
		else if(m_quickEquip)
		{
			QuickEquip();
		}
		else if (m_isDrag)
		{
			// アイテムを掴んでいる時
			// マウスカーソルを掴んでいるアイテムが追従する
			Vector2 localPosition = GetLocalPosition(Input.mousePosition);
			rectTransform.anchoredPosition = localPosition;
		}
	}

	// アイテムのサイズ情報を取得
	public Vector2Int GetSize()
	{
		return m_info.GetSize();
	}

    // アイテムのインデックスを取得
	public Vector2Int GetGridIndex()
	{
		return m_pos;
    }

	// ショートカット移動
	private void QuickMove()
	{
		// 現在の自分の入っている枠のタイプに応じて入れ替える
		m_inventoryManager.GetComponent<StashManager>().QuickMoveItem(m_gridType, gameObject, m_isEquip);
	}

	// 高速装備する
	private void QuickEquip()
	{
		// 現在の自分の入っている枠のタイプに応じて入れ替える
		m_equipmentManager.QuickEquip(gameObject);
	}

	// アイテムを移動させる前の準備
	public void ReadyMove()
	{
		// 当たり判定用の画像を非アクティブにする
		m_collider.SetActive(false);
		// 現時点の親を保存
		iconParent = transform.parent;
		// ドラッグ前の位置を記憶しておく
		prevPos = rectTransform.anchoredPosition;

		// 移動中用のオブジェクトを親に変更
		SetParentTransform(m_moveItemTransform);
		// 装備枠に入っていないとき
		if(!m_isEquip)
		{
			Release();
		}
	}

	// これまで入っていたマス目を解放（自身の型をそのまま使う）
	private void Release()
	{
		m_inventoryManager.GetComponent<StashManager>().MoveItem(
			gameObject,
			m_pos,
			GetSize(),
			false,
			m_gridType
		);
	}
	// アイテムを持ち上げる際の動き
	public void PointerDown()
	{
		if (!Input.GetMouseButtonDown(0)) return;
		if (m_quickMove || m_isDrag || m_quickEquip) return;
		if (m_inventoryManager.GetComponent<StashManager>().IsBuyMode())
		{
			// 購入モード
			if (m_gridType == GridType.Stash)
			{
				if (m_isEquip) return;
				// 購入モードの時は追従しないようにする
				m_inventoryManager.GetComponent<StashManager>().SetBuyItemInfo(
					GetComponent<Image>().sprite,
					name,
					GetValue(),
					m_index
					);

				ReadyMove();
			}
		}
		else
		{
			if (Input.GetKey("left ctrl"))
			{
				if (!m_inventoryManager.GetComponent<StashManager>().GetScavengerFlg()) return;
				// ショートカット開始
				m_quickMove = true;
			}
			else if(Input.GetKey("left alt") && GetWeaponType() != EquipmentType.None)
			{
				// 高速装備する
				m_quickEquip = true;
			}
			else
			{
				// マウスカーソルの追従開始
				m_isDrag = true;
			}
			ReadyMove();
		}
	}

	public void PointerUp(bool canSet, Transform nextPos = null)
	{
		// 購入モード && 購入予定になっている時はここまでしか走らない
		if (m_isSelected && m_inventoryManager.GetComponent<StashManager>().IsBuyMode()) return;
		// 当たり判定用の画像をアクティブにする
		m_collider.SetActive(true);

		// マウスカーソルの追従を終了
		m_isDrag = false;
		// クイック移動状態を終了
		m_quickMove = false;
		// クイック装備状態を終了
		m_quickEquip = false;

		if (canSet)
		{
			// 移動可能
			MoveItem(nextPos);
		}
		else
		{
			// 移動不可能
			ResetItem();
		}
	}

	public void MoveItem(Transform nextPos)
	{
		// 移動先の枠を親オブジェクトに設定
		SetParentTransform(nextPos);
		rectTransform.anchoredPosition = Vector2.zero;
	}

	public void ResetItem()
	{
		// 当たり判定用の画像をアクティブにする
		m_collider.SetActive(true);
		// 元あった位置に戻る
		SetParentTransform(iconParent);
		rectTransform.anchoredPosition = prevPos;
		if (!m_isEquip)
		{
			// 解放したマス目を埋めなおす
			FillGrid();
		}
	}

	// 解放したマス目を埋めなおす
	public void FillGrid()
	{
		m_inventoryManager.GetComponent<StashManager>().MoveItem(
			gameObject,
			m_pos,
			GetSize(),
			true,
			m_gridType
			);
	}

	// ScreenPositionからlocalPositionへの変換関数
	private Vector2 GetLocalPosition(Vector2 screenPosition)
	{
		Vector2 result = Vector2.zero;

		// screenPositionを親の座標系(parentRectTransform)に対応するよう変換する.
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			parentRectTransform, 
			screenPosition, 
			m_camera, 
			out result
			);

		return result;
	}

	// 自身の親が変更されたときに、RectTransformも一緒に変更する
	private void SetParentTransform(Transform parent)
	{
		transform.SetParent(parent, false);
		rectTransform = GetComponent<RectTransform>();
		parentRectTransform = rectTransform.parent as RectTransform;
	}

	// 外部から親を変更する
	public void ChangeParent(Transform parent)
	{
		SetParentTransform(parent);
    }

    // 選択された配列のインデックスを覚える
    public void SetGridIndex(Vector2Int index)
	{
		m_pos = index;
	}

	// マス目のタイプを取得
	public GridType GetGridType()
	{
		return m_gridType;
	}

	public void SetType(GridType type)
	{
		m_gridType = type;
	}

	public EquipmentType GetWeaponType()
	{
		//return m_equipmentType;
		return (EquipmentType)ItemData.equipmentType;
	}

	// 装備の場合に性能を返す
	public EquipmentParameter GetEquipmentInfo()
	{
		if(!m_equipmentInfo)
		{
			return null;
		}
        return m_equipmentInfo.TotalStatus;
	}

	// 装備状態の変更
	public void SetEquipValue(bool value, bool isMine = false, bool isChangeList = true)
	{
		m_isEquip = value;

		if (!isChangeList) return;
		m_inventoryManager.GetComponent<StashManager>().SetEquipment(gameObject, value, isMine);
	}
	// 装備状態を取得する
	public bool GetEquipValue()
	{
		return m_isEquip;
	}

	public void Remove()
	{
		Destroy(gameObject);
	}

	// オブジェクトのマスを開放 => 削除
	public void DeleteObject()
	{
		Release();
		Remove();
	}

	public GameObject GetPrefab()
	{
		return m_mine;
	}

	// 自分と紐づくリストのインデックスを変更
	public void ChangeIndex(int index)
	{
		m_index = index;
	}
	// リストのインデックスを取得
	public int GetIndex()
	{
		return m_index;
	}

	// オブジェクトの名前を取得
	public string GetName()
	{
		return m_itemData.displayName;
	}

	// 生成時にstashManagerとmoveTransformを設定する
	public void SetBaseInfo()
	{
		// 親オブジェクトからマネージャーを取得
		m_inventoryManager = transform.parent.GetComponent<MoveItemTransform>().GetStashManager();
		//Debug.Log(m_inventoryManager);
		// 移動中に格納される場所
		m_moveItemTransform = m_inventoryManager.GetComponent<StashManager>().GetMoveItemTransform();

		// カメラを設定
		m_camera = transform.parent.GetComponent<MoveItemTransform>().GetCamera();

		if (GetWeaponType() != EquipmentType.None)
		{
			m_equipmentManager = transform.parent.GetComponent<MoveItemTransform>().GetEquipmentManager().GetComponent<EquipmentManager>();
		}
	}

	// ---- アイテムの売買用の動き ----
	// アイテムの価値を取得する
	public int GetValue()
	{
		return m_itemData.price;
	}

	public void PointerDownForShop()
	{
		m_isSelected = true;
	}

	// アイテムが購入予定に選ばれたか取得
	public bool IsSelected()
	{
		return m_isSelected;
	}

	// 購入予定のアイテムから外す
	public void RemoveSelected()
	{
		m_isSelected = false;
	}

	// 購入予定アイテムの当たり判定を復活させる
	public void ResetHitCol()
	{
		m_collider.SetActive(true);
	}

	// --------------------------------
}
