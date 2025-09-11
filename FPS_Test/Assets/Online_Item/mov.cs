using Photon.Pun;
using UnityEngine;

// MonoBehaviourPunCallbacks���p�����āAphotonView�v���p�e�B���g����悤�ɂ���
public class AvatarController : MonoBehaviourPunCallbacks
{
	private void Update()
	{
		// ���g�����������I�u�W�F�N�g�����Ɉړ��������s��
		if (photonView.IsMine)
		{
			var input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
			transform.Translate(6f * Time.deltaTime * input.normalized);
		}
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
}