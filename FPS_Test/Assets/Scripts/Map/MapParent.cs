using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MapParent : MonoBehaviourPunCallbacks
{
	[PunRPC]
	void SetBake()
	{
		GetComponent<NavMeshSurface>().BuildNavMesh();
	}
}
