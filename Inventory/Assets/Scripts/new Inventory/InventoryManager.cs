using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
	private GraphicRaycaster raycaster;
	[SerializeField] private EventSystem eventSystem;

	/// <summary>
	/// マウスが乗っているUIのGameObjectを返す（なければnull）
	/// </summary>


	static private DragObject m_dragObject;
	static private readonly Vector2Int InventorySize = new Vector2Int(5, 5);
	static private readonly Vector2Int StashSize = new Vector2Int(5, 10);

	[SerializeField] List<GameObject> m_inventoryList;
	[SerializeField] List<GameObject> m_stashList;

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
			}
		}

		for (int y = 0; y < StashSize.y; ++y)
		{
			for (int x = 0; x < StashSize.x; ++x)
			{
				m_stash[x, y] = m_stashList[y * StashSize.x + x];
			}
		}
	}

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
			Debug.Log(results[0].gameObject.transform.parent);
			if (results[0].gameObject.transform.parent.TryGetComponent(out Icon icon))
			{
				return icon;
			}
		}

		// 何もなければnull
		return null;
	}

	public void SetDragObject(DragObject obj)
	{
		m_dragObject = obj;
	}

	static public void Check(Icon icon, Icon.IconType iconType)
	{
		switch (iconType)
		{
			case Icon.IconType.Inventory:
				/*
				for (int y = 0; y < InventorySize.y; ++y)
				{
					for (int x = 0; x < InventorySize.x; ++x)
					{
						// 今回マウスが乗っているIcon
						if (m_inventory[x, y] == icon)
						{


							return;
						}
					}
				}
				*/
				break;

			case Icon.IconType.Stash:

				Vector2Int stashPos = new Vector2Int(-1, -1);
				for (int y = 0; y < StashSize.y; ++y)
				{
					for (int x = 0; x < StashSize.x; ++x)
					{
						if (m_stash[x, y] == icon.gameObject)
						{
							stashPos.x = x;
							stashPos.y = y;
						}
					}
				}
				if (stashPos.x == -1 && stashPos.y == -1)
				{
					Debug.Log("ばぐってるよ");
				}

				bool empty = true;
				List<Icon> icons = new List<Icon>();
				List<Vector2Int> posList = new List<Vector2Int>();

				for (int y = 0; y < m_dragObject.GetSize().y; ++y)
				{
					for (int x = 0; x < m_dragObject.GetSize().x; ++x)
					{
						// 枠外
						if (stashPos.x + x >= StashSize.x || stashPos.y + y >= StashSize.y)
						{
							empty = false;
							break;
						}

						Debug.Log((stashPos.x + x) + ":" + (stashPos.y + y));
						Icon newIcon = m_stash[stashPos.x + x, stashPos.y + y].GetComponent<Icon>();
						// どこか一つにでも入っていたらfalse
						if (newIcon.GetOnFillUi())
						{
							empty = false;
						}
						icons.Add(newIcon);
						posList.Add(new Vector2Int(stashPos.x + x, stashPos.y + y));
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

			break;
		}
	}

	// インベントリ等からドラッグで取ったとき
	public void TakeObject(List<Vector2Int> posList)
	{
		foreach (Vector2Int pos in posList)
		{
			m_stash[pos.x, pos.y].GetComponent<Icon>().SetUi(false);
		}
	}
}
