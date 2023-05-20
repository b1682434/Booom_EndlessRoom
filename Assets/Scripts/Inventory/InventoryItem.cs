using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.InputSystem;

/// <summary>
/// 背包物体组件。
/// - 可以放入背包的物体应该添加此组件
/// - 放入背包内的物品不会因为重置而消失
/// </summary>
[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Collider))]
public class InventoryItem : SelectableObject
{
    [Header("Inventory")]
    [Tooltip("物品数据配置")]
    public InventoryItemData itemData;

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

    public bool IsInteractionTarget
    {
        get => _isInteractionTarget;
        protected set => _isInteractionTarget = value;
    }

    /// <summary>
    /// 重置组件 TODO: 重置组件可能会有修改
    /// </summary>
    private RevertBase _revertBase;

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
    public void OnObtained(Inventory inventory)
    {
        _revertBase.SetRecoveryEnabled(false);
        
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
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
            
            gameObject.layer = LayerMask.NameToLayer("Inventory");
        }
    }

    /// <summary>
    /// 物品被使用
    /// </summary>
    /// <param name="forceDestroy">强制销毁</param>
    public void OnConsumed(bool forceDestroy = false)
    {
        if (_inventory == null)
        {
            return;
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
            // TODO: 销毁物品，通知Inventory
            Destroy(gameObject);
        }
    }

    /****** SelectableObject ******/

    /// <summary>
    /// 玩家与场景中的背包物品交互（通常是拾取）。
    /// </summary>
    /// <param name="ItemID">手持物品的ItemID，此处用不到</param>
    public override void InteractRequestImpl(int ItemID)
    {
        opened = true;
        
        base.InteractRequestImpl(ItemID);
    }

    public override void MouseOver()
    {
        if (!IsInteractionTarget && !IsInteractionTarget)
        {
            IsInteractionTarget = true;
        }
        
        base.MouseOver();
    }

    public override void MouseExit()
    {
        IsInteractionTarget = false;
        
        base.MouseExit();
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
        if (outline != null)
        {
            outline.enabled = false;
        }

        _durability = itemData.durability;
    }

    protected void Start()
    {
        base.Start();

        objName = itemData.itemName;
        ObjType = itemData.itemId;
        mouseOVerWord = cannotOpenWord = emptyHandWord = itemData.itemName;
        openWord = "拿走了";
        
        Restart();
    }

    // public void OnMouseOver()
    // {
    //     throw new NotImplementedException();
    // }
}