using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectInspectCamera : MonoBehaviour
{

    public Camera objectInspectCamera;

    private void Update()
    {
        CheckObj();
    }
    void CheckObj()
    {
        Ray ray =  objectInspectCamera.ScreenPointToRay(Mouse.current.position.ReadValue()/6f);
        Debug.DrawLine(objectInspectCamera.transform.position, objectInspectCamera.transform.position + ray.direction * 5f);
    }
}
