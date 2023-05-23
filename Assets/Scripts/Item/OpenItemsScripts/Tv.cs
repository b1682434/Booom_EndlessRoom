using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tv : MonoBehaviour
{
    public AudioSource au;
    public AudioClip noiseSound, rightSound;
    public Renderer screenRenderer;
    public Material noiseMat, rightMat;
    public Clock clock;
    public void ToWrongChannel()
    {
        screenRenderer.material = noiseMat;
        au.clip = noiseSound;
        au.time = 0f;
        au.Play();
    }

    public void ToRightChannel()
    {
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
