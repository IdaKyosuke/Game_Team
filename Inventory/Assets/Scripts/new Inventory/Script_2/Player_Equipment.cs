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
	private int[] m_wepaonStatus = new int[(int)WeaponStatusType.Length];
	// 数値表示用のテキスト
	[SerializeField] List<TextMeshProUGUI> m_texts;

    // Start is called before the first frame update
    void Start()
    {
		// 数値をリセット
		for(int i = 0; i < (int)WeaponStatusType.Length; i++)
		{
			m_wepaonStatus[i] = 0;
		}
	}

    // Update is called once per frame
    void Update()
    {
		// 数値をリセット
		for (int i = 0; i < (int)WeaponStatusType.Length; i++)
		{
			m_wepaonStatus[i] = 0;
		}

		// 装備枠分回す
		foreach (GameObject item in m_Equipments)
		{
			// 装備枠が空の場合飛ばす
			if (item.transform.childCount == 0) continue;

			// 装備のステータスを加算
			Info_Equipment info = item.transform.GetChild(0).GetComponent<Info_Equipment>();
			for (int i = 0; i < (int)WeaponStatusType.Length; i++)
			{
				m_wepaonStatus[i] += info.GetInfo((Info_Equipment.WeaponStatusType)i);
			}
		}

		// テキストの編集
		for (int i = 0; i < (int)WeaponStatusType.Length; i++)
		{
			m_texts[i].SetText("{0}", m_wepaonStatus[i]);
		}
	}
}
