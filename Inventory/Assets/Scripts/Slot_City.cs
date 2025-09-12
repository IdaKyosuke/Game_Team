using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot_City : MonoBehaviour
{
	// �C���x���g���pUi
	[SerializeField] Image icon;               // �A�C�e���̃A�C�R��
	[SerializeField] GameObject removeButton;  // �A�C�e�����폜����{�^��
	private Test_Item item;               // �X���b�g�Ɋi�[�����A�C�e��

	// �A�C�e���̏ڍחpUi
	[SerializeField] GameObject Detail; //�ڍׂ̔w�i�i�A�C�e���ڍׂ̐e�j
	[SerializeField] Image DetailIcon;  // �A�C�e���̉摜
	[SerializeField] TextMeshProUGUI DetailText;    // �A�C�e���̐���
	[SerializeField] TextMeshProUGUI DetailPrice;   // �A�C�e���̒l�i
	[SerializeField] TextMeshProUGUI DetailName;   // �A�C�e���̖��O
	[SerializeField] TextMeshProUGUI DetailType;   // �A�C�e���̎��

	private void Start()
	{
		// �ڍׂ�\��
		Detail.SetActive(false);
	}

	// �A�C�e����ǉ�
	public void AddItem(Test_Item newItem)
	{
		// �C���x���g���pUi�̐ݒ�
		item = newItem;
		icon.sprite = newItem.icon;
		icon.enabled = true;
		removeButton.SetActive(true);
	}

	// �A�C�e�����N���A
	public void ClearItem()
	{
		// �C���x���g���pUi�̃��Z�b�g
		item = null;
		icon.sprite = null;
		icon.enabled = false;
		removeButton.SetActive(false);
	}

	// �A�C�e���̍폜
	public void OnRemoveButton()
	{
		Inventory_City.instance.RemoveItem(item);
	}

	// �A�C�e���̏ڍׂ�\��
	public void ShowDetail()
	{
		if (item == null) return;
		SetUi();
	}

	// �A�C�e���̏ڍׂ��\��
	public void HideDetail()
	{
		if (item == null) return;
		ResetUi();
	}

	// �A�C�e���̏ڍ�Ui�����Z�b�g
	private void ResetUi()
	{
		// �ڍחpUi�̃��Z�b�g
		DetailIcon.sprite = null;
		DetailText.SetText("");
		DetailPrice.SetText("");
		DetailName.SetText("");
		DetailType.SetText("");
		// �ڍׂ�\��
		Detail.SetActive(false);
	}

	// �A�C�e����Ui��ݒ�
	private void SetUi()
	{
		// �ڍחpUi�̐ݒ�
		DetailIcon.sprite = item.icon;
		DetailText.SetText(item.explain);
		DetailPrice.SetText(item.sell_price.ToString());
		DetailName.SetText(item.name);
		DetailType.SetText(item.ItemType());
		// �ڍׂ�\��
		Detail.SetActive(true);
	}
}
