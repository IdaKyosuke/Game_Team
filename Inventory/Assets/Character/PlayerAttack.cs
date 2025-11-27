using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] PlayerStatus m_status;
    [SerializeField] Condition m_condition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerStatus>().Damage(m_status.TotalStatus.physicalPower, AttackType.Physical, m_condition);
        }
    }
}