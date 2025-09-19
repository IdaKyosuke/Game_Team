using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
	// マウスが乗っているIconを取得するための変数
	private GraphicRaycaster raycaster;
	[SerializeField] private EventSystem eventSystem;

	// マウスが掴んでいるオブジェクト
	static private DragObject m_dragObject;
	// インベントリとスタッシュのサイズ
	static private readonly Vector2Int InventorySize = new Vector2Int(5, 5);
	static private readonly Vector2Int StashSize = new Vector2Int(5, 10);

	// インベントリとスタッシュのオブジェクトを入れるリスト
	[SerializeField] List<GameObject> m_inventoryList;
	[SerializeField] List<GameObject> m_stashList;

	// リストを二次元配列に変換
	static private GameObject[,] m_inventory = new GameObject[InventorySize.x, InventorySize.y];
	static private GameObject[,] m_stash = new GameObject[StashSize.x, StashSize.y];
	private void Awake()
	{
		raycaster = GameObject.Find("StashUi").GetComponent<GraphicRaycaster>();
		eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		for (int y = 0; y < InventorySize.y; ++y)
		{
			for (int x = 0; x < InventorySize.x; ++x)
			{
				m_inventory[x, y] = m_inventoryList[y * InventorySize.x + x];
				m_inventory[x, y].GetComponent<Icon>().SetType(Icon.IconType.Inventory);
			}
		}

		for (int y = 0; y < StashSize.y; ++y)
		{
			for (int x = 0; x < StashSize.x; ++x)
			{
				m_stash[x, y] = m_stashList[y * StashSize.x + x];
				m_stash[x, y].GetComponent<Icon>().SetType(Icon.IconType.Stash);
			}
		}
	}

	// マウスが乗っているIconを取得する
	public Icon GetIcon()
	{
		// PointerEventDataを作成
		PointerEventData pointerData = new PointerEventData(eventSystem)
		{
			position = Input.mousePosition
		};

		// Raycast結果を格納
		List<RaycastResult> results = new List<RaycastResult>();
		raycaster.Raycast(pointerData, results);

		// 何かに当たっていれば一番上のUIを返す
		if (results.Count > 0)
		{
			if (results[0].gameObject.transform.parent.TryGetComponent(out Icon icon))
			{
				return icon;
			}
		}

		// 何もなければnull
		return null;
	}

	static public Icon GetIcon(Vector2Int pos, Icon.IconType iconType)
	{
		GameObject[,] iconList = iconType == Icon.IconType.Inventory ? 
			m_inventory : m_stash;

		return iconList[pos.x, pos.y].GetComponent<Icon>();
	}

	// マウスが持っているオブジェクトを取得
	public void SetDragObject(DragObject obj)
	{
		m_dragObject = obj;
	}

	// オブジェクトを置こうとしている個所に空きがあるか確認
	static public void Check(Icon icon, Icon.IconType iconType)
	{
		bool empty = true;
		List<Icon> icons = new List<Icon>();
		List<Vector2Int> posList = new List<Vector2Int>();

		Vector2Int Inventory_StashSize = iconType == Icon.IconType.Inventory ?
			InventorySize : StashSize;

		GameObject[,] IconList = iconType == Icon.IconType.Inventory ?
			m_inventory : m_stash;

		// 今回の基点になるIconの座標を取得
		Vector2Int inventory_stashPos = new Vector2Int(-1, -1);
		for (int y = 0; y < Inventory_StashSize.y; ++y)
		{
			for (int x = 0; x < Inventory_StashSize.x; ++x)
			{
				if (IconList[x, y] == icon.gameObject)
				{
					inventory_stashPos.x = x;
					inventory_stashPos.y = y;
					break;
				}
			}
			if (inventory_stashPos.x != -1) break;
		}

		for (int y = 0; y < m_dragObject.GetSize().y; ++y)
		{
			for (int x = 0; x < m_dragObject.GetSize().x; ++x)
			{
				// 枠外
				if (inventory_stashPos.x + x >= Inventory_StashSize.x ||
					inventory_stashPos.y + y >= Inventory_StashSize.y )
				{
					empty = false;
					break;
				}

				Icon newIcon = IconList[inventory_stashPos.x + x, inventory_stashPos.y + y].GetComponent<Icon>();
				// どこか一つにでも入っていたらfalse
				if (newIcon.GetOnFillUi())
				{
					empty = false;
				}
				icons.Add(newIcon);
				posList.Add(new Vector2Int(inventory_stashPos.x + x, inventory_stashPos.y + y));
			}
			if (!empty) break;
		}

		// 全部の場所が開いている
		if (empty)
		{
			foreach (Icon newIcon in icons)
			{
				newIcon.SetUi(true);
			}
		}
		m_dragObject.DragEnd(empty, icon.GetComponent<RectTransform>().position, icon.transform, posList, iconType);
	}

	// インベントリ等からドラッグで取ったとき
	public void TakeObject(List<Vector2Int> posList, Icon.IconType iconType)
	{
		switch (iconType)
		{
			case Icon.IconType.Inventory:
				foreach (Vector2Int pos in posList)
				{
					m_inventory[pos.x, pos.y].GetComponent<Icon>().SetUi(false);
				}
			break;

			case Icon.IconType.Stash:
				foreach (Vector2Int pos in posList)
				{
					m_stash[pos.x, pos.y].GetComponent<Icon>().SetUi(false);
				}
			break;
		}
	}
}
