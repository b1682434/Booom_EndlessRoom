using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 过场图片UI初始化器
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class CinematicPicViewInitializer : MonoBehaviour
{
    [Tooltip("玩家Object")] public GameObject playerGameObject;
    
    private void OnEnable()
    {
        // TODO：
    }
}
