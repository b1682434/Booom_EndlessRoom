using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SelectableObject : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public int focusPriority = 15;
   

    private void Start()
    {
        DeFocus();
    }
    public void BeFocus()
    {
        //Camera.main.transform.GetComponent<CinemachineBlenderSettings>()
        Camera.main.transform.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        vCam.Priority = focusPriority;
    }
    public void DeFocus()
    {
        Camera.main.transform.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        vCam.Priority = 0;
    }
}
