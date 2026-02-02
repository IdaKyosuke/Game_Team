using Photon.Pun;
using UnityEngine;

public class MagicAttack : MonoBehaviour
{
    private const float Speed = 13;
    private const float LifeTime = 0.4f;

    private Rigidbody m_rb;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();

        //一定時間後に削除
        Destroy(gameObject, LifeTime);
    }

    private void FixedUpdate()
    {
        //前進
        m_rb.MovePosition(m_rb.position + transform.forward * Speed * Time.fixedDeltaTime);
    }

    public void Init(Vector3 dir, GameObject parent)
    {
        transform.forward = dir;
        GetComponent<Weapon_Collider>().Parent = parent;
        GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //攻撃したキャラクター自身には当たらないようにする
        if (other.transform.root.gameObject == GetComponent<Weapon_Collider>().Parent) return;

        //当たったら消える
        Destroy(gameObject);
    }
}