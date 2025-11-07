using UnityEngine;
using UnityEngine.UI;

public class LobbyButton : MonoBehaviour
{
    [SerializeField] LobbyButtonType m_buttonType;
    [SerializeField] LobbyButtonManager m_manager; 
    [SerializeField] Sprite m_originalSprite;   //Œ³‚Ì‰æ‘œ
    [SerializeField] Sprite m_clickSprite;      //ƒNƒŠƒbƒN‚³‚ê‚Ä‚¢‚éŽž‚Ì‰æ‘œ

    private Image m_image;

    public LobbyButtonType ButtonType => m_buttonType;

    private void Awake()
    {
        m_image = GetComponent<Image>();
    }

    public void OnClick()
    {
        m_image.sprite = m_clickSprite;
        m_manager.OnClick(m_buttonType);
    }

    public void OnRelease()
    {
        m_image.sprite = m_originalSprite;
    }
}
