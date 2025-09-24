using System.Numerics;

[System.Serializable]
public class ObjectEntity
{
	public int id;                  // ID
	public string objectName;       // 画像
	public int width;				// 横幅
	public int height;              // 縦幅
	public int rare;                // レアリティの数値(enam参照用)
	public string rareName;			// レアリティの表示名
	public string displayName;		// 表示名
}
