using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using SFB;
using DG.Tweening;
using TMPro;

//THE WAY THIS FUCKED UP SYSTEM WORKS
// 1. canPickNextEventObject is set to true on State Enter
// 2. When the player clicks on an object to start an event... canPickNextEventObject is set to false. And the Wheel picker opens up
// 3. When the player clicks on an element on the wheel, canPickNextEventObject will be set to true depending if the element(event) has a parameter or not
// If it has a parameter... then canPickNextEventObject will be set to true only when the parameter/delay has been set


public class PlayerObjectInteractionState_WiringTool : IPlayerObjectInteractionState 
{
        bool entered = false;
        bool clickedToExit = false;


    public IPlayerObjectInteractionState DoState(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(entered == false)
        {
            OnEnter(playerObjectInteraction);
            entered = true;
        }

        if(clickedToExit == true)
        {
            OnExit(playerObjectInteraction);
            return playerObjectInteraction.PlayerObjectInteractionStateIdle;
        }


        OnStay(playerObjectInteraction);

        return playerObjectInteraction.PlayerObjectInteractionStateWiringTool;
    }

        void OnEnter(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        playerObjectInteraction.toolTipText.text = "Left Mouse: <color=green>Wire <color=white>| Right Mouse: <color=red>Step back. <color=white> | H key: Show all connections";

        //playerObjectInteraction.wiringTool_model.SetActive(true);
        playerObjectInteraction.wiringTool_HUD.SetActive(true);


        playerObjectInteraction.canPickNextEventObject = true;
        playerObjectInteraction.wiringToolState = 0;

        //so that when the wiring tool starts on top of an object with events... the event list fills
        currentEventObject = null;

        playerObjectInteraction.ResetEventValues();

        DrawAllCurrentConnectionsInScene(playerObjectInteraction);

        //show hidden stuff
        ObjectVisibilityViewManager.Instance.currentViewMode = 1;
        ObjectVisibilityViewManager.Instance.TriggerCurrentViewModeFunctions();
    }


        GameObject currentPreviewLineRenderer;

        GameObject currentEventObject; //used to check if user is hovering over a different event

        void OnStay(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
            if(playerObjectInteraction.canPickNextEventObject)
            {

                 if (Physics.Raycast(playerObjectInteraction.gameObject.transform.position, playerObjectInteraction.gameObject.transform.forward, out playerObjectInteraction.objectHitInfo, Mathf.Infinity, playerObjectInteraction.collidableLayers_layerMask) && playerObjectInteraction.editModeOpen == false)
                {  
                    if(playerObjectInteraction.objectHitInfo.transform.GetComponent<EventHolderList>() && playerObjectInteraction.wiringToolState == 0)
                    {

                        if(currentEventObject == playerObjectInteraction.objectHitInfo.transform.gameObject)
                        {

                        }
                        else // on hovered over different event object... build the list
                        {
                        currentEventObject = playerObjectInteraction.objectHitInfo.transform.gameObject;


                        RecalculateLineColors(playerObjectInteraction);

                        
                        //if it is a pointer entity, get the list from the object pointing too
                        if(currentEventObject.GetComponent<GlobalParameterPointerEntity>())
                        {
                            //makes sure to assign the ON/DO even lists to the pointer. (you should be assigning this stuff when the global entity gets assigned but whatever)
                            currentEventObject.GetComponent<EventHolderList>().wheelPickerEventList_On = currentEventObject.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<EventHolderList>().wheelPickerEventList_On;
                            currentEventObject.GetComponent<EventHolderList>().wheelPickerEventList_Do = currentEventObject.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<EventHolderList>().wheelPickerEventList_Do;
                            EventsUI_EventDropDown_Manager.Instance.FillEventList(currentEventObject.GetComponent<EventHolderList>());
                        }
                        else
                        {
                            EventsUI_EventDropDown_Manager.Instance.FillEventList(currentEventObject.GetComponent<EventHolderList>());
                        }

                        }
                    }

                        //On click, opens and fills wheel picker, increments the state
                        if(playerObjectInteraction.wiringToolState == 0)
                        {

                            if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<EventHolderList>() || playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<GlobalParameterPointerEntity>())
                            {
                                if(Input.GetMouseButtonDown(0) && WheelPickerHandler.Instance.wheelPickerIsOpen == false && playerObjectInteraction.editModeOpen == false && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
                               {
                                    //make sure the object has a "ON" event wheel list!
                                 if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<EventHolderList>().wheelPickerEventList_On != null || playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<GlobalParameterPointerEntity>())
                                 {

                                    //ERROR CHECKING: for making sure an error appears when the player clicks on a pointer that has no entity on it
                                    if(playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>())
                                    {
                                        if(playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity() == null)
                                        {
                                            UINotificationHandler.Instance.SpawnNotification("<color=red>No data assigned!", UINotificationHandler.NotificationStateType.error);
                                            clickedToExit = true;
                                            return;
                                        }
                                    }


                                    //dont show event list because player is currently begun wiring
                                    currentEventObject = null;
                                    RecalculateLineColors(playerObjectInteraction);
                                    EventsUI_EventDropDown_Manager.Instance.EraseList();


                                    //play sound
                                    playerObjectInteraction.wiringToolSFX_soundType();

                                    
                                    //so that the player does not proceed when setting parameters
                                    playerObjectInteraction.canPickNextEventObject = false;


                                    //current line renderer, create and set both start and end positions to object clicked on
                                    currentPreviewLineRenderer = GameObject.Instantiate(playerObjectInteraction.wiringTool_LineRendererPrefab);
                                    currentPreviewLineRenderer.GetComponent<LineRenderer>().SetPosition(0, playerObjectInteraction.objectHitInfo.transform.position);
                                    currentPreviewLineRenderer.GetComponent<LineRenderer>().SetPosition(1, playerObjectInteraction.objectHitInfo.transform.position);

                                    //set the current object being wired
                                    //note, you dont have to check for pointer here... because the point should hold the events, NOT the global entity.
                                    //
                                    playerObjectInteraction.currentWiringObject = playerObjectInteraction.objectHitInfo.transform.gameObject;


                                    //brings up wheel menu and fills it depending what is on the object's event "ON" wheel picker prefab list


                                    //if it is a pointer...
                                    if(playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>())
                                    {
                                    WheelPickerHandler.Instance.Objectlist = playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<EventHolderList>().wheelPickerEventList_On;
                                    }
                                    else
                                    {
                                    WheelPickerHandler.Instance.Objectlist = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<EventHolderList>().wheelPickerEventList_On;
                                    }

                                    WheelPickerHandler.Instance.BuildWheelPickerFromList();
                                    WheelPickerHandler.Instance.BringUpWheelMenu();


                                    //automatically fill the ON wheel prefab list of the gameobject for states (and also refresh to show the newly added elements)
                                    //mostly used due to the specific mechanic of state picking


                                    //if it is a pointer...
                                    if(playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>())
                                    {
                                        if(playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<State_EventWheelListFiller>())
                                        {
                                            playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<State_EventWheelListFiller>().CreateAndFillWheelEventListWithStateNames_On();
                                            WheelPickerHandler.Instance.RefreshWheelPickerList();
                                        }
                                    }
                                    else
                                    {
                                        if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<State_EventWheelListFiller>())
                                        {
                                            playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<State_EventWheelListFiller>().CreateAndFillWheelEventListWithStateNames_On();
                                            WheelPickerHandler.Instance.RefreshWheelPickerList();
                                        }
                                    }





                                    playerObjectInteraction.wiringToolState++;
                                }
                                  }
                            }
                        }

                        //on click, only the id gets assigned in this state class... the other fields such as ON/DO/PARAMETER/DELAY get filled externally... very messy
                        else if(playerObjectInteraction.wiringToolState == 1)
                        {
                            if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<EventHolderList>() || playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<GlobalParameterPointerEntity>())
                            {
                                //make sure the object has a "DO" event wheel list!
                                if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<EventHolderList>().wheelPickerEventList_Do != null || playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<GlobalParameterPointerEntity>())
                                {  
                                    //set the end point of the line renderer to the object about to be clicked
                                    currentPreviewLineRenderer.GetComponent<LineRenderer>().SetPosition(1, playerObjectInteraction.objectHitInfo.transform.position);

                                    if(Input.GetMouseButtonDown(0) && WheelPickerHandler.Instance.wheelPickerIsOpen == false && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
                                    {

                                        //ERROR CHECKING: for making sure an error appears when the player clicks on a pointer that has no entity on it
                                        if(playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>())
                                        {
                                            if(playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity() == null)
                                            {
                                                UINotificationHandler.Instance.SpawnNotification("<color=red>No data assigned!", UINotificationHandler.NotificationStateType.error);
                                                clickedToExit = true;
                                                return;
                                            }
                                        }


    
                                        //play sound
                                        playerObjectInteraction.wiringToolSFX_soundType();

                                        //so that the player does not proceed when setting parameters
                                        playerObjectInteraction.canPickNextEventObject = false;

                                        playerObjectInteraction.id = playerObjectInteraction.objectHitInfo.transform.name;
  

                                        //brings up wheel menu and fills it depending what is on the object's event "DO" wheel picker prefab list
                                        
                                        //if it is a pointer...
                                        if(playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>())
                                        {
                                        WheelPickerHandler.Instance.Objectlist = playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<EventHolderList>().wheelPickerEventList_Do;
                                        }
                                        else
                                        {
                                        WheelPickerHandler.Instance.Objectlist = playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<EventHolderList>().wheelPickerEventList_Do;
                                        }
                                    
                                        WheelPickerHandler.Instance.BuildWheelPickerFromList();
                                        WheelPickerHandler.Instance.BringUpWheelMenu();


                                        //automatically fill the ON wheel prefab list of the gameobject for states (and also refresh to show the newly added elements)
                                        //turned the state entity into dialogue entity, not needed anymore

                                        /*
                                        //if it is a pointer...
                                        if(playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>())
                                        {
                                            if(playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<State_EventWheelListFiller>())
                                            {
                                                playerObjectInteraction.objectHitInfo.transform.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<State_EventWheelListFiller>().CreateAndFillWheelEventListWithStateNames_Do();
                                                WheelPickerHandler.Instance.RefreshWheelPickerList();
                                            }
                                        }
                                        else
                                        {
                                            if(playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<State_EventWheelListFiller>())
                                            {
                                                playerObjectInteraction.objectHitInfo.transform.gameObject.GetComponent<State_EventWheelListFiller>().CreateAndFillWheelEventListWithStateNames_Do();
                                                WheelPickerHandler.Instance.RefreshWheelPickerList();
                                            }
                                        }
                                        */


                                        playerObjectInteraction.wiringToolState++;
                                    }
                               }
                            }
                        }



                }
                else
                {
                    if(playerObjectInteraction.editModeOpen == false)
                    {
                        //so that the skybox does not mess up the UI or lines
                        if(currentEventObject) //remove hightlight if current object is not null
                        currentEventObject.GetComponent<EventHolderList>().Un_HighlightLinesOnHoverOverObject();

                        currentEventObject = null;
                        EventsUI_EventDropDown_Manager.Instance.EraseList();
                    }
                }
            }



            //used to reset the whole thing when the last step has been complete (this gets set right after the delay parameter is set)
            if(playerObjectInteraction.wiringToolState == 3)
            {
                playerObjectInteraction.currentWiringObject.GetComponent<EventHolderList>().lineRenderers.Add(currentPreviewLineRenderer.GetComponent<LineRenderer>());
                currentPreviewLineRenderer = null;

            //refresh connections
             DrawAllCurrentConnectionsInScene(playerObjectInteraction);

                currentEventObject = null;


                //play sound
                playerObjectInteraction.wiringToolSFX_soundFinished();

                playerObjectInteraction.wiringToolState = 0;                          
            }


            if(Input.GetMouseButtonDown(1) )
           {
            if(playerObjectInteraction.wiringToolState == 0)
            {
                            clickedToExit = true;
            }

            if(playerObjectInteraction.wiringToolState == 1)
            {
                EventsUI_DelayCanvasSingleton.Instance.windowAnimator.CloseWindow();
                EventsUI_ParameterCanvasSingleton.Instance.windowAnimator.CloseWindow();


                GameObject.Destroy(currentPreviewLineRenderer);
                currentPreviewLineRenderer = null;

                playerObjectInteraction.wiringToolSFX_soundReturn();
                playerObjectInteraction.wiringToolState = 0;                          
                WheelPickerHandler.Instance.CloseWheelPicker();
                playerObjectInteraction.canPickNextEventObject = true;

            }
            if(playerObjectInteraction.wiringToolState == 2)
            {
                EventsUI_DelayCanvasSingleton.Instance.windowAnimator.CloseWindow();
                EventsUI_ParameterCanvasSingleton.Instance.windowAnimator.CloseWindow();


                playerObjectInteraction.wiringToolSFX_soundReturn();
                playerObjectInteraction.wiringToolState = 1;                          
                WheelPickerHandler.Instance.CloseWheelPicker();
                playerObjectInteraction.canPickNextEventObject = true;
            }

           }


    
        
            if(Input.GetKeyDown(KeyCode.LeftShift) && playerObjectInteraction.wiringToolState == 0 && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
            {
                if(currentEventObject != null)
                {
                    if(currentEventObject.GetComponent<EventHolderList>().events.Count > 0)
                    {
                        playerObjectInteraction.editModeOpen = true;
                        EventsUI_EventDropDown_Manager.Instance.EnableListEditMode();
                    }
                }
            }


            //toggle all connections
            if(Input.GetKeyDown(KeyCode.H) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
            {
                if(showingAllConnections == true)
                {
                    showingAllConnections = false;
                    RecalculateLineColors(playerObjectInteraction);
                }
                else
                {
                    showingAllConnections = true;
                    RecalculateLineColors(playerObjectInteraction);
                }
            }


        }

        void OnExit(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
              //  playerObjectInteraction.wiringTool_model.SetActive(false);
                playerObjectInteraction.wiringTool_HUD.SetActive(false);
                EventsUI_EventDropDown_Manager.Instance.DisableListEditMode();

                if(currentPreviewLineRenderer)
                GameObject.Destroy(currentPreviewLineRenderer);

                DestroyAllLineRenderers(playerObjectInteraction);

                entered = false; // reset
                playerObjectInteraction.pickedUpObject = false;
                clickedToExit = false;
      
                    //hide wheel picker
                WheelPickerHandler.Instance.CloseWheelPicker();

      
                ObjectVisibilityViewManager.Instance.TurnOff_GizmoInteractionAndView();
                ObjectVisibilityViewManager.Instance.HideHiddenGameObjects();

                EventsUI_EventDropDown_Manager.Instance.EraseList();

        }


        void RecalculateLineColors(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
                        //Hide OR unlightlight all lines (depending on what player has toggled)
                        foreach(EventHolderList elist in allCurrentEventObjectsInScene)
                        {
                        if(showingAllConnections)
                        {
                            elist.Un_HighlightLinesOnHoverOverObject();
                        }
                        else
                        {
                            elist.HideLines();
                        }
                        }


                        //highlight the newly set current object
                        if(currentEventObject)
                        currentEventObject.GetComponent<EventHolderList>().HighlightLinesOnHoverOverObject();

        }




        EventHolderList[] allCurrentEventObjectsInScene;
        bool showingAllConnections = false;


        //note: this functions draw a line renderer for every event. this is so that the game knows the index order... for use like in   EventsUI_EventDropDown_Event.cs
        void DrawAllCurrentConnectionsInScene(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
            allCurrentEventObjectsInScene = GameObject.FindObjectsOfType(typeof(EventHolderList), true) as EventHolderList[];
         var d = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];

        for(int i = 0; i < allCurrentEventObjectsInScene.Length; i++)
        {
               // float incrementSpacer = 0; //for visibility and better clearance with multiple connections

                foreach(SaveAndLoadLevel.Event e in allCurrentEventObjectsInScene[i].events)
                {
                            GameObject obj = GameObject.Find(e.id);


                            GameObject lineRenderer = GameObject.Instantiate(playerObjectInteraction.wiringTool_LineRendererPrefab);
                            if(obj != null)
                            {
                                //styling
                                lineRenderer.GetComponent<LineRenderer>().startColor = EventLineStylingManager.Instance.eventLineColorStart_Standard;
                                lineRenderer.GetComponent<LineRenderer>().endColor = EventLineStylingManager.Instance.eventLineColorEnd_Standard;
                                lineRenderer.GetComponent<LineRenderer>().startWidth = EventLineStylingManager.Instance.eventLineWidthStart_Standard;
                                lineRenderer.GetComponent<LineRenderer>().endWidth = EventLineStylingManager.Instance.eventLineWidthEnd_Standard;

                                //first pos
                                lineRenderer.GetComponent<LineRenderer>().SetPosition(0, allCurrentEventObjectsInScene[i].transform.position); //+ new Vector3(0, incrementSpacer, 0));

                                lineRenderer.GetComponent<LineRenderer>().SetPosition(1, obj.transform.position);                        

                                //if it is a global event... then dont draw that line across the map...
                                if(obj.GetComponent<GlobalEventEntityComponent>())
                                {
                                    // lineRenderer.GetComponent<LineRenderer>().startColor = new Color(EventLineStylingManager.Instance.eventLineColorStart_Standard.r, EventLineStylingManager.Instance.eventLineColorStart_Standard.g, EventLineStylingManager.Instance.eventLineColorStart_Standard.b, EventLineStylingManager.Instance.eventLineColorStart_Standard.a + EventLineStylingManager.Instance.globalLineEventTransparencyMod) ;
                                    // lineRenderer.GetComponent<LineRenderer>().endColor = new Color(EventLineStylingManager.Instance.eventLineColorStart_Standard.r, EventLineStylingManager.Instance.eventLineColorStart_Standard.g, EventLineStylingManager.Instance.eventLineColorStart_Standard.b, EventLineStylingManager.Instance.eventLineColorStart_Standard.a + EventLineStylingManager.Instance.globalLineEventTransparencyMod) ;


                                    lineRenderer.GetComponent<LineRenderer>().SetPosition(1, allCurrentEventObjectsInScene[i].transform.position + new Vector3(0,0.1f,0));
                                }

                                //if the on and do events are on the same object... make it more clear to read
                                if(obj == allCurrentEventObjectsInScene[i])
                                {
                                    lineRenderer.GetComponent<LineRenderer>().SetPosition(1, obj.transform.position + new Vector3 (0,0.1f,0));
                                }

                                //incrementSpacer += 0.05f;
                            }
                            else
                            {
                                //if the object dosnt exist... then set the location to 0,0,0
                                lineRenderer.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0,0,0)); //+ new Vector3(0, incrementSpacer, 0));
                                lineRenderer.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0,0,0));                        

                            }


                            allCurrentEventObjectsInScene[i].lineRenderers.Add(lineRenderer.GetComponent<LineRenderer>());
                         
                }
        }
        }



        void DestroyAllLineRenderers(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
            allCurrentEventObjectsInScene = GameObject.FindObjectsOfType(typeof(EventHolderList), true) as EventHolderList[];
       
            for(int i = 0; i < allCurrentEventObjectsInScene.Length; i++)
            {
                allCurrentEventObjectsInScene[i].ClearLineRenderers();
            }
        }


}
