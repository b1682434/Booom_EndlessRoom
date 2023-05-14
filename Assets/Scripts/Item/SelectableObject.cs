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
            
            if (ItemID == keyID)//��
            {
                opened = true;
               // showWord = openWord;
            }
            else if(ItemID == 0)//����
            {
                showWord = emptyHandWord; //Ӧ�ÿ��Լ����Ż��� ��ҪԿ�׵�һ���࣬����Կ�׶��ַ�����һ�����࣬pickupһ��
            }
            else // ���˴�Ķ�������
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
