using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public enum EquipmentType
{
	Helmet,		// 頭
	Armor,		// 胴
	Gauntlet,	// 腕
	Shoes,		// 靴
	Weapon,		// 武器
	None,		// 武器じゃない

	Length,
}

public class GridIcon_Equipment : MonoBehaviour
{
	private bool m_onPointer = false;
	// UIにオブジェクトがのっているかどうか
	private bool m_fillUi = false;
	private bool m_pastInfo;

	// 装備枠のタイプ
	[SerializeField] EquipmentType m_type;

	// アイテム移動用の仮置きオブジェクト
	[SerializeField] GameObject m_moveItemTransform;

	// Start is called before the first frame update
	void Start()
	{
		m_pastInfo = m_fillUi;
		if(!m_moveItemTransform)
		{
			m_moveItemTransform = GameObject.FindWithTag("moveItemTransform");
		}
	}

	// Update is called once per frame
	void Update()
	{
		// 自分の上でドロップされたとき
		if (Input.GetMouseButtonUp(0) && m_onPointer)
		{
			Debug.Log("in");
			// 移動中のアイテムがないときは無視
			if (m_moveItemTransform.transform.childCount == 0) return;

			GameObject o = m_moveItemTransform.transform.GetChild(0).gameObject;
			// アイテムが装備じゃないとき || 装備枠に対応した装備じゃないときは無視
			if (
				o.GetComponent<Item_Object>().GetWeaponType() == EquipmentType.None ||
				o.GetComponent<Item_Object>().GetWeaponType() != m_type
				)
			{
				o.GetComponent<Item_Object>().PointerUp(false);
			}
			else
			{
				// すでに中身が設定されている時、一旦中身を取り出す
				if (transform.childCount != 0)
				{
					transform.GetChild(0).transform.SetParent(m_moveItemTransform.transform);
				}

				// 新しく装備する
				o.GetComponent<Item_Object>().PointerUp(true, transform);
			}

		}
	}

	// ポインターの状態を設定
	public void SetPointerInfo(bool info)
	{
		m_onPointer = info;
	}

	// ポインターが重なっているか
	public bool OnPointer()
	{
		return m_onPointer;
	}

	// 中身が埋まっているか
	public bool GetOnFillUi()
	{
		return m_fillUi;
	}

	public void SetUi(bool value)
	{
		m_fillUi = value;
	}
}
