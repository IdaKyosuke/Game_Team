using System.Numerics;

[System.Serializable]
public class MapObjectEntity
{
    public int id;                  // ID
    public string objectName;       // 画像
    public int width;               // 横幅
    public int height;              // 縦幅
    public int probability;         // アイテムそれぞれの抽選確立
    public string displayName;      // 表示名
	public int equipmentType;       // 装備アイテムタイプ
	public string typeName;         // 装備アイテムタイプ名
	public int price;				// 値段
}