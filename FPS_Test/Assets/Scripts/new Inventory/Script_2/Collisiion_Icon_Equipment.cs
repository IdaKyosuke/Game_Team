using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Collisiion_Icon_Equipment : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	// é©ï™ÇÃêe
	[SerializeField] GridIcon_Equipment m_icon;

	public void OnPointerEnter(PointerEventData eventData)
	{
		m_icon.SetPointerInfo(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		m_icon.SetPointerInfo(false);
	}
}
