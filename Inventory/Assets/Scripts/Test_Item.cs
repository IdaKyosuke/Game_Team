using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName = "ScriptableObject/Create Item")]
public class Test_Item : ScriptableObject
{
	// アイテム比較用番号
	public int number = 0;

	// アイテム名
	public new string name = "New Item";

	// アイテムアイコン
	public Sprite icon = null;

	// アイテムの説明
	public string explain = "New Explain";

	// アイテムの販売価格
	public int buy_price = 0;

	// アイテムの売値
	public int sell_price = 0;

	// アイテムの種類
	public enum Type
	{
		Material,
		ForHeal,
		ForMoney,
	}

	public Type type = Type.Material;

	// アイテムの種類を取得
	public string ItemType()
	{
		string name = "new type";

		switch(type)
		{
		case Type.Material:
			name = "素材アイテム";
				break;

		case Type.ForHeal:
			name = "回復アイテム";
				break;

		case Type.ForMoney:
			name = "換金アイテム";
			break;
		}
		return name;
	}

	// 使用できるかどうか
	public bool canUse = false;

	// 使用できるときの効果の種類
	public enum Effect
	{
		Heal,	// 体力回復
		AddHp,	// 最大体力増加
		AddAtk,	// 攻撃バフ
		RepairLight,	// ダンジョン内で明るさを回復する
	}

	// アイテムが持つ効果
	public Effect effect = Effect.Heal;
	// 効果量
	public int effectSize = 0;

	// 効果が付与されるプレイヤーを設定していないので一旦無視
	/*
	// 使用できる時の効果
	public void Use()
	{
		if(effect == Effect.RepairLight)
		{
			GameObject light = GameObject.FindWithTag("lightManager");
			light.GetComponent<Light_Manager>().RepairLight();
		}
		else
		{
			GameObject PlayerStatus = GameObject.FindWithTag("playerStatus");
			switch(effect)
			{
			case Effect.Heal:
				PlayerStatus.GetComponent<Player_DungeonStatus>().Heal(effectSize);
				Debug.Log(PlayerStatus.GetComponent<Player_DungeonStatus>().GetCurrentHp());
				break;

			case Effect.AddHp:
				PlayerStatus.GetComponent<Player_DungeonStatus>().AddHp(effectSize);
				Debug.Log(PlayerStatus.GetComponent<Player_DungeonStatus>().GetMaxHp());
				break;

			case Effect.AddAtk:
				PlayerStatus.GetComponent<Player_DungeonStatus>().AddAtk(effectSize);
				Debug.Log(PlayerStatus.GetComponent<Player_DungeonStatus>().GetAtk());
				break;

			case Effect.RepairLight:
				break;
			}
		}

	}
	*/
}
