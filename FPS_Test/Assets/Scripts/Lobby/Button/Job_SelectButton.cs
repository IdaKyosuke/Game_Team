using Photon.Pun.UtilityScripts;
using UnityEngine;

public class Job_SelectButton : MonoBehaviour
{
    [SerializeField] JobType m_jobType;

    public void OnClick()
    {
        //‘I‘ğ‚³‚ê‚½E‹Æ‚ğGameManager‚É“`‚¦‚é
        GameManager.Instance.PlayerJobType = m_jobType;
    }
}