using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetThingsdeActive : MonoBehaviour
{
    public GameObject TargetObject;
    public void SetActiveOrNot(bool active)
    {
        if (active)
        {
            TargetObject.SetActive(true);
        }
        else
        {
            TargetObject.SetActive(false);
        }
    }
}
