using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryViewController
{
    /// <summary>
    /// 容纳Grid的ListView
    /// </summary>
    private ScrollView _gridScrollView;

    private List<VisualElement> _grids = new List<VisualElement>();

    /// <summary>
    /// 玩家的背包组件
    /// </summary>
    private Inventory _inventory;

    /// <summary>
    /// 初始化背包格子UI
    /// </summary>
    /// <param name="rootElement">InventoryView根节点</param>
    /// <param name="gridTemplate">背包格子控件</param>
    public void InitializeInventoryGrids(Inventory inventory, VisualElement rootElement, VisualTreeAsset gridTemplate)
    {
        _gridScrollView = rootElement.Q<ScrollView>();
        _inventory = inventory;

        for (int i = 0; i < inventory.capacity; ++i)
        {
            var newGrid = gridTemplate.Instantiate();
            var newGridLogic = new InventoryGridController();
            newGridLogic.InitializeGridController(newGrid);
            newGridLogic.SetGridData(inventory, i);
            newGrid.userData = newGridLogic;
            _gridScrollView.Add(newGrid);
        }

        // 绑定更新
        _inventory.onItemInfoUpdate += RefreshGrid;
        _inventory.onSelectionChanged += SelectGrid;
        
        // TODO: 合成成功可以适当加点提示
        // _inventory.onCraftingSucceeded += (productIndex) => RefreshAllGrids();

        SelectGrid(0);
    }

    /// <summary>
    /// 刷新特定格子
    /// </summary>
    /// <param name="gridIndex"></param>
    public void RefreshGrid(int gridIndex)
    {
        if (_gridScrollView.ElementAt(gridIndex).userData is InventoryGridController gridController)
        {
            gridController.SetGridData(_inventory, gridIndex);
        }
    }

    /// <summary>
    /// 刷新所有格子
    /// </summary>
    public void RefreshAllGrids()
    {
        foreach (var grid in _gridScrollView.Children())
        {
            if (grid.userData is InventoryGridController gridController)
            {
                gridController.Refresh();
            }
        }
    }

    /// <summary>
    /// 选择某个格子
    /// </summary>
    public void SelectGrid(int gridIndex)
    {
        for (int i = 0; i < _gridScrollView.childCount; ++i)
        {
            if (_gridScrollView.ElementAt(i).userData is InventoryGridController gridController)
            {
                gridController.SetSelected(i == gridIndex);
            }
        }
    }
}