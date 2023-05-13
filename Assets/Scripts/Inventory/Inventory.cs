using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 背包组件。
/// - 应该添加到玩家身上
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class Inventory : MonoBehaviour
{
    public GameObject debugObject;

    [Tooltip("背包格子数")] public int capacity = 4;

    /// <summary>
    /// 背包物品，列表长度由capacity确定。
    /// </summary>
    [ItemCanBeNull] private List<InventoryItem> _inventoryItems = new List<InventoryItem>();

    /// <summary>
    /// 选中的物品index
    /// </summary>
    private int _selectedItemIndex = 0;

    /// <summary>
    /// 物品探测器。
    /// - 在场景中的物品与此碰撞体重叠时，玩家可以与其交互。
    /// - 设想中应该是一个椎体。
    /// </summary>
    [Tooltip("物品探测体积。在场景中的物品与此碰撞体重叠时，玩家可以与其交互。")] [SerializeField]
    private CollisionTrigger _itemDetector;

    /// <summary>
    /// 当前可交互的物品。
    /// - 多个物品处于物品探测器中时，最后与_itemDetector接触的物品最优先发生交互。
    /// </summary>
    private readonly List<InventoryItem> _interactableItems = new List<InventoryItem>();

    private StarterAssetsInputs _input;

    /// <summary>
    /// 拾取物品
    /// </summary>
    /// <param name="sceneItem">场景中的物品</param>
    private void PickUpItem(InventoryItem sceneItem)
    {
        if (!CanPickUpItem(sceneItem))
        {
            return;
        }

        // 更新可交互物品列表
        sceneItem.BeNotInteractionTarget();
        _interactableItems.Remove(sceneItem);
        RefreshInteractableItems();

        // 放入背包物品列表
        var index = FindSuitableGridForNewItem(sceneItem);
        if (_inventoryItems[index] is null)
        {
            // TODO
            _inventoryItems[index] = sceneItem;
            sceneItem.EnterInventory();
        }
        else
        {
            _inventoryItems[index].ObtainItem();
            Destroy(sceneItem.gameObject);
        }
        // TODO: 进入检视界面
        _selectedItemIndex = index;
        InspectItem(sceneItem);
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
        if (item is null)
        {
            return false;
        }

        // 判断item自身是否可交互
        if (item.IsInInventory || !item.itemData.canBePickUp || !item.IsInteractionTarget)
        {
            return false;
        }

        // 背包是否还有空位
        if (_inventoryItems.Contains(null))
        {
            return true;
        }

        // 判断是否可以以堆叠形式进入背包
        if (item.itemData.maxStack <= 1)
        {
            return false;
        }

        // 找到第一个可以堆叠的物品
        var stackItem = _inventoryItems.Find((InventoryItem itemInList) =>
        {
            // TODO: 这个比较没确定会不会有问题
            if (itemInList.itemData != item.itemData)
            {
                return false;
            }

            return itemInList.Stack < itemInList.itemData.maxStack;
        });
        if (stackItem is null)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 给新进入背包的物品找合适的位置。
    /// - 优先寻找可堆叠的位置，其次是空位。
    /// </summary>
    /// <param name="newItem">新来的物品</param>
    /// <returns>合适位置的index，没找到时返回-1</returns>
    private int FindSuitableGridForNewItem(InventoryItem newItem)
    {
        // 找到第一个可以堆叠的物品索引
        var stackIndex = _inventoryItems.FindIndex((itemInList) =>
        {
            // TODO: 这个比较没确定会不会有问题
            if (itemInList is null || itemInList.itemData != newItem.itemData)
            {
                return false;
            }

            return itemInList.Stack < itemInList.itemData.maxStack;
        });

        if (stackIndex >= 0)
        {
            return stackIndex;
        }

        // 找到第一个空位索引
        var nullIndex = _inventoryItems.FindIndex((itemInList) => itemInList is null);

        return nullIndex;
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

        if (item.IsInInventory)
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

        if (_interactableItems.Contains(item) == false)
        {
            return;
        }

        if (item.IsInInventory)
        {
            return;
        }

        // 使物品不可交互
        item.BeNotInteractionTarget();
        _interactableItems.Remove(item);
        RefreshInteractableItems();
    }

    /// <summary>
    /// 刷新各个可交互物品的可交互状态，只让第一个物品可交互
    /// </summary>
    private void RefreshInteractableItems()
    {
        if (_interactableItems.Count > 1)
        {
            for (int i = 1; i < _interactableItems.Count; ++i)
            {
                if (_interactableItems[i].IsInteractionTarget)
                {
                    _interactableItems[i].BeNotInteractionTarget();
                }
            }
        }

        if (_interactableItems.Count > 0 && !_interactableItems[0].IsInteractionTarget)
        {
            _interactableItems[0].BeInteractionTarget();
        }
    }

    void Start()
    {
        // TODO: 绑定_itemDetector的碰撞事件
        for (int i = 0; i < capacity; ++i)
        {
            _inventoryItems.Add(null);
        }

        _input = GetComponent<StarterAssetsInputs>();
        if (_input is not null)
        {
            _input.OnMouseScrollUpStart += SelectPrevItem;
            _input.OnMouseScrollDownStart += SelectNextItem;
            // _input.OnInspectItemStart += // 进入检视界面
        }

        if (_itemDetector is not null)
        {
            Debug.Log("Start() called");
            _itemDetector.OnTriggerEnterThis += OnTriggerEnter;
            _itemDetector.OnTriggerExitThis += OnTriggerExit;
        }
    }

    void Update()
    {
        if (debugObject is not null)
        {
            // Debug.Log(debugObject.transform.position);
        }
    }

    private void SelectPrevItem()
    {
        Debug.Log("SelectPrevItem() called");
    }

    private void SelectNextItem()
    {
        Debug.Log("SelectNextItem() called");
    }
}