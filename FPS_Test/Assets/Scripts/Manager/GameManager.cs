using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBase<GameManager>
{
	// �V�[���؂�ւ���
	[SerializeField] string m_gameScene;
	[SerializeField] string m_lobbyScene;

    // Start is called before the first frame update
    void Start()
    {
		// �ŏ��Ƀ��r�[�V�[����ǂݍ���
		SceneManager.LoadSceneAsync(m_lobbyScene, LoadSceneMode.Additive);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void StartGame()
	{
		// �Q�[����ǂݍ���
		SceneManager.LoadSceneAsync(m_gameScene, LoadSceneMode.Additive);
		// ���r�[�V�[�����A�����[�h
		SceneManager.UnloadSceneAsync(m_lobbyScene);
	}

	public void ReturnLobby()
	{
		// ���r�[�V�[����ǂݍ���
		SceneManager.LoadSceneAsync(m_gameScene, LoadSceneMode.Additive);
		// �Q�[�����A�����[�h
		SceneManager.UnloadSceneAsync(m_lobbyScene);
	}
}
