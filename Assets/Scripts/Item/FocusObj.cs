using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusObj : InteractableBase,IInteractRequest
{
   public Collider[] childCollider;//�������collider ���� focuss mode ��Żἤ��,����������ֻ����focusmode�Żᱻѡ��
   
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
