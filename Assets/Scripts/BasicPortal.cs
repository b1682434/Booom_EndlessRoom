using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPortal : MonoBehaviour,IInteractRequest
{

    public bool notFirstDoor = true;
    public Transform FrontPoint;
    public Transform BackPoint;
    public BasicPortal otherPortal;
    public Collider coll;
   public Animation anim;
    
    GameManger gm;
    Vector3 revertObjoffset;//��¼���ֻ���Ʒ�����λ�á��������ɵ�ʱ���ܹ����ֻ���Ʒ��ȷ��������ͬһ���λ��
    Vector3 otherPortalOffset;//��¼����һ���ŵ����λ�á����������ɵ�ʱ�����һ�����Ƶ���ȷ�����λ��
    public int ObjectType { get; set; }

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
        if (!anim.Play())
        {
            anim.Play();//�ڶ������¼��ײ��Ŀ����ر�
        }
        
    }
    public Vector3[] DoorWayPoints(Vector3 RequestedSourcePos)
    {
        Vector3[] wayPoints = new Vector3[2];
        Vector3 rspDir = RequestedSourcePos - transform.position;//��Ҵ�ŵ��泯����
        float angle = Vector3.Angle(transform.right, rspDir);
        
        
            if (angle > 90)//û�ߵ�һ�档����gizmo���滭��
            {
                wayPoints[1] = FrontPoint.position;
                wayPoints[0] = BackPoint.position;
           
            if (notFirstDoor)
            {
                otherPortal.transform.position = transform.position + otherPortalOffset;
                gm.GoThroughDoor((transform.position + otherPortal.transform.position) / 2+revertObjoffset);
               
            }
        }
            else//���ߵ�һ��
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
    }
    

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
