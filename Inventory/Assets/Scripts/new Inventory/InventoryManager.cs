using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
	// �}�E�X������Ă���Icon���擾���邽�߂̕ϐ�
	private GraphicRaycaster raycaster;
	[SerializeField] private EventSystem eventSystem;

	// �}�E�X���͂�ł���I�u�W�F�N�g
	static private DragObject m_dragObject;
	// �C���x���g���ƃX�^�b�V���̃T�C�Y
	static private readonly Vector2Int InventorySize = new Vector2Int(5, 5);
	static private readonly Vector2Int StashSize = new Vector2Int(5, 10);

	// �C���x���g���ƃX�^�b�V���̃I�u�W�F�N�g�����郊�X�g
	[SerializeField] List<GameObject> m_inventoryList;
	[SerializeField] List<GameObject> m_stashList;

	// ���X�g��񎟌��z��ɕϊ�
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

	// �}�E�X������Ă���Icon���擾����
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
			if (results[0].gameObject.transform.parent.TryGetComponent(out Icon icon))
			{
				return icon;
			}
		}

		// �����Ȃ����null
		return null;
	}

	static public Icon GetIcon(Vector2Int pos, Icon.IconType iconType)
	{
		GameObject[,] iconList = iconType == Icon.IconType.Inventory ? 
			m_inventory : m_stash;

		return iconList[pos.x, pos.y].GetComponent<Icon>();
	}

	// �}�E�X�������Ă���I�u�W�F�N�g���擾
	public void SetDragObject(DragObject obj)
	{
		m_dragObject = obj;
	}

	// �I�u�W�F�N�g��u�����Ƃ��Ă�����ɋ󂫂����邩�m�F
	static public void Check(Icon icon, Icon.IconType iconType)
	{
		bool empty = true;
		List<Icon> icons = new List<Icon>();
		List<Vector2Int> posList = new List<Vector2Int>();

		Vector2Int Inventory_StashSize = iconType == Icon.IconType.Inventory ?
			InventorySize : StashSize;

		GameObject[,] IconList = iconType == Icon.IconType.Inventory ?
			m_inventory : m_stash;

		// ����̊�_�ɂȂ�Icon�̍��W���擾
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
				// �g�O
				if (inventory_stashPos.x + x >= Inventory_StashSize.x ||
					inventory_stashPos.y + y >= Inventory_StashSize.y )
				{
					empty = false;
					break;
				}

				Icon newIcon = IconList[inventory_stashPos.x + x, inventory_stashPos.y + y].GetComponent<Icon>();
				// �ǂ�����ɂł������Ă�����false
				if (newIcon.GetOnFillUi())
				{
					empty = false;
				}
				icons.Add(newIcon);
				posList.Add(new Vector2Int(inventory_stashPos.x + x, inventory_stashPos.y + y));
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
	}

	// �C���x���g��������h���b�O�Ŏ�����Ƃ�
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
