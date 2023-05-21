using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品合成表
/// - 目前仅单纯存放数据
/// </summary>
[RequireComponent(typeof(Transform))]
public class InventoryCraftingRecipe : MonoBehaviour
{
    [Tooltip("原料类型")]
    public InventoryItemData ingredient;
    
    [Tooltip("产物，通常是Prefab")]
    public GameObject product;

    [Tooltip("合成位置容忍度。5.0f表示在合成位置5m内都可以合成成功。")]
    public float tolerance = 5.0f;
}
