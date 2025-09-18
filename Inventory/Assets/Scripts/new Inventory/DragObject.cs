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


	// �h���b�O�J�n���̏���
	public void OnPointerDown(PointerEventData eventData)
	{
		m_manager.TakeObject(m_setUiPos);
		transform.SetAsLastSibling();

		iconParent = transform.parent;
		transform.SetParent(m_canvs);
		m_manager.SetDragObject(this);
		//Debug.Log(m_data.width + "�~" + m_data.height);
		// �h���b�O�O�̈ʒu���L�����Ă���
		// RectTransform�̏ꍇ��position�ł͂Ȃ�anchoredPosition���g��
		prevPos = rectTransform.anchoredPosition;
		m_IsDragging = true;
		m_image.raycastTarget = false;
		//Debug.Log(m_objectData.width + ":" + m_objectData.height);
	}

	// �h���b�O���̏���
	private void Update()
	{
		// eventData.position����A�e�ɏ]��localPosition�ւ̕ϊ����s��
		// �I�u�W�F�N�g�̈ʒu��localPosition�ɕύX����
		if (m_IsDragging)
		{
			Vector2 localPosition = GetLocalPosition(Input.mousePosition);
			rectTransform.anchoredPosition = localPosition;

			if (Input.GetMouseButtonUp(0))
			{
				var icon = m_manager.GetIcon();
				if (icon != null)
				{
					icon.MouseUp();
				}
				else
				{
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
		m_iconType = iconType;
		Debug.Log(rectTransform.parent.InverseTransformPoint(position));
		// �I�u�W�F�N�g���h���b�O�O�̈ʒu�ɖ߂�
		m_IsDragging = false;
		m_image.raycastTarget = true;
		if (isEmpty)
		{
			float tmpZ = rectTransform.localPosition.z;
			rectTransform.localPosition = rectTransform.parent.InverseTransformPoint(position);
			rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, tmpZ);
			transform.SetParent(icon);
			m_setUiPos = posList;
		}
		else
		{
			rectTransform.anchoredPosition = prevPos;
		}
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
