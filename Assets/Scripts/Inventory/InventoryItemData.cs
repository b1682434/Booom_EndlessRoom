using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品数据。
/// </summary>
[CreateAssetMenu(menuName = "Booom/InventoryItem", order = 1)]
public class InventoryItemData : ScriptableObject
{
    [Tooltip("物品名称")]
    public string itemName;
    
    [Tooltip("物品2D图片，用于在背包栏中展示")]
    public Sprite itemIcon;

    [Tooltip("最大堆叠数量（暂不支持最大堆叠数量和可使用次数同时不为1）")]
    public int maxStack = 1;

    [Tooltip("是否能被捡起")]
    public bool canBePickUp = true;

    [Tooltip("可使用的次数")]
    public int durability = 1;

    [Tooltip("是否可恢复。若不可恢复，则不可以销毁。")]
    public bool recoverable = true;
    
    [Tooltip("是否是白板物品。通常不用修改。")]
    public bool isBlank = false;
}
