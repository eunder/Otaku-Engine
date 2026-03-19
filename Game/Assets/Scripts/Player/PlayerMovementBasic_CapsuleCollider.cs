using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBasic_CapsuleCollider : MonoBehaviour
{

    //used to keep the wall detectiong jumper trigger away(to prevent multiple OnEnter/Exit events from triggering)


   void OnTriggerStay(Collider other)
   {
     if(other.gameObject.layer == 14){
        PlayerMovementBasic.Instance.blockAboutToBeUsed = other.gameObject;
        PlayerMovementBasic.Instance.canWallJump = true;
     }
 }
 void OnTriggerExit(Collider other)
 {
     if(other.gameObject.layer == 14){
    PlayerMovementBasic.Instance.canWallJump = false;   
  }
 }

}
