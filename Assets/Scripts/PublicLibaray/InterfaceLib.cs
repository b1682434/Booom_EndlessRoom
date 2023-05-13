using System.Collections;
using System.Collections.Generic;
using UnityEngine;


  public interface IInteractRequest
    {
        void InteractRequest(int ItemID);

    void MouseOver();
    

        public int ObjectType { get; }

    public string returnWord { get; }
    public string objectName { get;}


}



