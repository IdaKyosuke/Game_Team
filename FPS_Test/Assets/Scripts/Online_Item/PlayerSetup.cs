using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
	[SerializeField] GameObject m_camera;
	[SerializeField] GameObject m_miniMapCamera;

	[PunRPC]
	void Update()
	{
		Debug.Log("カメラをセット");
        if (photonView.IsMine)
        {
			// 自分のカメラを有効化
			m_camera.SetActive(true);
			m_miniMapCamera.SetActive(true);
        }
        else
        {
			// 他人のカメラは無効化
			m_camera.SetActive(false);
			m_miniMapCamera	.SetActive(false);
        }
	}
}