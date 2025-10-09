using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragObject : MonoBehaviour,IPointerDownHandler
{
	[SerializeField] private ExcelData m_excelData;
	[SerializeField] private int m_id;
	[SerializeField] private Vector2Int m_positon;
	List<Vector2Int> m_setUiPos = new List<Vector2Int>();
	Transform m_canvs;
	Transform iconParent;
	Icon.IconType m_iconType = Icon.IconType.Inventory;
	InventoryManager m_manager;
	private ObjectEntity m_data;
	private bool m_IsDragging;
	private Vector2 prevPos; //保存しておく初期position
	private RectTransform rectTransform; // 移動したいオブジェクトのRectTransform
	private RectTransform parentRectTransform; // 移動したいオブジェクトの親(Panel)のRectTransform
	private Image m_image;

	private void Awake()
	{
		m_canvs = GameObject.Find("StashUi").transform;
		m_data = m_excelData.Object[m_id];
		m_IsDragging = false;
		rectTransform = GetComponent<RectTransform>();
		m_image = GetComponent<Image>();
		parentRectTransform = rectTransform.parent as RectTransform;
		m_manager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
	}

	private void Start()
	{ 
		m_manager.SetDragObject(this);
		InventoryManager.Check(InventoryManager.GetIcon(m_positon, Icon.IconType.Inventory), Icon.IconType.Inventory);
	}


	// ドラッグ開始時の処理
	public void OnPointerDown(PointerEventData eventData)
	{
		m_manager.TakeObject(m_setUiPos, m_iconType);
		// 描画順を最後尾
		transform.SetAsLastSibling();

		// 現時点の親を保存
		iconParent = transform.parent;
		// 一度親をキャンバスに変更
		transform.SetParent(m_canvs);
		m_manager.SetDragObject(this);
		// ドラッグ前の位置を記憶しておく
		prevPos = rectTransform.anchoredPosition;

		// 移動中に設定して当たり判定を一時的に消す
		m_IsDragging = true;
		m_image.raycastTarget = false;
	}

	// ドラッグ中の処理
	private void Update()
	{
		if (m_IsDragging)
		{
			Vector2 localPosition = GetLocalPosition(Input.mousePosition);
			rectTransform.anchoredPosition = localPosition;

			if (Input.GetMouseButtonUp(0))
			{
				// 設置するアイコンを取得
				var icon = m_manager.GetIcon();
				if (icon != null)
				{
					icon.MouseUp();
				}
				else
				{
					// 適切な設置場所じゃない場合元の位置に戻す
					m_IsDragging = false;
					m_image.raycastTarget = true;
					rectTransform.anchoredPosition = prevPos;
					transform.SetParent(iconParent);
					InventoryManager.Check(iconParent.GetComponent<Icon>(), m_iconType);
				}
			}
		}
	}

	public void DragEnd(bool isEmpty, Vector2 position, Transform icon, List<Vector2Int> posList, Icon.IconType iconType)
	{
		if (isEmpty)
		{
			// 設置ができる場合ポジションと親を設定する
			rectTransform.localPosition = (Vector2)rectTransform.parent.InverseTransformPoint(position);
			transform.SetParent(icon);
			m_setUiPos = posList;
			m_iconType = iconType;
		}
		else
		{
			// 設置できない場合元の位置に戻してCheck関数で元の位置のフラグも立てる
			rectTransform.anchoredPosition = prevPos;
			transform.SetParent(iconParent);
			InventoryManager.Check(iconParent.GetComponent<Icon>(), m_iconType);
		}
		m_IsDragging = false;
		m_image.raycastTarget = true;
	}

	// ScreenPositionからlocalPositionへの変換関数
	private Vector2 GetLocalPosition(Vector2 screenPosition)
	{
		Vector2 result = Vector2.zero;

		// screenPositionを親の座標系(parentRectTransform)に対応するよう変換する.
		RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPosition, Camera.main, out result);

		return result;
	}

	public Vector2 GetSize()
	{
		return new Vector2(m_data.width, m_data.height);
	}
}
