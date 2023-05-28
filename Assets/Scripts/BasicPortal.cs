using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPortal : InteractableBase,IInteractRequest
{

    public bool notFirstDoor = true;
    public Transform FrontPoint;
    public Transform BackPoint;
    public BasicPortal otherPortal;
    public Collider coll;
   public Animation anim;
    public AudioSource au;
    
    GameManger gm;
    Vector3 revertObjoffset;//记录与轮回物品的相对位置。方便生成的时候能够让轮回物品精确的生成在同一相对位置
    Vector3 otherPortalOffset;//记录与另一扇门的相对位置。方便在生成的时候把另一扇门移到精确的相对位置


    private void Awake()
    {
        coll = GetComponent<Collider>();
        anim = GetComponent<Animation>();
        gm = FindFirstObjectByType<GameManger>();
        
    }
    void Start()
    {
        revertObjoffset = gm.currentRevertObj.transform.position - (transform.position+otherPortal.transform.position)/2;
        otherPortalOffset = new Vector3(Mathf.Abs ((transform.position - otherPortal.transform.position).x),0,0) ;
    }
    
   public void InteractRequest(int ItemID)
    {
        au.Play();
        if (!anim.Play())
        {
          
            anim.Play();//在动画里记录碰撞体的开启关闭
            
            
        }
        
    }
    public Vector3[] DoorWayPoints(Vector3 RequestedSourcePos)
    {
        Vector3[] wayPoints = new Vector3[2];
        Vector3 rspDir = RequestedSourcePos - transform.position;//玩家大概的面朝方向
        float angle = Vector3.Angle(transform.right, rspDir);
        
        
            if (angle > 90)//没线的一面。线在gizmo里面画的
            {
                wayPoints[1] = FrontPoint.position;
                wayPoints[0] = BackPoint.position;
           
            if (notFirstDoor)
            {
                otherPortal.transform.position = transform.position + otherPortalOffset;
                gm.GoThroughDoor((transform.position + otherPortal.transform.position) / 2+revertObjoffset);
               
            }
        }
            else//有线的一面
            {
          
                wayPoints[0] = FrontPoint.position;
                wayPoints[1] = BackPoint.position;
            if (notFirstDoor)
            {
                otherPortal.transform.position = transform.position - otherPortalOffset;
                gm.GoThroughDoor((transform.position + otherPortal.transform.position) / 2 + revertObjoffset);

            }
        }
        
        
        return wayPoints;
    }//给出玩家路径图，并叫gamemanger生成新关卡

   

    /* public Vector3 PlayerTeleportTargetPos(Vector3 playerPos)
     {
         Vector3 playerOffsetToMe = playerPos - transform.position;
         return (playerOffsetToMe + otherPortal.transform.position);
     }*/

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 3);
    }

}
