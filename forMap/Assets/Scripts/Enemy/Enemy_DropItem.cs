using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Enemy_DropItem : MonoBehaviour
{
	// 数字が大きくなるにつれてドロップ率は落ちる
	[SerializeField] GameObject m_dropItem1;
	[SerializeField] GameObject m_dropItem2;
	[SerializeField] GameObject m_dropItem3;

	[SerializeField] int a;
	[SerializeField] int b;

	// 100%までの中の割合
	const int m_dropRate1 = 60;    // m_dropItem1の確率
	const int m_dropRate2 = 95;    // m_dropItem2の確率

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		// 自分が死亡したときに抽選してアイテムをドロップする
        //if(this.GetComponent<Enemy_Move>().IsDeath())
		{
			DropItem();
			Destroy(gameObject);
		}
    }

	private void DropItem()
	{
		// 0～100で確率抽選
		int random = UnityEngine.Random.Range(0, 101);

		if(random <= a)
		{
			// ドロップアイテム１
			Instantiate(m_dropItem1, transform.position, Quaternion.identity);
		}
		else if(a <= random && random <= a + b)
		{
			// ドロップアイテム２
			Instantiate(m_dropItem2, transform.position, Quaternion.identity);
		}
		else
		{
			// ドロップアイテム３
			Instantiate(m_dropItem3, transform.position, Quaternion.identity);
		}
	}
}
