using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum GridType
{
	Inventory,
	Stash,

	Length,
}

public class GridIcon : MonoBehaviour
{
	private bool m_onPointer = false;
	// UI�ɃI�u�W�F�N�g���̂��Ă��邩�ǂ���
	private bool m_fillUi = false;
	private bool m_pastInfo;

	// �A�C�e�����������Ƃ��ɏ���UI
	[SerializeField] List<Image> m_images = new List<Image>();

	// �����̃^�C�v
	private GridType m_type; 

	// Start is called before the first frame update
	void Start()
    {
		m_pastInfo = m_fillUi;
	}

    // Update is called once per frame
    void Update()
    {
		if(Input.GetMouseButtonUp(0) && m_onPointer)
		{
			GameObject.FindWithTag("inventoryManager").GetComponent<StashManager>().StartSet(m_type);
		}

		if (m_pastInfo != m_fillUi)
		{
			foreach(var g in m_images)
			{
				g.enabled = !m_fillUi;
			}
			m_pastInfo = m_fillUi;
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

	// �^�C�v���Z�b�g����
	public void SetType(GridType type)
	{
		m_type = type;
	}
}
