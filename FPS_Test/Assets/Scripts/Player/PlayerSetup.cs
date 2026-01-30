using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] List<SkinnedMeshRenderer> m_firstPersonModel;  //自身
    [SerializeField] List<MeshRenderer> m_firstPersonSowrd;  
    [SerializeField] List<SkinnedMeshRenderer> m_thirdPersonModel;  //相手
    [SerializeField] List<MeshRenderer> m_thirdPersonSowrd;  
    [SerializeField] GameObject[] m_camera;
	[SerializeField] GameObject m_miniMapCamera;

	void Start()
	{
        if (photonView.IsMine)
        {
            //プレイヤーのモデル
            for (int i = 0; i < m_firstPersonModel.Count; ++i)
            {
                m_firstPersonModel[i].enabled = true;
                m_thirdPersonModel[i].enabled = false;
            }

            //剣のモデル
            for (int i = 0; i < m_firstPersonSowrd.Count; ++i)
            {
                m_firstPersonSowrd[i].enabled = true;
                m_thirdPersonSowrd[i].enabled = false;
            }

            // 自分のカメラを有効化
            m_camera[0].SetActive(true);
            m_camera[1].SetActive(true);
            m_miniMapCamera.SetActive(true);
        }
        else
        {
            //プレイヤーのモデル
            for (int i = 0; i < m_firstPersonModel.Count; ++i)
            {
                m_firstPersonModel[i].enabled = false;
                m_thirdPersonModel[i].enabled = true;
            }

            //剣のモデル
            for (int i = 0; i < m_firstPersonSowrd.Count; ++i)
            {
                m_firstPersonSowrd[i].enabled = false;
                m_thirdPersonSowrd[i].enabled = true;
            }

            // 他人のカメラは無効化
            m_camera[0].SetActive(false);
			m_camera[1].SetActive(false);
            m_miniMapCamera	.SetActive(false);
        }
    }
}