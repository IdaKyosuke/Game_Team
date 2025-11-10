using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentStatusUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler  
{
    [SerializeField] GameObject m_ui;

    public void OnPointerEnter(PointerEventData eventData)
    { 
        m_ui.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       m_ui.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       m_ui.SetActive(false);
    }
}
