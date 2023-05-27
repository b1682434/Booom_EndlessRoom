using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 背包UI初始化组件
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class InventoryViewInitializer : MonoBehaviour
{
    [Tooltip("玩家Object")] public GameObject playerGameObject;
        
    [Tooltip("背包格子的UI控件")]
    public VisualTreeAsset gridAsset;
    
    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var inventoryViewController = new InventoryViewController();
        
        // 设置 inventoryView
        var inventory = playerGameObject.GetComponent<Inventory>();
        inventoryViewController.InitializeInventoryGrids(inventory, uiDocument.rootVisualElement, gridAsset);
        var inventoryView = uiDocument.rootVisualElement.Q<VisualElement>("InventoryViewRoot");
        inventoryView.userData = inventoryViewController;
    }
}
