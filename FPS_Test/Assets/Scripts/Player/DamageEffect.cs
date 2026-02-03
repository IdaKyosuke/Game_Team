using UnityEngine;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour
{
	private Image m_damageImage;
	private const float FadeSpeed = 2.0f;
	private float m_time = 0.0f;
	private bool m_isDamaged = false;

	void Start()
	{
		m_damageImage = GetComponent<Image>();
	}

	void FixedUpdate()
    {
        if (m_isDamaged)
		{
			m_time += Time.deltaTime;
			if (m_time > FadeSpeed)
			{
				Debug.Log(m_damageImage.color.a);
				m_damageImage.color = new Color(1, 1, 1, m_damageImage.color.a - 0.02f);
				if (m_damageImage.color.a <= 0.1f)
				{
					m_damageImage.color = new Color(1, 1, 1, 0);
					m_isDamaged = false;
					m_time = 0.0f;
				}
			}
		}
	}

	public void StartDamageEffect()
	{
		m_isDamaged = true;
	}
}
