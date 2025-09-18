using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragObject : MonoBehaviour,IPointerDownHandler
{
	[SerializeField] private ExcelData m_excelData;
	[SerializeField] private int m_id;
	[SerializeField] private Vector2Int m_positon;
	List<Vector2Int> m_setUiPos = new List<Vector2Int>();
	Transform m_canvs;
	Transform iconParent;
	Icon.IconType m_iconType = Icon.IconType.Inventory;
	InventoryManager m_manager;
	private ObjectEntity m_data;
	private bool m_IsDragging;
	private Vector2 prevPos; //�ۑ����Ă�������position
	private RectTransform rectTransform; // �ړ��������I�u�W�F�N�g��RectTransform
	private RectTransform parentRectTransform; // �ړ��������I�u�W�F�N�g�̐e(Panel)��RectTransform
	private Image m_image;

	private void Awake()
	{
		m_canvs = GameObject.Find("StashUi").transform;
		m_data = m_excelData.Object[m_id];
		m_IsDragging = false;
		rectTransform = GetComponent<RectTransform>();
		m_image = GetComponent<Image>();
		parentRectTransform = rectTransform.parent as RectTransform;
		m_manager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
	}

	private void Start()
	{ 
		m_manager.SetDragObject(this);
		InventoryManager.Check(InventoryManager.GetIcon(m_positon, Icon.IconType.Inventory), Icon.IconType.Inventory);
	}


	// �h���b�O�J�n���̏���
	public void OnPointerDown(PointerEventData eventData)
	{
		m_manager.TakeObject(m_setUiPos, m_iconType);
		// �`�揇���Ō��
		transform.SetAsLastSibling();

		// �����_�̐e��ۑ�
		iconParent = transform.parent;
		// ��x�e���L�����o�X�ɕύX
		transform.SetParent(m_canvs);
		m_manager.SetDragObject(this);
		// �h���b�O�O�̈ʒu���L�����Ă���
		prevPos = rectTransform.anchoredPosition;

		// �ړ����ɐݒ肵�ē����蔻����ꎞ�I�ɏ���
		m_IsDragging = true;
		m_image.raycastTarget = false;
	}

	// �h���b�O���̏���
	private void Update()
	{
		if (m_IsDragging)
		{
			Vector2 localPosition = GetLocalPosition(Input.mousePosition);
			rectTransform.anchoredPosition = localPosition;

			if (Input.GetMouseButtonUp(0))
			{
				// �ݒu����A�C�R�����擾
				var icon = m_manager.GetIcon();
				if (icon != null)
				{
					icon.MouseUp();
				}
				else
				{
					// �K�؂Ȑݒu�ꏊ����Ȃ��ꍇ���̈ʒu�ɖ߂�
					m_IsDragging = false;
					m_image.raycastTarget = true;
					rectTransform.anchoredPosition = prevPos;
					transform.SetParent(iconParent);
					InventoryManager.Check(iconParent.GetComponent<Icon>(), m_iconType);
				}
			}
		}
	}

	public void DragEnd(bool isEmpty, Vector2 position, Transform icon, List<Vector2Int> posList, Icon.IconType iconType)
	{
		if (isEmpty)
		{
			// �ݒu���ł���ꍇ�|�W�V�����Ɛe��ݒ肷��
			rectTransform.localPosition = (Vector2)rectTransform.parent.InverseTransformPoint(position);
			transform.SetParent(icon);
			m_setUiPos = posList;
			m_iconType = iconType;
		}
		else
		{
			// �ݒu�ł��Ȃ��ꍇ���̈ʒu�ɖ߂���Check�֐��Ō��̈ʒu�̃t���O�����Ă�
			rectTransform.anchoredPosition = prevPos;
			transform.SetParent(iconParent);
			InventoryManager.Check(iconParent.GetComponent<Icon>(), m_iconType);
		}
		m_IsDragging = false;
		m_image.raycastTarget = true;
	}

	// ScreenPosition����localPosition�ւ̕ϊ��֐�
	private Vector2 GetLocalPosition(Vector2 screenPosition)
	{
		Vector2 result = Vector2.zero;

		// screenPosition��e�̍��W�n(parentRectTransform)�ɑΉ�����悤�ϊ�����.
		RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPosition, Camera.main, out result);

		return result;
	}

	public Vector2 GetSize()
	{
		return new Vector2(m_data.width, m_data.height);
	}
}
