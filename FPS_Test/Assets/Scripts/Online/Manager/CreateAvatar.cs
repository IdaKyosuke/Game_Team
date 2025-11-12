using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MonoBehaviourPunCallbacksを継承して、PUNのコールバックを受け取れるようにする
public class CreateAvatar : MonoBehaviourPunCallbacks
{
	private static List<GameObject> m_player = new List<GameObject>();

	// ゲームサーバーへの接続が成功した時に呼ばれるコールバック
	void Awake()
	{
		if (PhotonNetwork.IsMasterClient) StartCoroutine(CreateCharactor());
	}

	IEnumerator CreateCharactor()
	{
		yield return new WaitForSeconds(0.5f); // 待つ

		for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
		{
			GameObject player = PhotonNetwork.InstantiateRoomObject("Player", new Vector3(0, 1, 0), Quaternion.identity);

			yield return new WaitForSeconds(0.1f);
			PhotonView playerView = player.GetComponent<PhotonView>();
			playerView.TransferOwnership(PhotonNetwork.PlayerList[i]);
			player.transform.GetChild(1).GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[i]);
			yield return new WaitForSeconds(0.1f);
			photonView.RPC(nameof(SetScripts), RpcTarget.All, playerView.ViewID);
            m_player.Add(player);
		}
	}

	[PunRPC]
	void SetScripts(int viewId)
	{
		PhotonView view = PhotonView.Find(viewId);
		view.GetComponent<StashController>().enabled = true;
		view.GetComponent<PlayerController>().enabled = true;
		view.GetComponent<PlayerSetup>().enabled = true;
    }

	public static List<GameObject> GetPlayerList
	{
		get {  return m_player; }
	}
}