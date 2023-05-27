using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.Events;

public class SmallMouse : MonoBehaviour
{
    public PathCreator runawayPcreators;
    public PathCreator eatPcreators;
    public UnityEvent runawayEvents;
    public UnityEvent eatEvents;
    public Transform mouseModelTrans;
    bool alreadyDid;
    bool getCaught;
    public Collider mouseColl;
    public void RunAway()
    {
        if (!alreadyDid)
        {
            alreadyDid = true;
            StartCoroutine(MoveAlongSpline(runawayPcreators));
        }
    }
    public void GetCaught()
    {
        if (!alreadyDid)
        {
            alreadyDid = true;
            StartCoroutine(MoveAlongSpline(eatPcreators));
            getCaught = true;
        }
    }
    IEnumerator MoveAlongSpline(PathCreator pcreate)
    {
        int i = 0;
        float length = pcreate.path.length;
        float lerp;
        for (; ; )
        {
            i += 1;
            lerp = Mathf.Lerp(0, 1, i / 100f);
            mouseModelTrans.position = pcreate.path.GetPointAtDistance(lerp * length);       
            mouseModelTrans.rotation = AdjustRot(pcreate.path.GetDirectionAtDistance(lerp * length));
           
            if (i >= 99)
            {
                StopAllCoroutines();
                if (getCaught)
                {
                    eatEvents.Invoke();
                    mouseColl.enabled = true;
                }
                else
                {
                    runawayEvents.Invoke();
                }
            }
           
            yield return new WaitForSeconds(0.02f);
        }
    }

    Quaternion AdjustRot(Vector3 direction)
    {
        return Quaternion.FromToRotation(Vector3.right, direction);
    }

}
