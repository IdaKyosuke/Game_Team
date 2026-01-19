using UnityEngine;

public class Button_SelectSkill : MonoBehaviour
{
    [SerializeField] int m_skillIndex;

    public void OnClick()
    {
        //選択されたパッシブスキルをGameManagerに伝える
        GameManager.Instance.PlayerPassiveSkill = m_skillIndex;

        //スキル選択画面を閉じる
        transform.parent.gameObject.SetActive(false);
    }
}
