using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Icon;

public class Item_Object : MonoBehaviour
{
	[SerializeField] Info_ItemSize m_info;
	private bool m_isPointerEnter = false;
	private bool m_isDrag = false;

	private RectTransform rectTransform; // 移動したいオブジェクトのRectTransform
	private RectTransform parentRectTransform; // 移動したいオブジェクトの親(Panel)のRectTransform
	private Vector2 prevPos; //保存しておく初期position
	Transform iconParent;

	private Transform m_moveItemTransform;  // 移動時に格納されるオブジェクト
	[SerializeField] GameObject m_collider; // 当たり判定用の画像

	// 配列のマス目
	private Vector2Int m_pos;

	private GameObject m_inventoryManager;

	private GridType m_gridType = GridType.Inventory;

	// 装備かどうか & 装備のタイプは
	[SerializeField] EquipmentType m_equipmentType = EquipmentType.None;

	// 装備の情報
	[SerializeField] Info_Equipment m_equipmentInfo;

	// Start is called before the first frame update
	void Start()
    {
		rectTransform = GetComponent<RectTransform>();
		parentRectTransform = rectTransform.parent as RectTransform;        
		// 現時点の親を保存
		iconParent = transform.parent;

		// 移動中に格納される場所
		m_moveItemTransform = GameObject.FindWithTag("moveItemTransform").transform;

		m_inventoryManager = GameObject.FindWithTag("inventoryManager");
	}

    // Update is called once per frame
    void Update()
    {
		// アイテムを掴んでいる時
        if(m_isDrag)
		{
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
	
	public void PointerEnter()
	{
		m_isPointerEnter = true;
	}

	public void PointerExit()
	{
		m_isPointerEnter = false;
	}

	// アイテムを持ち上げる際の動き
	public void PointerDown()
	{
		// 当たり判定用の画像を非アクティブにする
		m_collider.SetActive(false);
		// 現時点の親を保存
		iconParent = transform.parent;
		// ドラッグ前の位置を記憶しておく
		prevPos = rectTransform.anchoredPosition;
		// 移動中用のオブジェクトを親に変更
		SetParentTransform(m_moveItemTransform);
		// マウスカーソルの追従開始
		m_isDrag = true;

		// これまで入っていたマス目を解放
		m_inventoryManager.GetComponent<StashManager>().MoveItem(m_pos, GetSize(), false, m_gridType);
	}

	public void PointerUp(bool canSet, Transform nextPos = null)
	{
		// マウスカーソルの追従を終了
		m_isDrag = false;

		if(canSet)
		{
			// 移動可能
			// 移動先の枠を親オブジェクトに設定
			SetParentTransform(nextPos);
		}
		else
		{
			// 移動不可能
			// 元あった位置に戻る
			SetParentTransform(iconParent);
			// 解放したマス目を埋めなおす
			m_inventoryManager.GetComponent<StashManager>().MoveItem(m_pos, GetSize(), true, m_gridType);
		}
		rectTransform.anchoredPosition = prevPos;
		// 当たり判定用の画像をアクティブにする
		m_collider.SetActive(true);
	}

	// ScreenPositionからlocalPositionへの変換関数
	private Vector2 GetLocalPosition(Vector2 screenPosition)
	{
		Vector2 result = Vector2.zero;

		// screenPositionを親の座標系(parentRectTransform)に対応するよう変換する.
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			parentRectTransform, 
			screenPosition, 
			Camera.main, 
			out result
			);

		return result;
	}

	// 自身の親が変更されたときに、RectTransformも一緒に変更する
	private void SetParentTransform(Transform parent)
	{
		transform.SetParent(parent);
		rectTransform = GetComponent<RectTransform>();
		parentRectTransform = rectTransform.parent as RectTransform;
	}

	// 選択された配列のインデックスを覚える
	public void SetIndex(Vector2Int index)
	{
		m_pos = index;
	}

	public void SetType(GridType type)
	{
		m_gridType = type;
	}

	public EquipmentType GetWeaponType()
	{
		return m_equipmentType;
	}

	// 装備の場合に性能を返す
	public int GetEquipmentInfo(Info_Equipment.WeaponStatusType type)
	{
		return m_equipmentInfo.GetInfo(type);
	}
}
