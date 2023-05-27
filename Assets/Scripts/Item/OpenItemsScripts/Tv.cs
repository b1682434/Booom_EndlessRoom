using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tv : MonoBehaviour
{
    public AudioSource au;
    public AudioClip noiseSound, rightSound;
    public Renderer screenRenderer;
    public Material noiseMat, rightMat, oriMat;
    public Clock clock;
    bool inRightChannel;
    bool open;

    private void Start()
    {
        oriMat = screenRenderer.material;
    }
    public void ToWrongChannel()
    {
        inRightChannel = false;
        screenRenderer.material = noiseMat;
        au.clip = noiseSound;
        au.time = 0f;
        au.Play();
    }

    public void OpenTV()
    {
        open = !open;
        if (open)
        {
            if (inRightChannel) ToRightChannel();
            else ToWrongChannel();
        }
        else
        {
            screenRenderer.material = oriMat;
            au.Stop();
        }
    }

    public void ToRightChannel()
    {
        inRightChannel = true;
        screenRenderer.material = rightMat;
        au.clip = rightSound;
        if(clock.minutes * 60f + clock.second< au.clip.length)
        {
            au.time = clock.minutes * 60f + clock.second;
            print("playat" + au.time);
            au.Play();
        }
        else
        {
            print(clock.minutes * 60f + clock.second);
        }
        
    }
}
