using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerObjectInteractionState_Holding : IPlayerObjectInteractionState 
{
        bool entered = false;
        bool placedGameObject = false;
        bool holdingSpace = false;

        float offsetFromWall = 0.0f; //for position billboards

        bool rayCastHittingVoid = true;

        int holdingMode = 0; //0 = spin, 1 = offset


    public IPlayerObjectInteractionState DoState(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(entered == false)
        {
            OnEnter(playerObjectInteraction);
            entered = true;
        }


        OnStay(playerObjectInteraction);

            if(placedGameObject == true)
            {
                OnExit(playerObjectInteraction);
                return playerObjectInteraction.PlayerObjectInteractionStateIdle;
            }
        return playerObjectInteraction.PlayerObjectInteractionStateHolding;
    }


    
        void OnEnter(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
      holdingMode = 0;

      //play picked up sound
      playerObjectInteraction.objectPlacementAudioSource.pitch = Random.Range(playerObjectInteraction.objectPickUpClipPitch_min, playerObjectInteraction.objectPickUpClipPitch_max);
      playerObjectInteraction.objectPlacementAudioSource.PlayOneShot(playerObjectInteraction.objectPickUpClip, 1.0f);

  holdingSpace = false;

      playerObjectInteraction.toolTipText.text = "Left Mouse: <color=green>Place <color=white>| RIght Mouse: <color=red>Cancel <color=white>| Scroll Wheel: Rotate | C: Unlock Mouse | Del: <color=red>Destroy";
    }

        void OnStay(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {

          //prevent object being held from collision being turned on (via reset or something)
          playerObjectInteraction.currentlyHeldObject.GetComponent<Collider>().enabled = false;




         if(Input.GetKey(KeyCode.C))
        {
                    Cursor.lockState = CursorLockMode.None;
                    playerObjectInteraction.mouseLooker.enabled = false;

                    holdingSpace = true;
        }
        else
        {
                    Cursor.lockState = CursorLockMode.Locked;
                    playerObjectInteraction.mouseLooker.enabled = true;

                    holdingSpace = false;
        }



          if(holdingSpace == true) //if holding space, make the ray come out of mouse pos
          {
              Ray ray = playerObjectInteraction.gameObject.transform.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
              if (Physics.Raycast(ray, out playerObjectInteraction.objectHitInfo, Mathf.Infinity, playerObjectInteraction.holding_collidableLayers_layerMask))
              {
                rayCastHittingVoid = false;
              }
              else
              {
                rayCastHittingVoid = true;

                playerObjectInteraction.currentlyHeldObject.transform.position = playerObjectInteraction.holdingObject_PickedUpPosition;
                playerObjectInteraction.currentlyHeldObject.transform.rotation = playerObjectInteraction.holdingObject_PickedUpRotation;
              }
          }
          else
          {
              if (Physics.Raycast(playerObjectInteraction.gameObject.transform.position, playerObjectInteraction.gameObject.transform.forward, out playerObjectInteraction.objectHitInfo, Mathf.Infinity, playerObjectInteraction.holding_collidableLayers_layerMask))
              {
                rayCastHittingVoid = false;
              }
              else
              {
                rayCastHittingVoid = true;

                playerObjectInteraction.currentlyHeldObject.transform.position = playerObjectInteraction.holdingObject_PickedUpPosition;
                playerObjectInteraction.currentlyHeldObject.transform.rotation = playerObjectInteraction.holdingObject_PickedUpRotation;
              }
          }
       
       //cancel placement
       
        if(Input.GetMouseButtonDown(1) && playerObjectInteraction.isCurrentlyHoldingObject == true && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
           {
            playerObjectInteraction.currentlyHeldObject.GetComponent<Collider>().enabled = true;


             playerObjectInteraction.currentlyHeldObject.transform.localPosition = playerObjectInteraction.holdingObject_PickedUpPosition;
             playerObjectInteraction.currentlyHeldObject.transform.localRotation = playerObjectInteraction.holdingObject_PickedUpRotation;
          
             playerObjectInteraction.currentlyHeldObject.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
             playerObjectInteraction.isCurrentlyHoldingObject = false;
             placedGameObject = true;


            //CANCEL DELETE
            if(playerObjectInteraction.newlyBoughtObjectHasBeenPlaced == false)
            {
              playerObjectInteraction.newlyBoughtObjectHasBeenPlaced = true;

                                  //play destory object sound
            playerObjectInteraction.objectPlacementAudioSource.pitch = Random.Range(playerObjectInteraction.objectDestroyClipPitch_min, playerObjectInteraction.objectDestroyClipPitch_max);
            playerObjectInteraction.objectPlacementAudioSource.PlayOneShot(playerObjectInteraction.objectDestroyClip, 1.0f);

            
              SaveAndLoadLevel.Instance.allLoadedGameObjects.Remove(playerObjectInteraction.currentlyHeldObject);
              GameObject.Destroy(playerObjectInteraction.currentlyHeldObject);
            }

            offsetFromWall = 0.0f;

            playerObjectInteraction.currentlyHeldObject = null;
           }
      


          //confirm location
           if(Input.GetMouseButtonDown(0) && playerObjectInteraction.isCurrentlyHoldingObject == true && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
           {

            playerObjectInteraction.newlyBoughtObjectHasBeenPlaced = true;

            //if location is at center, cancel instead
            if(playerObjectInteraction.currentlyHeldObject.transform.position == Vector3.zero)
            {

              //if the SAVED location of held object is at center... delete it(happens when item newly bought) ELSE just place normal
              if(playerObjectInteraction.holdingObject_PickedUpPosition == Vector3.zero)
              {
                playerObjectInteraction.objectPlacementAudioSource.pitch = Random.Range(playerObjectInteraction.objectDestroyClipPitch_min, playerObjectInteraction.objectDestroyClipPitch_max);
                playerObjectInteraction.objectPlacementAudioSource.PlayOneShot(playerObjectInteraction.objectDestroyClip, 1.0f);
                playerObjectInteraction.isCurrentlyHoldingObject = false;
                placedGameObject = true;

                SaveAndLoadLevel.Instance.allLoadedGameObjects.Remove(playerObjectInteraction.currentlyHeldObject);
                GameObject.Destroy(playerObjectInteraction.currentlyHeldObject);

                playerObjectInteraction.currentlyHeldObject = null;
              }
              else 
              {
                playerObjectInteraction.currentlyHeldObject.GetComponent<Collider>().enabled = true;

                playerObjectInteraction.currentlyHeldObject.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
                playerObjectInteraction.isCurrentlyHoldingObject = false;
                placedGameObject = true;
                playerObjectInteraction.currentlyHeldObject = null;
              }

            }
            else
            {

              playerObjectInteraction.currentlyHeldObject.GetComponent<Collider>().enabled = true;

              playerObjectInteraction.currentlyHeldObject.GetComponent<GeneralObjectInfo>().UpdatePosition();
              playerObjectInteraction.currentlyHeldObject.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
              UpdatePositionOfChildrenOfPlacedObject(playerObjectInteraction, playerObjectInteraction.currentlyHeldObject.transform);


              //placement animation
              DOTween.Kill(playerObjectInteraction.currentlyHeldObject.transform);
              playerObjectInteraction.currentlyHeldObject.transform.localScale = new Vector3(1,1,1);
              playerObjectInteraction.currentlyHeldObject.transform.DOPunchScale(new Vector3(0.1f,0.1f,0.1f), 0.2f, 5, 1f).SetEase(Ease.OutElastic);

              
              //play placement sound
              playerObjectInteraction.objectPlacementAudioSource.pitch = Random.Range(playerObjectInteraction.objectPlaceClipPitch_min, playerObjectInteraction.objectPlaceClipPitch_max);
              playerObjectInteraction.objectPlacementAudioSource.PlayOneShot(playerObjectInteraction.objectPlaceClip, 1.0f);

              playerObjectInteraction.isCurrentlyHoldingObject = false;

              placedGameObject = true;
              playerObjectInteraction.currentlyHeldObject = null;

              offsetFromWall = 0.0f;
            }
             }
        

        //delete while holding
                   if(Input.GetKey(KeyCode.Delete) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
        {
            playerObjectInteraction.currentlyHeldObject.GetComponent<GeneralObjectInfo>().RemoveThisChildFromParent();

            //stop the video when the poster is deleted...

            if(playerObjectInteraction.currentlyHeldObject.GetComponent<PosterMeshCreator>())
            {
              playerObjectInteraction.currentlyHeldObject.GetComponent<VLC_MediaPlayer>().StopVideoPlayerIfPosterWhereVideoIsPlayingWasChanged(playerObjectInteraction.currentlyHeldObject.GetComponent<PosterMeshCreator>());
            }

                      //play destory object sound
            playerObjectInteraction.objectPlacementAudioSource.pitch = Random.Range(playerObjectInteraction.objectDestroyClipPitch_min, playerObjectInteraction.objectDestroyClipPitch_max);
            playerObjectInteraction.objectPlacementAudioSource.PlayOneShot(playerObjectInteraction.objectDestroyClip, 1.0f);

             GameObject.Destroy(playerObjectInteraction.currentlyHeldObject);
             SaveAndLoadLevel.Instance.allLoadedGameObjects.Remove(playerObjectInteraction.currentlyHeldObject);

            playerObjectInteraction.newlyBoughtObjectHasBeenPlaced = true;


             playerObjectInteraction.isCurrentlyHoldingObject = false;
             placedGameObject = true;

             GlobalUtilityFunctions.UpdateChildrenObjectsOfAllObjectsInMap();
        }


        //pressing to place poster on camera view
          if(Input.GetKeyDown(KeyCode.F) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
        {
             playerObjectInteraction.currentlyHeldObject.GetComponent<Collider>().enabled = true;

            playerObjectInteraction.newlyBoughtObjectHasBeenPlaced = true;


              //placement animation
              DOTween.Kill(playerObjectInteraction.currentlyHeldObject.transform);
              playerObjectInteraction.currentlyHeldObject.transform.localScale = new Vector3(1,1,1);
              playerObjectInteraction.currentlyHeldObject.transform.DOPunchScale(new Vector3(0.1f,0.1f,0.1f), 0.2f, 5, 1f).SetEase(Ease.OutElastic);

              
              //play placement sound
              playerObjectInteraction.objectPlacementAudioSource.pitch = Random.Range(playerObjectInteraction.objectPlaceClipPitch_min, playerObjectInteraction.objectPlaceClipPitch_max);
              playerObjectInteraction.objectPlacementAudioSource.PlayOneShot(playerObjectInteraction.objectPlaceClip, 1.0f);

              playerObjectInteraction.isCurrentlyHoldingObject = false;

              placedGameObject = true;

              offsetFromWall = 0.0f;


            //initiate more complex positioning logic if the thing being held was a poster. (cause it will align with view using an emulator process)
            if(playerObjectInteraction.currentlyHeldObject.GetComponent<PosterMeshCreator>())
            {
            GameObject cameraEmu = GameObject.Instantiate(playerObjectInteraction.ViewingFrameEmulator_StartPositionOnlyPrefab, new Vector3(0,0,0), Quaternion.identity);
            cameraEmu.GetComponent<ViewingFrameEmulator_StartPositionOnly>().InitiateCamera(playerObjectInteraction.currentlyHeldObject); 
            }
            else
            {
              playerObjectInteraction.currentlyHeldObject.transform.position = playerObjectInteraction.playerCamera.transform.position;
              playerObjectInteraction.currentlyHeldObject.transform.rotation = playerObjectInteraction.playerCamera.transform.rotation * Quaternion.Euler(playerObjectInteraction.currentlyHeldObject.GetComponent<HoldingObjectOffsetRotation>().positionWithView_offset);
            }


            playerObjectInteraction.currentlyHeldObject.GetComponent<GeneralObjectInfo>().UpdatePosition();
            playerObjectInteraction.currentlyHeldObject.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
            UpdatePositionOfChildrenOfPlacedObject(playerObjectInteraction, playerObjectInteraction.currentlyHeldObject.transform);


            playerObjectInteraction.currentlyHeldObject = null;
        }



        //switch modes (rotation, offset)
        if(Input.GetKeyDown(KeyCode.LeftShift) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
        {
          if(holdingMode == 0)
          {
            UINotificationHandler.Instance.SpawnNotification("Mode: offset");
            holdingMode++;
          }
          else if(holdingMode == 1)
          {
            UINotificationHandler.Instance.SpawnNotification("Mode: rotation");
            holdingMode = 0;
          }
        }






                 if(playerObjectInteraction.currentlyHeldObject)
          {
                  if(CheckIfIsBillboardOrOffsetMode(playerObjectInteraction))
                  {
                                  //off set from wall if it is a billboard
                          if (Input.GetAxis("Mouse ScrollWheel") > 0f && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false) // forward
                        {
                            offsetFromWall += 5.50f;
                        }
                        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false) // backwards
                        {
                            offsetFromWall -= 5.50f;
                        }
                  }
                  else
                  {
                                  //else, just rotate preview object
                          if (Input.GetAxis("Mouse ScrollWheel") > 0f && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false) // forward
                        {
                            playerObjectInteraction.rotationOffset+= 15.0f ;
                        }
                        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false) // backwards
                        {
                            playerObjectInteraction.rotationOffset -= 15.0f;
                        }

                  }
          }




          //object preview positioning
          if(playerObjectInteraction.currentlyHeldObject && rayCastHittingVoid == false)
          {

          playerObjectInteraction.currentlyHeldObject.transform.position = playerObjectInteraction.objectHitInfo.point + playerObjectInteraction.objectHitInfo.normal * playerObjectInteraction.wallOffset * (offsetFromWall + playerObjectInteraction.currentlyHeldObject.GetComponent<HoldingObjectOffsetRotation>().offsetFromWall);
         
          //if it is a billboard or offset mode... dont force rotation
          if(CheckIfIsBillboardOrOffsetMode(playerObjectInteraction) == false)
          {
          playerObjectInteraction.currentlyHeldObject.transform.rotation = (Quaternion.FromToRotation(playerObjectInteraction.currentlyHeldObject.transform.up, playerObjectInteraction.objectHitInfo.normal) * playerObjectInteraction.currentlyHeldObject.transform.rotation) * Quaternion.AngleAxis(playerObjectInteraction.rotationOffset, Vector3.up) * Quaternion.Euler(playerObjectInteraction.currentlyHeldObject.GetComponent<HoldingObjectOffsetRotation>().offset);
          }
          
         //forces the rotations into increments of 15
         playerObjectInteraction.currentlyHeldObject.transform.eulerAngles = new Vector3((Mathf.Round(playerObjectInteraction.currentlyHeldObject.transform.eulerAngles.x / 15.0f) * 15.0f), (Mathf.Round(playerObjectInteraction.currentlyHeldObject.transform.eulerAngles.y / 15.0f) * 15.0f), (Mathf.Round(playerObjectInteraction.currentlyHeldObject.transform.eulerAngles.z / 15.0f) * 15.0f));
      
          playerObjectInteraction.currentlyHeldObject.transform.rotation *= Quaternion.Euler(rotationOffset);
          /*
                //grid snapping
                var currentPos = playerObjectInteraction.objectHitInfo.point + playerObjectInteraction.objectHitInfo.normal * playerObjectInteraction.wallOffset;
                playerObjectInteraction.previewGameObject.transform.position =
                 new Vector3(Mathf.Round(currentPos.x * 100f) / 100f, Mathf.Round(currentPos.y  * 100f) / 100f, Mathf.Round(currentPos.z  * 100f) / 100f);
          */

          }

        rotationOffset = Vector3.zero;
        playerObjectInteraction.rotationOffset = 0.0f;


        }

        Vector3 rotationOffset; //mostly used to make sure gymbals are rotated by 90 degrees

        void OnExit(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
                playerObjectInteraction.newlyBoughtObjectHasBeenPlaced = true;

                entered = false; // reset
                placedGameObject = false;

                if(playerObjectInteraction.currentlyHeldObject)
                playerObjectInteraction.currentlyHeldObject.GetComponent<Collider>().enabled = true;


              Cursor.lockState = CursorLockMode.Locked;
              playerObjectInteraction.mouseLooker.enabled = true;
        }

  void PositionPosterOnView(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
            //For calculating height/width of frame
            Vector3 worldSpaceVertex_TopLeft = playerObjectInteraction.currentlyHeldObject.transform.TransformPoint(playerObjectInteraction.currentlyHeldObject.GetComponent<PosterMeshCreator>().vertices[3]);
            Vector3 worldSpaceVertex_BottomLeft = playerObjectInteraction.currentlyHeldObject.transform.TransformPoint(playerObjectInteraction.currentlyHeldObject.GetComponent<PosterMeshCreator>().vertices[2]);
            Vector3 worldSpaceVertex_BottomRight = playerObjectInteraction.currentlyHeldObject.transform.TransformPoint(playerObjectInteraction.currentlyHeldObject.GetComponent<PosterMeshCreator>().vertices[1]);

            float heightOfFrame = Vector3.Distance(worldSpaceVertex_BottomLeft, worldSpaceVertex_TopLeft);
            float widthOfFrame = Vector3.Distance(worldSpaceVertex_BottomLeft, worldSpaceVertex_BottomRight);

            float frustrumHeight = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * playerObjectInteraction.playerCamera.fieldOfView);
            float frustrumWide = frustrumHeight * playerObjectInteraction.playerCamera.aspect;
            
            float distance = 0;
            float frameAspect = widthOfFrame/heightOfFrame;
       
       
        if(playerObjectInteraction.currentlyHeldObject.GetComponent<PosterViewSettings>().alignmentMode == "fit")
        {
            if(frameAspect >= playerObjectInteraction.playerCamera.aspect)
            {
            distance = widthOfFrame / frustrumWide;
            }
            else
            {
            distance = heightOfFrame / frustrumHeight;
            }
        }
        if(playerObjectInteraction.currentlyHeldObject.GetComponent<PosterViewSettings>().alignmentMode == "fill")
        {
            if(frameAspect >= playerObjectInteraction.playerCamera.aspect)
            {
            distance = heightOfFrame / frustrumHeight;
            }
            else
            {
            distance = widthOfFrame / frustrumWide;
            }
        }


        distance += playerObjectInteraction.currentlyHeldObject.GetComponent<PosterMeshCreator>().distanceFromWall; // offset from object base
            
        playerObjectInteraction.playerCamera.transform.position = playerObjectInteraction.currentlyHeldObject.transform.position - distance * -playerObjectInteraction.currentlyHeldObject.transform.up;
        playerObjectInteraction.playerCamera.transform.SetParent(playerObjectInteraction.currentlyHeldObject.transform);

        //setting these euler angles makes the camera parented to the poster, face at the poster
        Quaternion desiredRotation = Quaternion.Euler(90f, 0f, 0f);
        playerObjectInteraction.playerCamera.transform.localRotation = desiredRotation;
        }


        //RECURSIVE FUNCTION TO UPDATE ALL HIERCHIES OF PLACED OBJECT
     private void UpdatePositionOfChildrenOfPlacedObject(PlayerObjectInteractionStateMachine playerObjectInteraction, Transform parent)
    {
        foreach (Transform child in parent)
        {
            //make sure its an entity and not just any object!!!
            if(child.GetComponent<GeneralObjectInfo>()) 
            child.GetComponent<GeneralObjectInfo>().UpdatePosition();

            // Recursively update children of this child
            UpdatePositionOfChildrenOfPlacedObject(playerObjectInteraction, child);
        }
    }

    //for multiple usage in if statements
    private bool CheckIfIsBillboardOrOffsetMode(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(playerObjectInteraction.currentlyHeldObject.GetComponent<PosterBillboard>() 
        &&  (playerObjectInteraction.currentlyHeldObject.GetComponent<PosterBillboard>().useBillboard 
        || playerObjectInteraction.currentlyHeldObject.GetComponent<PosterBillboard>().useCharacterBillboard) 
        || holdingMode == 1)
        {
          return true;
        }
        else
        {
          return false;
        }

    }


}
