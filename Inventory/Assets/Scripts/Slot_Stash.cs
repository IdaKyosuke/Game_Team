using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// スタッシュのプレイヤーインベントリ用
public class Slot_Stash : MonoBehaviour
{
	// インベントリ用Ui
	[SerializeField] Image icon;               // アイテムのアイコン
	[SerializeField] GameObject m_playerStatus;
	[SerializeField] GameObject Collider;   // カーソル判定用

	private Test_Item item;               // スロットに格納されるアイテム

	private void Start()
	{
		if (!m_playerStatus)
		{
			m_playerStatus = GameObject.FindWithTag("playerStatus");
		}

		// シーン開始時に中身があるかどうかで当たり判定の有無を決める
		if(item)
		{
			Collider.SetActive(true);
		}
		else
		{
			Collider.SetActive(false);
		}
	}

	// アイテムを追加
	public void AddItem(Test_Item newItem)
	{
		// インベントリ用Uiの設定
		item = newItem;
		icon.sprite = newItem.icon;
		icon.enabled = true;
		Collider.SetActive(true);
	}

	// アイテムをクリア
	public void ClearItem()
	{
		// インベントリ用Uiのリセット
		item = null;
		icon.sprite = null;
		icon.enabled = false;
		Collider.SetActive(false);
	}

	// インベントリからアイテムボックスに送る
	public void PutInBox()
	{
		// アイテムボックスにアイテムを送る
		Stash_Box.instance.AddItem(item);

		// プレイヤーのインベントリから送ったアイテムを削除する
		Inventory_City.instance.RemoveItem(item);
	}

	// アイテムボックスからインベントリに送る
	public void PutInInventory()
	{
		// プレイヤーのインベントリにアイテムを送る
		Inventory_City.instance.AddItem(item);

		// アイテムボックスからアイテムを削除する
		Stash_Box.instance.RemoveItem(item);
	}
}
