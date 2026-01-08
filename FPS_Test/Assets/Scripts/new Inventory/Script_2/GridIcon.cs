using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum GridType
{
	Inventory,
	Stash,
	Empty,

	Equipment,

	Length,
}

[DefaultExecutionOrder(-99)]
public class GridIcon : MonoBehaviour
{
	private bool m_onPointer = false;
	// UIにオブジェクトがのっているかどうか
	private bool m_fillUi = false;
	private bool m_pastInfo;

	Image m_image;
	// 自分のタイプ
	private GridType m_type;

	[SerializeField] GameObject m_parent;
	private StashManager m_stashManager; 

	// Start is called before the first frame update
	void Start()
    {
		m_pastInfo = m_fillUi;
		if(!m_image)
		{
			m_image = GetComponent<Image>();
		}
		m_stashManager = m_parent.GetComponent<Inventory_Parent>().GetStashManager();
	}

    // Update is called once per frame
    void Update()
    {
		// 自分の上でドロップされたとき
		if(Input.GetMouseButtonUp(0) && m_onPointer)
		{
			m_stashManager.StartSet(m_type);
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
		Debug.Log(transform.gameObject.name + ": set ui");
		m_fillUi = value;
        if (!m_image)
        {
            m_image = GetComponent<Image>();
        }
        m_image.enabled = !value;
	}

	// タイプをセットする
	public void SetType(GridType type)
	{
		m_type = type;
	}

	// タイプを取得する
	public GridType GetGridType()
	{
		return m_type;
	}
}
