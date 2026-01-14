using Photon.Pun;
using UnityEngine;

public class MagicAttack : MonoBehaviour
{
    private const float Speed = 10;
    private const float LifeTime = 0.3f;

    private GameObject m_parent;
    private Rigidbody m_rb;
    private Vector3 m_dir;

    public GameObject Parent
    {
        get { return m_parent; }
        set { m_parent = value; }
    }

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

    public void Init(Vector3 dir)
    {
        m_dir = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
       Destroy(gameObject);
    }
}