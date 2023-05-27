using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 背包物体组件。
/// - 可以放入背包的物体应该添加此组件
/// - 放入背包内的物品不会因为重置而消失
/// </summary>
[RequireComponent(typeof(Collider))]
public class InventoryItem : InteractableBase, IInteractRequest
{
    [Header("Inventory")] [Tooltip("物品数据配置")]
    public InventoryItemData itemData;

    [Header("Inspection Mode")] [Tooltip("物体中心点，用作检视模式的初始位置、旋转基准点")] [CanBeNull]
    public Transform pivotTransform;
    
    /// <summary>
    /// 物体合成表，目前仅支持一个物品仅有一种配方的合成。
    /// - 需要在物品的Prefab添加子GameObject，然后给子GameObject添加InventoryCraftingRecipe组件，并在组件中配置合成表参数。
    /// - 需要所有Recipe都满足条件才可成功合成。
    /// </summary>
    private List<InventoryCraftingRecipe> _craftingRecipes = new List<InventoryCraftingRecipe>();

    /****** 物品状态数据 ******/
    /// <summary>
    /// 物品数量
    /// </summary>
    private int _stack = 1;

    public int Stack
    {
        get => _stack;
    }

    /// <summary>
    /// 当前的可使用次数
    /// </summary>
    private int _durability;

    /// <summary>
    /// 拥有该物品的背包
    /// </summary>
    private Inventory _inventory;

    public bool IsInInventory
    {
        get => _inventory != null;
    }

    /// <summary>
    /// 是否是交互目标，需要在场景中
    /// </summary>
    private bool _isInteractionTarget = false;

    /// <summary>
    /// 是否是合成产物，合成产物的获取、重置逻辑与普通物品略有不同
    /// </summary>
    private bool _isProduct = false;

    public bool IsProduct
    {
        get => _isProduct;
        set => _isProduct = value;
    }

    public bool IsInteractionTarget
    {
        get => _isInteractionTarget;
        protected set => _isInteractionTarget = value;
    }

    /// <summary>
    /// 重置组件 TODO: 需要和重置系统交互
    /// </summary>
    private RevertBase _revertBase;


    /****** 物品操作方法 ******/
    /// <summary>
    /// 重置物品
    /// </summary>
    private void Restart()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 物品被获取
    /// </summary>
    public void ObtainedBy(Inventory inventory)
    {
        if (_revertBase != null) {
            _revertBase.SetRecoveryEnabled(false);
        }

        if (_inventory != null)
        {
            // 背包已经存在该物体，只增加数量
            _stack++;
        }
        else
        {
            _inventory = inventory;

            // Note: 将此gameobject放到一个玩家看不到的位置，并且禁用碰撞
            // 移动位置的逻辑在Inventory中
            var colliders = GetComponents<Collider>();
            foreach (var col in colliders)
            {
                col.enabled = false;
                col.isTrigger = true;
            }

            // TODO: 不知道为什么进入检视模式的物体总有白边
            // if (outline != null)
            // {
            //     Destroy(outline);
            // }

            gameObject.layer = LayerMask.NameToLayer("Inventory");
            var children = gameObject.GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Inventory");
            }
        }
    }

    /// <summary>
    /// 物品被使用
    /// </summary>
    /// <param name="forceDestroy">强制销毁</param>
    /// <returns>物品是否已损坏（销毁）</returns>
    public bool Consumed(bool forceDestroy = false)
    {
        if (_inventory == null)
        {
            return false;
        }

        if (forceDestroy)
        {
            _stack = 0;
        }
        else
        {
            _durability--;
            if (_durability <= 0)
            {
                _stack--;
                _durability = itemData.durability;
            }
        }

        if (_stack <= 0)
        { 
            DestroyImmediate(gameObject);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 物品进入检视模式
    /// </summary>
    public void EnterInspectionMode() {
        if (IsInInspectionMode) {
            return;
        }

        // 启用item的碰撞，使其可以被射线检测
        var colliders = GetComponents<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = true;
        }

        transform.localScale = inspectionScale;
    }

    /// <summary>
    /// 物品退出检视模式
    /// </summary>
    public void ExitInspectionMode() {
        if (!IsInInspectionMode) {
            return;
        }

        // 启用item的碰撞，使其可以被射线检测
        var colliders = GetComponents<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        transform.localScale = sceneScale;
    }

    /****** 检视模式相关 ******/

    /// <summary>
    /// 是否处于检视模式中
    /// </summary>
    private bool _isInInspectionMode = false;
    public bool IsInInspectionMode {
        get => _isInInspectionMode;
        protected set => _isInInspectionMode = value;
    }

    /// <summary>
    /// 在场景中的缩放
    /// </summary>
    [HideInInspector]
    public Vector3 sceneScale;

    /// <summary>
    /// 在检视模式中的缩放
    /// </summary>
    [HideInInspector]
    public Vector3 inspectionScale;

    /// <summary>
    /// 静态工具方法：检查两个物品能否被合成
    /// </summary>
    /// <returns>是否可以合成、合成位置</returns>
    public static bool CheckCrafting(InventoryItem itemA, InventoryItem itemB,
        [CanBeNull] out InventoryCraftingRecipe recipe)
    {
        if (CheckCraftingOneSided(itemA, itemB, out recipe))
        {
            return true;
        }

        if (CheckCraftingOneSided(itemB, itemA, out recipe))
        {
            return true;
        }

        recipe = null;
        return false;

        // 单向检查合成表的本地函数
        bool CheckCraftingOneSided(InventoryItem item1, InventoryItem item2,
            [CanBeNull] out InventoryCraftingRecipe outRecipe)
        {
            // 检查item2是否在item1的合成表中
            foreach (var recipe1 in item1._craftingRecipes)
            {
                
                
                if (recipe1.ingredient == item2.itemData)
                {
                    // 检查位置是否在合成位置附近
                    var boundSize = new Vector3();
                    boundSize.x = boundSize.y = boundSize.z = recipe1.tolerance * 2;
                    Bounds toleranceBounds = new Bounds(recipe1.transform.position, boundSize);
                    if (!toleranceBounds.Contains(item2.transform.position))
                    {
                        continue;
                    }

                    // 检查旋转是否在合成旋转附近
                    // 此处的 recipe1.tolerance * 300.0f 是一个大概的容忍范围，系数可以调整
                    if (Mathf.Abs(Quaternion.Angle(recipe1.transform.rotation, item2.transform.rotation)) <
                        recipe1.tolerance * 300.0f)
                    {
                        outRecipe = recipe1;
                        return true;
                    }
                }
            }

            outRecipe = null;
            return false;
        }
    }

    /// <summary>
    /// 静态工具方法：生成合成产物
    /// </summary>
    public static InventoryItem MakeProduct(GameObject prefabTemplate)
    {
        var productItem = Instantiate(prefabTemplate).GetComponent<InventoryItem>();
        if (productItem != null)
        {
            // 这个标志位暂时还没用到
            productItem.IsProduct = true;
        }

        return productItem;
    }
    
    public static InventoryItem MakeProduct(InventoryCraftingRecipe recipe)
    {
        return MakeProduct(recipe.product);
    } 

    /****** SelectableObject ******/

    /// <summary>
    /// 玩家与场景中的背包物品交互（通常是拾取）。
    /// </summary>
    /// <param name="ItemID">手持物品的ItemID</param>
    public void InteractRequest(int ItemID)
    {
    }

    public override void MouseOverImpl()
    {
        if (IsInInventory)
        {
            return;
        }

        if (!IsInteractionTarget)
        {
            IsInteractionTarget = true;
        }

        base.MouseOverImpl();
    }

    public override void MouseExitImpl()
    {
        IsInteractionTarget = false;

        base.MouseExitImpl();
    }

    protected void Awake()
    {
        _revertBase = GetComponent<RevertBase>();
        if (_revertBase != null)
        {
            if (itemData.recoverable)
            {
                _revertBase.onRecover += Restart;
            }
            else
            {
                _revertBase.SetRecoveryEnabled(false);
            }
        }

        outline = GetComponent<Outline>();

        _craftingRecipes.AddRange(GetComponentsInChildren<InventoryCraftingRecipe>());

        if (pivotTransform == null)
        {
            pivotTransform = transform;
        }

        _durability = itemData.durability;
        
        inspectionScale = pivotTransform.localScale;
        sceneScale = transform.localScale;
    }


    protected void Start()
    {
        objName = itemData.itemName;
        ObjType = itemData.itemId;
        // TODO: 可能需要自定义这些台词
        // mouseOVerWord = cannotOpenWord = emptyHandWord = itemData.itemName;
        openWord = "拿走了";

        Restart();
    }

    // private void Update()
    // {
    // }

    // public void OnMouseOver()
    // {
    //     throw new NotImplementedException();
    // }
}
