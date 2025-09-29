using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController
{
    static private bool m_isTransition = false;

    public enum Type
    {
        Title,
        Lobby,
    }

    //シーンを初期状態で読み込む
    static public void Redo(Type scene)
    {
        //既に遷移中なら受け付けない
        if (m_isTransition) return;

        //シーンの読み込み
        SceneManager.LoadScene(SceneName(scene));
    }

    //シーンの追加
    static public void Load(Type scene)
    {
        //既に遷移中なら受け付けない
        if (m_isTransition) return;

        //既に追加済みなら何もしない
        if (SceneManager.GetSceneByName(SceneName(scene)).isLoaded) return;

        //シーンの追加読み込み
        SceneManager.LoadScene(SceneName(scene), LoadSceneMode.Additive);
    }

    //シーンの除外
    static public void UnLoad(Type scene)
    {
        //既に遷移中なら受け付けない
        if (m_isTransition) return;

        //シーンの除外
        SceneManager.UnloadSceneAsync(SceneName(scene));
    }

    //シーン遷移
    static public void Transition(Type prevScene, Type nextScene)
    {
        //既に遷移中なら受け付けない
        if (m_isTransition) return;

        //既に追加済みなら何もしない
        if (SceneManager.GetSceneByName(SceneName(nextScene)).isLoaded) return;

        //シーン遷移開始
        m_isTransition = true;

        //フェードアウト
        Fade.FadeOut(1.0f, () =>
        {
            //次のシーンを読み込む
            SceneManager.LoadScene(SceneName(nextScene), LoadSceneMode.Additive);

            //前のシーンを除外する
            SceneManager.UnloadSceneAsync(SceneName(prevScene));

            //シーン遷移完了
            m_isTransition = false;

            //フェードイン
            Fade.FadeIn(1.0f);
        });
    }

    static private string SceneName(Type type)
    {
        switch (type)
        {
            case Type.Title:
                return "Title";

            case Type.Lobby:
                return "Lobby";

            default:
                Debug.Log("存在しないシーン");
                return "Unknown";
        }
    }
}