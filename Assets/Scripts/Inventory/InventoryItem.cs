using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包物体组件。
/// - 可以放入背包的物体应该添加此组件
/// </summary>
public class InventoryItem : MonoBehaviour
{
    [Tooltip("物品数据配置")] public InventoryItemData itemData;

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

    /// <summary>
    /// 是否是交互目标，需要在场景中
    /// </summary>
    private bool _isInteractionTarget = false;

    public bool IsInteractionTarget
    {
        get => _isInteractionTarget;
    }

    public bool IsInInventory
    {
        get => _isInInventory;
    }

    /// <summary>
    /// 重置组件
    /// </summary>
    private RevertBase _revertBase;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    void Start()
    {
        _revertBase = GetComponent<RevertBase>();
        if (_revertBase != null)
        {
            _revertBase.onRecover += Restart;
        }

        Restart();
    }

    /// <summary>
    /// 重置物品
    /// </summary>
    private void Restart()
    {
        if (itemData.recoverable)
        {
            // TODO: 生成一个新的物品，但只能生成一个
            Instantiate(this, _initialPosition, _initialRotation);
        }
    }

    /// <summary>
    /// 进入背包
    /// </summary>
    public void EnterInventory()
    {
        _isInInventory = true;
    }

    /// <summary>
    /// 获得物品。
    /// </summary>
    public void ObtainItem()
    {
        if (_isInInventory)
        {
            _stack++;
        }
    }

    /// <summary>
    /// 使用或消耗一次耐久。
    /// </summary>
    /// <param name="forceDestroy">销毁物品</param>
    public void ComsumeItem(bool forceDestroy = false)
    {
        // TODO
    }

    /// <summary>
    /// 成为交互目标
    /// </summary>
    public void BeInteractionTarget()
    {
        _isInteractionTarget = true;
        // TODO: UI提示
        // TODO: 高亮
        Debug.Log($"BeInteractionTarget() {gameObject}");
    }

    /// <summary>
    /// 成为非交互目标
    /// </summary>
    public void BeNotInteractionTarget()
    {
        _isInteractionTarget = false;
        // TODO: UI提示
        // TODO: 高亮
    }
}