using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;


public struct Grid
{
	private GameObject m_grid;	// �}�X�ڂ̃I�u�W�F�N�g
	private bool m_info;		// ���g�����܂��Ă��邩

	private Grid(GameObject grid = null, bool info = false)
	{
		m_grid = grid;
		m_info = info;
	}

	// �g�̏����Z�b�g
	public void SetGrid(GameObject grid)
	{
		m_grid = grid;
	}

	// �g��Transform���擾
	public Transform GetTransform()
	{
		return m_grid.transform;
	}

	// �|�C���^�[���d�Ȃ��Ă��邩
	public bool OnPointer()
	{
		return m_grid.GetComponent<GridIcon>().OnPointer();
	}

	// ���g�����邩���擾
	public bool GetInfo()
	{
		return m_grid.GetComponent<GridIcon>().GetOnFillUi();
	}

	// ���g�̏�Ԃ�ς���
	public void SetInfo(bool info)
	{
		m_grid.GetComponent<GridIcon>().SetUi(info);
	}

	// gridtype��ݒ肷��
	public void SetGridType(GridType type)
	{
		m_grid.GetComponent<GridIcon>().SetType(type);
	}
}

public class StashManager : MonoBehaviour
{
	// �X�^�b�V���p�T�C�Y
	[SerializeField] int m_stashWidth = 5;
	[SerializeField] int m_stashHeight = 10;

	// �C���x���g���p�T�C�Y
	[SerializeField] int m_inventoryWidth = 5;
	[SerializeField] int m_inventoryHeight = 5;

	[SerializeField] GameObject m_stashGridParent;  // �X�^�b�V���̃}�X�ڂ̐e�I�u�W�F�N�g
	[SerializeField] GameObject m_inventoryGridParent;	// �C���x���g���̃}�X�ڂ̐e�I�u�W�F�N�g

	private Grid[,] m_stashGridList;
	private Grid[,] m_inventoryGridList;
	private Transform m_moveItemTransform;
	private GridType m_checkType;

	// Start is called before the first frame update
	void Start()
    {
		// �X�^�b�V���p�}�X�ڂ̔z����쐬
		m_stashGridList = new Grid[m_stashWidth, m_stashHeight];
		// �C���x���g���p�}�X�ڂ̔z����쐬
		m_inventoryGridList = new Grid[m_inventoryWidth, m_inventoryHeight];

		// �A�C�e���ړ��p�̃I�u�W�F�N�g
		m_moveItemTransform = GameObject.FindWithTag("moveItemTransform").transform;

		// �X�^�b�V���p�z����쐬
		// �z��ƃ}�X�ڂ̏�Ԃ����킹��
		int count = 0;
		for(int i = 0; i < m_stashHeight; i++)
		{
			for(int j = 0; j < m_stashWidth; j++)
			{
				// �I�u�W�F�N�g��ǉ�
				GameObject g = m_stashGridParent.transform.GetChild(count).gameObject;
				m_stashGridList[j, i].SetGrid(g);
				count++;
				// ���g����ɂ���
				m_stashGridList[j, i].SetInfo(false);
				m_stashGridList[j, i].SetGridType(GridType.Stash);
			}
		}

		// �C���x���g���p�z����쐬
		count = 0;
		for (int i = 0; i < m_inventoryHeight; i++)
		{
			for (int j = 0; j < m_inventoryWidth; j++)
			{
				// �I�u�W�F�N�g��ǉ�
				GameObject g = m_inventoryGridParent.transform.GetChild(count).gameObject;
				m_inventoryGridList[j, i].SetGrid(g);
				count++;
				// ���g����ɂ���
				m_inventoryGridList[j, i].SetInfo(false);
				m_inventoryGridList[j, i].SetGridType(GridType.Inventory);
			}
		}

		m_checkType = GridType.Empty;
	}

    // Update is called once per frame
    void Update()
    {
		if(Input.GetMouseButtonUp(0))
		{
			SetItem();
			// ���I����Ԃɖ߂�
			m_checkType = GridType.Empty;
		}
    }

	// �A�C�e�����󂫘g�ɃZ�b�g����
	public void SetItem()
	{
		// �ړ����̃A�C�e�����Ȃ��Ƃ��͖���
		if (m_moveItemTransform.childCount <= 0) return;

		// �A�C�e���ړ��p�̃I�u�W�F�N�g�̒��g���m�F
		GameObject item = m_moveItemTransform.GetChild(0).gameObject;

		Grid[,] list = null;
		int height = 0;
		int width = 0;

		switch(m_checkType)
		{
			case GridType.Stash:
				list = m_stashGridList;
				height = m_stashHeight;
				width = m_stashWidth;
				break;

			case GridType.Inventory:
				list = m_inventoryGridList;
				height = m_inventoryHeight;
				width = m_inventoryWidth;
				break;

			// GridType.Empty
			default:
				// �}�X�ڂ�I�����Ă��Ȃ��Ƃ�
				item.GetComponent<Item_Object>().PointerUp(false);
				return;
		}

		// ���X�g����
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				// �J�[�\���̏d�Ȃ��Ă���}�X����̎�
				if (list[j, i].OnPointer() && !list[j, i].GetInfo())
				{
					if (CheckSpace(new Vector2Int(j, i), item.GetComponent<Item_Object>().GetSize()))
					{
						// �ړ���̎q�I�u�W�F�N�g�ɐݒ肷�� or ���̈ʒu�ɖ߂�
						item.GetComponent<Item_Object>().PointerUp(
							true,
							list[j, i].GetTransform()
							);
						// ��_�̃C���f�b�N�X��ێ�
						item.GetComponent<Item_Object>().SetIndex(new Vector2Int(j, i));
						// ���݂�gridtype��ۊ�
						item.GetComponent<Item_Object>().SetType(m_checkType);
					}
					else
					{
						// ���̈ʒu�ɖ߂�
						item.GetComponent<Item_Object>().PointerUp(false);
					}

					return;
				}
			}
		}

		// �I���}�X���󂢂Ă��Ȃ��Ƃ�
		item.GetComponent<Item_Object>().PointerUp(false);
	}

	// �A�C�e��������X�y�[�X���m�F
	private bool CheckSpace(Vector2Int startGrid, Vector2Int size)
	{
		Debug.Log(startGrid.x + (size.x - 1));
		Debug.Log(startGrid.y + (size.y - 1));

		Grid[,] list = null;
		int height = 0;
		int width = 0;

		switch(m_checkType)
		{
			case GridType.Stash:
				list = m_stashGridList;
				height = m_stashHeight;
				width = m_stashWidth;
				break;

			case GridType.Inventory:
				list = m_inventoryGridList;
				height = m_inventoryHeight;
				width = m_inventoryWidth;
				break;
		}

		// �g�O�ɂ͂ݏo���Ƃ��͂��������m�F���Ȃ�
		if (startGrid.x + (size.x - 1) >= width) return false;
		if (startGrid.y + (size.y - 1) >= height) return false;

		// ���g���m�F
		for (int i = startGrid.x; i < startGrid.x + size.x; i++)
		{
			for(int j = startGrid.y; j < startGrid.y + size.y; j++)
			{
				if (list[i, j].GetInfo())
				{
					// ���g������Ƃ���false
					return false;
				}
			}
		}

		// �X�y�[�X���󂢂Ă���Ƃ��͒��g�������Ă��邱�Ƃɂ���
		for (int i = startGrid.x; i < startGrid.x + size.x; i++)
		{
			for (int j = startGrid.y; j < startGrid.y + size.y; j++)
			{
				list[i, j].SetInfo(true);
			}
		}

		return true;
	}

	// �w�肵���}�X�̏�Ԃ�ύX����
	public void MoveItem(Vector2Int basePos, Vector2Int size, bool info, GridType type)
	{
		// �A�h���X�R�s�[
		Grid[,] list = null;

		switch (type)
		{
			case GridType.Stash:
				list = m_stashGridList;
				break;

			case GridType.Inventory:
				list = m_inventoryGridList;
				break;
		}

		// �X�y�[�X���󂢂Ă���Ƃ��͒��g�������Ă��邱�Ƃɂ���
		for (int i = basePos.x; i < basePos.x + size.x; i++)
		{
			for (int j = basePos.y; j < basePos.y + size.y; j++)
			{
				list[i, j].SetInfo(info);
			}
		}
	}

	// �A�C�e����u���邩��T��
	public void StartSet(GridType type)
	{
		m_checkType = type;
	}
}
