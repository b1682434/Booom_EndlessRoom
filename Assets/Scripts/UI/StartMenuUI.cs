using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class StartMenuUI : MonoBehaviour
{
    public CinemachineVirtualCamera startVcam;
   

    public void StartGame()
    {
      
        startVcam.Priority = 9;
        Cursor.lockState = CursorLockMode.Locked;
        Invoke("DeactiveSelf", 3f);
    }

    void DeactiveSelf()
    {
       gameObject.SetActive(false);
        print("exed");
    }
}
