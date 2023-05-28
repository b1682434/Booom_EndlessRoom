using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzhoControl : MonoBehaviour
{
    public GameObject fuzhou;
   public void OpenedSafetyDoor()
    {
        FindFirstObjectByType<GameManger>().fuzhouShouldShow = false;
       
    }

    public void OpenFuzhou()
    {
        if (FindFirstObjectByType<GameManger>().fuzhouShouldShow)
        {
            fuzhou.SetActive(true);
        }
    }
}
