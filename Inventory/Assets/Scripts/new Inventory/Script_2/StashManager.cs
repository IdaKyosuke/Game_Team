using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;


public struct Grid
{
	private GameObject m_grid;	// マス目のオブジェクト
	private bool m_info;		// 中身が埋まっているか

	private Grid(GameObject grid = null, bool info = false)
	{
		m_grid = grid;
		m_info = info;
	}

	// 枠の情報をセット
	public void SetGrid(GameObject grid)
	{
		m_grid = grid;
	}

	// 枠のTransformを取得
	public Transform GetTransform()
	{
		return m_grid.transform;
	}

	// ポインターが重なっているか
	public bool OnPointer()
	{
		return m_grid.GetComponent<GridIcon>().OnPointer();
	}

	// 中身があるかを取得
	public bool GetInfo()
	{
		return m_grid.GetComponent<GridIcon>().GetOnFillUi();
	}

	// 中身の状態を変える
	public void SetInfo(bool info)
	{
		m_grid.GetComponent<GridIcon>().SetUi(info);
	}

	// gridtypeを設定する
	public void SetGridType(GridType type)
	{
		m_grid.GetComponent<GridIcon>().SetType(type);
	}
}

public class StashManager : MonoBehaviour
{
	// スタッシュ用サイズ
	[SerializeField] int m_stashWidth = 5;
	[SerializeField] int m_stashHeight = 10;

	// インベントリ用サイズ
	[SerializeField] int m_inventoryWidth = 5;
	[SerializeField] int m_inventoryHeight = 5;

	[SerializeField] GameObject m_stashGridParent;  // スタッシュのマス目の親オブジェクト
	[SerializeField] GameObject m_inventoryGridParent;	// インベントリのマス目の親オブジェクト

	private Grid[,] m_stashGridList;
	private Grid[,] m_inventoryGridList;
	private Transform m_moveItemTransform;
	private GridType m_checkType;

	// Start is called before the first frame update
	void Start()
    {
		// スタッシュ用マス目の配列を作成
		m_stashGridList = new Grid[m_stashWidth, m_stashHeight];
		// インベントリ用マス目の配列を作成
		m_inventoryGridList = new Grid[m_inventoryWidth, m_inventoryHeight];

		// アイテム移動用のオブジェクト
		m_moveItemTransform = GameObject.FindWithTag("moveItemTransform").transform;

		// スタッシュ用配列を作成
		// 配列とマス目の状態を合わせる
		int count = 0;
		for(int i = 0; i < m_stashHeight; i++)
		{
			for(int j = 0; j < m_stashWidth; j++)
			{
				// オブジェクトを追加
				GameObject g = m_stashGridParent.transform.GetChild(count).gameObject;
				m_stashGridList[j, i].SetGrid(g);
				count++;
				// 中身を空にする
				m_stashGridList[j, i].SetInfo(false);
				m_stashGridList[j, i].SetGridType(GridType.Stash);
			}
		}

		// インベントリ用配列を作成
		count = 0;
		for (int i = 0; i < m_inventoryHeight; i++)
		{
			for (int j = 0; j < m_inventoryWidth; j++)
			{
				// オブジェクトを追加
				GameObject g = m_inventoryGridParent.transform.GetChild(count).gameObject;
				m_inventoryGridList[j, i].SetGrid(g);
				count++;
				// 中身を空にする
				m_inventoryGridList[j, i].SetInfo(false);
				m_inventoryGridList[j, i].SetGridType(GridType.Inventory);
			}
		}

		m_checkType = GridType.Empty;
	}

    // Update is called once per frame
    void Update()
    {
		if(Input.GetMouseButtonUp(0))
		{
			SetItem();
			// 未選択状態に戻す
			m_checkType = GridType.Empty;
		}
    }

	// アイテムを空き枠にセットする
	public void SetItem()
	{
		// 移動中のアイテムがないときは無視
		if (m_moveItemTransform.childCount <= 0) return;

		// アイテム移動用のオブジェクトの中身を確認
		GameObject item = m_moveItemTransform.GetChild(0).gameObject;

		Grid[,] list = null;
		int height = 0;
		int width = 0;

		switch(m_checkType)
		{
			case GridType.Stash:
				list = m_stashGridList;
				height = m_stashHeight;
				width = m_stashWidth;
				break;

			case GridType.Inventory:
				list = m_inventoryGridList;
				height = m_inventoryHeight;
				width = m_inventoryWidth;
				break;

			// GridType.Empty
			default:
				// マス目を選択していないとき
				item.GetComponent<Item_Object>().PointerUp(false);
				return;
		}

		// リストを回す
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				// カーソルの重なっているマスが空の時
				if (list[j, i].OnPointer() && !list[j, i].GetInfo())
				{
					if (CheckSpace(new Vector2Int(j, i), item.GetComponent<Item_Object>().GetSize()))
					{
						// 移動先の子オブジェクトに設定する or 元の位置に戻す
						item.GetComponent<Item_Object>().PointerUp(
							true,
							list[j, i].GetTransform()
							);
						// 基点のインデックスを保持
						item.GetComponent<Item_Object>().SetIndex(new Vector2Int(j, i));
						// 現在のgridtypeを保管
						item.GetComponent<Item_Object>().SetType(m_checkType);
					}
					else
					{
						// 元の位置に戻す
						item.GetComponent<Item_Object>().PointerUp(false);
					}

					return;
				}
			}
		}

		// 選択マスが空いていないとき
		item.GetComponent<Item_Object>().PointerUp(false);
	}

	// アイテムが入るスペースを確認
	private bool CheckSpace(Vector2Int startGrid, Vector2Int size)
	{
		Debug.Log(startGrid.x + (size.x - 1));
		Debug.Log(startGrid.y + (size.y - 1));

		Grid[,] list = null;
		int height = 0;
		int width = 0;

		switch(m_checkType)
		{
			case GridType.Stash:
				list = m_stashGridList;
				height = m_stashHeight;
				width = m_stashWidth;
				break;

			case GridType.Inventory:
				list = m_inventoryGridList;
				height = m_inventoryHeight;
				width = m_inventoryWidth;
				break;
		}

		// 枠外にはみ出すときはそもそも確認しない
		if (startGrid.x + (size.x - 1) >= width) return false;
		if (startGrid.y + (size.y - 1) >= height) return false;

		// 中身を確認
		for (int i = startGrid.x; i < startGrid.x + size.x; i++)
		{
			for(int j = startGrid.y; j < startGrid.y + size.y; j++)
			{
				if (list[i, j].GetInfo())
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
				list[i, j].SetInfo(true);
			}
		}

		return true;
	}

	// 指定したマスの状態を変更する
	public void MoveItem(Vector2Int basePos, Vector2Int size, bool info, GridType type)
	{
		// アドレスコピー
		Grid[,] list = null;

		switch (type)
		{
			case GridType.Stash:
				list = m_stashGridList;
				break;

			case GridType.Inventory:
				list = m_inventoryGridList;
				break;
		}

		// スペースが空いているときは中身が入っていることにする
		for (int i = basePos.x; i < basePos.x + size.x; i++)
		{
			for (int j = basePos.y; j < basePos.y + size.y; j++)
			{
				list[i, j].SetInfo(info);
			}
		}
	}

	// アイテムを置けるかを探す
	public void StartSet(GridType type)
	{
		m_checkType = type;
	}
}
