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
	/// �}�E�X������Ă���UI��GameObject��Ԃ��i�Ȃ����null�j
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
		// PointerEventData���쐬
		PointerEventData pointerData = new PointerEventData(eventSystem)
		{
			position = Input.mousePosition
		};

		// Raycast���ʂ��i�[
		List<RaycastResult> results = new List<RaycastResult>();
		raycaster.Raycast(pointerData, results);

		// �����ɓ������Ă���Έ�ԏ��UI��Ԃ�
		if (results.Count > 0)
		{
			Debug.Log(results[0].gameObject.transform.parent);
			if (results[0].gameObject.transform.parent.TryGetComponent(out Icon icon))
			{
				return icon;
			}
		}

		// �����Ȃ����null
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
						// ����}�E�X������Ă���Icon
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
					Debug.Log("�΂����Ă��");
				}

				bool empty = true;
				List<Icon> icons = new List<Icon>();
				List<Vector2Int> posList = new List<Vector2Int>();

				for (int y = 0; y < m_dragObject.GetSize().y; ++y)
				{
					for (int x = 0; x < m_dragObject.GetSize().x; ++x)
					{
						// �g�O
						if (stashPos.x + x >= StashSize.x || stashPos.y + y >= StashSize.y)
						{
							empty = false;
							break;
						}

						Debug.Log((stashPos.x + x) + ":" + (stashPos.y + y));
						Icon newIcon = m_stash[stashPos.x + x, stashPos.y + y].GetComponent<Icon>();
						// �ǂ�����ɂł������Ă�����false
						if (newIcon.GetOnFillUi())
						{
							empty = false;
						}
						icons.Add(newIcon);
						posList.Add(new Vector2Int(stashPos.x + x, stashPos.y + y));
					}
					if (!empty) break;
				}
				
				// �S���̏ꏊ���J���Ă���
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

	// �C���x���g��������h���b�O�Ŏ�����Ƃ�
	public void TakeObject(List<Vector2Int> posList)
	{
		foreach (Vector2Int pos in posList)
		{
			m_stash[pos.x, pos.y].GetComponent<Icon>().SetUi(false);
		}
	}
}
