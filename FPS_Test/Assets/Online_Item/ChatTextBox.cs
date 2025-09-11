using Photon.Pun;
using TMPro;
using UnityEngine;

public class ChatTextBox : MonoBehaviourPunCallbacks
{
	private TMP_InputField m_setText;
	private int m_textId;

	private void Start()
	{
		m_setText = GetComponent<TMP_InputField>();
	}


	public void SetText()
    {
		if (!string.IsNullOrEmpty(m_setText.text) && m_setText.text.Substring(0, 1) == "\\")
		{
			// �ꕶ���ڂ��o�b�N�X���b�V���̏ꍇ���̌�̕�������v���C���[�̃j�b�N�l�[���ɂ���
			foreach (GameObject player in SampleScene.GetPlayerList)
			{
				player.GetComponent<Move>().ChangeName(m_setText.text.Substring(1));
			}		
		}

		// �e�L�X�g�ɕ����������Ă���Ƃ�
		if (!string.IsNullOrEmpty(m_setText.text))
		{
			Debug.Log("���������ɓ����Ă���");

			m_textId = PhotonNetwork.Instantiate("Chat", new Vector2(0, 0), Quaternion.identity).GetComponent<PhotonView>().ViewID;
			
			foreach (GameObject player in SampleScene.GetPlayerList)
			{
				string name = player.GetComponent<Move>().GetName();
				if (name != null)
				{
					UpdateText(
					$"{name} :" + m_setText.text,
					m_textId);
				}
			}
			
			
			m_setText.DeactivateInputField();
		}
		else
		{
			Debug.Log("���������ɓ����Ă��Ȃ�");
		}
	}

	// �e�L�X�g��ύX����֐�
    public void UpdateText(string newText, int id)
	{
		photonView.RPC(nameof(RPC_UpdateText), RpcTarget.All, newText, id);
    }

    // �S�N���C�A���g�ŌĂ΂��RPC
    [PunRPC]
    void RPC_UpdateText(string newText, int id)
    {
		Transform parent = GameObject.FindWithTag("Content").transform;
		PhotonView view = PhotonView.Find(id);
		GameObject m_text = view.gameObject;
		m_text.transform.SetParent(parent);
		m_text.transform.localScale = Vector3.one;
        m_text.GetComponent<TextMeshProUGUI>().text = newText;
		m_text = null;
    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (!string.IsNullOrEmpty(m_setText.text))
			{
				m_setText.text = string.Empty;
                m_setText.DeactivateInputField();
                return;
			}
			m_setText.ActivateInputField();
		}
	}
}
