using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIbasicVoid : MonoBehaviour
{
    // Start is called before the first frame update
    public void ReturnToMenu()
    {
       
        SceneManager.LoadScene(0);
    }

   
    public void LoadSelectLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
