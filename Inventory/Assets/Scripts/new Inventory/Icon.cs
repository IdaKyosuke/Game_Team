using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	Image icon;
	[SerializeField] Image icon1;
	[SerializeField] Image icon2;
	[SerializeField] Image icon3;
	public enum IconType
	{
		Inventory,
		Stash,

		Length
	}

	IconType m_iconType; 
	private bool m_onPointer = false;

	// UIにオブジェクトがのっているかどうか
	private bool m_fillUi = false;

	private void Awake()
	{
		icon = GetComponent<Image>();
		m_iconType = transform.parent.name == "Grid" ? IconType.Inventory : IconType.Stash;
	}

	private void Update()
	{
		icon.enabled = !m_fillUi;
		icon1.enabled = !m_fillUi;
		icon2.enabled = !m_fillUi;
		icon3.enabled = !m_fillUi;
	}

	public void MouseUp()
	{
		if (m_onPointer)
		{
			InventoryManager.Check(this, m_iconType);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		m_onPointer = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		m_onPointer = false;
	}

	public bool GetOnFillUi()
	{
		return m_fillUi;
	}

	public void SetUi(bool value)
	{
		m_fillUi = value;
	}
}
