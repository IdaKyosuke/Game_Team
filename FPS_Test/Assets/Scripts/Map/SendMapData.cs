using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SendMapData : MonoBehaviourPunCallbacks
{
	[SerializeField] List<Transform> m_enemyPortalPos;		// “G
	[SerializeField] List<Transform> m_playerTreasurePos;	// •ó” 

	public List<Transform> GetEnemyPortalPos()
	{
		return m_enemyPortalPos;
	}

	public List<Transform> GetSpawnPos()
	{
		return m_playerTreasurePos;
	}

	public void ReqestSetParent()
	{
		photonView.RPC(nameof(SetParent), RpcTarget.All);
	}

	[PunRPC]
	void SetParent()
	{
		GameObject mapParent = GameObject.FindWithTag("MapParent");
		transform.SetParent(mapParent.transform);
		Debug.Log("setParent");
	}
}
