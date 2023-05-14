using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
public class GameManger : MonoBehaviour
{
    
    public GameObject currentRevertObj;
    FirstPersonController fpsCtrl;
    CinemachineBrain cmBrain;
    CinemachineVirtualCamera currentVitrualCam;
   
  
    GameObject[] revertObjs = new GameObject[2];
    private void Start()
    {
        fpsCtrl = FindFirstObjectByType<FirstPersonController>();
        revertObjs[0] = currentRevertObj;
        revertObjs[1] = Instantiate(revertObjs[0], revertObjs[0].transform);
        revertObjs[1].transform.parent = null;
        revertObjs[1].SetActive(false);
        cmBrain = Camera.main.GetComponent<CinemachineBrain>();
        
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
  public  void GoThroughDoor( Vector3 postion )
    {
        //GameObject newObj = Instantiate(nextRevertObj, postion, nextRevertObj.transform.rotation);
        
        revertObjs[1].transform.position = postion;
        revertObjs[1].SetActive(true);
        Invoke("DestroyOldRertOBJ", 2f);
    }
    public void DestroyOldRertOBJ()
    {
        Destroy(revertObjs[0]);
        revertObjs[0] = revertObjs[1];
        revertObjs[1] = Instantiate(revertObjs[0], revertObjs[0].transform);
        revertObjs[1].transform.parent = null;
        revertObjs[1].SetActive(false);
    }


    public void ReturnToMainu()
    {
        SceneManager.LoadScene(0);
    }
}
