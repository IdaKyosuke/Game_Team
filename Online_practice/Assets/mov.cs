using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

// MonoBehaviourPunCallbacksを継承して、photonViewプロパティを使えるようにする
public class AvatarController : MonoBehaviourPunCallbacks
{
	private void Update()
	{
		// 自身が生成したオブジェクトだけに移動処理を行う
		if (photonView.IsMine)
		{
			var input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
			transform.Translate(6f * Time.deltaTime * input.normalized);
		}

		if (photonView.Owner.IsMasterClient)
		{
			Debug.Log($"{photonView.Owner.NickName}({photonView.OwnerActorNr})");
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("ぶつかった");
	}

	private void OnCollisionExit(Collision collision)
	{
		Debug.Log("離れた");
	}
}