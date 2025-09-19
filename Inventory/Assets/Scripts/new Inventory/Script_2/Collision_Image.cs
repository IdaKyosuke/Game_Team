using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Collision_Image : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	// �����̐e
	[SerializeField] GridIcon m_icon;

	public void OnPointerEnter(PointerEventData eventData)
	{
		m_icon.SetPointerInfo(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		m_icon.SetPointerInfo(false);
	}
}
