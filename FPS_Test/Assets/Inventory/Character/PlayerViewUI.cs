using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerViewUI : MonoBehaviour
{
    [SerializeField] GameObject m_frame;
    [SerializeField] GameObject[] m_attackTypeUI;
    [SerializeField] TextMeshProUGUI m_levelText;
    [SerializeField] Slider[] m_statusUI;

    private PlayerStatus m_playerStatus;
    private StashController m_stashController;

    private void Start()
    {
        //コンポーネントの取得
        m_playerStatus = transform.root.GetComponent<PlayerStatus>();
        m_stashController = transform.root.GetComponent<StashController>();

        //UIの初期化
        m_statusUI[0].maxValue = m_playerStatus.Current.hp;
        m_statusUI[0].value = m_playerStatus.Current.hp;

        m_statusUI[1].maxValue = m_playerStatus.Current.mp;
        m_statusUI[1].value = m_playerStatus.Current.mp;
    }

    private void Update()
    {
        //スタッシュを開いていたらUIを非表示にする
        m_frame.SetActive(!m_stashController.IsOpen);
        if (m_stashController.IsOpen) return;

        //ステータスの更新
        m_statusUI[0].value = m_playerStatus.Current.hp;
        m_statusUI[1].value = m_playerStatus.Current.mp;

        //レベルの更新
        m_levelText.text = "Lv. " + m_playerStatus.Level.ToString();
    }

    public void SetIcon(AttackType attackType)
    {
        //攻撃タイプの更新
        switch (attackType)
        {
            case AttackType.Physical:
                m_attackTypeUI[0].SetActive(true);
                m_attackTypeUI[1].SetActive(false);
                break;

            case AttackType.Magical:
                m_attackTypeUI[0].SetActive(false);
                m_attackTypeUI[1].SetActive(true);
                break;

            case AttackType.Cleric:
                m_attackTypeUI[0].SetActive(true);
                m_attackTypeUI[1].SetActive(false);
                break;
        }
    }
}
