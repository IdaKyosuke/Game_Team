using Photon.Pun;
using TMPro;

// MonoBehaviourPunCallbacksを継承して、photonViewプロパティを使えるようにする
public class AvatarNameDisplay : MonoBehaviourPunCallbacks
{
	private TextMeshPro nameLabel;

	private void Start()
	{
		nameLabel = GetComponent<TextMeshPro>();
	}

	void Update()
	{
		// プレイヤー名とプレイヤーIDを表示する
		nameLabel.text = $"{photonView.Owner.NickName}";
	}
}