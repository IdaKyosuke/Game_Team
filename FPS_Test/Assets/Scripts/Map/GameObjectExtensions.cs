using UnityEngine;

public static class GameObjectExtensions
{
	/// <summary>
	/// 自分自身を含むすべての子オブジェクトのレイヤーを設定します
	/// </summary>
	public static void SetLayerRecursively(
		this Transform self,
		int layer
	)
	{
		self.gameObject.layer = layer;

		foreach (Transform n in self)
		{
			SetLayerRecursively(n, layer);
		}
	}
}
