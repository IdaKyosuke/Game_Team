using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] float m_speed;
	[SerializeField] float m_deleteTime;    // è¡Ç¶ÇÈÇ‹Ç≈ÇÃéûä‘
	private Rigidbody m_rb;
	private float m_count = 0;

    // Start is called before the first frame update
    void Start()
    {
		// éÀåÇ
        m_rb = GetComponent<Rigidbody>();
		Vector3 vec = GameObject.FindWithTag("Muzzle").transform.forward;
		m_rb.AddForce (vec * m_speed);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
		m_count += Time.deltaTime;
		if(m_count >= m_deleteTime)
		{
			Destroy(this.gameObject);
		}
    }

	private void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			Destroy(this.gameObject);
		}
	}
}
