using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FocusObj : InteractableBase,IInteractRequest
{
   public Collider[] childCollider;//子物体的collider 进入 focuss mode 后才会激活,这样子物体只有在focusmode才会被选中
   public Collider selfColl;
    public CinemachineVirtualCamera vcam;
    GameManger gm;

    // Start is called before the first frame update
    void Start()
    {
        DeFocus();
        //selfColl = GetComponent<Collider>();
        gm = FindObjectOfType<GameManger>();
    }

    // Update is called once per frame
   public void InteractRequest(int ItemID)
    {
        
        selfColl.enabled = false;
        gm.EnterFocusMode(vcam);
        if (outline != null)
        {
            outline.enabled = false;
        }
        Invoke("EnableChildColl",1.5f);
    }
    void EnableChildColl()
    {
        foreach (Collider coll in childCollider)
        {
            coll.enabled = true;
        }
    }

    public void DeFocus()//使用event激活。 很乱，但先这样吧
    {
        foreach (Collider coll in childCollider)
        {
            coll.enabled = false;
        }
        selfColl.enabled = true;
    }
}
