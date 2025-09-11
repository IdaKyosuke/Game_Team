using Photon.Pun;
using TMPro;
using UnityEngine;

public class ChatTextBox : MonoBehaviourPunCallbacks
{
	[SerializeField] private Transform m_chatParent;
	private TMP_InputField m_setText;

	private void Start()
	{
		m_setText = GetComponent<TMP_InputField>();
	}


	public void SetText()
    {
		// テキストに文字が入っているとき
		if (!string.IsNullOrEmpty(m_setText.text) || photonView.ControllerActorNr == 0)
		{
			Debug.Log("文字が中に入っている");
			GameObject text = PhotonNetwork.Instantiate("Chat", new Vector2(0, 0), Quaternion.identity);
			text.transform.SetParent(m_chatParent);

			// プレイヤー名とテキストを表示
			text.GetComponent<TextMeshProUGUI>().text =
				$"{PhotonNetwork.NickName}" + $"({photonView.ControllerActorNr}) :" + m_setText.text;
			m_setText.DeactivateInputField();
		}
		else
		{
			Debug.Log("文字が中に入っていない");
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (!string.IsNullOrEmpty(m_setText.text))
			{
				m_setText.text = string.Empty;
				return;
			}
			m_setText.ActivateInputField();
		}
	}
}
