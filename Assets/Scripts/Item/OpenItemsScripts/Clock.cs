using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : InteractableBase,IInteractRequest
{
    public AudioSource au;
    public float second = 0;
    public int eventStartTime;

    public bool canBeInteracted = true;
    bool soundPlayed;
    public string timeString;
   public int minutes = 0;
    void Update()
    {
        second += Time.deltaTime;
        if (second > 60)
        {
            minutes += 1;
            second = 0;
        }
        timeString = minutes.ToString("d2");
        if (!soundPlayed)
        {
            if (second > eventStartTime)
            {
                soundPlayed = true;
                au.Play();
            }
        }
    }

    
    public void InteractRequest(int ItemID)
    {
        if (canBeInteracted)
        {
         
            if (au != null)
            {
                au.Stop();
                /*au.time = time;
                au.Play();*/
                canBeInteracted = false;
            }

        }
    }

}
