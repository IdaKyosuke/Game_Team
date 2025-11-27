using UnityEngine;

public class PlayerAnime : MonoBehaviour
{
    [SerializeField] Weapon_Collider m_collider; //UŒ‚—p‚Ì“–‚½‚è”»’è

	private bool m_isAttack = false;

    public void OnAttack1()
    {
		// UŒ‚’†‚Í–³‹
		if (m_isAttack) return;
		m_collider.StartAttack();
		m_isAttack = true;
    }

    public void OnAttack1End()
    {
		// UŒ‚’†ˆÈŠO‚Í–³‹
		if (!m_isAttack) return;
		m_collider.EndAttack();
		m_isAttack = false;
    }

	public bool IsAttack()
	{
		return m_isAttack;
	}
}