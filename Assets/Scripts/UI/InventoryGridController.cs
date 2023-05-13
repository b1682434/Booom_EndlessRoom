using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGridController
{
    /// <summary>
    /// 物品名称
    /// </summary>
    private string _itemName;
    
    /// <summary>
    /// 物品图标
    /// </summary>
    private Sprite _itemIcon;
    
    /// <summary>
    /// 当前物品数量
    /// </summary>
    private int _stack;

    /// <summary>
    /// 格子中存放的物品
    /// </summary>
    private InventoryItem _item;

    public void SetGridData(InventoryItem item)
    {
    }
    
    public void SetGridData(InventoryItemData itemData)
    {
    }
}
