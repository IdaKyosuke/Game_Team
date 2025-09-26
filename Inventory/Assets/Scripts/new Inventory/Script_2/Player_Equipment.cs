using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using TMPro;

public class Player_Equipment : MonoBehaviour
{
	// 装備枠のリスト
	[SerializeField] List<GameObject> m_Equipments = new List<GameObject>();
	// 実数値のリスト
	private int[] m_weaponStatus = new int[(int)WeaponStatusType.Length];
	// 数値表示用のテキスト
	[SerializeField] List<TextMeshProUGUI> m_texts;

    // Start is called before the first frame update
    void Start()
    {
		// 数値をリセット
		for(int i = 0; i < (int)WeaponStatusType.Length; i++)
		{
			m_weaponStatus[i] = 0;
		}
	}

    // Update is called once per frame
    void Update()
    {
		// 数値をリセット
		for (int i = 0; i < (int)WeaponStatusType.Length; i++)
		{
			m_weaponStatus[i] = 0;
		}

		// 装備枠分回す
		foreach (GameObject item in m_Equipments)
		{
			// 装備枠が空の場合0を加算していく
			if (item.transform.childCount == 0)
			{
				for (int i = 0; i < (int)WeaponStatusType.Length; i++)
				{
					m_weaponStatus[i] += 0;
				}
			}
			else
			{
				// 装備のステータスを加算
				Item_Object info = item.transform.GetChild(0).GetComponent<Item_Object>();
				for (int i = 0; i < (int)WeaponStatusType.Length; i++)
				{
					m_weaponStatus[i] += info.GetEquipmentInfo((Info_Equipment.WeaponStatusType)i);
				}
			}
		}

		// テキストの編集
		for (int i = 0; i < (int)WeaponStatusType.Length; i++)
		{
			m_texts[i].SetText("{0}", m_weaponStatus[i]);
		}
	}
}
