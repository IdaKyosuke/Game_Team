using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

// MonoBehaviourPunCallbacks���p�����āAPUN�̃R�[���o�b�N���󂯎���悤�ɂ���
public class CreateAvatar : MonoBehaviourPunCallbacks
{
	private static List<GameObject> player = new List<GameObject>();

	// �Q�[���T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
	public void CreateCharactor()
	{
		// �����_���ȍ��W�Ɏ��g�̃A�o�^�[�i�l�b�g���[�N�I�u�W�F�N�g�j�𐶐�����
		var position = new Vector3(Random.Range(-30f, 30f), 1, Random.Range(-30f, 30f));
		player.Add(PhotonNetwork.Instantiate("Avatar", position, Quaternion.identity));
	}

	public static List<GameObject> GetPlayerList
	{
		get {  return player; }
	}
}