using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

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

		if (photonView.Owner.IsMasterClient)
		{
			Debug.Log($"{photonView.Owner.NickName}({photonView.OwnerActorNr})");
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("�Ԃ�����");
	}

	private void OnCollisionExit(Collision collision)
	{
		Debug.Log("���ꂽ");
	}
}