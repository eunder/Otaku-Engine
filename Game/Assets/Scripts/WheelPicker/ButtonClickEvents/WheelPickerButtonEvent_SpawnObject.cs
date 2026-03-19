using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPickerButtonEvent_SpawnObject : MonoBehaviour
{
    public GameObject objectPrefabToSpawn;
    private bool objectReady = false;
    GameObject obj;
        public void SpawnNewlyBoughtObject()
    {
         obj = Instantiate(objectPrefabToSpawn, new Vector3(0f,0f,0f), Quaternion.identity);
        Invoke("theRest"  , 0.0f);  // THERE SEEMS TO BE A PROBLEM WHEN THE MESH BEING CREATED TOO LATE AND CAUSES THE TRIGGER COLLIDER THING TO MESS UP!!!
    }
PlayerObjectInteractionStateMachine playerInteractionMachine;
void Start()
{
           playerInteractionMachine = GameObject.Find("MainPlayerCam").GetComponent<PlayerObjectInteractionStateMachine>();
}
      public void theRest()
      {

        //make player pick it up
              playerInteractionMachine.enabled = true;
              playerInteractionMachine.isCurrentlyHoldingObject = true;
              playerInteractionMachine.currentlyHeldObject = obj;
              playerInteractionMachine.currentlyHeldObject.name = System.Guid.NewGuid().ToString();

              playerInteractionMachine.holdingObject_PickedUpPosition = Vector3.zero;
              playerInteractionMachine.holdingObject_PickedUpRotation = Quaternion.identity;
              

              //loop through children and update the general info
              for(int i = 0; i < playerInteractionMachine.currentlyHeldObject.transform.childCount; i++)
              {
                if(playerInteractionMachine.currentlyHeldObject.transform.GetChild(i).GetComponent<GeneralObjectInfo>())
                {
                playerInteractionMachine.currentlyHeldObject.transform.GetChild(i).name = System.Guid.NewGuid().ToString();
                playerInteractionMachine.currentlyHeldObject.transform.GetChild(i).GetComponent<GeneralObjectInfo>().UpdateID();
                playerInteractionMachine.currentlyHeldObject.transform.GetChild(i).GetComponent<GeneralObjectInfo>().UpdateParent();
                }
              }

              playerInteractionMachine.currentlyHeldObject.GetComponent<GeneralObjectInfo>().UpdateID();
              playerInteractionMachine.currentlyHeldObject.GetComponent<GeneralObjectInfo>().UpdateParent();
              playerInteractionMachine.currentlyHeldObject.GetComponent<GeneralObjectInfo>().UpdateChildren();



              if(playerInteractionMachine.currentlyHeldObject.GetComponent<PosterMeshCreator>())
              {
             // playerInteractionMachine.currentlyHeldObject.GetComponent<PosterMeshCreator>().AssignNewMatInstance();
             // playerInteractionMachine.currentlyHeldObject.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial = playerInteractionMachine.currentlyHeldObject.GetComponent<PosterMeshCreator>().imageMat; //without this, there would be no material(pink)
           
              playerInteractionMachine.currentlyHeldObject.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial.name = playerInteractionMachine.currentlyHeldObject.GetComponent<GeneralObjectInfo>().id;

              SaveAndLoadLevel.Instance.allLoadedPosters.Add(playerInteractionMachine.currentlyHeldObject);
              }


              playerInteractionMachine.currentlyHeldObject.GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();

                    
              foreach (Transform child in playerInteractionMachine.currentlyHeldObject.transform)
              {
                if(child.GetComponent<GeneralObjectInfo>())
                child.GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();
              }

    
              playerInteractionMachine.pickedUpObject = true;
              playerInteractionMachine.currentlyHeldObject.GetComponent<Collider>().enabled = false;




              //enable xray mode
              if(objectPrefabToSpawn.layer == 16)
              {
                ObjectVisibilityViewManager.Instance.currentViewMode = 1;
                ObjectVisibilityViewManager.Instance.TriggerCurrentViewModeFunctions();
              }

              //for make sure the object is deleted on cancel
              PlayerObjectInteractionStateMachine.Instance.newlyBoughtObjectHasBeenPlaced = false;



              //close wheel
                GameObject wheelPicker = GameObject.Find("CANVAS_WHEELPICKER");
                wheelPicker.GetComponent<WheelPickerHandler>().CloseWheelPicker();

              //play bought item sound
            //  wheelPicker.GetComponentInChildren<WheelPickerSoundEvents>().AudioEvent_BoughtItem();
    
    
      }

}
