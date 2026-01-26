using UnityEngine;

public class Job_SelectButton : MonoBehaviour
{
    [SerializeField] JobType m_jobType;
    [SerializeField] GameObject m_frame;
    [SerializeField] Job_SelectButton[] m_otherButtons;

	private SkillText_Manager m_textManager;

    private void Awake()
    {
		// スキルのテキストを変更するためのマネージャーを取得
		m_textManager = GameObject.FindWithTag("skillTextManager").GetComponent<SkillText_Manager>();
        //選択されている職業ボタンの枠画像を表示する
        m_frame.SetActive(GameManager.Instance.PlayerJobType == m_jobType);

		// 現在選択されている職業と一致している時にスキルテキストを変更する
		if (m_jobType == GameManager.Instance.PlayerJobType)
		{
			// スキルのテキストを設定
			m_textManager.ChangeSkillText((int)m_jobType);
		}
    }

    public void OnClick()
    {
        //既に選択されている場合は何もしない
        if (GameManager.Instance.PlayerJobType == m_jobType) return;

        //選択された職業をGameManagerに伝える
        GameManager.Instance.PlayerJobType = m_jobType;

        //選択された職業ボタンの枠画像を表示する
        m_frame.SetActive(true);

        //他の職業ボタンの枠画像を非表示にする
        foreach (var button in m_otherButtons)
        {
            button.Release();
        }

		// スキルのテキストを設定
		m_textManager.ChangeSkillText((int)m_jobType);
    }

    public void Release()
    {
        m_frame.SetActive(false);
    }
}