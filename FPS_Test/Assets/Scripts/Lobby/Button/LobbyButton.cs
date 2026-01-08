using UnityEngine;
using UnityEngine.UI;

public class LobbyButton : MonoBehaviour
{
    [SerializeField] LobbyButtonType m_buttonType;
    [SerializeField] LobbyButtonManager m_manager; 
    [SerializeField] Sprite m_originalSprite;   //元の画像
    [SerializeField] Sprite m_clickSprite;      //クリックされている時の画像

	private Button_Function m_func = null;

    private Image m_image;

    public LobbyButtonType ButtonType => m_buttonType;

    private void Awake()
    {
        m_image = GetComponent<Image>();
		SetButtonType(m_buttonType);
	}

    public void OnClick()
    {
		if(m_func != null) m_func.PushThis();
        m_image.sprite = m_clickSprite;
        m_manager.OnClick(m_buttonType);
    }

    public void OnRelease()
    {
		if(m_func != null) m_func.PushOther();
        m_image.sprite = m_originalSprite;
    }
	// どのボタンの関数を呼ぶかを設定する
	private void SetButtonType(LobbyButtonType type)
	{
		switch(type)
		{
			case LobbyButtonType.Battle:
				m_func = new Button_Battle();
				Debug.Log("battle");
				break;

			case LobbyButtonType.Job:
				m_func = new Button_Job();
				Debug.Log("job");
				break;

			case LobbyButtonType.Shop:
				m_func = new Button_Shop();
				Debug.Log("shop");
				break;

			case LobbyButtonType.Setting:
				m_func = new Button_Setting();
				Debug.Log("setting");
				break;

			case LobbyButtonType.ReturnLobby:
				m_func = new Button_ReturnLobby();
				Debug.Log("returnLobby");
				break;
		}

		if(m_func != null)
		{
			m_func.Initialize();
		}
	}
}
