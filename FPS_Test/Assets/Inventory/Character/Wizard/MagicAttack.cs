using Photon.Pun;
using UnityEngine;

public class MagicAttack : MonoBehaviour
{
    [SerializeField] float m_speed = 5;
    [SerializeField] float m_lifeTime = 0.4f;

    private Vector3 m_dir;

    private void Start()
    {
        //ˆê’èŠÔŒã‚Éíœ
        Destroy(gameObject, m_lifeTime);
    }

    private void FixedUpdate()
    {
        //‘O•û‚ÉˆÚ“®
        transform.position += m_dir * m_speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        //‰½‚©‚É“–‚½‚Á‚½‚çíœ
        Destroy(gameObject);
    }

    public void Init(Vector3 dir)
    { 
        m_dir = dir;
    }
}