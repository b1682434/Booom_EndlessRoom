using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Outline))]
public class InteractableBase : MonoBehaviour, IMouseOver
{
    [Tooltip("物品名称")] public string objName;
    [Tooltip("物品id")] public int ObjType;
    [Tooltip("被准星怼着时候该说啥")] public string mouseOVerWord;
    [Tooltip("拿了错误东西开门该说啥")] public string cannotOpenWord;
    [Tooltip("空手调查时候该说啥")] public string emptyHandWord;
    [Tooltip("打开了该说啥")] public string openWord;
    [Tooltip("这个不动就行")] [HideInInspector] public string showWord; //最终传回给接口的文本

    public string returnWord
    {
        get { return showWord; }
    }

    public string objectName
    {
        get { return objName; }
    }

    public int ObjectType
    {
        get { return ObjType; }
    }

    public Outline outline;
    bool corrotineAlreadyRunning =false;
    bool overByMouse;

    public void MouseOver()
    {
        MouseOverImpl();
    }

    public virtual void MouseOverImpl()
    {
        overByMouse = true;
        // showWord = mouseOVerWord;
        if (!corrotineAlreadyRunning)
        {
            StartCoroutine("CheckWhetherStillOverByTheMouse");
            showWord = mouseOVerWord;
            corrotineAlreadyRunning = true;
            if (outline != null)
            {
                outline.enabled = true;
            }
            
            showWord = mouseOVerWord;//感觉有点不行。 被打开的话应该是会变化的。好像没必要放这儿？门可以另起
           // outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }

    public void MouseExit()
    {
        MouseExitImpl();
    }

    public virtual void MouseExitImpl()
    {
        outline.enabled = false;
        corrotineAlreadyRunning = false;
    }

    IEnumerator CheckWhetherStillOverByTheMouse()
    {
        for (;;)
        {
            overByMouse = false;
            yield return new WaitForSeconds(0.02f); //等一会，如果值不跟新，说明不再被看着了,那就关闭轮廓线并结束此协程
            if (!overByMouse)
            {
                if (outline != null)
                {
                    outline.enabled = false;
                }
                MouseExit();
                
                corrotineAlreadyRunning = false;
                StopAllCoroutines();
            }
            
        }//虽然方法很狗屎，但似乎取消被聚焦那块也可以这么做？解耦。判断条件就是vcam的priority值等于多少
    }

    // Start is called before the first frame update
}