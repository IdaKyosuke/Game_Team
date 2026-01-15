using System.Collections;
using System.Collections.Generic;
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
    private Job m_job;
    private StashController m_stashController;

    private void Start()
    {
        //コンポーネントの取得
        m_playerStatus = transform.root.GetComponent<PlayerStatus>();
        m_job = transform.root.GetComponent<Job>();
        m_stashController = transform.root.GetComponent<StashController>();

        //UIの初期化
        m_statusUI[0].maxValue = m_playerStatus.CurrentHP;
        m_statusUI[0].value = m_playerStatus.CurrentHP;

        m_statusUI[1].maxValue = m_playerStatus.CurrentMP;
        m_statusUI[1].value = m_playerStatus.CurrentMP;
    }

    private void Update()
    {
        //スタッシュを開いていたらUIを非表示にする
        m_frame.SetActive(!m_stashController.IsOpen);
        if (m_stashController.IsOpen) return;

        //攻撃タイプの更新
        switch (m_job.AttackType)
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

        //ステータスの更新
        m_statusUI[0].value = m_playerStatus.CurrentHP;
        m_statusUI[1].value = m_playerStatus.CurrentMP;

        //レベルの更新
        m_levelText.text = "Lv. " + m_playerStatus.Level.ToString();
    }
}
