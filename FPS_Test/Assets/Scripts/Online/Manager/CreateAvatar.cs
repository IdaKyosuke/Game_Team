using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UniRx;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

// MonoBehaviourPunCallbacksを継承して、PUNのコールバックを受け取れるようにする
[DefaultExecutionOrder(10)]

public class CreateAvatar : MonoBehaviourPunCallbacks
{
	private static List<GameObject> m_player = new List<GameObject>();

	// ゲームサーバーへの接続が成功した時に呼ばれるコールバック
	async void Awake()
	{
		//if (PhotonNetwork.IsMasterClient) StartCoroutine(CreateCharactor());
		if (PhotonNetwork.IsMasterClient) await CreateCharactor();
	}

	async UniTask CreateCharactor()
	{
		var token = this.GetCancellationTokenOnDestroy();
		//yield return new WaitForSeconds(0.5f); // 待つ

		await UniTask.DelayFrame(5);

		for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
		{
			GameObject player = PhotonNetwork.InstantiateRoomObject("Player", new Vector3(0, 1, 0), Quaternion.identity);
			await UniTask.WaitUntil(() => player != null, cancellationToken: token);
			// プレイヤーを取得するまで待つ
			//GameObject player = await GetPlayer();

			//yield return new WaitForSeconds(0.1f);
			PhotonView playerView = player.GetComponent<PhotonView>();
			playerView.TransferOwnership(PhotonNetwork.PlayerList[i]);
			player.transform.GetChild(1).GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[i]);
			await UniTask.WaitUntil(() => playerView.Owner == PhotonNetwork.PlayerList[i], cancellationToken: token);
			//yield return new WaitForSeconds(0.3f);
			//await TransferOwnership(player, i);
			photonView.RPC(nameof(SetScripts), RpcTarget.All, playerView.ViewID);
            m_player.Add(player);
		}
	}

	async UniTask<GameObject> GetPlayer()
	{
		await UniTask.DelayFrame(0);	
		return PhotonNetwork.InstantiateRoomObject("Player", new Vector3(0, 1, 0), Quaternion.identity);
	}

	async UniTask TransferOwnership(GameObject player, int index)
	{
		await UniTask.DelayFrame(0);
		player.transform.GetChild(1).GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[index]);
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