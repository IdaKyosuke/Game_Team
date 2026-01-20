using UnityEngine;

public enum EquipmentType
{
	Helmet,		// 頭
	Armor,		// 胴
	Gauntlet,	// 腕
	Shoes,		// 靴
	Weapon,		// 武器
	None,       // 武器じゃない
	Potion,		// ポーション

	Length,
}

[DefaultExecutionOrder(-60)]
public class GridIcon_Equipment : MonoBehaviour
{
	private bool m_onPointer = false;
	// UIにオブジェクトがのっているかどうか
	private bool m_fillUi = false;
	private bool m_pastInfo;

	// 装備枠のタイプ
	[SerializeField] EquipmentType m_type;

	// アイテム移動用の仮置きオブジェクト
	Transform m_moveItemTransform;

	// stashManagerを貰うための親
	[SerializeField] GameObject m_parent;

	// 自分の装備枠かどうか
	[SerializeField] bool m_isMine = false;

	private StashManager m_stashManager;

	// Start is called before the first frame update
	void Start()
	{
		m_pastInfo = m_fillUi;
		if(!m_moveItemTransform)
		{
			m_stashManager = m_parent.GetComponent<Inventory_Parent>().GetStashManager();
			m_moveItemTransform = m_stashManager.GetMoveItemTransform();
		}
	}

	// Update is called once per frame
	void Update()
	{
		// 自分の上でドロップされたとき
		if (Input.GetMouseButtonUp(0) && m_onPointer)
		{
			m_stashManager.StartSet(GridType.Equipment);
			Equip();
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

	// 装備枠のタイプを取得
	public EquipmentType GetEquipmentType()
	{
		return m_type;
	}

	// 装備可能か確認 => 可能なら装備
	private void Equip()
	{
		// 移動中のアイテムがないときは無視
		if (m_moveItemTransform.childCount == 0) return;

		GameObject o = m_moveItemTransform.GetChild(0).gameObject;
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
			if (transform.childCount != 0)
			{
				// すでに中身が設定されている時、新しく追加したものを元の場所に戻す
				o.GetComponent<Item_Object>().PointerUp(false);
			}
			else
			{
				// 新しく装備する
				o.GetComponent<Item_Object>().PointerUp(true, transform);
				// 新しく装備された物を装備状態にする
				o.GetComponent<Item_Object>().SetEquipValue(true, m_isMine);

				// 武器の時だけプレイヤーに状態異常を付与する
				if(o.GetComponent<Item_Object>().GetWeaponType() == EquipmentType.Weapon)
				{
					o.GetComponent<EquipmentStatus>().SetPassive();
				}
			}
		}

		m_stashManager.StartSet(GridType.Empty);
	}

	public void QuickEquip(GameObject item, bool firstSetItemFlg = true)
	{
		// 中身があるときは飛ばす
		if (transform.childCount != 0)
		{
			item.GetComponent<Item_Object>().PointerUp(false);
			return;
		}

		// 装備を枠に入れる
		item.GetComponent<Item_Object>().PointerUp(true, transform);
		// 装備状態にする
		item.GetComponent<Item_Object>().SetEquipValue(true, m_isMine, firstSetItemFlg);
	}
}
