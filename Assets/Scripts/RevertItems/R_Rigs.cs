using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_Rigs : RevertBase
{
    Rigidbody rb;
    
    protected override void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    protected override void Recover()
    {
        base.Recover();
        transform.position = beginPos;
        transform.rotation = beginQuaternion;
        rb.isKinematic = true;
    }

    protected override void StartMoving()
    {
        rb.isKinematic = false;
    }
}