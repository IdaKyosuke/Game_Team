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
		// �e�L�X�g�ɕ����������Ă���Ƃ�
		if (!string.IsNullOrEmpty(m_setText.text) || photonView.ControllerActorNr == 0)
		{
			Debug.Log("���������ɓ����Ă���");
			GameObject text = PhotonNetwork.Instantiate("Chat", new Vector2(0, 0), Quaternion.identity);
			text.transform.SetParent(m_chatParent);

			// �v���C���[���ƃe�L�X�g��\��
			text.GetComponent<TextMeshProUGUI>().text =
				$"{PhotonNetwork.NickName}" + $"({photonView.ControllerActorNr}) :" + m_setText.text;
			m_setText.DeactivateInputField();
		}
		else
		{
			Debug.Log("���������ɓ����Ă��Ȃ�");
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
