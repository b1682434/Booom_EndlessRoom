using System.Collections;
using System.Collections.Generic;
using UnityEngine;


  public interface IInteractRequest//���
    {
        void InteractRequest(int ItemID);

    //void MouseOver();
    

        public int ObjectType { get; }//��Ʒid

    public string returnWord { get; }//����֮��˵ɶ
    //public string objectName { get;}//�о��ӿں���Ҫ����û��


}

public interface IMouseOver
{

    public string returnWord { get; }//���������˵ɶ
    void MouseOver();
}



