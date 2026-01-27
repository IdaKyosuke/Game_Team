using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PotionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI m_itemName;
    [SerializeField] TextMeshProUGUI m_itemText;
    [SerializeField] Potion m_potion;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //説明文表示
        transform.GetChild(0).gameObject.SetActive(true);
        m_itemName.text = m_potion.Data.potionName;
        m_itemText.text = m_potion.Data.description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StashManager manager = transform.root.GetComponent<StashManager>();

        // 左クリックでポーション使用
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            int index = transform.parent.GetComponent<Item_Object>().GetIndex();
            manager.RemoveInventory(manager.GetItemList()[index]);
        }
    }

    //親が非表示になったら自身を削除
    private void OnDisable()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
