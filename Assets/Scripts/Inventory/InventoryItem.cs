using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包物体组件。
/// - 可以放入背包的物体应该添加此组件
/// </summary>
[RequireComponent(typeof(Outline))]
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

    /********** 视觉效果 **********/
    private Outline _outline;
    [SerializeField] private GameObject _interactionTip;
    private Transform _cameraTransform;

    void Start()
    {
        _revertBase = GetComponent<RevertBase>();
        if (_revertBase != null)
        {
            _revertBase.onRecover += Restart;
        }

        _outline = GetComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
        _outline.OutlineWidth = 8.0f;
        _outline.enabled = false;

        _cameraTransform = Camera.main.transform;

        Restart();
    }

    private void LateUpdate()
    {
        if (_isInteractionTarget && _interactionTip is not null)
        {
            _interactionTip.transform.rotation =
                Quaternion.LookRotation(_interactionTip.transform.position - _cameraTransform.position);
        }
    }

    /// <summary>
    /// 重置物品
    /// </summary>
    private void Restart()
    {
        // if (itemData.recoverable)
        // {
        //     // TODO: 生成一个新的物品，但只能生成一个
        //     Instantiate(this, _initialPosition, _initialRotation);
        // }
    }

    /// <summary>
    /// 进入背包
    /// </summary>
    public void EnterInventory()
    {
        _isInInventory = true;
        _outline.enabled = false;
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
    public void ConsumeItem(bool forceDestroy = false)
    {
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

    /// <summary>
    /// 成为交互目标
    /// </summary>
    public void BeInteractionTarget()
    {
        _isInteractionTarget = true;

        // TODO: UI提示
        _interactionTip.SetActive(true);
        _outline.enabled = true;

        Debug.Log($"BeInteractionTarget() {gameObject}");
    }

    /// <summary>
    /// 成为非交互目标
    /// </summary>
    public void BeNotInteractionTarget()
    {
        _isInteractionTarget = false;

        // TODO: UI提示
        _interactionTip.SetActive(false);
        _outline.enabled = false;
    }
}