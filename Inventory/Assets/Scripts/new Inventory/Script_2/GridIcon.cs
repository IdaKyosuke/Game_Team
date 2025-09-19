using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum GridType
{
	Inventory,
	Stash,

	Length,
}

public class GridIcon : MonoBehaviour
{
	private bool m_onPointer = false;
	// UIにオブジェクトがのっているかどうか
	private bool m_fillUi = false;
	private bool m_pastInfo;

	// アイテムが入ったときに消すUI
	[SerializeField] List<Image> m_images = new List<Image>();

	// 自分のタイプ
	private GridType m_type; 

	// Start is called before the first frame update
	void Start()
    {
		m_pastInfo = m_fillUi;
	}

    // Update is called once per frame
    void Update()
    {
		if(Input.GetMouseButtonUp(0) && m_onPointer)
		{
			GameObject.FindWithTag("inventoryManager").GetComponent<StashManager>().StartSet(m_type);
		}

		if (m_pastInfo != m_fillUi)
		{
			foreach(var g in m_images)
			{
				g.enabled = !m_fillUi;
			}
			m_pastInfo = m_fillUi;
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

	// タイプをセットする
	public void SetType(GridType type)
	{
		m_type = type;
	}
}
