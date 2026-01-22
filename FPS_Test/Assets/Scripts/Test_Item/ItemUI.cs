using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] TextMeshProUGUI m_itemText;
    [SerializeField] Potion m_potion;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //説明文表示
        transform.GetChild(0).gameObject.SetActive(true);
        m_itemText.text = m_potion.Data.m_description;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    //親が非表示になったら自身を削除
    private void OnDisable()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
