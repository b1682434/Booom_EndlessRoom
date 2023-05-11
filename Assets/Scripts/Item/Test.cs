using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Test : MonoBehaviour
{
    public Image aimUI;
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
       // Ray ray = Camera.main.ViewportPointToRay (aimUI.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(aimUI.transform.position);
        // Vector3 uiWorldPos = Camera.main.ScreenToWorldPoint(aimUI.transform.position);
        //Debug.DrawLine(Camera.main.transform.position, uiWorldPos);
        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
        }
        else
        {
            Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position+ ray.direction * 20, Color.green);
        }
    }
}
