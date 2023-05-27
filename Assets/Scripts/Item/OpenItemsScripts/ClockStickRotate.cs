using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockStickRotate : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0, Space.Self);
    }
}
