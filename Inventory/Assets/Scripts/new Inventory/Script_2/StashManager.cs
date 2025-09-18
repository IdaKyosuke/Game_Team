using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
	private GameObject m_grid = null;	// マス目のオブジェクト
	private bool m_info = false;		// 中身が埋まっているか

	// 枠の情報をセット
	public void SetGrid(GameObject grid)
	{
		m_grid = grid;
	}

	public GameObject GetObject()
	{
		return m_grid;
	}

	// 枠のTransformを取得
	public Transform GetTransform()
	{
		return m_grid.transform;
	}

	// 中身があるかを取得
	public bool GetInfo()
	{
		return m_grid.GetComponent<Icon>().GetOnFillUi();
	}

	// 中身の状態を変える
	public void SetInfo(bool info)
	{
		m_grid.GetComponent<Icon>().SetUi(info);
	}
}

public class StashManager : MonoBehaviour
{
	[SerializeField] int m_width = 5;
	[SerializeField] int m_height = 10;

	[SerializeField] GameObject m_gridParent;	// マス目の親オブジェクト

	private Grid[,] m_gridList;
	private Transform m_moveItemTransform;

	// Start is called before the first frame update
	void Start()
    {
		// マス目の配列を作成
		m_gridList = new Grid[m_width, m_height];

		// アイテム移動用のオブジェクト
		m_moveItemTransform = GameObject.FindWithTag("moveItemTransform").transform;


		// 配列とマス目の状態を合わせる
		int count = 0;
		for(int i = 0; i < m_height; i++)
		{
			for(int j = 0; j < m_width; j++)
			{
				
				// オブジェクトを追加
				GameObject g = m_gridParent.transform.GetChild(count).gameObject;
				Debug.Log(g);
				m_gridList[i, j].SetGrid(g);
				count++;
				// 中身を空にする
				m_gridList[i, j].SetInfo(false);
				
				Debug.Log(m_gridList[i, j].GetObject());
			}
		}
    }

    // Update is called once per frame
    void Update()
    {
		if(Input.GetMouseButtonUp(0))
		{
			// 移動中のアイテムがないときは無視
			if(m_moveItemTransform.childCount <= 0) return;

			Debug.Log(0);

			// リストを回すカーソルが重なっているマスを見つける
			for(int i = 0; i < m_height; i++)
			{
				for(int j = 0; j < m_width; j++)
				{
					// カーソルの重なっているマスが空の時
					if (m_gridList[i, j].GetInfo())
					{
						// アイテム移動用のオブジェクトの中身を確認
						GameObject item = m_moveItemTransform.gameObject;

						// 移動先の子オブジェクトに設定する or 元の位置に戻す
						item.GetComponent<Item_Object>().PointerUp(
							CheckSpace(new Vector2Int(i, j), item.GetComponent<Item_Object>().GetSize()),
							m_gridList[i, j].GetTransform()
							);
					}
				}
			}
		}
    }

	// アイテムが入るスペースを確認
	private bool CheckSpace(Vector2Int startGrid, Vector2Int size)
	{
		// 枠外にはみ出すときはそもそも確認しない
		if(startGrid.x + size.x >= m_width) return false;
		if(startGrid.y + size.y >= m_height) return false;

		// 中身を確認
		for(int i = startGrid.x; i < startGrid.x + size.x; i++)
		{
			for(int j = startGrid.y; j < startGrid.y + size.y; j++)
			{
				if (m_gridList[i, j].GetInfo())
				{
					// 中身があるときはfalse
					return false;
				}
			}
		}

		// スペースが空いているときは中身が入っていることにする
		for (int i = startGrid.x; i < startGrid.x + size.x; i++)
		{
			for (int j = startGrid.y; j < startGrid.y + size.y; j++)
			{
				m_gridList[i, j].SetInfo(true);
			}
		}

		return true;
	}
}
