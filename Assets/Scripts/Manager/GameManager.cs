using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBase<GameManager>
{
	// �V�[���؂�ւ���
	[SerializeField] UnityEditor.SceneAsset m_gameScene;
	[SerializeField] UnityEditor.SceneAsset m_lobbyScene;

	// Start is called before the first frame update
	void Start()
    {
		// �ŏ��Ƀ��r�[�V�[����ǂݍ���
		SceneManager.LoadSceneAsync(m_lobbyScene.name, LoadSceneMode.Additive);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void StartGame()
	{
		// �Q�[����ǂݍ���
		SceneManager.LoadSceneAsync(m_gameScene.name, LoadSceneMode.Additive);
		// ���r�[�V�[�����A�����[�h
		SceneManager.UnloadSceneAsync(m_lobbyScene.name);
	}

	public void ReturnLobby()
	{
		// ���r�[�V�[����ǂݍ���
		SceneManager.LoadSceneAsync(m_gameScene.name, LoadSceneMode.Additive);
		// �Q�[�����A�����[�h
		SceneManager.UnloadSceneAsync(m_lobbyScene.name);
	}
}
