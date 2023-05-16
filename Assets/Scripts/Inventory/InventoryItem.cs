using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包物体组件。
/// - 可以放入背包的物体应该添加此组件
/// - 放入背包内的物品不会因为重置而消失
/// </summary>
[RequireComponent(typeof(Outline))]
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
    /// 是否在背包内
    /// </summary>
    private bool _isInInventory = false;

    public bool IsInInventory
    {
        get => _isInInventory;
    }

    /// <summary>
    /// 是否是交互目标，需要在场景中
    /// </summary>
    private bool _isInteractionTarget = false;

    public bool IsInteractionTarget
    {
        get => _isInteractionTarget;
    }

    /// <summary>
    /// 重置组件 TODO
    /// </summary>
    private RevertBase _revertBase;

    /// <summary>
    /// 重置物品
    /// </summary>
    private void Restart()
    {
    }

    /// <summary>
    /// 获取物品
    /// </summary>
    public void OnObtained()
    {
        _revertBase.SetRecoveryEnabled(false);
        
        if (_isInInventory)
        {
            _stack++;
        }
        else
        {
            _isInInventory = true;
            // Note: 将此gameobject放到一个玩家看不到的位置，并且禁用碰撞
            // 移动位置的逻辑在Inventory中
            var colliders = GetComponents<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
            // Note: 不销毁是为了检视物品的时候不用重新生成
        }
    }

    /// <summary>
    /// 使用或消耗一次耐久。
    /// </summary>
    /// <param name="forceDestroy">强制销毁物品</param>
    public void OnConsumed(bool forceDestroy = false)
    {
        if (!_isInInventory)
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
            // TODO: 销毁物品
        }
    }

    /****** SelectableObject ******/

    /// <summary>
    /// 玩家与场景中的背包物品交互（通常是拾取）。
    /// </summary>
    /// <param name="ItemID">手持物品的ItemID，此处用不到</param>
    public void InteractRequest(int ItemID)
    {
    }

    public override void MouseOver()
    {
        if (!_isInteractionTarget && !_isInInventory)
        {
            _isInteractionTarget = true;
        }
        
        base.MouseOver();
    }

    public override void MouseExit()
    {
        _isInteractionTarget = false;
        
        base.MouseExit();
    }
    
    protected void Start()
    {
        base.Start();

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
        if (outline is not null)
        {
            outline.enabled = false;
        }

        _durability = itemData.durability;

        Restart();
    }
    
    // public void OnMouseOver()
    // {
    //     throw new NotImplementedException();
    // }
}