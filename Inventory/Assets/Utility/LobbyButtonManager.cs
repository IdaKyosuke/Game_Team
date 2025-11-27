using UnityEngine;

public enum LobbyButtonType
{
    Battle,
    Job,
    Shop,
    Setting,
}

public class LobbyButtonManager : MonoBehaviour
{
    //いずれかのボタンがクリックされたとき
    public void OnClick(LobbyButtonType buttonType)
    { 
        foreach (Transform child in transform)
        {
            //クリックされたボタン以外のボタンを元に戻す
            LobbyButton lobbyButton = child.GetComponent<LobbyButton>();
            if (lobbyButton.ButtonType != buttonType) lobbyButton.OnRelease();
        }
    }
}
