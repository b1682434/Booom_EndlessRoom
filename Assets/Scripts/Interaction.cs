using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

 
public class Interaction : MonoBehaviour
{
        FirstPersonController fps;

        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        public LayerMask portalLayerMask;
    public LayerMask itemsLayerMask;
    public float interLength = 5f;
    public GameObject cineMachineTarget;
        Collider collToIgnore;
    bool movingThroughPortal;
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
        }

 
         void Update()
    {
        if (!movingThroughPortal)
        {
            if (_input.jump)
            {
                _input.jump = false;
                PortalInteract();

            }
            if (_input.esc)
            {
                _input.esc = false;
                FindAnyObjectByType<GameManger>().ReturnToMainu();
            }
        }
           
         }
        void PortalInteract()
        {
            RaycastHit hit;
            if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, interLength, portalLayerMask))
            {
                fps.enabled = false;
                BasicPortal thisPortal = hit.collider.gameObject.GetComponent<BasicPortal>();
                Vector3 thisPortalPos = hit.collider.gameObject.transform.position;
                Vector3 otherPortalPos = thisPortal.otherPort.transform.position;
                Vector3 posOffset = thisPortalPos - transform.position;
                Vector3 potraledPos = otherPortalPos - posOffset;
                transform.position = potraledPos;
            doorFacingVec = hit.normal;
                collToIgnore = thisPortal.otherPort.coll;
                Physics.IgnoreCollision(GetComponent<CharacterController>(), collToIgnore,true);
                thisPortal.otherPort.OpenDoorAnim();


            FindFirstObjectByType<GameManger>().revertAll();
            
           // FindAnyObjectByType<GameManger>().revertAll();
            StartCoroutine(MoveThroughDoor(thisPortal.otherPort.enterPoint.position,thisPortal.otherPort.startPoint.position));
            movingThroughPortal = true;
        }
            else
            {
               ItemsInteraction();
            }

        }

    void ItemsInteraction()
    {
        RaycastHit hit;
        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, interLength, itemsLayerMask))
        {
           
            
        }
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
            }

            if (i>100)
            {
                    StopAllCoroutines();
                    Physics.IgnoreCollision(GetComponent<CharacterController>(), collToIgnore, false);
                    fps.enabled = true;
                FindAnyObjectByType<GameManger>().StartMovingAll();
                movingThroughPortal = false;
            }
                
                yield return new WaitForSeconds(0.02f);
            }
        }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(cineMachineTarget.transform.position, cineMachineTarget.transform.position + cineMachineTarget.transform.forward * interLength);
    }
}

