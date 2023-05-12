using System.Collections;
using System.Collections.Generic;
using UnityEngine;


  public interface IInteractRequest
    {
        void InteractRequest(int ItemID);
        public int ObjectType { get; set; }

}



