using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Info InventorySize")]
public class Info_InventorySize : ScriptableObject
{
    public enum InventoryType
    {
        Inventory,  // プレイヤーが持っている（装備枠 + アイテム欄）
        Stash,      // アイテム欄のみ

        Length,
    }

    [SerializeField] InventoryType inventoryType;
    [SerializeField] int height;
    [SerializeField] int width;

    public Vector2Int GetSize => new Vector2Int(height, width);

    public InventoryType GetInventoryType => inventoryType;

    //[Serializable]
    //public class InventorySizeDictionary
    //{ 
    //    InventoryType inventoryType;
    //    int height;
    //    int width;

    //    public Vector2Int GetSize => new Vector2Int(height, width);

    //    public InventoryType GetInventoryType => inventoryType;
    //}

    //[SerializeField] List<InventorySizeDictionary> inventorySizeList;
}
