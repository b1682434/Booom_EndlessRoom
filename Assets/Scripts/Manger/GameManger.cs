using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManger : MonoBehaviour
{
    //GameObject[] revertObjs;
    public GameObject currentRevertObj;
   
  
    GameObject[] revertObjs = new GameObject[2];
    private void Start()
    {
        
        revertObjs[0] = currentRevertObj;
        revertObjs[1] = Instantiate(revertObjs[0], revertObjs[0].transform);
        revertObjs[1].transform.parent = null;
        revertObjs[1].SetActive(false);

        // revertObj =   GameObject.FindGameObjectsWithTag("Revert");
    }
    // Start is called before the first frame update
   /* public void revertAll()
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

    }*/

  public  void GoThroughDoor( Vector3 postion )
    {
        //GameObject newObj = Instantiate(nextRevertObj, postion, nextRevertObj.transform.rotation);
        
        revertObjs[1].transform.position = postion;
        revertObjs[1].SetActive(true);
        Invoke("DestroyOldRertOBJ", 2f);
      
       
    }

    public void DestroyOldRertOBJ()
    {
        Destroy(revertObjs[0]);
        revertObjs[0] = revertObjs[1];
        revertObjs[1] = Instantiate(revertObjs[0], revertObjs[0].transform);
        revertObjs[1].transform.parent = null;
        revertObjs[1].SetActive(false);
    }


    public void ReturnToMainu()
    {
        SceneManager.LoadScene(0);
    }
}
