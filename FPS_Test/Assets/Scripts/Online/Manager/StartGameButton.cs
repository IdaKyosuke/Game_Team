using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviourPunCallbacks
{
	[SerializeField] TextMeshProUGUI m_startButtonText;
	[SerializeField] TextMeshProUGUI m_roomMemberNum;
	[SerializeField] int m_maxPlayerAmount;
	GameManager m_start;
	bool gameScene = false;
	bool clickStart = false;
	bool isProcessingRoom = false;

    // Start is called before the first frame update
    void Start()
    {
        m_start = GameManager.Instance;
		m_roomMemberNum.gameObject.SetActive(false);
	}

	public void JoinRandomRoom()
	{
		// ランダムな部屋に入る
		if (!clickStart)
		{
			// ボタンを押して部屋に入れた場合
			if (TryJoinRandomRoom())
			{
				clickStart = true;
				m_startButtonText.text = "STOP";
			}
		}
		else
		{
			// ボタンを二回目押したとき
			if (TryLeaveRoom())
			{
				clickStart = false;
				m_startButtonText.text = "START";
			}
		}
	}


	private void CreateRoom()
	{
		RoomOptions options = new RoomOptions();
		options.MaxPlayers = m_maxPlayerAmount;

		const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
		int length = Random.Range(5, 16); // 5〜15文字
		System.Text.StringBuilder sb = new System.Text.StringBuilder(length);

		for (int i = 0; i < length; i++)
		{
			sb.Append(chars[Random.Range(0, chars.Length)]);
		}
		string roomName = sb.ToString();
		// ルームを作成して参加する
		Debug.Log("部屋を作成");
		PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
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

		m_roomMemberNum.gameObject.SetActive(PhotonNetwork.InRoom);
	}

	// 部屋に入れなかった場合
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		// ランダムな部屋に入れなかった場合部屋を作る
		CreateRoom();
		base.OnJoinRandomFailed(returnCode, message);
	}

	// 何らかの理由で部屋を作れなかった場合
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		CreateRoom();
		base.OnCreateRoomFailed(returnCode, message);
	}

	// 非同期処理によるエラーの防止
	private bool TryJoinRandomRoom()
	{
		if (isProcessingRoom || !PhotonNetwork.IsConnectedAndReady) return false;
		isProcessingRoom = true;
		Debug.Log("ランダムルームに参加希望");

		PhotonNetwork.JoinRandomRoom();
		return true;
	}
	private bool TryLeaveRoom()
	{
		if (isProcessingRoom) return false;
		isProcessingRoom = true;
		Debug.Log("部屋退出");

		PhotonNetwork.LeaveRoom();
		return true;
	}

	public override void OnJoinedRoom()
	{
		isProcessingRoom = false;
		base.OnJoinedRoom();
	}  

	public override void OnLeftRoom()
	{
		isProcessingRoom = false;
		base.OnLeftRoom();
	}
}
