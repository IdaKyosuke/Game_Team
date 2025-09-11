using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBase<GameManager>
{
	// シーン切り替え先
	[SerializeField] UnityEditor.SceneAsset m_gameScene;
	[SerializeField] UnityEditor.SceneAsset m_lobbyScene;

	// Start is called before the first frame update
	void Start()
    {
		// 最初にロビーシーンを読み込む
		SceneManager.LoadSceneAsync(m_lobbyScene.name, LoadSceneMode.Additive);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void StartGame()
	{
		// ゲームを読み込む
		SceneManager.LoadSceneAsync(m_gameScene.name, LoadSceneMode.Additive);
		// ロビーシーンをアンロード
		SceneManager.UnloadSceneAsync(m_lobbyScene.name);
	}

	public void ReturnLobby()
	{
		// ロビーシーンを読み込む
		SceneManager.LoadSceneAsync(m_gameScene.name, LoadSceneMode.Additive);
		// ゲームをアンロード
		SceneManager.UnloadSceneAsync(m_lobbyScene.name);
	}
}
