using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
//using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryGridController
{
    /****** 物品 ******/

    /// <summary>
    /// 背包
    /// </summary>
    private Inventory _inventory;
    
    /// <summary>
    /// 背包物品的位置
    /// </summary>
    private int _itemIndex;

    /// <summary>
    /// 格子中存放的物品
    /// </summary>
    private InventoryItem Item
    {
        get => _inventory.GetInventoryItem(_itemIndex);
    }

    /****** UI状态 ******/
    private bool _selected = false;

    /// <summary>
    /// 是否被选中
    /// </summary>
    public bool Selected
    {
        get => _selected;
        protected set => _selected = value;
    }

    /****** UI元素 ******/
    private VisualElement _gridRoot;
    
    /// <summary>
    /// 物品数量标签
    /// </summary>
    private Label _stackNumberLabel;

    private VisualElement _stackNumberRootElement;

    /// <summary>
    /// 物品图标UI
    /// </summary>
    private VisualElement _itemIconElement;

    /// <summary>
    /// 选中框
    /// </summary>
    private VisualElement _selectionFrameElement;

    public void InitializeGridController(VisualElement gridRoot)
    {
        _gridRoot = gridRoot;
        _stackNumberRootElement = gridRoot.Q<VisualElement>("StackNumberRoot");
        _stackNumberLabel = gridRoot.Q<Label>("StackNumber");
        _itemIconElement = gridRoot.Q<VisualElement>("ItemIcon");
        _selectionFrameElement = gridRoot.Q<VisualElement>("SelectionFrame");
        
        gridRoot.RegisterCallback<MouseDownEvent>(MouseDownCallback);
    }

    private void MouseDownCallback(MouseDownEvent evt)
    {
        if (Selected)
        {
            return;
        }

        _inventory.AddInspectionItemToSparePos(Item);
    }

    /// <summary>
    /// 设置绑定的物品
    /// </summary>
    public void SetGridData(Inventory inventory, int itemIndex)
    {
        _inventory = inventory;
        _itemIndex = itemIndex;
        Refresh();
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    public void Refresh()
    {
        if (Item == null)
        {
            _itemIconElement.style.backgroundImage = new StyleBackground(StyleKeyword.None);
            _stackNumberRootElement.style.display = DisplayStyle.None;
        }
        else
        {
            _itemIconElement.style.backgroundImage = new StyleBackground(Item.itemData.itemIcon);
            _stackNumberRootElement.style.display = Item.Stack > 1 ? DisplayStyle.Flex : DisplayStyle.None;
            _stackNumberLabel.text = Item.Stack.ToString();
        }
    }

    /// <summary>
    /// 设置格子是否被选中
    /// </summary>
    public void SetSelected(bool newSelected)
    {
        Selected = newSelected;
        _selectionFrameElement.style.display = Selected ? DisplayStyle.Flex : DisplayStyle.None;
    }
}