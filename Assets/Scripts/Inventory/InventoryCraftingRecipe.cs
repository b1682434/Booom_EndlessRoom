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

    [Tooltip("合成位置容忍度。0.16f表示在合成位置0.16m内都可以合成成功。若不限制位置则设个大值，比如10")]
    public float tolerance = 0.16f;

    
    public bool craftingStatus = false;
}
