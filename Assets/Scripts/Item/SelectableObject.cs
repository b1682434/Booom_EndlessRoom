using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SelectableObject : InteractableBase, IInteractRequest,IMouseOver
{
    public CinemachineVirtualCamera vCam;
    public int focusPriority = 15;
    public bool needKeyToOpen;
    public int keyID;
    public bool focusObj;
    bool opened;
    GameManger gm;
 
 private void Start()
    {
        gm = FindObjectOfType<GameManger>();
    }
    public void BeFocus()
    {
        //Camera.main.transform.GetComponent<CinemachineBlenderSettings>()
        Camera.main.transform.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        vCam.Priority = focusPriority;
    }
    

    public void InteractRequest(int ItemID)
    {
        if (focusObj)
        {
            gm.EnterFocusMode(vCam);
        }
        if (needKeyToOpen)
        {
            
            if (ItemID == keyID)//打开
            {
                opened = true;
               // showWord = openWord;
            }
            else if(ItemID == 0)//空手
            {
                showWord = emptyHandWord; //应该可以继续优化。 需要钥匙的一个类，多种钥匙多种方法的一个子类，pickup一个
            }
            else // 拿了错的东西开门
            {
                showWord = cannotOpenWord;
            }
        }
        if (opened)
        {
            showWord = openWord;
        }
    }

    
}
