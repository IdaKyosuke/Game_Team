using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
	private GameObject m_grid = null;	// �}�X�ڂ̃I�u�W�F�N�g
	private bool m_info = false;		// ���g�����܂��Ă��邩

	// �g�̏����Z�b�g
	public void SetGrid(GameObject grid)
	{
		m_grid = grid;
	}

	public GameObject GetObject()
	{
		return m_grid;
	}

	// �g��Transform���擾
	public Transform GetTransform()
	{
		return m_grid.transform;
	}

	// ���g�����邩���擾
	public bool GetInfo()
	{
		return m_grid.GetComponent<Icon>().GetOnFillUi();
	}

	// ���g�̏�Ԃ�ς���
	public void SetInfo(bool info)
	{
		m_grid.GetComponent<Icon>().SetUi(info);
	}
}

public class StashManager : MonoBehaviour
{
	[SerializeField] int m_width = 5;
	[SerializeField] int m_height = 10;

	[SerializeField] GameObject m_gridParent;	// �}�X�ڂ̐e�I�u�W�F�N�g

	private Grid[,] m_gridList;
	private Transform m_moveItemTransform;

	// Start is called before the first frame update
	void Start()
    {
		// �}�X�ڂ̔z����쐬
		m_gridList = new Grid[m_width, m_height];

		// �A�C�e���ړ��p�̃I�u�W�F�N�g
		m_moveItemTransform = GameObject.FindWithTag("moveItemTransform").transform;


		// �z��ƃ}�X�ڂ̏�Ԃ����킹��
		int count = 0;
		for(int i = 0; i < m_height; i++)
		{
			for(int j = 0; j < m_width; j++)
			{
				
				// �I�u�W�F�N�g��ǉ�
				GameObject g = m_gridParent.transform.GetChild(count).gameObject;
				Debug.Log(g);
				m_gridList[i, j].SetGrid(g);
				count++;
				// ���g����ɂ���
				m_gridList[i, j].SetInfo(false);
				
				Debug.Log(m_gridList[i, j].GetObject());
			}
		}
    }

    // Update is called once per frame
    void Update()
    {
		if(Input.GetMouseButtonUp(0))
		{
			// �ړ����̃A�C�e�����Ȃ��Ƃ��͖���
			if(m_moveItemTransform.childCount <= 0) return;

			Debug.Log(0);

			// ���X�g���񂷃J�[�\�����d�Ȃ��Ă���}�X��������
			for(int i = 0; i < m_height; i++)
			{
				for(int j = 0; j < m_width; j++)
				{
					// �J�[�\���̏d�Ȃ��Ă���}�X����̎�
					if (m_gridList[i, j].GetInfo())
					{
						// �A�C�e���ړ��p�̃I�u�W�F�N�g�̒��g���m�F
						GameObject item = m_moveItemTransform.gameObject;

						// �ړ���̎q�I�u�W�F�N�g�ɐݒ肷�� or ���̈ʒu�ɖ߂�
						item.GetComponent<Item_Object>().PointerUp(
							CheckSpace(new Vector2Int(i, j), item.GetComponent<Item_Object>().GetSize()),
							m_gridList[i, j].GetTransform()
							);
					}
				}
			}
		}
    }

	// �A�C�e��������X�y�[�X���m�F
	private bool CheckSpace(Vector2Int startGrid, Vector2Int size)
	{
		// �g�O�ɂ͂ݏo���Ƃ��͂��������m�F���Ȃ�
		if(startGrid.x + size.x >= m_width) return false;
		if(startGrid.y + size.y >= m_height) return false;

		// ���g���m�F
		for(int i = startGrid.x; i < startGrid.x + size.x; i++)
		{
			for(int j = startGrid.y; j < startGrid.y + size.y; j++)
			{
				if (m_gridList[i, j].GetInfo())
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
				m_gridList[i, j].SetInfo(true);
			}
		}

		return true;
	}
}
