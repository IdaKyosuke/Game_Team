using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PotionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] TextMeshProUGUI m_itemText;
    [SerializeField] TextMeshProUGUI m_itemName;
    [SerializeField] Potion m_potion;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //説明文表示
        transform.GetChild(0).gameObject.SetActive(true);
        m_itemText.text = m_potion.Data.description;
        m_itemName.text = m_potion.Data.potionName;
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
