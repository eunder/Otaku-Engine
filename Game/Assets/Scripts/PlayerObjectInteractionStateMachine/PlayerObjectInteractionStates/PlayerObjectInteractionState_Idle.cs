using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using System;
using System.Threading.Tasks;
using System.IO;

public class PlayerObjectInteractionState_Idle : IPlayerObjectInteractionState 
{
        bool entered = false;
        bool clickedOnDoor = false; //use to transition to ClickedOnDoor State
        bool clickedToViewPoster = false;
        float door_hold = 0f;


    public IPlayerObjectInteractionState DoState(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(entered == false)
        {
            OnEnter(playerObjectInteraction);
            entered = true;
        }


        OnStay(playerObjectInteraction);

            if(playerObjectInteraction.pickedUpObject == true || playerObjectInteraction.isCurrentlyHoldingObject)
            {
                OnExit(playerObjectInteraction);
                return playerObjectInteraction.PlayerObjectInteractionStateHolding;
            }
            if( clickedOnDoor == true)
            {
                OnExit(playerObjectInteraction);
                return playerObjectInteraction.PlayerObjectInteractionStateClickedOnDoor;
            }
            if( clickedToViewPoster == true)
            {
                OnExit(playerObjectInteraction);
                return playerObjectInteraction.PlayerObjectInteractionStateViewingFrame;
            }
        return playerObjectInteraction.PlayerObjectInteractionStateIdle;
    }


    
        void OnEnter(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
          playerObjectInteraction.centerDot_Canvas.SetActive(true);

      clickedToViewPoster = false;
    }

        void OnStay(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {


         if (Physics.Raycast(playerObjectInteraction.gameObject.transform.position, playerObjectInteraction.gameObject.transform.forward, out playerObjectInteraction.objectHitInfo, Mathf.Infinity, playerObjectInteraction.collidableLayers_layerMask) && WheelPickerHandler.Instance &&!WheelPickerHandler.Instance.wheelPickerIsOpen
         && SaveAndLoadLevel.Instance.isLevelLoaded == true)
        { 

            //Click notifier mechanic
            if(playerObjectInteraction.isCurrentlyHoldingObject == false 
            && DialogueBoxStateMachine.Instance.dialogueObjectList.Count <= 0 
            && DialogueChoiceManager.Instance.dialogueChoiceBoxIsActive == false
            && VLC_MediaPlayer_ControlsGUI_ButtonToggleCanvas.Instance.isActive == false
            && EscapeToggleToolBar.Instance.toolbar.activeSelf == false
            && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false
            && InputEventManager_Counter.Instance.InputEventManagerWindow.activeSelf == false
            && InputEventManager_String.Instance.InputEventManagerWindow.activeSelf == false)
           {
                      if(GlobalUtilityFunctions.IsWithinClickRange(playerObjectInteraction.objectHitInfo.distance, playerObjectInteraction.objectHitInfo.transform.gameObject))
                      {
                          playerObjectInteraction.ClickerNotifier_gameObject.SetActive(true);
                          playerObjectInteraction.ClickerNotifier_gameObject.transform.position = playerObjectInteraction.objectHitInfo.point;
                      }
                      else
                      {
                            playerObjectInteraction.ClickerNotifier_gameObject.SetActive(false);
                      }

           }
           else
           {
            playerObjectInteraction.ClickerNotifier_gameObject.SetActive(false);
           }











            if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<CustomeItemType>())
            {

            // click to view frame OR play video                                                                                                                                                                    //if there is no dialogue or dialogue choices on screen
            if(Input.GetMouseButtonDown(0) && playerObjectInteraction.isCurrentlyHoldingObject == false && playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<PosterMeshCreator>() && DialogueBoxStateMachine.Instance.dialogueObjectList.Count <= 0 && DialogueChoiceManager.Instance.dialogueChoiceBoxIsActive == false
            && VLC_MediaPlayer_ControlsGUI_ButtonToggleCanvas.Instance.isActive == false
            && EscapeToggleToolBar.Instance.toolbar.activeSelf == false
            && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false
            && InputEventManager_Counter.Instance.InputEventManagerWindow.activeSelf == false
            && InputEventManager_String.Instance.InputEventManagerWindow.activeSelf == false)
           {
                GlobalUtilityFunctions.EvaluateClickRange(playerObjectInteraction.objectHitInfo.distance, playerObjectInteraction.objectHitInfo.transform.gameObject);
           }



            // click on door
            if(Input.GetMouseButtonDown(0) && playerObjectInteraction.isCurrentlyHoldingObject == false && playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<CustomeItemType>().itemType == CustomeItemType.TypeOfItem.door)
           {
             //clickedOnDoor = true;
            // GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<DoorInfo>().pathFileToLoad;

            //playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<DoorLoadRoomPreview>().ClickedToLoadPreview(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<DoorInfo>().pathFileToLoad);
           // GameObject.Find("DoorLoadRoomPreview").GetComponent<DoorLoadRoomPreview>().ClickedToLoadPreview(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<DoorInfo>().pathFileToLoad, playerObjectInteraction.objectHitInfo.transform.gameObject);
           }
            }

            //click on block
            if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>())
            {
                if(Input.GetMouseButtonDown(0) && DialogueBoxStateMachine.Instance.dialogueObjectList.Count <= 0 && DialogueChoiceManager.Instance.dialogueChoiceBoxIsActive == false
                && VLC_MediaPlayer_ControlsGUI_ButtonToggleCanvas.Instance.isActive == false
                && EscapeToggleToolBar.Instance.toolbar.activeSelf == false
                && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false
                && InputEventManager_Counter.Instance.InputEventManagerWindow.activeSelf == false
                && InputEventManager_String.Instance.InputEventManagerWindow.activeSelf == false)
                {                
                  GlobalUtilityFunctions.EvaluateClickRange(playerObjectInteraction.objectHitInfo.distance, playerObjectInteraction.objectHitInfo.transform.gameObject);
                }
            }


            //hover over dome element
            if(playerObjectInteraction.objectHitInfo.transform.tag == "DomeElement" && playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<OnPlayerRayHover>())
            {
              playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<OnPlayerRayHover>().onHover = true;

            //on click while hovering
                  if(Input.GetMouseButtonDown(0))
            {
              playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<OnPlayerClick>().onClickElementActivateEvent();
            }
            }


            //edit object
        if(Input.GetKeyDown(KeyCode.LeftShift) && EditModeStaticParameter.isInEditMode == true && ItemEditStateMachine.Instance.currentObjectEditing == null)
           {
               if(playerObjectInteraction.objectHitInfo.transform.tag == "Item" && ItemEditStateMachine.Instance.currentState == ItemEditStateMachine.Instance.ItemEditStateMachineStateIdle)
            {
              if(playerObjectInteraction.objectHitInfo.transform.GetComponent<PosterMeshCreator>())
              {
                playerObjectInteraction.currentlyHeldObject = playerObjectInteraction.objectHitInfo.transform.gameObject;
                playerObjectInteraction.itemEditorStateMachine.target = playerObjectInteraction.currentlyHeldObject.GetComponent<CustomeItemType>().transform;
                playerObjectInteraction.itemEditorStateMachine.currentObjectEditing = playerObjectInteraction.currentlyHeldObject;
                playerObjectInteraction.itemEditorStateMachine.currentItemType = CustomeItemType.TypeOfItem.poster;
              }

              if(playerObjectInteraction.objectHitInfo.transform.GetComponent<DoorInfo>())
              {
                playerObjectInteraction.currentlyHeldObject = playerObjectInteraction.objectHitInfo.transform.gameObject;
                playerObjectInteraction.itemEditorStateMachine.target = playerObjectInteraction.currentlyHeldObject.GetComponent<CustomeItemType>().transform;
                playerObjectInteraction.itemEditorStateMachine.currentObjectEditing = playerObjectInteraction.currentlyHeldObject;
                playerObjectInteraction.itemEditorStateMachine.currentItemType = CustomeItemType.TypeOfItem.door;
              }
            }
   
            if(playerObjectInteraction.objectHitInfo.transform.GetComponent<BlockFaceTextureUVProperties>())
            {
              playerObjectInteraction.itemEditorStateMachine.currentObjectEditing = playerObjectInteraction.objectHitInfo.transform.gameObject;
            }
            if(playerObjectInteraction.objectHitInfo.transform.GetComponent<DialogueContentObject>())
            {
                playerObjectInteraction.itemEditorStateMachine.currentObjectEditing = playerObjectInteraction.objectHitInfo.transform.gameObject;
            }
            if(playerObjectInteraction.objectHitInfo.transform.GetComponent<AudioSourceObject>())
            {
                playerObjectInteraction.itemEditorStateMachine.currentObjectEditing = playerObjectInteraction.objectHitInfo.transform.gameObject;
            }
              if(playerObjectInteraction.objectHitInfo.transform.GetComponent<Counter>())
            {
                playerObjectInteraction.itemEditorStateMachine.currentObjectEditing = playerObjectInteraction.objectHitInfo.transform.gameObject;
            }
            if(playerObjectInteraction.objectHitInfo.transform.GetComponent<State>())
            {
                playerObjectInteraction.itemEditorStateMachine.currentObjectEditing = playerObjectInteraction.objectHitInfo.transform.gameObject;
            }
            if(playerObjectInteraction.objectHitInfo.transform.GetComponent<PathNode>())
            {
                playerObjectInteraction.itemEditorStateMachine.currentObjectEditing = playerObjectInteraction.objectHitInfo.transform.gameObject;
            }
            if(playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>())
            {
                playerObjectInteraction.itemEditorStateMachine.currentObjectEditing = playerObjectInteraction.objectHitInfo.transform.gameObject;
            }
            if(playerObjectInteraction.objectHitInfo.transform.GetComponent<Date>())
            {
                playerObjectInteraction.itemEditorStateMachine.currentObjectEditing = playerObjectInteraction.objectHitInfo.transform.gameObject;
            }
            if(playerObjectInteraction.objectHitInfo.transform.GetComponent<LightEntity>())
            {
                playerObjectInteraction.itemEditorStateMachine.currentObjectEditing = playerObjectInteraction.objectHitInfo.transform.gameObject;
            }
            if(playerObjectInteraction.objectHitInfo.transform.GetComponent<PlayerMoverEventComponent>())
            {
                playerObjectInteraction.itemEditorStateMachine.currentObjectEditing = playerObjectInteraction.objectHitInfo.transform.gameObject;
            }
            if(playerObjectInteraction.objectHitInfo.transform.GetComponent<PrefabEntity>())
            {
                playerObjectInteraction.itemEditorStateMachine.currentObjectEditing = playerObjectInteraction.objectHitInfo.transform.gameObject;
            }
           }

          //tooltip stuff
          if(playerObjectInteraction.toolTipText)
          {
            playerObjectInteraction.toolTipText.text = "Q: Tool Menu | C: Video Controls | V: Fly mode | G: View Mode";
          //  if(playerObjectInteraction.centerDot_Canvas)
          //  playerObjectInteraction.centerDot_Canvas.SetActive(true);

            if(playerObjectInteraction.objectHitInfo.transform.tag == "Item" && playerObjectInteraction.objectHitInfo.transform.GetComponent<PosterMeshCreator>())
            {
            playerObjectInteraction.toolTipText.text = "Left Mouse: <color=green>view <color=white> | RIght Mouse: Pick up | Shift: edit";
          //  if(playerObjectInteraction.centerDot_Canvas)
          //  playerObjectInteraction.centerDot_Canvas.SetActive(false);
            }
            if(playerObjectInteraction.objectHitInfo.transform.tag == "Item" && playerObjectInteraction.objectHitInfo.transform.GetComponent<DoorInfo>())
            {
            playerObjectInteraction.toolTipText.text = "<color=white>RIght Mouse: Pick up | Shift: edit";
            }
            if(playerObjectInteraction.objectHitInfo.transform.gameObject.name == "ButtonLoadRoom")
            {
              playerObjectInteraction.toolTipText.text = "Left Mouse: <color=green>Enter Room";
            }
            if(playerObjectInteraction.objectHitInfo.transform.gameObject.name == "ButtonLoadRoomLoadRoomPreview")
            {
              playerObjectInteraction.toolTipText.text = "Left Mouse: <color=green>Load Preview";
            }

          }


        //hold to start going through door
        if(Input.GetMouseButton(0) && playerObjectInteraction.objectHitInfo.transform.gameObject.name == "ButtonLoadRoom" && WorkshopDownloadProgressBar.Instance.downloadingItemFound == false)
            {
              door_hold += Time.deltaTime;
              playerObjectInteraction.toolTipText.text = "  OPENING DOOR...";
            }
            else
            {
              door_hold = 0.0f;
            }

        //load room preview
        if(Input.GetMouseButton(0) && playerObjectInteraction.objectHitInfo.transform.gameObject.name == "ButtonLoadRoomLoadRoomPreview")
            {
            GameObject.Find("DoorLoadRoomPreview").GetComponent<DoorLoadRoomPreview>().ClickedToLoadPreview(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInParent<DoorInfo>().pathFileToLoad, playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInParent<DoorInfo>().transform.gameObject);
            }


        }
        else //looking into void
        {

          playerObjectInteraction.ClickerNotifier_gameObject.SetActive(false);


          if(playerObjectInteraction.toolTipText != null)
            playerObjectInteraction.toolTipText.text = "Q: Tool Menu | C: Video Controls | V: Fly mode | G: View Mode";
            if(playerObjectInteraction.centerDot_Canvas)
            playerObjectInteraction.centerDot_Canvas.SetActive(true);
        }


  //PICKING OBJECT UP MECHANIC. needs to be its own raycast wrap because it uses a different layermask. so that blocks with lighting shaders dont get in the way
  if (Physics.Raycast(playerObjectInteraction.gameObject.transform.position, playerObjectInteraction.gameObject.transform.forward, out playerObjectInteraction.objectHitInfo, Mathf.Infinity, playerObjectInteraction.holding_collidableLayers_layerMask) 
        && WheelPickerHandler.Instance 
        &&!WheelPickerHandler.Instance.wheelPickerIsOpen 
        && ItemEditStateMachine.Instance.currentState == ItemEditStateMachine.Instance.ItemEditStateMachineStateIdle 
        && EscapeToggleToolBar.toolBarisOpened == false
        && SaveAndLoadLevel.Instance.isLevelLoaded == true)
        {  

           if(Input.GetMouseButtonDown(1) && playerObjectInteraction.isCurrentlyHoldingObject == false && EditModeStaticParameter.isInEditMode == true)
           {
            if(playerObjectInteraction.objectHitInfo.transform.tag == "Item" && playerObjectInteraction.objectHitInfo.transform.GetComponent<GeneralObjectInfo>())
            {
                 if(playerObjectInteraction.objectHitInfo.transform.GetComponent<GeneralObjectInfo>().CheckIfObjectIsBeingPlayedOnPath() == false)
              {

              playerObjectInteraction.isCurrentlyHoldingObject = true;
              playerObjectInteraction.currentlyHeldObject = playerObjectInteraction.objectHitInfo.transform.gameObject;
              playerObjectInteraction.currentlyHeldObject.GetComponent<Collider>().enabled = false;
              
              playerObjectInteraction.holdingObject_PickedUpPosition = playerObjectInteraction.currentlyHeldObject.GetComponent<GeneralObjectInfo>().position_original;
              playerObjectInteraction.holdingObject_PickedUpRotation = playerObjectInteraction.currentlyHeldObject.GetComponent<GeneralObjectInfo>().rotation_original;
              

              GameObject.Instantiate(playerObjectInteraction.itemPickUp_Particle, playerObjectInteraction.objectHitInfo.point, (Quaternion.FromToRotation(playerObjectInteraction.currentlyHeldObject.transform.up, playerObjectInteraction.objectHitInfo.normal) * playerObjectInteraction.currentlyHeldObject.transform.rotation) * Quaternion.AngleAxis(playerObjectInteraction.rotationOffset, Vector3.up));

              //used to set the currently held item in-hand-view
              //  playerObjectInteraction.currentlyHeldObject.transform.parent = playerObjectInteraction.item_position;
              //  playerObjectInteraction.currentlyHeldObject.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
              //  playerObjectInteraction.currentlyHeldObject.transform.localRotation = Quaternion.Euler(0.0f,0.0f,0.0f);

                  playerObjectInteraction.pickedUpObject = true;
                }
              else
              {
                UINotificationHandler.Instance.SpawnNotification("<color=red> Object is being played on a path.");
              }
           }
        }
        }
        else //looking into void
        {

          if(playerObjectInteraction.toolTipText != null)
            playerObjectInteraction.toolTipText.text = "Q: Tool Menu | C: Video Controls | V: Fly mode | G: View Mode";
            if(playerObjectInteraction.centerDot_Canvas)
            playerObjectInteraction.centerDot_Canvas.SetActive(true);
        }





          //go trhough door when held long enough
          if(door_hold >= 1)
          {
              door_hold = 0f; //used to prevent message spam on hold
              
               if(string.IsNullOrEmpty(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInParent<DoorInfo>().pathFileToLoad))
               {
                GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>Path is empty! <color=white> Assign a level first.", UINotificationHandler.NotificationStateType.error);
               }
               else if(GlobalUtilityFunctions.IsDigitsOnly(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInParent<DoorInfo>().pathFileToLoad))
               {
                  LoadWorkShop(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInParent<DoorInfo>().pathFileToLoad, playerObjectInteraction.objectHitInfo.transform);
               }
               else
               {
              clickedOnDoor = true;

              GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInParent<DoorInfo>().pathFileToLoad;
              playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponentInParent<DoorInfo>().transform.DOLocalRotate(new Vector3(0,15,0), 2, RotateMode.LocalAxisAdd);
               }
          }


        }

        void OnExit(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
                playerObjectInteraction.ClickerNotifier_gameObject.SetActive(false);

                entered = false; // reset
                playerObjectInteraction.pickedUpObject = false;
        }



    public static event Action<float> DownloadProg;

    public async Task LoadWorkShop(string path, Transform doorTrans) //make it a task
    {
        //download the item
            PlayerObjectInteractionState_Idle.DownloadProg = delegate (float fl)
            {
                Debug.Log("Download prog: " + fl);
            };

            var itemInfo = await Steamworks.Ugc.Item.GetAsync( Convert.ToUInt64(path) );
            await itemInfo?.Subscribe();

            WorkshopDownloadProgressBar.Instance.RefreshQueBar();

            await itemInfo?.DownloadAsync(DownloadProg);



            //needed to get the path of the file
            DirectoryInfo dir = new DirectoryInfo(itemInfo?.Directory);            
            FileInfo[] info = dir.GetFiles("*.*");
            foreach (FileInfo f in info) 
            {
                Debug.Log(f.ToString());
                if(f.ToString().Contains(".json"))
                {
                clickedOnDoor = true;
                GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = f.ToString();
                doorTrans.gameObject.GetComponentInParent<DoorInfo>().transform.DOLocalRotate(new Vector3(0,15,0), 2, RotateMode.LocalAxisAdd);
                break;
                }
            }



    }



}
