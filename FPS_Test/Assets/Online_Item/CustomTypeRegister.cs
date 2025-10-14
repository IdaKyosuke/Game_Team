using ExitGames.Client.Photon;
using System;
using UnityEngine;

public class CustomTypeRegister : MonoBehaviour
{
	private void Awake()
	{
		PhotonPeer.RegisterType(
			typeof(ItemList),
			0,
			SerializeItemList,
			DeserializeItemList
		);

		PhotonPeer.RegisterType(
			typeof(Info_InventorySize),
			1,
			SerializeInfo_InventorySize,
			DeserializeInfo_InventorySize
		);
	}

	private static byte[] SerializeItemList(object customObject)
	{
		ItemList data = (ItemList)customObject;

		// int:4, float:4, bool:1, stringは長さ+文字列、Vector2: 8 Vector3:12
		// 今回は簡単化のため stringは固定長20バイトにするbyte[]
		// Vector2Int, int, float, string, bool
		byte[] bytes = new byte[8 + 4 + 4 + 20 + 1];
		int offset = 0;

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

		// stringは固定長20バイトでUTF-8エンコード
		byte[] nameBytes = new byte[20];
		byte[] tmp = System.Text.Encoding.UTF8.GetBytes(data.GetPrefabName());
		System.Array.Copy(tmp, nameBytes, Mathf.Min(tmp.Length, 20));   // 文字数が20を超えた場合は切り捨て
		Buffer.BlockCopy(nameBytes, 0, bytes, offset, 20);

		return bytes;
	}


	private static byte[] SerializeInfo_InventorySize(object customObject)
	{
		Info_InventorySize info = (Info_InventorySize)customObject;

		byte[] bytes = new byte[4*3];
		int offset = 0;

		int[] ints = new int[] { (int)info.GetInventoryType, info.GetSize.x, info.GetSize.y };
		System.Buffer.BlockCopy(ints, 0, bytes, offset, 4*ints.Length);

		return bytes;
	}

	// byte配列からItemListに復元する
	private static object DeserializeItemList(byte[] bytes)
	{
		ItemList data = new ItemList();
		int offset = 0;

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
		byte[] nameBytes = new byte[20];
		System.Buffer.BlockCopy(bytes, 0, nameBytes, offset, 20);
		data.SetPrefabName(System.Text.Encoding.UTF8.GetString(nameBytes).TrimEnd('\0'));

		return data;
	}

	private static object DeserializeInfo_InventorySize(byte[] bytes)
	{
		Info_InventorySize info = new Info_InventorySize();
		int offset = 0;

		// int復元
		int[] ints = new int[3];
		System.Buffer.BlockCopy(bytes, 0, ints, offset, 4 * ints.Length);
		info.SetInventoryType(ints[0]);
		info.SetSize(ints[1], ints[2]);

		return info;
	}
}
