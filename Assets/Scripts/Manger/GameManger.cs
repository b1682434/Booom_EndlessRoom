using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.InputSystem;
public class GameManger : MonoBehaviour
{
    
    public GameObject currentRevertObj;

    public GameObject[] roomPrefab;
    public int darkRoomNumber;
    int currentRoomNumber;

    FirstPersonController fpsCtrl;
    CinemachineBrain cmBrain;
    CinemachineVirtualCamera currentVitrualCam;
    private PlayerInput _playerInput;
    public bool fuzhouShouldShow = true;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        Screen.fullScreen = true;//强制设置分辨率
    }

    GameObject[] revertObjs = new GameObject[2];
    private void Start()
    {
        fpsCtrl = FindFirstObjectByType<FirstPersonController>();
        _playerInput = FindFirstObjectByType<PlayerInput>();
        revertObjs[1] = currentRevertObj;
        /*revertObjs[1] = Instantiate(revertObjs[0], revertObjs[0].transform);
        revertObjs[1].transform.parent = null;
        revertObjs[1].SetActive(false);*/
        cmBrain = Camera.main.GetComponent<CinemachineBrain>();
        Cursor.lockState = CursorLockMode.Confined;
        
    }
    
    public void EnterFocusMode(CinemachineVirtualCamera targetVcam)
    {
        if(currentVitrualCam!= null)
        {
            currentVitrualCam.Priority = 0;
        }
        currentVitrualCam = targetVcam;
        currentVitrualCam.Priority = 15;
        cmBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        cmBrain.m_DefaultBlend.m_Time = 2f;
        fpsCtrl.EnterFocusMode();
    }
    public void ExitFocusVcam()
    {
        if (currentVitrualCam != null)
        {
            currentVitrualCam.Priority = 0;
        }      
        cmBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
       
    }

    public void ChangeRoomPrefab()
    {
        currentRoomNumber = Mathf.Clamp(currentRoomNumber + 1, 0, roomPrefab.Length);
    }
  public  void GoThroughDoor( Vector3 postion )
    {
        if (currentRoomNumber == darkRoomNumber)
        {
            ChangeEnvironmentLighting(Color.black, 2f);
        }
        //GameObject newObj = Instantiate(nextRevertObj, postion, nextRevertObj.transform.rotation);
        GameObject newObj = Instantiate(roomPrefab[currentRoomNumber], postion, revertObjs[1].transform.rotation);
        newObj.transform.parent = null; 
        revertObjs[0] = revertObjs[1];
        revertObjs[1] = newObj;
        /* revertObjs[1].transform.position = postion;
         revertObjs[1].SetActive(true);*/
         Invoke("DestroyOldRertOBJ", 2f);
        
    }
    public void DestroyOldRertOBJ()
    {
        Destroy(revertObjs[0]);
       /* Destroy(revertObjs[0]);
        revertObjs[0] = revertObjs[1];
        revertObjs[1] = Instantiate(revertObjs[0], revertObjs[0].transform);
        revertObjs[1].transform.parent = null;
        revertObjs[1].SetActive(false);*/
    }

    public void ChangeEnvironmentLighting(Color lightColor,float time)
    {
        // RenderSettings.ambientLight = lightColor;
        StopAllCoroutines();
        StartCoroutine(ChangeLight(lightColor,time));
    }
    IEnumerator ChangeLight(Color lightColor, float time)
    {
        float i = 0;
       

        for(; ; )
        { i++;
            float lerp = i / 50 / time;
            RenderSettings.ambientLight= Color.Lerp(RenderSettings.ambientLight, lightColor, lerp);

                if (lerp >= 1)
            {
                StopAllCoroutines();
            }
            yield return new WaitForSeconds(0.02f);
        }
        
    }
    public void ReturnToMainu()
    {
        SceneManager.LoadScene(0);
    }
}
