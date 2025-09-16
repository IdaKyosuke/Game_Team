using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status", menuName = "ScriptableObject/Create Status")]

// プレイヤーのステータスをシーン間で受け渡す
public class PlayerStatus : ScriptableObject
{
	// 所持金の最大値
	private int m_maxMoney = 999999;

	// プレイヤー死亡時にステータスを初期化する用
	[SerializeField] int initMoney = 0;
	[SerializeField] int initAtk = 0;
	[SerializeField] int initHp = 20;
	[SerializeField] int initLv = 1;

	// 初期の値
	public int firstAtk = 0;       // 攻撃力
	public int firstMoney = 0;        // 所持金
	public int firstHp = 0;     // 体力の最大値
	public int currentHp = 0;   // 現在の体力
	public int firstLv = 1;		// レベル

	// 強化段階
	public int WeaponLevel = 0; // 武器レベル
	public int BlackSmithLevel = 0; // 鍛冶場のレベル
	public int ShopLevel = 0;	// 売店のレベル

	// アイテムリストは参照渡しする
	public List<Test_Item> itemList = new List<Test_Item>();	// 所持アイテムリスト
	public List<Test_Item> stashList = new List<Test_Item>();	// アイテムボックスの中身

	// ---- 値を変更する関数 ----
	// 攻撃力を変更する(ダンジョン用)
	public void SetAtk(int atk)
	{
		firstAtk = atk;
	}

	// 攻撃力を変更(街用)
	public void SetCityAtk(int atk)
	{
		initAtk = initAtk + atk;
		// 鍛冶屋で強化された分を適用する
		firstAtk = initAtk;
	}

	// 所持金を変更する
	public void SetMoney(int money)
	{
		firstMoney = money;
		if(firstMoney >= m_maxMoney)
		{
			// 所持金を最大値に補正する
			firstMoney = m_maxMoney;
		}
	}

	// 体力の最大値を変更する
	public void SetMaxHp(int hp)
	{
		firstHp = hp;
	}

	// 体力の現在値を変更
	public void SetCurrentHp(int hp)
	{
		currentHp = hp;
	}

	// ダンジョン内でレベルをあげる
	public void SetLv(int lv) 
	{
		firstLv++;
	}

	// 死亡時にリセットされるステータス
	public void ResetStatus()
	{
		firstMoney = initMoney;	// 所持金をリセット
		itemList.Clear();		// インベントリをリセット
	}

	// ダンジョンを出たときに絶対リセットされるステータス
	public void ReturnCity()
	{
		firstAtk = initAtk;     // 攻撃力を街での強化値に戻す
		firstHp = initHp;       // 体力を街の値に戻す
		currentHp = firstHp;    // 体力の現在値を街の体力に合わせる
		firstLv = initLv;		// Lvを1に戻す
	}

	// 武器の強化値を上げる
	public void AddWeaponLevel()
	{
		WeaponLevel = WeaponLevel + 1;
	}

	// 鍛冶場のレベルをあげる
	public void AddBlackSmithLevel()
	{
		BlackSmithLevel = BlackSmithLevel + 1;
	}

	// 売店のレベルをあげる
	public void AddShopLevel()
	{
		ShopLevel = ShopLevel + 1;
	}
}
