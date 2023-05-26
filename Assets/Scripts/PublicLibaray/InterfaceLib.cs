using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractRequest  //点击
{
    void InteractRequest(int ItemID);

    //void MouseOver();
    
    public int ObjectType { get; }  //物品id

    public string returnWord { get; }//点了之后说啥
    //public string objectName { get;}//感觉接口好像要名字没用
    
}

public interface IMouseOver
{
    public string returnWord { get; }//鼠标放上面会说啥
    
    /// <summary>
    /// 鼠标移入
    /// </summary>
    void MouseOver();

    /// <summary>
    /// 鼠标移出
    /// </summary>
    void MouseExit();
}

public interface IOpenItem
{
    void ItemsOpened();
}
