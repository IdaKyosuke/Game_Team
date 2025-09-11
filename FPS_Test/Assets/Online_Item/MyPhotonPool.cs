using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MyPhotonPool : MonoBehaviourPunCallbacks, IPunPrefabPool
{
	public List<GameObject> PrefabList;

	public void Start()
	{
		// Pool‚Ì¶¬ƒCƒxƒ“ƒg‚ğ‘‚«Š·‚¦‚é
		PhotonNetwork.PrefabPool = this;
	}

	public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
	{
		foreach (var s in PrefabList)
		{
			if (s.name == prefabId)
			{
				var go = Instantiate(s, position, rotation);
				go.SetActive(false);
				return go;
			}
		}

		return null;
	}

	public void Destroy(GameObject go)
	{
		GameObject.Destroy(go);
	}
}