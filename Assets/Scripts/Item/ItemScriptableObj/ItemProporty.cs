using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemPropertyScriptableObject", order = 1)]
public class ItemProporty : ScriptableObject
{
    // Start is called before the first frame update
    public string itemName;
    public int itemID;
    
}
