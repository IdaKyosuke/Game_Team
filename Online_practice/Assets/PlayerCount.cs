using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerCount : MonoBehaviourPunCallbacks
{
	[SerializeField] TextMeshProUGUI text;

	void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			int count = PhotonNetwork.PlayerList.Length;
			text.text = count + "/4";
		}
	}
}
