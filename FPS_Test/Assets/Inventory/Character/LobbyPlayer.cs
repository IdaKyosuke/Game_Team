using UnityEngine;

public class LobbyPlayer : MonoBehaviour
{
    [SerializeField] GameObject[] m_jobModel;

    private void Start()
    {
        //‘I‘ğ‚³‚ê‚Ä‚¢‚éE‹Æ‚Ìƒ‚ƒfƒ‹‚ğ•\¦
        for (int i = 0; i < m_jobModel.Length; ++i)
        { 
            m_jobModel[i].SetActive(GameManager.Instance.PlayerJobType == (JobType)i);
        }
    }
}