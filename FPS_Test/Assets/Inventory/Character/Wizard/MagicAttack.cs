using Photon.Pun;
using UnityEngine;

public class MagicAttack : MonoBehaviour
{
    private const float Speed = 10;
    private const float LifeTime = 0.3f;

    private Rigidbody m_rb;
    private Vector3 m_dir;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();

        //ˆê’èŠÔŒã‚Éíœ
        Destroy(gameObject, LifeTime);
    }

    private void FixedUpdate()
    {
        //‘Oi
        m_rb.MovePosition(m_rb.position + m_dir * Speed * Time.fixedDeltaTime);
    }

    public void Init(Vector3 dir, GameObject parent)
    {
        m_dir = dir;
        GetComponent<Weapon_Collider>().Parent = parent;
        GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
       Destroy(gameObject);
    }
}