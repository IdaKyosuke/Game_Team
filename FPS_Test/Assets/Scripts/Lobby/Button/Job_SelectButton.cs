using UnityEngine;

public class Job_SelectButton : MonoBehaviour
{
    [SerializeField] JobType m_jobType;
    [SerializeField] GameObject m_frame;
    [SerializeField] GameObject m_jobModel;
    [SerializeField] Job_SelectButton[] m_otherButtons;
	[SerializeField] GridIcon_Equipment m_grid;
	[SerializeField] GameObject m_caution;

	private SkillText_Manager m_textManager;

    private void Start()
    {
        // スキルのテキストを変更するためのマネージャーを取得
        m_textManager = GameObject.FindWithTag("skillTextManager").GetComponent<SkillText_Manager>();

        //選択されている職業ボタンの枠画像を表示する
        m_frame.SetActive(GameManager.Instance.PlayerJobType == m_jobType);

        //選択されている職業のモデルのみ表示する
        m_jobModel.SetActive(GameManager.Instance.PlayerJobType == m_jobType);

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

		if(m_grid.CheckWeaponType() != JobType.None && 
			m_grid.CheckWeaponType() != m_jobType
		)
		{
			// 装備枠に変更したいジョブが装備できない武器が入っている時は警告文
			if(!m_caution.activeSelf)
			{
				m_caution.SetActive(true);
				m_caution.GetComponent<Text_Caution>().ShowText();
			}
			return;
		}

		//選択された職業をGameManagerに伝える
		GameManager.Instance.PlayerJobType = m_jobType;

        //選択された職業ボタンの枠画像を表示する
        m_frame.SetActive(true);

        //他の職業ボタンのReleaseを呼び出す
        foreach (var button in m_otherButtons)
        {
            button.Release();
        }

        //選択された職業のモデルのみ表示する
        m_jobModel.SetActive(true);

        // スキルのテキストを設定
        m_textManager.ChangeSkillText((int)m_jobType);
    }

    public void Release()
    {
        m_frame.SetActive(false);

        m_jobModel.SetActive(false);
    }
}