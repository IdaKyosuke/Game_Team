using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
	// シーン切り替え先
	[SerializeField] string m_managerScene;
	[SerializeField] string m_titleScene;
	[SerializeField] Image m_black;
	[SerializeField] float m_blackFadeTime;
	[SerializeField] float m_textFadeTime;
	[SerializeField] CanvasGroup m_anyPress;
	[SerializeField] AudioSource m_click;

	private bool m_isTextFade = false;
	private bool m_isFadeIn = false;

	private void Start()
	{
		Color color = m_black.color;
		color.a = 0;
		m_black.color = color;
	}

	void Update()
	{
		// タイトル => ロビー
		if (Input.anyKeyDown)
		{
			m_click.Play();
			StartCoroutine(Fade(m_black, true, m_blackFadeTime, true));
		}

		// テキストのフェード
		if(!m_isTextFade)
		{
			StartCoroutine(CanvasFade(m_isFadeIn));
		}
	}

	IEnumerator Fade(
		Image ui, 
		bool isFadeIn,
		float fadeTime,
		bool LoadLobby = false
	)
	{
		float elapsedTime = 0f;
		Color color = ui.color;
		float start = isFadeIn ? 0f : 1f;
		float end = isFadeIn ? 1f : 0f;

		while (elapsedTime < fadeTime)
		{
			elapsedTime += Time.deltaTime;

			color.a = Mathf.Lerp(start, end, elapsedTime / fadeTime);
			ui.color = color;
			yield return null;
		}

		color.a = 0f;
		ui.color = color;

		if(LoadLobby)
		{
			// ゲームを読み込む
			SceneManager.LoadSceneAsync(m_managerScene);
			// ロビーシーンをアンロード
			SceneManager.UnloadSceneAsync(m_titleScene);
		}
	}

	IEnumerator CanvasFade(bool isFadeIn)
	{
		m_isTextFade = true;
		float elapsed = 0f;
		float start = isFadeIn ? 0f : 1f;
		float end = isFadeIn ? 1f : 0f;

		while (elapsed < m_textFadeTime)
		{
			elapsed += Time.deltaTime;
			m_anyPress.alpha = Mathf.Lerp(start, end, elapsed / m_textFadeTime);
			yield return null;
		}

		m_anyPress.alpha = 0f;
		m_isFadeIn = !m_isFadeIn;
		m_isTextFade = false;
	}
}
