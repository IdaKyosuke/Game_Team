using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameButton : MonoBehaviour
{
	GameManager m_start;

    // Start is called before the first frame update
    void Start()
    {
        m_start = GameManager.Instance;
	}

	public void StartGame()
	{
		m_start.StartGame();
	}
}
