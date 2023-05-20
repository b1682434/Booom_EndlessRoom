using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("功能组件")] [Tooltip("检视相机，需要能渲染Inventory层")] [CanBeNull]
    public Camera inspectionCamera;

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
                onSelectionChanged?.Invoke(_selectedItemIndex);
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
            if (SelectedItem == null)
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

    private readonly Vector3 INVENTORY_POS = Vector3.back;
    private readonly Vector3 INSPECTION_POS = new Vector3(0.0f, -0.15f, 1.0f);

    /// <summary>
    /// PlayerInput，用来切换操作映射
    /// </summary>
    private PlayerInput _playerInput;

    private StarterAssetsInputs _input;

    private readonly string INSPECTION_MODE_ACTION_MAP = "InspectionMode";
    private readonly string PLAYER_ACTION_MAP = "Player";

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

        // 背包有同类物品时不进入检视模式
        var enterInspectionMode = !_inventoryItems.Exists((item) =>
        {
            return item != null && item.itemData == sceneItem.itemData;
        });

        // 放入背包物品列表
        var index = FindSuitableGridForNewItem(sceneItem);
        var itemObtained = _inventoryItems[index];
        if (itemObtained == null)
        {
            var ownedItem = Instantiate(sceneItem, transform);
            _inventoryItems[index] = ownedItem;
            itemObtained = ownedItem;

            ownedItem.transform.localPosition = INVENTORY_POS;
        }

        itemObtained.OnObtained(this);

        // TODO: sceneItem 变成可重置状态
        sceneItem.gameObject.SetActive(false);

        SelectedItemIndex = index;
        onItemInfoUpdate?.Invoke(index);

        // 进入检视界面
        if (enterInspectionMode)
        {
            InspectItem(SelectedItem);
        }
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

        onItemInfoUpdate?.Invoke(SelectedItemIndex);
        if (indexToUse != SelectedItemIndex)
        {
            onItemInfoUpdate?.Invoke(indexToUse);
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
    /// 检视模式中的物品
    /// </summary>
    private List<InventoryItem> _inspectionItems = new List<InventoryItem>();

    /// <summary>
    /// 检视模式中正在操作的对象
    /// </summary>
    [CanBeNull] private InventoryItem _operationTarget;

    /// <summary>
    /// 检视模式下物品所在的平面、上方向、右方向
    /// </summary>
    private Plane _inspectionPlane;

    private Vector3 _inspectionRotationUpAxis;
    private Vector3 _inspectionRotationRightAxis;

    private Vector3 _tempRelativePos;

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
            inspectionCamera.gameObject.SetActive(false);

            // 切换Player输入
            _playerInput.SwitchCurrentActionMap(PLAYER_ACTION_MAP);
            _input.SetCursorState(true);

            while (_inspectionItems.Count > 0)
            {
                RemoveInspectionItem(_inspectionItems[0]);
            }
        }
        else
        {
            // 进入检视模式
            if (itemToInspect == null || !_inventoryItems.Contains(itemToInspect))
            {
                // 场景1：检视空位。设想：检视失败提示音
                return;
            }

            if (inspectionCamera == null)
            {
                return;
            }

            // 计算放置检视物品的平面、上方向、右方向
            // Note: 检视平面的位置是固定了，但可以在代码调整，以达到比较好的效果
            var inspectionCameraTransform = inspectionCamera.transform;
            _inspectionPlane = new Plane(-inspectionCameraTransform.forward,
                inspectionCameraTransform.position + inspectionCameraTransform.forward * 1.0f);
            _inspectionRotationUpAxis = inspectionCameraTransform.up;
            _inspectionRotationRightAxis = inspectionCameraTransform.right;

            AddInspectionItem(SelectedItem);

            // 切换检视模式输入
            _playerInput.SwitchCurrentActionMap(INSPECTION_MODE_ACTION_MAP);
            _input.SetCursorState(false);

            InspectionMode = true;
            inspectionCamera.enabled = true;
            inspectionCamera.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 添加检视物品
    /// </summary>
    public void AddInspectionItem(InventoryItem itemToAdd)
    {
        if (itemToAdd == null || _inspectionItems.Contains(itemToAdd))
        {
            return;
        }

        // 启用item的碰撞，使其可以被射线检测
        var colliders = itemToAdd.GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }

        // 放到inspectionCamera面前
        var itemTransform = itemToAdd.transform;
        itemTransform.parent = inspectionCamera.transform;
        var pivotPos = GetPosFromInspectionPlane(new Vector2(Screen.width / 2 , Screen.height / 2));
        itemTransform.position = pivotPos + itemTransform.position - itemToAdd.pivotTransform.position;
        itemTransform.rotation = Quaternion.identity;
        _inspectionItems.Add(itemToAdd);
    }

    /// <summary>
    /// 移除检视物品
    /// </summary>
    public void RemoveInspectionItem(InventoryItem itemToRemove)
    {
        if (itemToRemove == null || !_inspectionItems.Contains(itemToRemove))
        {
            return;
        }

        // 关闭item的碰撞
        var colliders = itemToRemove.GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        // 放回原位
        var itemTransform = itemToRemove.transform;
        itemTransform.parent = transform;
        itemTransform.localPosition = INVENTORY_POS;
        _inspectionItems.Remove(itemToRemove);
    }

    /// <summary>
    /// 检测操作对象，鼠标按下时点击的物体为操作物体
    /// </summary>
    private void DetectOperationTargat()
    {
        if (_operationTarget != null || inspectionCamera == null)
        {
            return;
        }

        // 射线检测操作对象
        RaycastHit hit;
        Ray ray = inspectionCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out hit, 10.0f, 1 << LayerMask.NameToLayer("Inventory"),
                QueryTriggerInteraction.Collide))
        {
            _operationTarget = hit.transform.GetComponent<InventoryItem>();
        }

        if (_operationTarget != null)
        {
            float distance;
            _inspectionPlane.Raycast(ray, out distance);
            _tempRelativePos = _operationTarget.transform.position - ray.GetPoint(distance);
        }
    }

    /// <summary>
    /// 清空操作对象
    /// </summary>
    private void ResetOperationTarget()
    {
        if (_operationTarget != null)
        {
            CheckComposite(_operationTarget);
        }

        _operationTarget = null;
    }

    /// <summary>
    /// 检测检视模式中的物体是否满足合成条件
    /// </summary>
    private void CheckComposite(InventoryItem itemToCheck)
    {
        if (itemToCheck == null)
        {
            return;
        }

        // TODO: 检查合成表
        Debug.Log("检查合成表");
    }

    /****** 内部工具方法 ******/

    /// <summary>
    /// 有东西被交互时就看看是不是背包物品，是的话就捡起来
    /// - 绑定在FirstPersonController的onInteraction上
    /// </summary>
    /// <param name="inter">被交互物</param>
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
        if (item == null)
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
        if (stackItem == null)
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
            if (itemInList == null || itemInList.itemData != newItem.itemData)
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
        var nullIndex = _inventoryItems.FindIndex((itemInList) => itemInList == null);

        return nullIndex;
    }

    /// <summary>
    /// 找出屏幕坐标投影到检视平面上的点的世界坐标
    /// </summary>
    private Vector3 GetPosFromInspectionPlane(Vector2 screenPos)
    {
        Ray ray = inspectionCamera.ScreenPointToRay(screenPos);
        float distance;
        _inspectionPlane.Raycast(ray, out distance);
        return ray.GetPoint(distance);
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
        _input = GetComponent<StarterAssetsInputs>();
        if (_input != null)
        {
            _input.OnScrollUpStart += SelectPrevItem;
            _input.OnScrollDownStart += SelectNextItem;
        }

        _playerInput = GetComponent<PlayerInput>();

        // 检视模式相关变量获取
        if (inspectionCamera == null)
        {
            // 根据名称自动设置inspectionCamera
            var cameras = gameObject.GetComponentsInChildren<Camera>(true);

            const string defaultInspectionCameraName = "InspectionCamera";
            foreach (var cameraComponent in cameras)
            {
                if (cameraComponent.gameObject.name == defaultInspectionCameraName)
                {
                    inspectionCamera = cameraComponent;
                    break;
                }
            }
        }

        // 检视模式相关输入事件绑定
        if (_input != null)
        {
            _input.OnEnterInspectionModeStart += InspectSelectedItem;
            _input.OnExitInspectionModeStart += InspectSelectedItem;
            _input.OnMoveInspectionItemStart += DetectOperationTargat;
            _input.OnRotateInspectionItemStart += DetectOperationTargat;
            _input.OnMoveInspectionItemEnd += ResetOperationTarget;
            _input.OnRotateInspectionItemEnd += ResetOperationTarget;
        }

        // 交互
        var fpc = GetComponent<FirstPersonController>();
        if (fpc != null)
        {
            fpc.onInteraction += OnInteraction;
        }
    }

    void Update()
    {
        // Debug.Log(Mouse.current.position.ReadValue());

        // 检视模式下的输入处理
        if (InspectionMode)
        {
            // 移动和旋转操作目标
            if (_operationTarget != null)
            {
                var moveItem = _input.moveInspectionItem;
                if (moveItem)
                {
                    var mousePos = GetPosFromInspectionPlane(Mouse.current.position.ReadValue());
                    _operationTarget.transform.position = mousePos + _tempRelativePos;
                }

                var rotateDelta = _input.rotateDeltaInInspectionMode;
                if (rotateDelta != Vector2.zero)
                {
                    var trans = _operationTarget.transform;
                    var pivotTrans = _operationTarget.pivotTransform;
                    trans.RotateAround(pivotTrans.position, _inspectionRotationUpAxis, rotateDelta.x * -0.2f);
                    trans.RotateAround(pivotTrans.position, _inspectionRotationRightAxis, rotateDelta.y * 0.2f);
                }
            }
        }
    }
}