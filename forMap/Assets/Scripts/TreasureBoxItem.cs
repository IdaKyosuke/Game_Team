using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static TreasureBoxItem;

public class TreasureBoxItem : MonoBehaviour
{
	public enum TreasureType
	{
		Common,
		Rare,
		Unipue,
		Legendary,

		Length,
	}

	[SerializeField] ExcelData m_excelData;
	[SerializeField] TreasureType m_treasureType;
	private List<ObjectEntity> m_treasureList = new List<ObjectEntity>();

	void Start()
	{
		// 抽選会数を設定
		int selectAmount = Random.Range(
			m_excelData.treasureBox[(int)m_treasureType].itemMin,
			m_excelData.treasureBox[(int)m_treasureType].itemMax + 1);

		SelectTreasureItem(selectAmount);
	}

	public void GetItem()
	{
		TryGetComponent(out TreasureAnime treasure);
		if (treasure.opentreasure) return;
		for (int i = 0; i < m_treasureList.Count; i++)
		{
			Debug.Log(m_treasureList[i].displayName);
		}
	}

	private void SelectTreasureItem(int selectAmount)
	{
		// 宝箱のレアリティの抽選の数値を取得
		int[] probability = new int[(int)TreasureType.Length];
		probability[0] = m_excelData.treasureBox[(int)m_treasureType].common;
		probability[1] = m_excelData.treasureBox[(int)m_treasureType].rare;
		probability[2] = m_excelData.treasureBox[(int)m_treasureType].unique;
		probability[3] = m_excelData.treasureBox[(int)m_treasureType].legendary;

		// 宝箱のレアリティに応じて確定のレアリティのアイテムを一つ抽選
		m_treasureList.Add(SelectObject(m_treasureType));

		// selectAmountの数だけ抽選する
		for (int i = 0; i < selectAmount; ++i)
		{
			TreasureType treasureType = SelectRarity(probability);
			ObjectEntity treasureItem =  SelectObject(treasureType);

			m_treasureList.Add(treasureItem);
		}

		/////////////////////////////////////////////////
		// 宝箱の中に空きがあるかどうかを調べる適なやつ//
		/////////////////////////////////////////////////
	}

	private TreasureType SelectRarity(int[] probability)
	{
		// レアリティの抽選
		int raritySelectNum = Random.Range(0, 101);
		int rarityNum = 0;

		for (int i = 0; i < (int)TreasureType.Length; ++i)
		{
			raritySelectNum -= probability[i];
			if (raritySelectNum < 0)
			{
				rarityNum = i;
				break;
			}
		}
		return (TreasureType)rarityNum;
	}

	private ObjectEntity SelectObject(TreasureType type)
	{
		// 今回の抽選されたレアリティに応じてアイテム情報を取得
		List<ObjectEntity> objectData = m_excelData.common;
		switch (type)
		{
			case TreasureType.Common:
				objectData = m_excelData.common;
				break;

			case TreasureType.Rare:
				objectData = m_excelData.rare;
				break;

			case TreasureType.Unipue:
				objectData = m_excelData.unique;
				break;

			case TreasureType.Legendary:
				objectData = m_excelData.legendary;
				break;
		}

		int objectSelectNum = Random.Range(0, 101);
		int objectIndex = 0;
		// アイテムを抽選
		for (int j = 0; j < objectData.Count; ++j)
		{
			objectSelectNum -= objectData[j].probability;
			if (objectSelectNum <= 0)
			{
				objectIndex = j;
				break;
			}
		}
		return objectData[objectIndex];
	}
}
