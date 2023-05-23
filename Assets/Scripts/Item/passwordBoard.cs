using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class passwordBoard : MonoBehaviour
{
    public UnityEvent openEvent;
    public UnityEvent wrongEvent;
    public UnityEvent inputEvent;
    public string inputedNum = null;
    [Tooltip("正确密码")]
    public string targetNum = "1234";
    public AudioClip clearSound, wrongSound, openSound,inputSound;
    public AudioSource au;

    // Start is called before the first frame update
   
    public void inputNumber(string inputedString)
    {
        au.clip = inputSound;
        au.Play();
        inputedNum = inputedNum + inputedString;
        inputEvent.Invoke();
    }
    public void VerifyPassword()
    {
        if(inputedNum == targetNum)
        {
            openEvent.Invoke();
            au.clip = openSound;
            au.Play();
        }
        else
        {
            inputedNum = null;
            au.clip = wrongSound;
            au.Play();
            wrongEvent.Invoke();
        }
    }
    public void ClearPassword()
    {
        inputedNum = null;
        au.clip = clearSound;
        au.Play();
    }
}
