using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Outline))]
public class InteractableBase : MonoBehaviour
{

    [Tooltip("物品名称")]
    public string objName;
    [Tooltip("物品id")]
    public int ObjType;
    [Tooltip("被准星怼着时候该说啥")]
    public string mouseOVerWord;
    [Tooltip("拿了错误东西开门该说啥")]
    public string cannotOpenWord;
    [Tooltip("空手调查时候该说啥")]
    public string emptyHandWord;
    [Tooltip("打开了该说啥")]
    public string openWord;
    [Tooltip("这个不动就行")]
    public string showWord;//最终传回给接口的文本
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
    public void MouseOver()//mouseover的接口代码。可能放这儿不大合适，可能会移动位置
    {
       
        overByMouse = true;
       // showWord = mouseOVerWord;
        if (!corrotineAlreadyRunning)
        {
            StartCoroutine("CheckWhetherStillOverByTheMouse");
            showWord = mouseOVerWord;
            corrotineAlreadyRunning = true;
            outline.enabled = true;
            showWord = mouseOVerWord;//感觉有点不行。 被打开的话应该是会变化的。好像没必要放这儿？门可以另起
           // outline.OutlineMode = Outline.Mode.OutlineVisible;
            
        }
        
        
    }
    IEnumerator CheckWhetherStillOverByTheMouse()
    {
        
        for(; ; )
        {
            overByMouse = false;
            yield return new WaitForSeconds(0.02f);//等一会，如果值不跟新，说明不再被看着了,那就关闭轮廓线并结束此协程
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
