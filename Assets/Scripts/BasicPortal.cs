using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPortal : MonoBehaviour
{
    
    public BasicPortal otherPort;
    public Transform enterPoint;
    public Transform startPoint;
    public Collider coll;
    Animation anim;
   
    private void Awake()
    {
        coll = GetComponent<Collider>();
        anim = GetComponent<Animation>();
    }
    
    public void OpenDoorAnim()
    {
        anim.Play();
    }
    
}
