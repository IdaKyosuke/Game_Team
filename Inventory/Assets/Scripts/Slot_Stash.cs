using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// �X�^�b�V���̃v���C���[�C���x���g���p
public class Slot_Stash : MonoBehaviour
{
	// �C���x���g���pUi
	[SerializeField] Image icon;               // �A�C�e���̃A�C�R��
	[SerializeField] GameObject m_playerStatus;
	[SerializeField] GameObject Collider;   // �J�[�\������p

	private Test_Item item;               // �X���b�g�Ɋi�[�����A�C�e��

	private void Start()
	{
		if (!m_playerStatus)
		{
			m_playerStatus = GameObject.FindWithTag("playerStatus");
		}

		// �V�[���J�n���ɒ��g�����邩�ǂ����œ����蔻��̗L�������߂�
		if(item)
		{
			Collider.SetActive(true);
		}
		else
		{
			Collider.SetActive(false);
		}
	}

	// �A�C�e����ǉ�
	public void AddItem(Test_Item newItem)
	{
		// �C���x���g���pUi�̐ݒ�
		item = newItem;
		icon.sprite = newItem.icon;
		icon.enabled = true;
		Collider.SetActive(true);
	}

	// �A�C�e�����N���A
	public void ClearItem()
	{
		// �C���x���g���pUi�̃��Z�b�g
		item = null;
		icon.sprite = null;
		icon.enabled = false;
		Collider.SetActive(false);
	}

	// �C���x���g������A�C�e���{�b�N�X�ɑ���
	public void PutInBox()
	{
		// �A�C�e���{�b�N�X�ɃA�C�e���𑗂�
		Stash_Box.instance.AddItem(item);

		// �v���C���[�̃C���x���g�����瑗�����A�C�e�����폜����
		Inventory_City.instance.RemoveItem(item);
	}

	// �A�C�e���{�b�N�X����C���x���g���ɑ���
	public void PutInInventory()
	{
		// �v���C���[�̃C���x���g���ɃA�C�e���𑗂�
		Inventory_City.instance.AddItem(item);

		// �A�C�e���{�b�N�X����A�C�e�����폜����
		Stash_Box.instance.RemoveItem(item);
	}
}
