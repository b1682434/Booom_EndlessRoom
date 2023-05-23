using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class NeedKeyObject : InteractableBase, IInteractRequest                               
{
    
    public int keyID;
    bool opened;
    public UnityEvent itemOpen;
    
    
    public void InteractRequest(int ItemID)
    {
        print(ItemID);
        if (opened)
        {
            showWord = openWord;
        }
        else
        {
            if (ItemID == keyID)//打开
            {
                opened = true;
                showWord = openWord;
                itemOpen.Invoke();
            }
            else if (ItemID == 0)//空手
            {
                showWord = emptyHandWord; //应该可以继续优化。 需要钥匙的一个类，多种钥匙多种方法的一个子类，pickup一个
            }
            else // 拿了错的东西开门
            {
                showWord = cannotOpenWord;
            }

        }
        
    }

    
}
