using Photon.Pun;

public class PlayerSetup : MonoBehaviourPun
{
    void Start()
    {
        if (photonView.IsMine)
        {
            // �����̃J������L����
            gameObject.SetActive(true);
        }
        else
        {
            // ���l�̃J�����͖�����
            gameObject.SetActive(false);
        }
    }
}