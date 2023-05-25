using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class NeedKeyObject : InteractableBase, IInteractRequest                               
{
    

    public bool consumeItems = true;
    public int keyID;
    bool opened;
    public UnityEvent itemOpen;
    public AudioClip cannnotOpenSouond, openSound;
    public AudioSource au;
    
    
   
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
                PlaySound(openSound);
                if (consumeItems)
                {
                    FindFirstObjectByType<FirstPersonController>().ConsumeObj();
                }
            }
            else if (ItemID == 0)//空手
            {
                PlaySound(cannnotOpenSouond);
                showWord = emptyHandWord; //应该可以继续优化。 需要钥匙的一个类，多种钥匙多种方法的一个子类，pickup一个
            }
            else // 拿了错的东西开门
            {
                PlaySound(cannnotOpenSouond);
                showWord = cannotOpenWord;
            }

        }
        
    }

    public void PlaySound(AudioClip clip)
    {

        if (clip != null && au != null)
        {
            au.clip = clip;
            au.Play();
        }
    }
    
}
