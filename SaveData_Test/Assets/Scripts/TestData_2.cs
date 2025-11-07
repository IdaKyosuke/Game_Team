using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using UnityEditorInternal;
using Unity.VisualScripting;

[Serializable]
// jsonファイルを作成してセーブ
public class TestData_2 : ISerializationCallbackReceiver
{
	private static TestData_2 m_instance = null;

	public static TestData_2 Instance
	{
		get 
		{ 
			if(m_instance == null)
			{
				// データをロードする
				Load();
			}
			return m_instance; 
		}
	}

	// SaveData(TestData)をJsonに変換したテキスト(リロード時に何度も読み込まないように保持)
	[SerializeField] private static string m_jsonText = "";

	// ---- 保存されるデータ("public" か "SerializeField" をつける) ----
	public string m_name = "NoName";
	public int m_lv = 1;
	public int m_money = 0;
	public List<int> m_sample = new List<int>() { 0,1,2,3 };

	[SerializeField] private string m_testDictJson = "";
	
	// "Dictionary"や"Object"はそのまま保存ができないので"string"に変換して保存する
	public Dictionary<string, int> m_testDict = new Dictionary<string, int>() 
	{
		{ "key1", 0   },
		{ "key2", 50  },
		{ "key3", 100 }
	};

	// ---- シリアライズ, デシリアライズ時のコールバック ----
	// SaveData(TestData)→Jsonの変換の前に実行
	public void OnBeforeSerialize()
	{
		// Dictionaryはそのままで保存されないのでシリアライズしてテキストで保存
		m_testDictJson = Serialize(m_testDict);
	}

	// SaveData(TestData)→Jsonの変換の後に実行
	public void OnAfterDeserialize()
	{
		// 保存されているテキストがあればDictionaryにデシリアライズする
		if(!string.IsNullOrEmpty(m_testDictJson))
		{
			m_testDict = Desirialize<Dictionary<string, int>>(m_testDictJson);
		}
	}

	// 引数のオブジェクトをシリアライズして返す
	private static string Serialize<T>(T obj)
	{
		BinaryFormatter bf = new BinaryFormatter();
		MemoryStream  ms = new MemoryStream();
		bf.Serialize(ms, obj);
		return Convert.ToBase64String(ms.GetBuffer());
	}

	// 引数のテキストを指定されたクラスにデシリアライズして返す
	private static T Desirialize<T>(string str)
	{
		BinaryFormatter bf = new BinaryFormatter();
		MemoryStream ms = new MemoryStream(Convert.FromBase64String(str));
		return (T)bf.Deserialize(ms);
	}

	// ---- データを再読み込みする ----
	public void Reload()
	{
		JsonUtility.FromJsonOverwrite(GetJson(), this);
	}

	// データを読み込む
	private static void Load()
	{
		m_instance = JsonUtility.FromJson<TestData_2>(GetJson());
	}

	// ---- 保存しているJsonを取得 ----
	private static string GetJson()
	{
		// すでにJsonを取得している時はそれを返す
		if(!string.IsNullOrEmpty(m_jsonText))
		{
			return m_jsonText;
		}

		// Jsonを保存している場所のパスを取得
		string path = GetFilePath();

		// Jsonが存在しているかを調べてから取得して変換(存在しない場合は新たなクラスを作成してJsonに変換)
		if (File.Exists(path))
		{
			m_jsonText = File.ReadAllText(path);
		}
		else
		{
			m_jsonText = JsonUtility.ToJson(new TestData_2());
		}

		return m_jsonText;
	}

	// ---- データをJsonにして保存 ----
	public void Save()
	{
		m_jsonText = JsonUtility.ToJson(this);
		File.WriteAllText(GetFilePath(), m_jsonText);
	}

	// ---- データを全て削除し、初期化 ----
	public void Delete()
	{
		m_jsonText = JsonUtility.ToJson(new TestData_2());
		Reload();
	}

	// ---- 保存先のパスを取得 ----
	private static string GetFilePath()
	{
		string path = "SaveData_2";

		// エディタ上ではAssetsと同じ階層
#if UNITY_EDITOR
		path += ".json";
#else
		path = Application.persistentDataPath + "/" + path;
#endif

		Debug.Log(path);

		return path;
	}
}
