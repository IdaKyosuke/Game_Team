using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	[SerializeField] GameObject m_bullet;
	[SerializeField] Transform m_muzzle;   // 銃口
	[SerializeField] float m_rate;  // 発射待機時間
	private float m_count;		// 発射待機時間カウント用
	private bool m_isShot = false;

    // Start is called before the first frame update
    void Start()
    {
		m_count = 0;
	}

    // Update is called once per frame
    void Update()
    {
		// 左クリックで射撃
        if(!m_isShot && Input.GetMouseButton(0))
		{
			Instantiate(m_bullet, m_muzzle.position, m_muzzle.rotation, m_muzzle);
			m_isShot = true;
		}

		if(m_isShot)
		{
			m_count += Time.deltaTime;
			if(m_count >= m_rate)
			{
				m_isShot = false;
				m_count = 0;
			}
		}
    }
}
