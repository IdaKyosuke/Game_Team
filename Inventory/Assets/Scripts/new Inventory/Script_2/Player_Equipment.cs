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
	private int[] m_weaponStatus = new int[(int)WeaponStatusType.Length];
	// ���l�\���p�̃e�L�X�g
	[SerializeField] List<TextMeshProUGUI> m_texts;

    // Start is called before the first frame update
    void Start()
    {
		// ���l�����Z�b�g
		for(int i = 0; i < (int)WeaponStatusType.Length; i++)
		{
			m_weaponStatus[i] = 0;
		}
	}

    // Update is called once per frame
    void Update()
    {
		// ���l�����Z�b�g
		for (int i = 0; i < (int)WeaponStatusType.Length; i++)
		{
			m_weaponStatus[i] = 0;
		}

		// �����g����
		foreach (GameObject item in m_Equipments)
		{
			// �����g����̏ꍇ0�����Z���Ă���
			if (item.transform.childCount == 0)
			{
				for (int i = 0; i < (int)WeaponStatusType.Length; i++)
				{
					m_weaponStatus[i] += 0;
				}
			}
			else
			{
				// �����̃X�e�[�^�X�����Z
				Item_Object info = item.transform.GetChild(0).GetComponent<Item_Object>();
				for (int i = 0; i < (int)WeaponStatusType.Length; i++)
				{
					m_weaponStatus[i] += info.GetEquipmentInfo((Info_Equipment.WeaponStatusType)i);
				}
			}
		}

		// �e�L�X�g�̕ҏW
		for (int i = 0; i < (int)WeaponStatusType.Length; i++)
		{
			m_texts[i].SetText("{0}", m_weaponStatus[i]);
		}
	}
}
