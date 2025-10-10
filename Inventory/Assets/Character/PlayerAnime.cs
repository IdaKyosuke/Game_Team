using UnityEngine;

public class PlayerAnime : MonoBehaviour
{
    [SerializeField] GameObject m_collider; //UŒ‚—p‚Ì“–‚½‚è”»’è

    private Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void OnAttack1()
    {
        m_collider.SetActive(true);
    }

    public void OnAttack1End()
    { 
        m_collider.SetActive(false);
        m_animator.SetBool("Attack", false);
    }
}
