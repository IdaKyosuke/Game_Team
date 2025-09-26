using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public enum EquipmentType
{
	Helmet,		// ��
	Armor,		// ��
	Gauntlet,	// �r
	Shoes,		// �C
	Weapon,		// ����
	None,		// ���킶��Ȃ�

	Length,
}

public class GridIcon_Equipment : MonoBehaviour
{
	private bool m_onPointer = false;
	// UI�ɃI�u�W�F�N�g���̂��Ă��邩�ǂ���
	private bool m_fillUi = false;
	private bool m_pastInfo;

	// �����g�̃^�C�v
	[SerializeField] EquipmentType m_type;

	// �A�C�e���ړ��p�̉��u���I�u�W�F�N�g
	[SerializeField] GameObject m_moveItemTransform;

	// Start is called before the first frame update
	void Start()
	{
		m_pastInfo = m_fillUi;
		if(!m_moveItemTransform)
		{
			m_moveItemTransform = GameObject.FindWithTag("moveItemTransform");
		}
	}

	// Update is called once per frame
	void Update()
	{
		// �����̏�Ńh���b�v���ꂽ�Ƃ�
		if (Input.GetMouseButtonUp(0) && m_onPointer)
		{
			Debug.Log("in");
			// �ړ����̃A�C�e�����Ȃ��Ƃ��͖���
			if (m_moveItemTransform.transform.childCount == 0) return;

			GameObject o = m_moveItemTransform.transform.GetChild(0).gameObject;
			// �A�C�e������������Ȃ��Ƃ� || �����g�ɑΉ�������������Ȃ��Ƃ��͖���
			if (
				o.GetComponent<Item_Object>().GetWeaponType() == EquipmentType.None ||
				o.GetComponent<Item_Object>().GetWeaponType() != m_type
				)
			{
				o.GetComponent<Item_Object>().PointerUp(false);
			}
			else
			{
				// ���łɒ��g���ݒ肳��Ă��鎞�A��U���g�����o��
				if (transform.childCount != 0)
				{
					transform.GetChild(0).transform.SetParent(m_moveItemTransform.transform);
				}

				// �V������������
				o.GetComponent<Item_Object>().PointerUp(true, transform);
			}

		}
	}

	// �|�C���^�[�̏�Ԃ�ݒ�
	public void SetPointerInfo(bool info)
	{
		m_onPointer = info;
	}

	// �|�C���^�[���d�Ȃ��Ă��邩
	public bool OnPointer()
	{
		return m_onPointer;
	}

	// ���g�����܂��Ă��邩
	public bool GetOnFillUi()
	{
		return m_fillUi;
	}

	public void SetUi(bool value)
	{
		m_fillUi = value;
	}
}
