using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品数据。
/// </summary>
[CreateAssetMenu(menuName = "Booom/InventoryItem", order = 1)]
public class InventoryItemData : ScriptableObject, IEquatable<InventoryItemData>
{
    [Header("物品信息")]
    [Tooltip("物品id，0为无效物品")] public int itemId;
    [Tooltip("物品名称")] public string itemName;
    [Tooltip("物品2D图片，用于在背包栏中展示")] public Sprite itemIcon;
    [Tooltip("物品描述")] public string itemDescription;
    
    [Header("物品属性")]
    [Tooltip("最大堆叠数量")] public int maxStack = 1;
    [Tooltip("是否能被捡起")] public bool canBePickUp = true;
    [Tooltip("可使用的次数")] public int durability = 1;
    [Tooltip("是否可恢复。若不可恢复，则不可以销毁。")] public bool recoverable = true;

    public virtual bool Equals(InventoryItemData other)
    {
        return other != null && itemId == other.itemId;
    }
}