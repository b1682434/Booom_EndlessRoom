using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DifferentKeyObj : InteractableBase, IInteractRequest   
{
    public int KeyIDV1;
    public int KeyIDV2;
    bool opened;
    public UnityEvent itemOpenV1;
    public UnityEvent itemOpenV2;
    public AudioSource au;
    public AudioClip openv1sound, openV2sound, failSound;
    public void InteractRequest(int ItemID)
    {

        if (opened)
        {
            showWord = openWord;
        }
        else
        {
            if (ItemID == KeyIDV1)//打开
            {
                opened = true;
                showWord = openWord;
                itemOpenV1.Invoke();
                au.clip = openv1sound;
                au.Play();
            }else if (ItemID == KeyIDV2)
            {
                opened = true;
                showWord = openWord;
                itemOpenV2.Invoke();
                au.clip = openV2sound;
                au.Play();
            }
            else if (ItemID == 0)//空手
            {
                showWord = emptyHandWord; //应该可以继续优化。 需要钥匙的一个类，多种钥匙多种方法的一个子类，pickup一个
            }
            else // 拿了错的东西开门
            {
                showWord = cannotOpenWord;
                au.clip = failSound;
                au.Play();

            }

        }

    }
}
