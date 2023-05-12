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
    /// 当前的可使用次数
    /// </summary>
    private int _durability;

    /// <summary>
    /// 是否在背包内
    /// </summary>
    private bool _isInInventory;

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
    /// 是否可交互
    /// </summary>
    /// <returns></returns>
    public bool CanBeInteracted()
    {
        return _isInInventory == false;
    }

    /// <summary>
    /// 变成可交互状态
    /// </summary>
    public void BeInteractive()
    {
        // TODO: UI提示
        // TODO: 高亮
    }

    /// <summary>
    /// 变成不可交互状态
    /// </summary>
    public void BeNonInteractive()
    {
        // TODO: UI提示
        // TODO: 高亮
    }
}