using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbishBinObj :InteractableBase,IInteractRequest
{
    FirstPersonController fps;
    public GameObject ui;
  
    void Start()
    {
        DeactiveUI();
        fps = FindFirstObjectByType<FirstPersonController>();
    }
    public void InteractRequest(int ItemID)
    {
        ui.SetActive(true);
    }
    public void ConsumeItem()
    {
        fps.ConsumeObj();
        DeactiveUI();
    }
    public void DeactiveUI()
    {
        ui.SetActive(false);
    }
}
