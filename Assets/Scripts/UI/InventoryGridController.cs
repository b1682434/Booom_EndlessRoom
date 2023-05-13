using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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

    /// <summary>
    /// 物品数量标签
    /// </summary>
    private Label _stackLabel;

    /// <summary>
    /// 物品图标UI element
    /// </summary>
    private VisualElement _itemIconElement;

    public void InitializeGridController(VisualElement gridRoot)
    {
        _stackLabel = gridRoot.Q<Label>("StackNumber");
        _itemIconElement = gridRoot.Q("ItemIcon");
        
        // _stackLabel.Bind();
    }

    public void SetGridData(InventoryItem item)
    {
    }
    
    public void SetGridData(InventoryItemData itemData)
    {
    }
}
