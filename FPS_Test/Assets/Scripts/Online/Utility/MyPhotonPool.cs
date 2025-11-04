using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[DefaultExecutionOrder(-50)]
public class MyPhotonPool : MonoBehaviourPunCallbacks, IPunPrefabPool
{
	public List<GameObject> PrefabList;

	public void Start()
	{
		// Pool‚Ì¶¬ƒCƒxƒ“ƒg‚ğ‘‚«Š·‚¦‚é
		PhotonNetwork.PrefabPool = this;
		foreach (var a in PrefabList)
		{
			//Debug.Log(a.name);
		}
	}

	public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
	{
		//Debug.Log(prefabId);
		//Debug.Log(PrefabList.Count);
		//Debug.Log(PrefabList[19].name);

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