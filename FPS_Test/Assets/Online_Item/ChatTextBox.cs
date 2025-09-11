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
			// 一文字目がバックスラッシュの場合その後の文字列をプレイヤーのニックネームにする
			foreach (GameObject player in SampleScene.GetPlayerList)
			{
				player.GetComponent<Move>().ChangeName(m_setText.text.Substring(1));
			}		
		}

		// テキストに文字が入っているとき
		if (!string.IsNullOrEmpty(m_setText.text))
		{
			Debug.Log("文字が中に入っている");

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
			Debug.Log("文字が中に入っていない");
		}
	}

	// テキストを変更する関数
    public void UpdateText(string newText, int id)
	{
		photonView.RPC(nameof(RPC_UpdateText), RpcTarget.All, newText, id);
    }

    // 全クライアントで呼ばれるRPC
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
