using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot_City : MonoBehaviour
{
	// インベントリ用Ui
	[SerializeField] Image icon;               // アイテムのアイコン
	[SerializeField] GameObject removeButton;  // アイテムを削除するボタン
	private Test_Item item;               // スロットに格納されるアイテム

	// アイテムの詳細用Ui
	[SerializeField] GameObject Detail; //詳細の背景（アイテム詳細の親）
	[SerializeField] Image DetailIcon;  // アイテムの画像
	[SerializeField] TextMeshProUGUI DetailText;    // アイテムの説明
	[SerializeField] TextMeshProUGUI DetailPrice;   // アイテムの値段
	[SerializeField] TextMeshProUGUI DetailName;   // アイテムの名前
	[SerializeField] TextMeshProUGUI DetailType;   // アイテムの種類

	private void Start()
	{
		// 詳細を表示
		Detail.SetActive(false);
	}

	// アイテムを追加
	public void AddItem(Test_Item newItem)
	{
		// インベントリ用Uiの設定
		item = newItem;
		icon.sprite = newItem.icon;
		icon.enabled = true;
		removeButton.SetActive(true);
	}

	// アイテムをクリア
	public void ClearItem()
	{
		// インベントリ用Uiのリセット
		item = null;
		icon.sprite = null;
		icon.enabled = false;
		removeButton.SetActive(false);
	}

	// アイテムの削除
	public void OnRemoveButton()
	{
		Inventory_City.instance.RemoveItem(item);
	}

	// アイテムの詳細を表示
	public void ShowDetail()
	{
		if (item == null) return;
		SetUi();
	}

	// アイテムの詳細を非表示
	public void HideDetail()
	{
		if (item == null) return;
		ResetUi();
	}

	// アイテムの詳細Uiをリセット
	private void ResetUi()
	{
		// 詳細用Uiのリセット
		DetailIcon.sprite = null;
		DetailText.SetText("");
		DetailPrice.SetText("");
		DetailName.SetText("");
		DetailType.SetText("");
		// 詳細を表示
		Detail.SetActive(false);
	}

	// アイテムのUiを設定
	private void SetUi()
	{
		// 詳細用Uiの設定
		DetailIcon.sprite = item.icon;
		DetailText.SetText(item.explain);
		DetailPrice.SetText(item.sell_price.ToString());
		DetailName.SetText(item.name);
		DetailType.SetText(item.ItemType());
		// 詳細を表示
		Detail.SetActive(true);
	}
}
