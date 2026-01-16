using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItemPrefav
{
	public void GetPrefav(ItemList info, int index)
	{
		// ƒvƒŒƒnƒu‚ðŽæ“¾
		Loader.LoadGameObjectAsync(info.ItemData.objectName).Completed += op =>
		{
			GameObject obj = op.Result;

			obj.GetComponent<Item_Object>().ChangeIndex(index);
			obj.GetComponent<Item_Object>().ItemData = info.ItemData;

			info.SetActiveObject(obj);
		};
	}
}
