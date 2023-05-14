using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Outline))]
public class InteractableBase : MonoBehaviour
{

    [Tooltip("��Ʒ����")]
    public string objName;
    [Tooltip("��Ʒid")]
    public int ObjType;
    [Tooltip("��׼������ʱ���˵ɶ")]
    public string mouseOVerWord;
    [Tooltip("���˴��������Ÿ�˵ɶ")]
    public string cannotOpenWord;
    [Tooltip("���ֵ���ʱ���˵ɶ")]
    public string emptyHandWord;
    [Tooltip("���˸�˵ɶ")]
    public string openWord;
    [Tooltip("�����������")]
    public string showWord;//���մ��ظ��ӿڵ��ı�
    public string returnWord
    {
        get
        {
            return showWord;
        }
    }
    
    public string objectName
    {
        get
        {
            return objName;
        }
    }
  
    public int ObjectType
    {
        get
        {
            return ObjType;
        }
    }

    public Outline outline;
    bool corrotineAlreadyRunning =false;
   public bool overByMouse;
    public void MouseOver()//mouseover�Ľӿڴ��롣���ܷ����������ʣ����ܻ��ƶ�λ��
    {
       
        overByMouse = true;
       // showWord = mouseOVerWord;
        if (!corrotineAlreadyRunning)
        {
            StartCoroutine("CheckWhetherStillOverByTheMouse");
            showWord = mouseOVerWord;
            corrotineAlreadyRunning = true;
            outline.enabled = true;
            showWord = mouseOVerWord;//�о��е㲻�С� ���򿪵Ļ�Ӧ���ǻ�仯�ġ�����û��Ҫ��������ſ�������
           // outline.OutlineMode = Outline.Mode.OutlineVisible;
            
        }
        
        
    }
    IEnumerator CheckWhetherStillOverByTheMouse()
    {
        
        for(; ; )
        {
            overByMouse = false;
            yield return new WaitForSeconds(0.02f);//��һ�ᣬ���ֵ�����£�˵�����ٱ�������,�Ǿ͹ر������߲�������Э��
            if (!overByMouse)
            {
                outline.enabled = false;
                corrotineAlreadyRunning = false;
                StopAllCoroutines();

            }
            
        }
    }

    // Start is called before the first frame update

}
