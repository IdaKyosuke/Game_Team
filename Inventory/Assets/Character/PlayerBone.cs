using UnityEngine;

public class PlayerBone : MonoBehaviour
{
    private GameObject m_parent;

    //一番上の親オブジェクトが持っている死亡フラグを渡す
    public bool IsDeath => m_parent.GetComponent<PlayerMove>().IsDeath;

    //一番上の親オブジェクトを渡す
    public GameObject Parent => m_parent;

    void Start()
    {
        //一番上の親オブジェクトを取得
        m_parent = transform.root.gameObject;
    }
}
