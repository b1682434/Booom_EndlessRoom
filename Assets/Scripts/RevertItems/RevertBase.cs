using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevertBase : MonoBehaviour
{
    public void RevertFunc()
    {
        Recover();
    }

    public void StartMovFunc()
    {
        StartMoving();
    }
    // Start is called before the first frame update
    protected virtual void Recover()
    {

    }
    protected virtual void StartMoving()
    {

    }

}
