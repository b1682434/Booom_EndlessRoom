using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeEnvironmetLighting : MonoBehaviour
{
    GameManger gm;
    public Color lightColor;
    private void Start()
    {
        gm = FindFirstObjectByType<GameManger>();
    }
    // Start is called before the first frame update
    public void ChangeToBlack()
    {
        gm.ChangeEnvironmentLighting(Color.black, 2f);
    }
    public void BackToNormalColor()
    {
        gm.ChangeEnvironmentLighting(lightColor, 0.1f);
    }
}
