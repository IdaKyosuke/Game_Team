using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentStatusUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler  
{

    public void OnPointerEnter(PointerEventData eventData)
    { 
        transform.GetChild(2).gameObject.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
		transform.GetChild(2).gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
		transform.GetChild(2).gameObject.SetActive(false);
    }

    //e‚ª”ñ•\¦‚É‚È‚Á‚½‚ç©g‚ğíœ
    private void OnDisable()
    {
		transform.GetChild(2).gameObject.SetActive(false);
    }
}