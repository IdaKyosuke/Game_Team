using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridIcon : MonoBehaviour
{
	private bool m_onPointer = false;
	// UI�ɃI�u�W�F�N�g���̂��Ă��邩�ǂ���
	private bool m_fillUi = false;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnPointerEnter(PointerEventData eventData)
	{
		m_onPointer = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		m_onPointer = false;
	}

	public bool GetOnFillUi()
	{
		return m_fillUi;
	}
	public void SetUi(bool value)
	{
		m_fillUi = value;
	}
}
