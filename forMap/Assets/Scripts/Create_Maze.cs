using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create_Maze : MonoBehaviour
{
	enum Direction
	{
		Up,
		Down,
		Left,
		Right,

		Length,
	}

	[SerializeField] int m_frameSize = 7;
	private List<List<bool>> m_map = new List<List<bool>>();

	[SerializeField] GameObject m_mapPrefab;

	private int m_size = 45;

	// Start is called before the first frame update
	void Start()
    {
		// �T���p�}�b�v���쐬
		for(int i = 0; i < m_frameSize; i++)
		{
			m_map.Add(new List<bool>());

			for(int j = 0; j < m_frameSize; j++)
			{
				m_map[i].Add(false);
			}
		}

		// �����}�X��I���i�X�^�[�g������W�j
		int x = 0 + Random.Range(0, m_frameSize / 2) * 2;
		int y = 0 + Random.Range(0, m_frameSize / 2) * 2;

		// �}�b�v�쐬
		MakeMap(x, y);

		// �}�b�v�z�u
		SetPrefab();
	}

	private void MakeMap(int x, int y)
	{
		// ����true�ɂ���
		m_map[x][y] = true;

		// �E�ׂ肪�����F�H
		if (x + 2 < m_frameSize && !m_map[x + 2][y])  // if���͍����画�f����Ă���
		{
			m_map[x + 1][y] = true;
			MakeMap(x + 2, y);
		}
		// ���ׂ肪�����F�H
		if (x - 2 >= 0 && !m_map[x - 2][y])  // if���͍����画�f����Ă���
		{
			m_map[x - 1][y] = true;
			MakeMap(x - 2, y);
		}
		// ���ׂ肪�����F�H
		if (y + 2 < m_frameSize && !m_map[x][y + 2]) // if���͍����画�f����Ă���
		{
			m_map[x][y + 1] = true;
			MakeMap(x, y + 2);
		}
		// ��ׂ肪�����F�H
		if (y - 2 >= 0 && !m_map[x][y - 2])  // if���͍����画�f����Ă���
		{
			m_map[x][y - 1] = true;
			MakeMap(x, y - 2);
		}
	}

	private void SetPrefab()
	{
		for(int i = 0; i < m_frameSize; i++)
		{
			for (int j = 0; j < m_frameSize; j++)
			{
				if (m_map[i][j])
				{
					Instantiate(m_mapPrefab, new Vector3(m_size * i, 0, m_size * j), Quaternion.identity);
				}
			}
		}
	}
}
