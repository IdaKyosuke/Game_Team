using UnityEngine;

public class Button_Battle : Button_Function
{
	private GameObject m_startButton;

	public override void Initialize()
	{
        m_startButton = GameObject.FindWithTag("gameStartButton");
    }

	public override void PushThis()
	{
		m_startButton.SetActive(true);
    }

	public override void PushOther()
	{
		m_startButton.SetActive(false);
    }
}
