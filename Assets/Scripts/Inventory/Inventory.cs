using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// 物品数据变化时触发，index为变化的物品的位置。
/// </summary>
public delegate void OnInventoryItemUpdate(int index);

/// <summary>
/// 背包组件。
/// - 应该添加到玩家身上
/// </summary>
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(StarterAssetsInputs))]
[RequireComponent(typeof(FirstPersonController))] // 需要它的交互功能
public class Inventory : MonoBehaviour
{
    [Header("背包属性")] [Tooltip("背包格子数")] public int capacity = 4;

    /****** 背包状态 ******/

    /// <summary>
    /// 物品事件。
    /// </summary>
    public OnInventoryItemUpdate onItemInfoUpdate;

    public OnInventoryItemUpdate onInspectItemFailed;

    public OnInventoryItemUpdate onSelectionChanged;

    /// <summary>
    /// 背包物品，列表长度由capacity确定。
    /// </summary>
    [ItemCanBeNull] private List<InventoryItem> _inventoryItems = new List<InventoryItem>();

    /// <summary>
    /// 获取特定位置的背包物品
    /// </summary>
    public InventoryItem GetInventoryItem(int index)
    {
        if (index < 0)
        {
            return null;
        }

        return index < _inventoryItems.Count ? _inventoryItems[index] : null;
    }

    
    private int _selectedItemIndex = 0;

    /// <summary>
    /// 选中的物品index
    /// - 设置该值会触发onSelectionChanged
    /// </summary>
    public int SelectedItemIndex
    {
        get => _selectedItemIndex;
        protected set
        {
            var oldValue = _selectedItemIndex;
            _selectedItemIndex = Math.Clamp(value, 0, _inventoryItems.Count - 1);
            if (oldValue != _selectedItemIndex)
            {
                onSelectionChanged(_selectedItemIndex);
            }
        }
    }

    /// <summary>
    /// 当前选中物品的id（将来服务于IInteractRequest）
    /// </summary>
    public int SelectedItemId
    {
        get
        {
            if (_inventoryItems[SelectedItemIndex] is null)
            {
                // TODO: 暂时用0作为空位id
                return 0;
            }

            return _inventoryItems[SelectedItemIndex].itemData.itemId;
        }
    }

    /****** 可用的物品操作 ******/

    /// <summary>
    /// 拾取物品
    /// </summary>
    /// <param name="sceneItem">场景中的物品</param>
    public void PickUpItem(InventoryItem sceneItem)
    {
        if (!CanPickUpItem(sceneItem))
        {
            return;
        }

        // 放入背包物品列表
        var index = FindSuitableGridForNewItem(sceneItem);
        var itemObtained = _inventoryItems[index];
        if (itemObtained is null)
        {
            var ownedItem = Instantiate(sceneItem, transform);
            _inventoryItems[index] = ownedItem;
            itemObtained = ownedItem;

            ownedItem.transform.localPosition = Vector3.back * 1.0f;
        }

        itemObtained.OnObtained(this);

        // TODO: sceneItem 变成可重置状态
        sceneItem.gameObject.SetActive(false);

        SelectedItemIndex = index;
        onItemInfoUpdate(index);
        
        // TODO: 进入检视界面
        InspectItem(_inventoryItems[index]);
    }

    /// <summary>
    /// 检视物品。
    /// </summary>
    /// <param name="ownedItem">持有的物品</param>
    public void InspectItem([CanBeNull] InventoryItem ownedItem)
    {
        if (ownedItem is null)
        {
            // 场景1：检视空位。设想：检视失败提示音
            return;
        }

        // TODO: 进入检视界面
    }

    /// <summary>
    /// 检视当前选中物品。
    /// </summary>
    public void InspectSelectedItem()
    {
        InspectItem(_inventoryItems[SelectedItemIndex]);
    }

    /// <summary>
    /// 使用物品。
    /// - 使用、合成、销毁操作均使用这个方法。
    /// </summary>
    /// <param name="ownedItem">持有的物品</param>
    public void UseItem(InventoryItem ownedItem)
    {
        int indexToUse = _inventoryItems.IndexOf(ownedItem);

        // TODO: 使用逻辑

        onItemInfoUpdate(SelectedItemIndex);
        if (indexToUse != SelectedItemIndex)
        {
            onItemInfoUpdate(indexToUse);
        }
    }

    /// <summary>
    /// 切换到上一个物品
    /// </summary>
    public void SelectPrevItem()
    {
        SelectedItemIndex--;
    }

    /// <summary>
    /// 切换到下一个物品
    /// </summary>
    public void SelectNextItem()
    {
        SelectedItemIndex++;
    }

    /****** 内部工具方法 ******/

    /// <summary>
    /// 有东西被交互时就看看是不是背包物品，是的话就捡起来
    /// - 绑定在FirstPersonController的onInteraction上
    /// </summary>
    /// <param name="inter">交互物</param>
    private void OnInteraction(IInteractRequest inter)
    {
        if (inter is InventoryItem item)
        {
            PickUpItem(item);
        }
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

        // 判断物品本身是否可堆叠
        if (item.itemData.maxStack <= 1)
        {
            return false;
        }

        // 找到第一个将item堆叠上去的物品
        var stackItem = _inventoryItems.Find((itemInList) =>
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
    public bool IsSelectionBlank()
    {
        return _inventoryItems[SelectedItemIndex] is null;
    }

    /****** MonoBehaviour ******/

    void Start()
    {
        // 初始化背包物品列表，null表示空位
        for (int i = 0; i < capacity; ++i)
        {
            _inventoryItems.Add(null);
        }
        
        // 绑定玩家输入事件
        var input = GetComponent<StarterAssetsInputs>();
        if (input is not null)
        {
            input.OnScrollUpStart += SelectPrevItem;
            input.OnScrollDownStart += SelectNextItem;
            input.OnInspectItemStart += InspectSelectedItem;
        }

        // 绑定交互事件
        var fpc = GetComponent<FirstPersonController>();
        if (fpc is not null)
        {
            fpc.onInteraction += OnInteraction;
        }
    }

    void Update()
    {
    }
}