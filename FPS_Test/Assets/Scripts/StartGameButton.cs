using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class StartGameButton : MonoBehaviourPunCallbacks
{
	[SerializeField] TMP_InputField m_createRoomName;
	[SerializeField] TextMeshProUGUI m_roomMemberNum;
	GameManager m_start;

    // Start is called before the first frame update
    void Start()
    {
        m_start = GameManager.Instance;
		m_roomMemberNum.gameObject.SetActive(false);
	}

	public void JoinOrCreateRoom()
	{
		// 中身に名前がある場合
		if (!string.IsNullOrEmpty(m_createRoomName.text))
		{
			// ルームを作成して参加する
			PhotonNetwork.JoinOrCreateRoom(m_createRoomName.text, new RoomOptions(), TypedLobby.Default);

			m_createRoomName.gameObject.SetActive(false);
			m_roomMemberNum.gameObject.SetActive(true);
		}
	}

	private void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			m_roomMemberNum.text = PhotonNetwork.PlayerList.Length.ToString() + "/4";

			if (PhotonNetwork.PlayerList.Length >= 4) StartGame();
		}
	}

	private void StartGame()
	{
		m_start.StartGame();
	}
}
