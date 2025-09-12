using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

// MonoBehaviourPunCallbacksを継承して、PUNのコールバックを受け取れるようにする
public class CreateAvatar : MonoBehaviourPunCallbacks
{
	private static List<GameObject> player = new List<GameObject>();

	// ゲームサーバーへの接続が成功した時に呼ばれるコールバック
	public void CreateCharactor()
	{
		// ランダムな座標に自身のアバター（ネットワークオブジェクト）を生成する
		var position = new Vector3(Random.Range(-30f, 30f), 1, Random.Range(-30f, 30f));
		player.Add(PhotonNetwork.Instantiate("Avatar", position, Quaternion.identity));
	}

	public static List<GameObject> GetPlayerList
	{
		get {  return player; }
	}
}