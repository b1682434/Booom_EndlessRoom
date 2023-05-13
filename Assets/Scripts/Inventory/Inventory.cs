using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 背包组件。
/// - 应该添加到玩家身上
/// </summary>
public class Inventory : MonoBehaviour
{
    [Tooltip("背包格子数")] public int capacity = 4;

    /// <summary>
    /// 背包物品
    /// </summary>
    private InventoryItem[] _inventoryItems;

    /// <summary>
    /// 选中的物品
    /// </summary>
    private InventoryItem _selectedItem;

    /// <summary>
    /// 物品探测器。
    /// - 在场景中的物品与此碰撞体重叠时，玩家可以与其交互。
    /// - 设想中应该是一个椎体。
    /// </summary>
    [Tooltip("物品探测体积。在场景中的物品与此碰撞体重叠时，玩家可以与其交互。")]
    [SerializeField]
    private MeshCollider _itemDetector;

    /// <summary>
    /// 当前可交互的物品。
    /// - 多个物品处于物品探测器中时，最后与_itemDetector接触的物品最优先发生交互。
    /// </summary>
    private List<InventoryItem> _interactableItems;

    /// <summary>
    /// 拾取物品
    /// </summary>
    /// <param name="sceneItem">场景中的物品</param>
    private void PickUpItem(InventoryItem sceneItem)
    {
        if (CanPickUpItem(sceneItem))
        {
            // TODO: 加入背包，进入检视界面
        }
    }

    /// <summary>
    /// 检视物品。
    /// </summary>
    /// <param name="ownedItem">持有的物品</param>
    private void InspectItem(InventoryItem ownedItem)
    {
        // 进入检视界面
    }

    /// <summary>
    /// 使用物品。
    /// - 使用、合成、销毁操作均使用这个方法。
    /// </summary>
    /// <param name="ownedItem">持有的物品</param>
    private void UseItem(InventoryItem ownedItem)
    {
    }
    
    /// <summary>
    /// 是否可以拾取物品
    /// </summary>
    private bool CanPickUpItem(InventoryItem item)
    {
        // TODO
        return true;
    }
    
    /// <summary>
    /// 当前选中的背包位置是否包含物品
    /// </summary>
    private bool IsSelectionBlank()
    {
        // TODO
        return true;
    }

    // TODO: 自定义碰撞体
    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<InventoryItem>();
        if (item == null)
        {
            return;
        }

        if (_interactableItems.Contains(item))
        {
            return;
        }

        if (!item.CanBeInteracted())
        {
            return;
        }

        // 使物品可交互
        _interactableItems.Insert(0, item);
        RefreshInteractableItems();
    }

    private void OnTriggerExit(Collider other)
    {
        var item = other.GetComponent<InventoryItem>();
        if (item == null)
        {
            return;
        }

        if (!_interactableItems.Contains(item))
        {
            return;
        }

        if (!item.CanBeInteracted())
        {
            return;
        }

        // 使物品不可交互
        _interactableItems.Remove(item);
        RefreshInteractableItems();
    }

    /// <summary>
    /// 刷新各个可交互物品的可交互状态，只让第一个物品可交互
    /// </summary>
    private void RefreshInteractableItems()
    {
        foreach (var interactableItem in _interactableItems)
        {
            interactableItem.BeNonInteractive();
        }

        if (_interactableItems.Count > 0)
        {
            _interactableItems[0].BeInteractive();
        }
    }

    void Start()
    {
        // TODO: 绑定_itemDetector的碰撞事件
    }

    void Update()
    {
        // 检测输入，调用相应的操作函数
    }
}