using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopRigidBody : MonoBehaviour
{
    private void Start()
    {
        Invoke("StopPhysicSim", 0.5f);
    }
    public void StopPhysicSim()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
