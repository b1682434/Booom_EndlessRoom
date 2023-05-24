using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMoveandRot : MonoBehaviour
{
    public Transform targetTrans;
    Vector3 targetPos;
    Quaternion targetRot;
    public bool move = true;
    public bool rot;
    public float speed = 2f;
    bool coroutineStarted = false;
    public AudioSource au;
    public AudioClip clip;
    private void Start()
    {
        targetPos = targetTrans.position;
        targetRot = targetTrans.rotation;
    }
    public void startTrans()
    {
        if (!coroutineStarted)
        {
            coroutineStarted = true;
            StartCoroutine("TransTransforms");
            if (au != null)
            {
                au.clip = clip;
                au.Play();
            }
        }
    }
    IEnumerator TransTransforms()
    {
        for(; ; )
        {
            if (move)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            }
            if (rot)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, speed * Time.deltaTime);
            }
            yield return new WaitForSeconds(0.02f);
        }
        
    }
}
