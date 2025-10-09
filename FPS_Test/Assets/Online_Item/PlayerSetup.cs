using Photon.Pun;

public class PlayerSetup : MonoBehaviourPun
{
    void Start()
    {
        if (photonView.IsMine)
        {
            // 自分のカメラを有効化
            gameObject.SetActive(true);
        }
        else
        {
            // 他人のカメラは無効化
            gameObject.SetActive(false);
        }
    }
}