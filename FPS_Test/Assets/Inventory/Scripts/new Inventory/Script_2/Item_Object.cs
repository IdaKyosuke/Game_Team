using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Icon;

public class Item_Object : MonoBehaviour
{
	[SerializeField] Info_ItemSize m_info;
	private bool m_isPointerEnter = false;
	private bool m_isDrag = false;		// カーソルを追従しているか
	private bool m_quickMove = false;	// ショートカット移動をしているか
	private bool m_quickEquip = false;	// 高速装備を行う

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
	private bool m_isEquip = false;		// 現在装備されているか

	// 装備の情報
	private EquipmentStatus m_equipmentInfo;

	// デバッグ用
	[SerializeField] string m_name;
	[SerializeField] GameObject m_mine;

	// テスト用（自分のリストのindex）
	private int m_index;

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

		// 移動中に格納される場所
		m_moveItemTransform = GameObject.FindWithTag("moveItemTransform").transform;

		m_inventoryManager = GameObject.FindWithTag("inventoryManager");

		// スケールを1にする
		rectTransform.localScale = Vector3.one;
		Debug.Log(transform.name);
		/*
		Loader.LoadGameObjectAsync(m_name).Completed += op =>
		{
			m_mine = op.Result;
			Addressables.Release(op);
		};
		*/
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

    public void PointerEnter()
	{
		m_isPointerEnter = true;
	}

	public void PointerExit()
	{
		m_isPointerEnter = false;
	}

	// ショートカット移動
	private void QuickMove()
	{
		// 現在の自分の入っている枠のタイプに応じて入れ替える
		GameObject.FindWithTag("inventoryManager").GetComponent<StashManager>().QuickMoveItem(m_gridType, gameObject, m_isEquip);
	}

	// 高速装備する
	private void QuickEquip()
	{
		// 現在の自分の入っている枠のタイプに応じて入れ替える
		GameObject.FindWithTag("equipmentManager").GetComponent<PlayerStatus>().QuickEquip(gameObject);
	}

	// アイテムを移動させる前の準備
	private void ReadyMove()
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
			// これまで入っていたマス目を解放
			m_inventoryManager.GetComponent<StashManager>().MoveItem(
				gameObject,
				m_pos, 
				GetSize(),
				false, 
				m_gridType,
				true
				);
		}
	}

	// アイテムを持ち上げる際の動き
	public void PointerDown()
	{
		if (m_quickMove || m_isDrag || m_quickEquip) return;

		if(Input.GetKey("left ctrl"))
		{
			// ショートカット開始
			m_quickMove = true;
		}
		else if(Input.GetKey("left alt") && m_equipmentType != EquipmentType.None)
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

	public void PointerUp(bool canSet, Transform nextPos = null)
	{
		// マウスカーソルの追従を終了
		m_isDrag = false;
		// クイック移動状態を終了
		m_quickMove = false;
		// クイック装備状態を終了
		m_quickEquip = false;

		if (canSet)
		{
			// 移動可能
			// 移動先の枠を親オブジェクトに設定
			SetParentTransform(nextPos);
			rectTransform.anchoredPosition = Vector2.zero;
		}
		else
		{
			// 移動不可能
			// 元あった位置に戻る
			SetParentTransform(iconParent);
			rectTransform.anchoredPosition = prevPos;
			if(!m_isEquip)
			{
				// 解放したマス目を埋めなおす
				m_inventoryManager.GetComponent<StashManager>().MoveItem(
					gameObject, 
					m_pos, 
					GetSize(), 
					true,
					m_gridType,
					true
					);
			}
		}
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

	public void SetType(GridType type)
	{
		m_gridType = type;
	}

	public EquipmentType GetWeaponType()
	{
		return m_equipmentType;
	}

	// 装備の場合に性能を返す
	public EquipmentParameter GetEquipmentInfo()
	{
        return m_equipmentInfo.TotalStatus;
	}

	// 装備状態の変更
	public void SetEquipValue(bool value)
	{
		m_isEquip = value;
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


	public GameObject GetPrefab()
	{
		return m_mine;
	}

	// ---- デバッグ用 ----
	private void OnDestroy()
	{

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
		return m_name;
	}
}
