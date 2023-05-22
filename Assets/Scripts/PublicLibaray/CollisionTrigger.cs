using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TriggerDelegate(Collider other);
public delegate void CollisionDelegate(Collision other);

/// <summary>
/// 用于将一个gameObject的碰撞事件传递到其他脚本中
/// - 出于性能考虑，暂不支持OnTriggerStay和OnCollisionStay的传递
/// </summary>
[RequireComponent(typeof(Collider))]
public class CollisionTrigger : MonoBehaviour
{
    public TriggerDelegate OnTriggerEnterThis;
    public TriggerDelegate OnTriggerExitThis;
    public CollisionDelegate OnCollisionEnterThis;
    public CollisionDelegate OnCollisionExitThis;
    
    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterThis(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitThis(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionEnterThis(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        OnCollisionExitThis(collision);
    }
}
