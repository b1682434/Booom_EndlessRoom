using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevertBase : MonoBehaviour
{
    /// <summary>
    /// 重置时触发
    /// </summary>
    public delegate void OnRecoverDelegate();
    public OnRecoverDelegate onRecover;
    
    public void RevertFunc()
    {
        Recover();
    }

    public void StartMovFunc()
    {
        StartMoving();
    }
    // Start is called before the first frame update
    protected virtual void Recover()
    {
        onRecover();
    }
    protected virtual void StartMoving()
    {

    }

}
