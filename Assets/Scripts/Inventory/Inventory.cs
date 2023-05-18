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

    [Header("功能组件")] [Tooltip("检视相机，需要能渲染Inventory层")] public Camera inspectionCamera;

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

    public InventoryItem SelectedItem => _inventoryItems[_selectedItemIndex];

    /// <summary>
    /// 当前选中物品的id（将来服务于IInteractRequest接口）
    /// </summary>
    public int SelectedItemId
    {
        get
        {
            if (SelectedItem is null)
            {
                // TODO: 暂时用0作为空位id
                return 0;
            }

            return SelectedItem.itemData.itemId;
        }
    }

    private bool _inspectionMode;

    /// <summary>
    /// 是否处于检视模式
    /// </summary>
    public bool InspectionMode
    {
        get => _inspectionMode;
        protected set => _inspectionMode = value;
    }
    
    private readonly Vector3 _inventoryPos = Vector3.back;
    private readonly Vector3 _inspectionPos = Vector3.forward * 0.5f;

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

            ownedItem.transform.localPosition = _inventoryPos;
        }

        itemObtained.OnObtained(this);

        // TODO: sceneItem 变成可重置状态
        sceneItem.gameObject.SetActive(false);

        SelectedItemIndex = index;
        onItemInfoUpdate(index);

        // TODO: 进入检视界面
        InspectItem(SelectedItem);
    }

    /// <summary>
    /// 检视当前选中物品。
    /// </summary>
    public void InspectSelectedItem()
    {
        InspectItem(SelectedItem);
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
    
    /****** 检视模式 ******/
    
    /// <summary>
    /// 检视中的物品
    /// </summary>
    private List<InventoryItem> _inspectionItems = new List<InventoryItem>();
    
    /// <summary>
    /// 进入/退出检视模式
    /// </summary>
    /// <param name="itemToInspect">要检视的物品</param>
    public void InspectItem([CanBeNull] InventoryItem itemToInspect)
    {
        if (InspectionMode)
        {
            // 退出检视模式
            InspectionMode = false;
            inspectionCamera.enabled = false;
            // TODO: 恢复视角转动和移动输入
            // TODO: 隐藏、锁定鼠标

            while (_inspectionItems.Count > 0)
            {
                RemoveInspectionItem(_inspectionItems[0]);
            }
        }
        else
        {
            // 进入检视模式
            if (itemToInspect is null || !_inventoryItems.Contains(itemToInspect))
            {
                // 场景1：检视空位。设想：检视失败提示音
                return;
            }

            if (inspectionCamera is null)
            {
                return;
            }

            AddInspectionItem(SelectedItem);

            // TODO: 禁用视角转动和移动输入
            // TODO: 显示、解锁鼠标
            InspectionMode = true;
            inspectionCamera.enabled = true;
        }
    }

    /// <summary>
    /// 添加检视物品
    /// </summary>
    public void AddInspectionItem(InventoryItem itemToAdd)
    {
        if (itemToAdd is null || _inspectionItems.Contains(itemToAdd))
        {
            return;
        }

        var itemTransform = itemToAdd.transform;
        itemTransform.parent = inspectionCamera.transform;
        itemTransform.localPosition = _inspectionPos;
        _inspectionItems.Add(itemToAdd);
    }

    /// <summary>
    /// 移除检视物品
    /// </summary>
    public void RemoveInspectionItem(InventoryItem itemToRemove)
    {
        if (itemToRemove is null || !_inspectionItems.Contains(itemToRemove))
        {
            return;
        }

        var itemTransform = itemToRemove.transform;
        itemTransform.parent = transform;
        itemTransform.localPosition = _inventoryPos;
        _inspectionItems.Remove(itemToRemove);
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