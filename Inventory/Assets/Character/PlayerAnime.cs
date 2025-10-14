using UnityEngine;

public class PlayerAnime : MonoBehaviour
{
    [SerializeField] GameObject m_collider; //UŒ‚—p‚Ì“–‚½‚è”»’è

    private Animator m_animator;
	private bool m_isAttack = false;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void OnAttack1()
    {
		// UŒ‚’†‚Í–³‹
		if (m_isAttack) return;
		m_collider.SetActive(true);
		m_isAttack = true;
    }

    public void OnAttack1End()
    {
		// UŒ‚’†ˆÈŠO‚Í–³‹
		if (!m_isAttack) return;
		m_collider.SetActive(false);
		m_isAttack = false;
    }

	public bool IsAttack()
	{
		return m_isAttack;
	}
}