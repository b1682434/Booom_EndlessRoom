using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevertBase : MonoBehaviour
{
    /// <summary>
    /// 重置时触发，用于给其他组件挂载重置时的逻辑
    /// （由于部分物品既可以交互又会被重置，所以不能继承此类，适合用这个委托挂载重置时的逻辑）
    /// </summary>
    public delegate void OnRecoverDelegate();
    public OnRecoverDelegate onRecover;

    /// <summary>
    /// 是否可重置
    /// </summary>
    private bool _recoveryEnabled = true;

    public bool RecoveryEnabled
    {
        get => _recoveryEnabled;
    }

    /// <summary>
    /// 初始位置和旋转
    /// </summary>
    protected Vector3 beginPos;
    protected Quaternion beginQuaternion;

    protected virtual void Start()
    {
        var transformCache = transform;
        beginPos = transformCache.position;
        beginQuaternion = transformCache.rotation;
    }

    public void SetRecoveryEnabled(bool newEnable)
    {
        _recoveryEnabled = newEnable;
    }

    public void RevertFunc()
    {
        Recover();
    }

    public void StartMovFunc()
    {
        StartMoving();
    }
    
    /// <summary>
    /// 重置逻辑。
    /// - Note: 重写此方法的子类需要注意，base.Recover()需要在重写方法的最前面调用。
    /// </summary>
    protected virtual void Recover()
    {
        if (_recoveryEnabled)
        {
            return;
        }

        onRecover();
    }
    
    protected virtual void StartMoving()
    {

    }

}
