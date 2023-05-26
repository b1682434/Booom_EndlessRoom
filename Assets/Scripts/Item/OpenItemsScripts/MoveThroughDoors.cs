using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class MoveThroughDoors : MonoBehaviour
{
    FirstPersonController fps;
    public Transform targetTransform;
    public CinemachineVirtualCamera vcam;
    GameManger gm;
    private void Start()
    {
        fps = FindFirstObjectByType<FirstPersonController>();
        gm = FindFirstObjectByType<GameManger>();
    }
    public void MoveToTargetPos()
    {
        fps.gameObject.transform.position = targetTransform.position;
        gm.ExitFocusVcam();
        gm.EnterFocusMode(vcam);
        fps.gameObject.GetComponent<PlayerInput>().enabled = false;
        Invoke("ReturnPlayerControl", 2f);
    }

    void ReturnPlayerControl()
    {
        gm.ExitFocusVcam();
        
        fps.gameObject.transform.rotation = vcam.transform.rotation;
        fps.gameObject.GetComponent<PlayerInput>().enabled = true;
        fps.ExitFocusMode();
    }
}
