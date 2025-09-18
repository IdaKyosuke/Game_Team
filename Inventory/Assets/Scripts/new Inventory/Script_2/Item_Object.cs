using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Icon;

public class Item_Object : MonoBehaviour
{
	[SerializeField] Info_ItemSize m_info;
	private bool m_isPointerEnter = false;
	private bool m_isDrag = false;

	private RectTransform rectTransform; // �ړ��������I�u�W�F�N�g��RectTransform
	private RectTransform parentRectTransform; // �ړ��������I�u�W�F�N�g�̐e(Panel)��RectTransform
	private Vector2 prevPos; //�ۑ����Ă�������position
	Transform iconParent;

	private Transform m_moveItemTransform;  // �ړ����Ɋi�[�����I�u�W�F�N�g
	[SerializeField] GameObject m_collider;	// �����蔻��p�̉摜

	// Start is called before the first frame update
	void Start()
    {
		rectTransform = GetComponent<RectTransform>();
		parentRectTransform = rectTransform.parent as RectTransform;        
		// �����_�̐e��ۑ�
		iconParent = transform.parent;

		// �ړ����Ɋi�[�����ꏊ
		m_moveItemTransform = GameObject.FindWithTag("moveItemTransform").transform;
	}

    // Update is called once per frame
    void Update()
    {
		// �A�C�e����͂�ł��鎞
        if(m_isDrag)
		{
			// �}�E�X�J�[�\����͂�ł���A�C�e�����Ǐ]����
			Vector2 localPosition = GetLocalPosition(Input.mousePosition);
			rectTransform.anchoredPosition = localPosition;
		}
    }

	// �A�C�e���̃T�C�Y�����擾
	public Vector2Int GetSize()
	{
		return m_info.GetSize();
	}
	
	public void PointerEnter()
	{
		m_isPointerEnter = true;
	}

	public void PointerExit()
	{
		m_isPointerEnter = false;
	}

	public void PointerDown()
	{
		// �����蔻��p�̉摜���A�N�e�B�u�ɂ���
		m_collider.SetActive(false);
		// �����_�̐e��ۑ�
		iconParent = transform.parent;
		// �h���b�O�O�̈ʒu���L�����Ă���
		prevPos = rectTransform.anchoredPosition;
		// �ړ����p�̃I�u�W�F�N�g��e�ɕύX
		SetParentTransform(m_moveItemTransform);
		// �}�E�X�J�[�\���̒Ǐ]�J�n
		m_isDrag = true;
	}

	public void PointerUp(bool canSet, Transform nextPos = null)
	{
		// �}�E�X�J�[�\���̒Ǐ]���I��
		m_isDrag = false;

		if(canSet)
		{
			// �ړ��\
			// �ړ���̘g��e�I�u�W�F�N�g�ɐݒ�
			SetParentTransform(nextPos);
			rectTransform.anchoredPosition = prevPos;
			// �����蔻��p�̉摜���A�N�e�B�u�ɂ���
			m_collider.SetActive(true);
		}
		else
		{
			// �ړ��s�\
			// ���������ʒu�ɖ߂�
			SetParentTransform(iconParent);
			rectTransform.anchoredPosition = prevPos;
			// �����蔻��p�̉摜���A�N�e�B�u�ɂ���
			m_collider.SetActive(true);
		}
	}

	// ScreenPosition����localPosition�ւ̕ϊ��֐�
	private Vector2 GetLocalPosition(Vector2 screenPosition)
	{
		Vector2 result = Vector2.zero;

		// screenPosition��e�̍��W�n(parentRectTransform)�ɑΉ�����悤�ϊ�����.
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			parentRectTransform, 
			screenPosition, 
			Camera.main, 
			out result
			);

		return result;
	}

	// ���g�̐e���ύX���ꂽ�Ƃ��ɁARectTransform���ꏏ�ɕύX����
	private void SetParentTransform(Transform parent)
	{
		transform.SetParent(parent);
		rectTransform = GetComponent<RectTransform>();
		parentRectTransform = rectTransform.parent as RectTransform;
	}
}
