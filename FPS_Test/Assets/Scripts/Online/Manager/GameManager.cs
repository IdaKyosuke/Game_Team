using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBase<GameManager>
{
	// シーン切り替え先
	[SerializeField] SceneAsset m_gameScene;
	[SerializeField] SceneAsset m_lobbyScene;
	[SerializeField] SceneAsset m_mapScene;

    // Start is called before the first frame update
    void Start()
    {
		// 最初にロビーシーンを読み込む
		SceneManager.LoadSceneAsync(m_lobbyScene.name, LoadSceneMode.Additive);

		// プレイヤー自身の名前を"Player"に設定する
		PhotonNetwork.NickName = "Player";

		// PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
		PhotonNetwork.ConnectUsingSettings();
	}

	public void StartGame()
	{
		// ゲームを読み込む
		SceneManager.LoadSceneAsync(m_gameScene.name, LoadSceneMode.Additive);
		SceneManager.LoadSceneAsync(m_mapScene.name, LoadSceneMode.Additive);
		// ロビーシーンをアンロード
		SceneManager.UnloadSceneAsync(m_lobbyScene.name);
	}

	public void ReturnLobby()
	{
		PhotonNetwork.LeaveRoom();
		// ロビーシーンを読み込む
		SceneManager.LoadSceneAsync(m_lobbyScene.name, LoadSceneMode.Additive);
		// ゲームをアンロード
		SceneManager.UnloadSceneAsync(m_gameScene.name);
	}
}
