using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBase<GameManager>
{
	// シーン切り替え先
	[SerializeField] string m_gameScene;
	[SerializeField] string m_lobbyScene;

    // Start is called before the first frame update
    void Start()
    {
		// 最初にロビーシーンを読み込む
		SceneManager.LoadSceneAsync(m_lobbyScene, LoadSceneMode.Additive);

		// プレイヤー自身の名前を"Player"に設定する
		PhotonNetwork.NickName = "Player";

		// PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
		PhotonNetwork.ConnectUsingSettings();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void StartGame()
	{
		// ゲームを読み込む
		SceneManager.LoadSceneAsync(m_gameScene, LoadSceneMode.Additive);
		// ロビーシーンをアンロード
		SceneManager.UnloadSceneAsync(m_lobbyScene);
	}

	public void ReturnLobby()
	{
		// ロビーシーンを読み込む
		SceneManager.LoadSceneAsync(m_gameScene, LoadSceneMode.Additive);
		// ゲームをアンロード
		SceneManager.UnloadSceneAsync(m_lobbyScene);
	}
}
