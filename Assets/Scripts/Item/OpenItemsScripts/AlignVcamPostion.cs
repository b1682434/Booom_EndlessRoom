using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignVcamPostion : MonoBehaviour
{
    public Transform vcamTrans;

    public void AlignView()
    {
        vcamTrans.position = Camera.main.transform.position;
        vcamTrans.rotation = Camera.main.transform.rotation;
    }
    // Start is called before the first frame update
   
}
