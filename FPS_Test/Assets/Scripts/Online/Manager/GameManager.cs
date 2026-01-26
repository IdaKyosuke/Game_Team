using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBase<GameManager>
{
	// シーン切り替え先
	[SerializeField] string m_gameScene;
	[SerializeField] string m_lobbyScene;
	[SerializeField] string m_mapScene;

    // 選択されたプレイヤーの職業
    private JobType m_playerJob;
    public JobType PlayerJobType
	{ 
		get { return m_playerJob; }
		set { m_playerJob = value; }
    }

	//選択された職業のパッシブスキル
	private int m_playerPassiveSkill;
	public int PlayerPassiveSkill
    {
		get { return m_playerPassiveSkill; }
		set { m_playerPassiveSkill = value; }
    }

    void Start()
    {
		// 最初にロビーシーンを読み込む
		SceneManager.LoadSceneAsync(m_lobbyScene, LoadSceneMode.Additive);

		// プレイヤー自身の名前を"Player"に設定する
		PhotonNetwork.NickName = "Player";

        //初期の職業は戦士に設定
		m_playerJob = JobType.Warrior;

        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
	}

	public void StartGame()
	{
		// ゲームを読み込む
		SceneManager.LoadSceneAsync(m_gameScene, LoadSceneMode.Additive);
		SceneManager.LoadSceneAsync(m_mapScene, LoadSceneMode.Additive);
		// ロビーシーンをアンロード
		SceneManager.UnloadSceneAsync(m_lobbyScene);
	}

	public void ReturnLobby(bool isMine)
	{
		if (!isMine) return;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;

		PhotonNetwork.LeaveRoom();
		// ロビーシーンを読み込む
		SceneManager.LoadSceneAsync(m_lobbyScene, LoadSceneMode.Additive);
		// ゲームをアンロード
		SceneManager.UnloadSceneAsync(m_gameScene);
		SceneManager.UnloadSceneAsync(m_mapScene);
	}
}
