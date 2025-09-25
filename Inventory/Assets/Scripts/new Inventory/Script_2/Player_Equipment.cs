using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using TMPro;

public class Player_Equipment : MonoBehaviour
{
	// �����g�̃��X�g
	[SerializeField] List<GameObject> m_Equipments = new List<GameObject>();
	// �����l�̃��X�g
	private int[] m_wepaonStatus = new int[(int)WeaponStatusType.Length];
	// ���l�\���p�̃e�L�X�g
	[SerializeField] List<TextMeshProUGUI> m_texts;

    // Start is called before the first frame update
    void Start()
    {
		// ���l�����Z�b�g
		for(int i = 0; i < (int)WeaponStatusType.Length; i++)
		{
			m_wepaonStatus[i] = 0;
		}
	}

    // Update is called once per frame
    void Update()
    {
		// ���l�����Z�b�g
		for (int i = 0; i < (int)WeaponStatusType.Length; i++)
		{
			m_wepaonStatus[i] = 0;
		}

		// �����g����
		foreach (GameObject item in m_Equipments)
		{
			// �����g����̏ꍇ��΂�
			if (item.transform.childCount == 0) continue;

			// �����̃X�e�[�^�X�����Z
			Info_Equipment info = item.transform.GetChild(0).GetComponent<Info_Equipment>();
			for (int i = 0; i < (int)WeaponStatusType.Length; i++)
			{
				m_wepaonStatus[i] += info.GetInfo((Info_Equipment.WeaponStatusType)i);
			}
		}

		// �e�L�X�g�̕ҏW
		for (int i = 0; i < (int)WeaponStatusType.Length; i++)
		{
			m_texts[i].SetText("{0}", m_wepaonStatus[i]);
		}
	}
}
