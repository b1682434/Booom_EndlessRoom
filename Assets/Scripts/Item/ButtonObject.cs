using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ButtonObject : InteractableBase, IInteractRequest
{
    public bool canBeInteracted = true;
    public UnityEvent ItemOpen;


    public void InteractRequest(int ItemID)
    {
        if (canBeInteracted)
        {
            ItemOpen.Invoke();
            print("ButtonPressed");
        }
    }
    


    
}
