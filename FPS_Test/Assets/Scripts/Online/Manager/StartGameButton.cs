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
	[SerializeField] int m_maxPlayerAmount;
	GameManager m_start;
	bool gameScene = false;

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
			RoomOptions options = new RoomOptions();
			options.MaxPlayers = m_maxPlayerAmount;

			// ルームを作成して参加する
			PhotonNetwork.JoinOrCreateRoom(m_createRoomName.text, options, TypedLobby.Default);

			m_createRoomName.gameObject.SetActive(false);
			m_roomMemberNum.gameObject.SetActive(true);
		}
	}

	private void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			m_roomMemberNum.text = PhotonNetwork.PlayerList.Length.ToString() + "/" + m_maxPlayerAmount;

			if (gameScene) return;
			if (PhotonNetwork.PlayerList.Length >= m_maxPlayerAmount ||
				Input.GetKeyDown(KeyCode.F5))
			{
				gameScene = true;
				PhotonNetwork.CurrentRoom.IsOpen = false;
				m_start.StartGame();
			}
		}
	}
}
