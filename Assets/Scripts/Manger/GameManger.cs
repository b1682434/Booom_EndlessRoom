using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManger : MonoBehaviour
{
    GameObject[] revertObj;
    private void Start()
    {
      revertObj =   GameObject.FindGameObjectsWithTag("Revert");
    }
    // Start is called before the first frame update
    public void revertAll()
    {
        foreach(GameObject obj in revertObj)
        {
            obj.GetComponent<RevertBase>().RevertFunc();
        }
    }
    public void StartMovingAll()
    {
        foreach (GameObject obj in revertObj)
        {
            obj.GetComponent<RevertBase>().StartMovFunc();
        }

    }

    public void ReturnToMainu()
    {
        SceneManager.LoadScene(0);
    }
}
