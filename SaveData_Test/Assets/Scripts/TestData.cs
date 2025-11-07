using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

// unityのデフォルト機能でセーブを行う
public class TestData : MonoBehaviour
{
	public string m_name;
	public int m_lv;
	public int m_playerMoney;

	[SerializeField] List<TextMeshProUGUI> m_list;

	private void Update()
	{
		m_list[0].text = m_name;
		m_list[1].text = m_lv.ToString();
		m_list[2].text = m_playerMoney.ToString();
	}

	// セーブ
	public void Save()
	{
		// クラスの変数をJsonに変換
		string json = JsonUtility.ToJson(this);

		// playerprefsに保存
		PlayerPrefs.SetString("SavedData", json);
		PlayerPrefs.Save();

		Debug.Log("save");
	}

	// ロード
	public void Load()
	{
		// PlayerPrefsからjson形式のデータを取得
		string json = PlayerPrefs.GetString("SavedData");

		// このクラスが持つ変数を上書き
		JsonUtility.FromJsonOverwrite(json, this);

		Debug.Log("load");
	}
}
