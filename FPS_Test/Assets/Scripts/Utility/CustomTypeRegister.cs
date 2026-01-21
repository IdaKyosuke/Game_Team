using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;

public class CustomTypeRegister : MonoBehaviour
{
	private void Awake()
	{
		bool ret;
		ret = PhotonPeer.RegisterType(
			typeof(ItemList),
			0,
			SerializeItemData,
			DeserializeItemData
		);

		ret = PhotonPeer.RegisterType(
			typeof(List<ItemList>),
			1,
			SerializeItemList,
			DeserializeItemList
		);

		ret = PhotonPeer.RegisterType(
			typeof(Info_InventorySize),
			2,
			SerializeInfo_InventorySize,
			DeserializeInfo_InventorySize
		);

		ret = PhotonPeer.RegisterType(
			typeof(MapObjectEntity),
			3,
			SerializeMapObjectEntity,
			DeserializeMapObjectEntity
			);
	}

	private static byte[] SerializeItemData(object customObject)
	{
		Debug.Log("SerializeItemData s");

		ItemList data = (ItemList)customObject;

		// int:4, float:4, bool:1, stringは長さ+文字列、Vector2: 8 Vector3:12
		// 今回は簡単化のため stringは固定長40バイトにするbyte[]
		// Vector2Int, int, float, string, bool
		const int ItemListSize = 8 + 4 + 4 + 1 * 2 + 4 * 6 + 40 * 2 * 2;

		// cout, ItemListSize * count
		byte[] bytes = new byte[ItemListSize];
		int offset = 0;

		Protocol.Serialize(data.GetGridIndex().x, bytes, ref offset);
		Protocol.Serialize(data.GetGridIndex().y, bytes, ref offset);
		Protocol.Serialize(data.m_id, bytes, ref offset);

		Protocol.Serialize(data.m_attack, bytes, ref offset);

		Protocol.Serialize((short)(data.IsEquip() ? 1 : 0), bytes, ref offset);

		// ===================MapObjectEntity===================//
		Protocol.Serialize(data.ItemData.id, bytes, ref offset);

		// stringは固定長40バイトでUTF-8エンコード
		byte[] nameBytes = new byte[40];
		byte[] tmp = System.Text.Encoding.UTF8.GetBytes(data.ItemData.objectName);
		System.Array.Copy(tmp, nameBytes, Mathf.Min(tmp.Length, 40));   // 文字数が40を超えた場合は切り捨て
		foreach (byte b in nameBytes)
		{
			Protocol.Serialize((short)b, bytes, ref offset);
		}

		Protocol.Serialize(data.ItemData.width, bytes, ref offset);
		Protocol.Serialize(data.ItemData.height, bytes, ref offset);
		Protocol.Serialize(data.ItemData.probability, bytes, ref offset);

		// stringは固定長40バイトでUTF-8エンコード
		nameBytes = new byte[40];
		tmp = System.Text.Encoding.UTF8.GetBytes(data.ItemData.displayName);
		System.Array.Copy(tmp, nameBytes, Mathf.Min(tmp.Length, 40));   // 文字数が40を超えた場合は切り捨て
		foreach (byte b in nameBytes)
		{
			Protocol.Serialize((short)b, bytes, ref offset);
		}
		Protocol.Serialize(data.ItemData.equipmentType, bytes, ref offset);
		Protocol.Serialize(data.ItemData.price, bytes, ref offset);

		string str = "";
		foreach (var b in bytes)
		{
			str += b + " ";
		}
		Debug.Log(str);

		Debug.Log("SerializeItemList e:" + bytes);

		return bytes;
	}

	private static byte[] SerializeItemList(object customObject)
	{
		Debug.Log("SerializeItemList s");

		List<ItemList> dataList = (List<ItemList>)customObject;

		// int:4, float:4, bool:1, stringは長さ+文字列、Vector2: 8 Vector3:12
		// 今回は簡単化のため stringは固定長40バイトにするbyte[]
		// Vector2Int, int, float, string, bool
		// boolとstringはstortにするから×2
		const int ItemListSize = 8 + 4 + 4 + 1 * 2 + 4 * 6 + 40 * 2 * 2;

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

			// ===================MapObjectEntity===================//
			Protocol.Serialize(data.ItemData.id, bytes, ref offset);

			// stringは固定長40バイトでUTF-8エンコード
			byte[] nameBytes = new byte[40];
			byte[] tmp = System.Text.Encoding.UTF8.GetBytes(data.ItemData.objectName);
			System.Array.Copy(tmp, nameBytes, Mathf.Min(tmp.Length, 40));   // 文字数が40を超えた場合は切り捨て
			foreach (byte b in nameBytes)
			{
				Protocol.Serialize((short)b, bytes, ref offset);
			}

			Protocol.Serialize(data.ItemData.width, bytes, ref offset);
			Protocol.Serialize(data.ItemData.height, bytes, ref offset);
			Protocol.Serialize(data.ItemData.probability, bytes, ref offset);

			// stringは固定長40バイトでUTF-8エンコード
			nameBytes = new byte[40];
			tmp = System.Text.Encoding.UTF8.GetBytes(data.ItemData.displayName);
			System.Array.Copy(tmp, nameBytes, Mathf.Min(tmp.Length, 40));   // 文字数が40を超えた場合は切り捨て
			foreach (byte b in nameBytes)
			{
				Protocol.Serialize((short)b, bytes, ref offset);
			}
			Protocol.Serialize(data.ItemData.equipmentType, bytes, ref offset);
			Protocol.Serialize(data.ItemData.price, bytes, ref offset);

			c++;
		}

		string str = "";
		foreach(var b in bytes)
		{
			str += b + " ";
		}
		Debug.Log(str);

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

		Debug.Log("SerializeInfo_InventorySize e");
		foreach(var b in bytes)
		{
			Debug.Log(b);
		}

		return bytes;
	}

	private static byte[] SerializeMapObjectEntity(object customObject)
	{
		Debug.Log("SerializeMapObjectEntity s");
		MapObjectEntity mapObject = (MapObjectEntity)customObject;

		byte[] bytes = new byte[4 * 6 + 40 * 3];
		int offset = 0;

		Protocol.Serialize(mapObject.id, bytes, ref offset);

		// stringは固定長40バイトでUTF-8エンコード
		byte[] nameBytes = new byte[40];
		byte[] tmp = System.Text.Encoding.UTF8.GetBytes(mapObject.objectName);
		System.Array.Copy(tmp, nameBytes, Mathf.Min(tmp.Length, 40));   // 文字数が40を超えた場合は切り捨て
		foreach (byte b in nameBytes)
		{
			Protocol.Serialize((short)b, bytes, ref offset);
		}

		Protocol.Serialize(mapObject.width, bytes, ref offset);
		Protocol.Serialize(mapObject.height, bytes, ref offset);
		Protocol.Serialize(mapObject.probability, bytes, ref offset);

		// stringは固定長40バイトでUTF-8エンコード
		nameBytes = new byte[40];
		tmp = System.Text.Encoding.UTF8.GetBytes(mapObject.displayName);
		System.Array.Copy(tmp, nameBytes, Mathf.Min(tmp.Length, 40));   // 文字数が40を超えた場合は切り捨て
		foreach (byte b in nameBytes)
		{
			Protocol.Serialize((short)b, bytes, ref offset);
		}
		Protocol.Serialize(mapObject.equipmentType, bytes, ref offset);
		Protocol.Serialize(mapObject.price, bytes, ref offset);

		Debug.Log("SerializeMapObjectEntity e");

		return bytes;
	}

	private static object DeserializeItemData(byte[] bytes)
	{
		Debug.Log("DeserializeItemData s");

		int offset = 0;

		string strBytes = "";
		foreach (var b in bytes)
		{
			strBytes += b + " ";
		}
		Debug.Log(strBytes);

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

		//=====================MapObjectEntity======================//
		data.ItemData = new MapObjectEntity();
		int id;
		Protocol.Deserialize(out id, bytes, ref offset);
		data.ItemData.id = id;

		byte[] nameBytes = new byte[40];
		for (int j = 0; j < 40; j++)
		{
			short str;
			Protocol.Deserialize(out str, bytes, ref offset);
			nameBytes[j] = (byte)str;
		}
		data.ItemData.objectName = (System.Text.Encoding.UTF8.GetString(nameBytes).TrimEnd('\0'));

		int w, h, probability;
		Protocol.Deserialize(out w, bytes, ref offset);
		Protocol.Deserialize(out h, bytes, ref offset);
		Protocol.Deserialize(out probability, bytes, ref offset);
		data.ItemData.width = w;
		data.ItemData.height = h;
		data.ItemData.probability = probability;

		nameBytes = new byte[40];
		for (int j = 0; j < 40; j++)
		{
			short str;
			Protocol.Deserialize(out str, bytes, ref offset);
			nameBytes[j] = (byte)str;
		}
		data.ItemData.displayName = (System.Text.Encoding.UTF8.GetString(nameBytes).TrimEnd('\0'));

		int equipmentType, price;
		Protocol.Deserialize(out equipmentType, bytes, ref offset);
		Protocol.Deserialize(out price, bytes, ref offset);
		data.ItemData.equipmentType = equipmentType;
		data.ItemData.price = price;

		Debug.Log("DeserializeItemData e:");

		return data;
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

			//=================MapObjectEntity==================//
			data.ItemData = new MapObjectEntity();
			int id;
			Protocol.Deserialize(out id, bytes, ref offset);
			data.ItemData.id = id;

			byte[] nameBytes = new byte[40];
			for (int j = 0; j < 40; j++)
			{
				short str;
				Protocol.Deserialize(out str, bytes, ref offset);
				nameBytes[j] = (byte)str;
			}
			data.ItemData.objectName = (System.Text.Encoding.UTF8.GetString(nameBytes).TrimEnd('\0'));

			int w, h, probability;
			Protocol.Deserialize(out w, bytes, ref offset);
			Protocol.Deserialize(out h, bytes, ref offset);
			Protocol.Deserialize(out probability, bytes, ref offset);
			data.ItemData.width = w;
			data.ItemData.height = h;
			data.ItemData.probability = probability;

			nameBytes = new byte[40];
			for (int j = 0; j < 40; j++)
			{
				short str;
				Protocol.Deserialize(out str, bytes, ref offset);
				nameBytes[j] = (byte)str;
			}
			data.ItemData.displayName = (System.Text.Encoding.UTF8.GetString(nameBytes).TrimEnd('\0'));

			int equipmentType, price;
			Protocol.Deserialize(out equipmentType, bytes, ref offset);
			Protocol.Deserialize(out price, bytes, ref offset);
			data.ItemData.equipmentType = equipmentType;
			data.ItemData.price = price;

			items.Add(data);

			c++;
		}

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

		Debug.Log("DeserializeInfo_InventorySize e:" + info);

		return info;
	}

	private static object DeserializeMapObjectEntity(byte[] bytes)
	{
		Debug.Log("DeserializeMapObjectEntity s");

		MapObjectEntity mapObject = new MapObjectEntity();
		int offset = 0;

		int id;
		Protocol.Deserialize(out id, bytes, ref offset);
		mapObject.id = id;

		byte[] nameBytes = new byte[40];
		for (int j = 0; j < 40; j++)
		{
			short str;
			Protocol.Deserialize(out str, bytes, ref offset);
			nameBytes[j] = (byte)str;
		}
		mapObject.objectName = (System.Text.Encoding.UTF8.GetString(nameBytes).TrimEnd('\0'));

		int w, h, probability;
		Protocol.Deserialize(out w, bytes, ref offset);
		Protocol.Deserialize(out h, bytes, ref offset);
		Protocol.Deserialize(out probability, bytes, ref offset);
		mapObject.width = w;
		mapObject.height = h;
		mapObject.probability = probability;

		nameBytes = new byte[40];
		for (int j = 0; j < 40; j++)
		{
			short str;
			Protocol.Deserialize(out str, bytes, ref offset);
			nameBytes[j] = (byte)str;
		}
		mapObject.displayName = (System.Text.Encoding.UTF8.GetString(nameBytes).TrimEnd('\0'));

		int equipmentType, price;
		Protocol.Deserialize(out equipmentType, bytes, ref offset);
		Protocol.Deserialize(out price, bytes, ref offset);
		mapObject.equipmentType = equipmentType;
		mapObject.price = price;

		Debug.Log("DeserializeMapObjectEntity e");

		return mapObject;
	}
}
