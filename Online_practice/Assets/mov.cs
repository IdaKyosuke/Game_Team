using JetBrains.Annotations;
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
	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("ぶつかった");
	}

	public void ChangeName(string name)
	{
		if (photonView.IsMine)
		{
			photonView.Owner.NickName = name;
		}
	}

	public string GetName()
	{
		if (photonView.IsMine)
		{
			return photonView.Owner.NickName;
		}
		else
		{
			return null;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		Debug.Log("離れた");
	}
}