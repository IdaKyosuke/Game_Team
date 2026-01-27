using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] TextMeshProUGUI m_itemName;
    [SerializeField] TextMeshProUGUI m_itemText;
    [SerializeField] TextMeshProUGUI m_itemValue;

    private Item_Object m_item;

    private void Start()
    {
        m_item = transform.parent.GetComponent<Item_Object>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //説明文表示
        transform.GetChild(0).gameObject.SetActive(true);
        m_itemText.text = "換金用アイテム";
        m_itemName.text = m_item.ItemData.displayName;
        m_itemValue.text = "売却額 [ ＄ " + (m_item.ItemData.price / 2).ToString() + " ]";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}