using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryViewController
{
    /// <summary>
    /// 容纳Grid的ListView
    /// </summary>
    private ListView _gridListView;

    private int[] _intValues = { 1, 2, 3, 4 };

    /// <summary>
    /// 玩家的背包组件
    /// </summary>
    private Inventory _inventory;
    
    /// <summary>
    /// 初始化背包格子UI
    /// </summary>
    /// <param name="rootElement">InventoryView根节点</param>
    /// <param name="gridTemplate">背包格子控件</param>
    public void InitializeInventoryGrid(Inventory inventory, VisualElement rootElement, VisualTreeAsset gridTemplate)
    {
        _gridListView = rootElement.Q<ListView>();
        _inventory = inventory;

        _gridListView.makeItem = () =>
        {
            var newGrid = gridTemplate.Instantiate();
            var newGridLogic = new InventoryGridController();
            newGrid.userData = newGridLogic;
            return newGrid;
        };

        _gridListView.bindItem = (item, index) =>
        {
            
        };

        _gridListView.fixedItemHeight = 144.0f;
        _gridListView.itemsSource = _intValues;

        Debug.Log("InitializeInventoryGrid() called.");
        
    }
}
