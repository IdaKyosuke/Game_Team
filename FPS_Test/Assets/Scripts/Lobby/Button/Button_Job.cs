using UnityEngine;

public class Button_Job : Button_Function
{
    private GameObject m_jobUi;

    public override void Initialize()
    {
        m_jobUi = GameObject.FindWithTag("jobUi");

        if (m_jobUi.activeSelf)
        {
            m_jobUi.SetActive(false);
        }
    }

    public override void PushThis()
    {
        m_jobUi.SetActive(true);
    }

    public override void PushOther()
	{
        m_jobUi.SetActive(false);
    }
}