using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusObj : InteractableBase,IInteractRequest
{
   public Collider[] childCollider;//子物体的collider 进入 focuss mode 后才会激活,这样子物体只有在focusmode才会被选中
   
    // Start is called before the first frame update
    void Start()
    {
        DeFocus();
    }

    // Update is called once per frame
   public void InteractRequest(int ItemID)
    {
        foreach(Collider coll in childCollider)
        {
            coll.enabled = true;
        }

    }

    public void DeFocus()
    {
        foreach (Collider coll in childCollider)
        {
            coll.enabled = false;
        }
    }
}
