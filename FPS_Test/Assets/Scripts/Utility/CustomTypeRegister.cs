using ExitGames.Client.Photon;
using System.Collections.Generic;
using System;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class CustomTypeRegister : MonoBehaviour
{
	private void Awake()
	{
		bool ret;
		ret = PhotonPeer.RegisterType(
			typeof(List<ItemList>),
			0,
			SerializeItemList,
			DeserializeItemList
		);

		ret = PhotonPeer.RegisterType(
			typeof(Info_InventorySize),
			1,
			SerializeInfo_InventorySize,
			DeserializeInfo_InventorySize
		);
	}

	private static byte[] SerializeItemList(object customObject)
	{
		Debug.Log("SerializeItemList s");

		List<ItemList> dataList = (List<ItemList>)customObject;

		// int:4, float:4, bool:1, stringは長さ+文字列、Vector2: 8 Vector3:12
		// 今回は簡単化のため stringは固定長40バイトにするbyte[]
		// Vector2Int, int, float, string, bool
		const int ItemListSize = 8 + 4 + 4 + 40 * 2 + 1 * 2;

		// cout, ItemListSize * count
		byte[] bytes = new byte[4 + ItemListSize * dataList.Count];
		int offset = 0;

		Protocol.Serialize(dataList.Count, bytes, ref offset);

		int c = 0;
		foreach(var data in dataList)
		{
			Protocol.Serialize(data.GetGridIndex().x, bytes, ref offset);
			Protocol.Serialize(data.GetGridIndex().y, bytes, ref offset);
			Protocol.Serialize(data.m_id, bytes, ref offset);

			Protocol.Serialize(data.m_attack, bytes, ref offset);

			Protocol.Serialize((short)(data.IsEquip() ? 1 : 0), bytes, ref offset);

			// stringは固定長40バイトでUTF-8エンコード
			byte[] nameBytes = new byte[40];
			byte[] tmp = System.Text.Encoding.UTF8.GetBytes(data.GetPrefabName());
	//		byte[] tmp = System.Text.Encoding.UTF8.GetBytes("hogehoge");
			System.Array.Copy(tmp, nameBytes, Mathf.Min(tmp.Length, 40));   // 文字数が40を超えた場合は切り捨て
			foreach(byte b in nameBytes)
			{
				Protocol.Serialize((short)b, bytes, ref offset);
			}

			c++;
		}

		string str = "";
		foreach(var b in bytes)
		{
			str += b + " ";
		}
		Debug.Log(str);



		/*

		// Helperでintとfloatとboolをbyteに変換
		int[] ints = new int[] {data.GetGridIndex().x, data.GetGridIndex().y, data.m_id};
		System.Buffer.BlockCopy(ints, 0, bytes, offset, 4*ints.Length);
		offset += 4 * ints.Length;

		float[] floats = new float[] { data.m_attack };
		System.Buffer.BlockCopy(floats, 0, bytes, offset, 4 * floats.Length);
		offset += 4* floats.Length;

		// boolは一バイト
		bytes[offset] = (byte)(data.IsEquip() ? 1 : 0);
		offset += 1;

		// stringは固定長40バイトでUTF-8エンコード
		byte[] nameBytes = new byte[40];
		byte[] tmp = System.Text.Encoding.UTF8.GetBytes(data.GetPrefabName());
		System.Array.Copy(tmp, nameBytes, Mathf.Min(tmp.Length, 40));   // 文字数が40を超えた場合は切り捨て
		Buffer.BlockCopy(nameBytes, 0, bytes, offset, 40);
		*/

		Debug.Log("SerializeItemList e:" + bytes);

		return bytes;
	}


	private static byte[] SerializeInfo_InventorySize(object customObject)
	{
		Debug.Log("SerializeInfo_InventorySize s");

		Info_InventorySize info = (Info_InventorySize)customObject;

		byte[] bytes = new byte[4*3];
		int offset = 0;


		Protocol.Serialize((int)info.GetInventoryType, bytes, ref offset);
		Protocol.Serialize(info.GetSize.x, bytes, ref offset);
		Protocol.Serialize(info.GetSize.y, bytes, ref offset);

		/*
		int[] ints = new int[] { (int)info.GetInventoryType, info.GetSize.x, info.GetSize.y };
		System.Buffer.BlockCopy(ints, 0, bytes, offset, 4*ints.Length);
		*/

		Debug.Log("SerializeInfo_InventorySize e");
		foreach(var b in bytes)
		{
			Debug.Log(b);
		}

		return bytes;
	}

	// byte配列からItemListに復元する
	private static object DeserializeItemList(byte[] bytes)
	{
		Debug.Log("DeserializeItemList s");

		int offset = 0;

		string strBytes = "";
		foreach (var b in bytes)
		{
			strBytes += b + " ";
		}
		Debug.Log(strBytes);

		int count;
		Protocol.Deserialize(out count, bytes, ref offset);

		int c = 0;
		List<ItemList> items = new List<ItemList>();
		for(int i=0; i<count;i++)
		{
			ItemList data = ScriptableObject.CreateInstance<ItemList>();

			int x, y;
			Protocol.Deserialize(out x, bytes, ref offset);
			Protocol.Deserialize(out y, bytes, ref offset);
			data.SetGridIndex(new Vector2Int(x, y));

			Protocol.Deserialize(out data.m_id, bytes, ref offset);


			Protocol.Deserialize(out data.m_attack, bytes, ref offset);

			short equipInfo;
			Protocol.Deserialize(out equipInfo, bytes, ref offset);
			data.SetEquipInfo(equipInfo == 1);


			byte[] nameBytes = new byte[40];
			for(int j=0; j<40; j++)
			{
				short str;
				Protocol.Deserialize(out str, bytes, ref offset);
				nameBytes[j] = (byte)str;
			}
			data.SetPrefabName(System.Text.Encoding.UTF8.GetString(nameBytes).TrimEnd('\0'));

			items.Add(data);

			c++;
		}



		/*
		// int復元
		int[] ints = new int[3];
		System.Buffer.BlockCopy(bytes, 0, ints, offset, 4*ints.Length);
		data.SetGridIndex(new Vector2Int(ints[0], ints[1]));
		data.m_id = ints[2];
		offset += 4 * ints.Length;

		// floatの復元
		float[] floats = new float[1];
		System.Buffer.BlockCopy(bytes, 0, floats, offset, 4 * floats.Length);
		data.m_attack = floats[0];
		offset += 4* floats.Length;

		// boolの復元
		data.SetEquipInfo(bytes[offset] == 1);
		offset++;

		// stringの復元
		byte[] nameBytes = new byte[40];
		System.Buffer.BlockCopy(bytes, 0, nameBytes, offset, 40);
		data.SetPrefabName(System.Text.Encoding.UTF8.GetString(nameBytes).TrimEnd('\0'));
		*/

		Debug.Log("DeserializeItemList e:" + items.Count);

		return items;
	}

	private static object DeserializeInfo_InventorySize(byte[] bytes)
	{
		Debug.Log("DeserializeInfo_InventorySize s");

		Info_InventorySize info = ScriptableObject.CreateInstance<Info_InventorySize>();
		int offset = 0;

		int inventoryType;
		Protocol.Deserialize(out inventoryType, bytes, ref offset);
		info.SetInventoryType(inventoryType);

		int x, y;
		Protocol.Deserialize(out x, bytes, ref offset);
		Protocol.Deserialize(out y, bytes, ref offset);
		info.SetSize(x, y);

		/*
		// int復元
		int[] ints = new int[3];
		System.Buffer.BlockCopy(bytes, 0, ints, offset, 4 * ints.Length);
		info.SetInventoryType(ints[0]);
		info.SetSize(ints[1], ints[2]);
		*/

		Debug.Log("DeserializeInfo_InventorySize e:" + info);

		return info;
	}
}
