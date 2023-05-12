using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

 
public class Interaction : MonoBehaviour
{
     FirstPersonController fps;
   private StarterAssetsInputs _input;
   private GameObject _mainCamera;

    public LayerMask portalLayerMask;
    
    public float interLength = 5f;
    public GameObject cineMachineTarget;
    [Header("互动模式下的瞄准图标相关")]
    public Image aimUI;
   
    public float aimUIMovSpeed;
    Vector2 defaultAimUIPos;
        Collider collToIgnore;
  public  playerInputState pinputState = playerInputState.Walking;
  
    SelectableObject focusingObj;
    Vector3 doorFacingVec;
        private void Awake()
        {
           
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }
        void Start()
        {
            fps = GetComponent<FirstPersonController>();
            _input = GetComponent<StarterAssetsInputs>();
        defaultAimUIPos = aimUI.rectTransform.position;
        }

 
    void Update()
    {
        /* if (_input.esc)
         {
             _input.esc = false;
             FindAnyObjectByType<GameManger>().ReturnToMainu();
         }*/
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Vector3 uiWorldPos = Camera.main.ScreenToWorldPoint(aimUI.transform.position);
        Debug.DrawLine(Camera.main.transform.position, uiWorldPos);
        /*if(Physics.Raycast(ray,out hit))
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point,Color.red);
        }
        else
        {
            Debug.DrawLine(Camera.main.transform.position, ray.direction * 20,Color.green);
        }*/
       
        switch (pinputState)
        {
            case playerInputState.Walking:

                if (_input.jump)
                {
                    _input.jump = false;
                   // PortalInteract();
                }
                if (_input.confirm)
                {
                    _input.confirm = false;
                    Interact();
                }
                break;
            case playerInputState.Interacting:
                FocusMode();
               
                break;
            case playerInputState.Donothing:
                break;
        }
        
           
         }
        /*void PortalInteract()
        {
            RaycastHit hit;
            if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, interLength, portalLayerMask))
            {
              BasicPortal thisPortal = hit.collider.gameObject.GetComponent<BasicPortal>();
              if (thisPortal != null)
              {
                fps.enabled = false;

                Vector3 thisPortalPos = hit.collider.gameObject.transform.position;
                Vector3 otherPortalPos = thisPortal.otherPort.transform.position;
                Vector3 posOffset = thisPortalPos - transform.position;
                Vector3 potraledPos = otherPortalPos - posOffset;
                transform.position = potraledPos;
                doorFacingVec = hit.normal;
                collToIgnore = thisPortal.otherPort.coll;
                Physics.IgnoreCollision(GetComponent<CharacterController>(), collToIgnore, true);
                //thisPortal.otherPort.OpenDoorAnim();
                FindFirstObjectByType<GameManger>().revertAll();
                //StartCoroutine(MoveThroughDoor(thisPortal.otherPort.enterPoint.position, thisPortal.otherPort.startPoint.position));
                pinputState = playerInputState.Donothing;
              }

        }
        else
        {
            Interact();
        }

        }*/

    void Interact()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(aimUI.transform.position);
        if(Physics.Raycast(ray, out hit, interLength))
        {
            focusingObj = hit.transform.GetComponent<SelectableObject>();
            if (focusingObj != null)
            {
                focusingObj.BeFocus();
                fps.enabled = false;
                pinputState = playerInputState.Interacting;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;

            }
        }
    }
    void FocusMode()
    {
       
        aimUI.rectTransform.position = Mouse.current.position.ReadValue();
       /* Vector2 inputDir = _input.look.normalized * new Vector2(1, -1)*aimUIMovSpeed*Time.deltaTime;
        float rectPosX = Mathf.Clamp(aimUI.rectTransform.anchoredPosition.x + inputDir.x, -Screen.width / 2, Screen.width / 2);
        float rectPosY = Mathf.Clamp(aimUI.rectTransform.anchoredPosition.y + inputDir.y, -Screen.height / 2, Screen.height / 2);
        aimUI.rectTransform.anchoredPosition = new Vector2(rectPosX,rectPosY);*/

        if (_input.confirm)
        {
            _input.confirm = false;
               RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(aimUI.transform.position);
            if (Physics.Raycast(ray, out hit))
            {
            }
        }
        if (_input.jump)
        {
            _input.jump = false;
            
            ExitFocusMode();
           
        }


    }
    public void ExitFocusMode()
    {
        if (focusingObj != null)
        {
            focusingObj.DeFocus();
            fps.enabled = true;
            focusingObj = null;
        }
        aimUI.rectTransform.position= defaultAimUIPos;
        print("exed");
        Cursor.lockState = CursorLockMode.Locked;
        pinputState = playerInputState.Walking;
    }
 
        
        IEnumerator MoveThroughDoor(Vector3 doorPointPos,Vector3 startPointPos)
        {
        int i = 0;
            for (; ; )
            {
            i += 1;
            if (i >50)
            {
                transform.position = Vector3.Lerp(transform.position, doorPointPos, Mathf.Clamp((i - 50f)/50f,0,1));//移动
                
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, startPointPos, i/50f); //到门前
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.forward, doorFacingVec * -1), i / 50f);
                //cineMachineTarget.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.forward, doorFacingVec * -1), i / 50f);//另外一个分开/目前代码导致的问题：rotation无法完全控制！
            }

            if (i>100)
            {
                    StopAllCoroutines();
                    Physics.IgnoreCollision(GetComponent<CharacterController>(), collToIgnore, false);
                    fps.enabled = true;
               // FindAnyObjectByType<GameManger>().StartMovingAll();
                pinputState = playerInputState.Walking;
            }
                
                yield return new WaitForSeconds(0.02f);
            }
        }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        //Gizmos.DrawLine(cineMachineTarget.transform.position, cineMachineTarget.transform.position + cineMachineTarget.transform.forward * interLength);
        //StaticFunctions.RectTransformToScreenSpace(aimUI.rectTransform, Camera.main, false);
        // Gizmos.DrawRay(RectTransformUtility.ScreenPointToRay(Camera.main, aimUI.rectTransform.position));

        /* Gizmos.DrawSphere(Camera.main.transform.position, 0.3f);
         Ray ray = Camera.main.ScreenPointToRay(aimUI.transform.position);
         Gizmos.DrawSphere(ray.direction * 10 + Camera.main.transform.position, 2f);
         Gizmos.DrawLine(Camera.main.transform.position, ray.direction * 10 + Camera.main.transform.position);*/
        
    }
}

