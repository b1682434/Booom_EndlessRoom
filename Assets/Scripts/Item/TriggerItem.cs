using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerItem : MonoBehaviour
{
    public UnityEvent TriggerEvent;
    public string targetTag;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == targetTag)
        {
            TriggerEvent.Invoke();
        }
        GetComponent<Collider>().enabled = false;
    }
}
