using UnityEngine;

public class Button_Job : Button_Function
{
    private GameObject m_jobUi;
	private GameObject m_caution;

    public override void Initialize()
    {
        m_jobUi = GameObject.FindWithTag("jobUi");
		m_caution = GameObject.FindWithTag("caution");

		if (m_caution.activeSelf)
		{
			m_caution.SetActive(false);
		}
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
		m_caution.SetActive(false);
        m_jobUi.SetActive(false);
    }
}