using Photon.Pun;
using TMPro;

// MonoBehaviourPunCallbacks���p�����āAphotonView�v���p�e�B���g����悤�ɂ���
public class AvatarNameDisplay : MonoBehaviourPunCallbacks
{
	private TextMeshPro nameLabel;

	private void Start()
	{
		nameLabel = GetComponent<TextMeshPro>();
	}

	void Update()
	{
		// �v���C���[���ƃv���C���[ID��\������
		nameLabel.text = $"{photonView.Owner.NickName}";
	}
}