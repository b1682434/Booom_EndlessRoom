using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//给保险箱用的 两道钥匙门
public class DuoKeyObject : InteractableBase, IInteractRequest
{
    public int KeyIDV1;
    public int KeyIDV2;
    bool opened;
    bool key1Opened;
    public UnityEvent itemOpenV1;
    public UnityEvent itemOpenV2;
    public string key1RequireWord;
    public string key2RequireWord;
    public void InteractRequest(int ItemID)
    {

        if (opened)
        {
            showWord = openWord;
        }
        else
        {
            if (key1Opened)
            {
                if (ItemID == KeyIDV1)//打开
                {
                    opened = true;
                    showWord = openWord;
                    itemOpenV1.Invoke();
                }
                else if (ItemID == 0)//空手
                {
                    showWord = key1RequireWord; //应该可以继续优化。 需要钥匙的一个类，多种钥匙多种方法的一个子类，pickup一个
                }
                else // 拿了错的东西开门
                {
                    showWord = cannotOpenWord;
                }
            }
            else
            {
                if (ItemID == KeyIDV2)//打开
                {
                    opened = true;
                    showWord = openWord;
                    itemOpenV2.Invoke();
                }
                else if (ItemID == 0)//空手
                {
                    showWord = key2RequireWord; ; //应该可以继续优化。 需要钥匙的一个类，多种钥匙多种方法的一个子类，pickup一个
                }
                else // 拿了错的东西开门
                {
                    showWord = cannotOpenWord;
                }
            }


        }

    }
}
