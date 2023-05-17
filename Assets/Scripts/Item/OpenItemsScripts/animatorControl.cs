using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animatorControl : MonoBehaviour
{
    public Animator anim;
    public string animBoolName;
    //public bool onlyOnce = true;
    public void ItemsOpened()
    {
        anim.SetBool(animBoolName, true);
    }

}
