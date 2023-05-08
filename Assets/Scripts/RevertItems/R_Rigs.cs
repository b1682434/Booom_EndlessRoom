using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_Rigs : RevertBase
{
    Rigidbody rb;
    Vector3 beginPos;
    Quaternion beginQuaternion;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        beginPos = transform.position;
        beginQuaternion = transform.rotation;

    }

    // Update is called once per frame
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
