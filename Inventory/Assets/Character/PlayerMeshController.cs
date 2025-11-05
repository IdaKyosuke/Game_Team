using UnityEngine;

public class PlayerMeshController : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer[] m_playerMeshs;

    private void Start()
    {
        //第三者用のプレイヤーメッシュを非表示にする
        foreach (var mesh in m_playerMeshs)
        {
            mesh.enabled = false;
        }
    }
}
