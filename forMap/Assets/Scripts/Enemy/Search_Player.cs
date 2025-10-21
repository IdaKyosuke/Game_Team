using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵がプレイヤーを発見するためのコライダー
public class Search_Player : MonoBehaviour
{
	[SerializeField] GameObject m_enemy;    // コライダーの親オブジェクト
	[SerializeField] float m_rayLength;	// レイの長さ
	private Vector3 m_rayDir;	// レイを飛ばす方向

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	private void OnTriggerStay(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			if(CheckRay(other.gameObject))
			{
				// プレイヤーを認識
				m_enemy.GetComponent<Enemy_Nav>().InCombat();
			}
		}
	}

	// 実際にレイを飛ばす
	private bool CheckRay(GameObject player)
	{
		m_rayDir = player.transform.position - transform.position;
		// 自分自身からプレイヤーに対してレイを作成
		Ray ray = new Ray(transform.position, m_rayDir);

		RaycastHit hit;
		if(Physics.Raycast(ray, out hit))
		{
			if(hit.collider.gameObject.CompareTag("Player"))
			{
				// 敵とプレイヤーの間に何も障害物が無い場合
				return true;
			}
		}
		return false;
	}
}
