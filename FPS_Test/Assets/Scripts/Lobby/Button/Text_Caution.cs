using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Text_Caution : MonoBehaviour
{
	[SerializeField] float m_textFadeTime;
	[SerializeField] CanvasGroup m_group;

	private void Start()
	{
		if(!m_group)
		{
			m_group = GetComponent<CanvasGroup>();
		}
	}

	public void ShowText()
	{
		StartCoroutine(CanvasFade());
	}

	IEnumerator CanvasFade()
	{
		float elapsed = 0f;

		while (elapsed < m_textFadeTime)
		{
			elapsed += Time.deltaTime;
			m_group.alpha = Mathf.Lerp(1f, 0f, elapsed / m_textFadeTime);
			yield return null;
		}

		m_group.alpha = 0f;
		this.gameObject.SetActive(false);
	}
}
