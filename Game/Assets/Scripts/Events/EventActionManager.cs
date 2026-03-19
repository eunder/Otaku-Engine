using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class EventActionManager : MonoBehaviour
{
    private static EventActionManager _instance;
    public static EventActionManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    //note.. the "GlobalUtilityFunctions.InsertVariableValuesInText" is used to convert variables inside <> into values




    //NOTE ABOUT GLOBAL ENTITY EVENT:  OnGlobalEntityCounterTrigger -> DO. 
    //these types of abstraction work in a very weird way. when increment() is called... the object id is passed... so that the TryPlayEvent(fromObj, "OnCounterTrigger") knows 
    //which object it came from. TLDR, it uses the fromObj event list because the POINTERS hold the event list, NOT the global entity.



    //NOTE ABOUT CERTAIN EVENTS: Checking values
    //some events that require the function to check the values (like checking which state event there is OR timeline events) require you to use the TryPlayEvent_Single.


    bool colorSwitch = false;

    //First, there will be an attempt
    public void TryPlayEvent(GameObject obj,string onAction)
    {
            //for event log
            bool detectEventSourceOnce = false;

             List<SaveAndLoadLevel.Event> eventsToLoopThrough = new List<SaveAndLoadLevel.Event>();
             eventsToLoopThrough = obj.GetComponentInChildren<EventHolderList>().events;
 

        foreach(SaveAndLoadLevel.Event e in eventsToLoopThrough)
        {
            //if the onAction event is found, execute a coroutine called the same as onAction string
            if(onAction.Equals(e.onAction))
            {

                if(!(e.happenOnce == true && e.hasTriggered == true))
                {
                    //for event log
                    if(detectEventSourceOnce == false)
                    {
                        colorSwitch = !colorSwitch;
                        detectEventSourceOnce = true;
                    }
                    if(colorSwitch)
                    {
                        ResetLevelParametersManager.Instance.AddInfoToLogScreen(e.onAction + "->" + e.doAction + "<br>");
                    }
                    else
                    {
                        ResetLevelParametersManager.Instance.AddInfoToLogScreen("<color=white>"+e.onAction + "->" + e.doAction + "</color><br>");
                    }



                Debug.Log("Started Event Coroutine: " + e);
                StartCoroutine(e.onAction, e);
                }

                if(e.happenOnce)
                {
                    e.hasTriggered = true;
               //     ResetLevelParametersManager.Instance.AddEntityChangeToList(obj,"<color=white> 'Trigger Once' was set to true. This event will not trigger anymore: <br> </color>" +  e.onAction + " -> " + e.doAction);
                }

            }
        }
    }



        //plays the single event, instead of checking the entire event list with the same event name (mostly used for audio/video timeline events)
        public void TryPlayEvent_Single(SaveAndLoadLevel.Event e)
        {       
                //for event log
                bool detectEventSourceOnce = false;


                if(!(e.happenOnce == true && e.hasTriggered == true))
                {
                    //for event log
                    if(detectEventSourceOnce == false)
                    {
                        colorSwitch = !colorSwitch;
                        detectEventSourceOnce = true;
                    }
                    if(colorSwitch)
                    {
                        ResetLevelParametersManager.Instance.AddInfoToLogScreen(e.onAction + "->" + e.doAction + "<br>");
                    }
                    else
                    {
                        ResetLevelParametersManager.Instance.AddInfoToLogScreen("<color=white>"+e.onAction + "->" + e.doAction + "</color><br>");
                    }



                    Debug.Log("Started Event Coroutine: " + e);
                    StartCoroutine(e.doAction, e);
                }

                if(e.happenOnce)
                {
                    e.hasTriggered = true;

                   // if(obj != null)
                    //ResetLevelParametersManager.Instance.AddEntityChangeToList(obj,"<color=white> 'Trigger Once' was set to true. This event will not trigger anymore: <br> </color>" +  e.onAction + " -> " + e.doAction);
                }


        }



    public void TryPlayEvent_Global(string onAction)
    {
        //for event log
        bool detectEventSourceOnce = false;

        foreach(SaveAndLoadLevel.Event e in GlobalEventEntityComponent.Instance.GetComponentInChildren<EventHolderList>().events)
        {
            //if the onAction event is found, execute a coroutine called the same as onAction string
            if(onAction.Equals(e.onAction))
            {
                //for event log
                if(detectEventSourceOnce == false)
                {
                    colorSwitch = !colorSwitch;
                    detectEventSourceOnce = true;
                }
                if(colorSwitch)
                {
                    ResetLevelParametersManager.Instance.AddInfoToLogScreen(e.onAction + "->" + e.doAction + "<br>");
                }
                else
                {
                    ResetLevelParametersManager.Instance.AddInfoToLogScreen("<color=white>"+e.onAction + "->" + e.doAction + "</color><br>");
                }


                StartCoroutine(e.onAction, e);
            }
        }
    }

    //very similar to the previous "TryPlayEvent", except it makes sure to only play events when:   event.OnParamater = obj.state.currentstate
    public void TryPlayEvent_DialogueBox(GameObject obj,string onAction)
    {
             //for event log
             bool detectEventSourceOnce = false;

             List<SaveAndLoadLevel.Event> eventsToLoopThrough = new List<SaveAndLoadLevel.Event>();
    
                eventsToLoopThrough = obj.GetComponentInChildren<EventHolderList>().events;
 

        foreach(SaveAndLoadLevel.Event e in eventsToLoopThrough)
        {
            //if the onAction event is found, execute a coroutine called the same as onAction string
            if(e.onParamater.Equals(obj.GetComponent<State>().currentState))
            {

                if(!(e.happenOnce == true && e.hasTriggered == true))
                {
                //for event log
                if(detectEventSourceOnce == false)
                {
                    colorSwitch = !colorSwitch;
                    detectEventSourceOnce = true;
                }
                if(colorSwitch)
                {
                    ResetLevelParametersManager.Instance.AddInfoToLogScreen(e.onAction + "->" + e.doAction + "<br>");
                }
                else
                {
                    ResetLevelParametersManager.Instance.AddInfoToLogScreen("<color=white>"+e.onAction + "->" + e.doAction + "</color><br>");
                }


                Debug.Log("Started Event Coroutine: " + e);
                StartCoroutine(e.onAction, e);
                }

                //Warn the player this event happened if they set it to happen once
                if(e.happenOnce)
                {
                    e.hasTriggered = true;
               //     ResetLevelParametersManager.Instance.AddEntityChangeToList(obj,"<color=white> 'Trigger Once' was set to true. This event will not trigger anymore: <br> </color>" +  e.onAction + " -> " + e.doAction);
                }

            }
        }
    }


    public void StopAllEvents()
    {
        StopAllCoroutines();
    }


    IEnumerator OnClick(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnHide(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }
    IEnumerator OnUnHide(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnToggle(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnViewPoster(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnGifRestart(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnVideoEnd(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnVideoReachTime(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }


    
    IEnumerator OnReachEndDestination(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }
    IEnumerator OnReachStartDestination(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnPlayerEnter(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }
    
    IEnumerator OnPlayerExit(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }
    IEnumerator OnObjectEnter(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }
    IEnumerator OnObjectExit(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }
    IEnumerator OnDialogueStart(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }
    IEnumerator OnDialogueEnd(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnDialogueExit(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnAudioSourceStart(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }
    IEnumerator OnAudioSourceEnd(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnAudioSourceReachTime(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }


    IEnumerator OnCounterTrigger(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnDialogueChoice(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    
    IEnumerator OnDialogueChoiceTimeOut(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }


    IEnumerator OnAnyDialogueChoicePicked(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }


    IEnumerator OnScanned(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

        IEnumerator OnGlued(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }



    IEnumerator OnPathReachEnd(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnPathReachStart(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }
    IEnumerator OnPathPassWaypoint(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnPrefabLoad(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    IEnumerator OnGlobalEvent(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }

    // DO
    IEnumerator Hide(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                //this is purely added in the event that the player is parented to the block(moving platform), so that the player does not get decativated while inside the block
                foreach(Transform child in obj.transform)
                {
                    if(child == PlayerMovementBasic.Instance.transform)
                    {
                        PlayerMovementBasic.Instance.RemovePlayerFromPlatform();
                    }
                }


                if(obj.GetComponent<GeneralObjectInfo>().canBeDeactivatedByEngine)
                {
                    if(obj.activeSelf == true)
                    {
                        TryPlayEvent(obj, "OnHide");
                        TryPlayEvent(obj, "OnToggle");
                    }

                    obj.GetComponent<GeneralObjectInfo>().isActive = false;
                    obj.SetActive(false);

                    ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
                }
        }
    }


    IEnumerator UnHide(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            if(obj.GetComponent<GeneralObjectInfo>().canBeDeactivatedByEngine)
            {
                if(obj.activeSelf == false)
                {
                    TryPlayEvent(obj, "OnUnHide");
                    TryPlayEvent(obj, "OnToggle");
                }

                obj.GetComponent<GeneralObjectInfo>().isActive = true;
                obj.SetActive(true);

                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
            }
        }
    }



    IEnumerator ResetPosition(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                obj.GetComponentInChildren<GeneralObjectInfo>().ResetPosition();
        }
    }


    IEnumerator Toggle(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                if(obj.gameObject.activeSelf)
                {
                    if(obj.GetComponent<GeneralObjectInfo>().canBeDeactivatedByEngine)
                    {
                        TryPlayEvent(obj, "OnHide");
                        TryPlayEvent(obj, "OnToggle");

                        obj.GetComponent<GeneralObjectInfo>().isActive = false;
                        obj.SetActive(false);

                        ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
                    }
                }
                else
                {
                     if(obj.GetComponent<GeneralObjectInfo>().canBeDeactivatedByEngine)
                    {
                        TryPlayEvent(obj, "OnUnHide");
                        TryPlayEvent(obj, "OnToggle");

                        obj.GetComponent<GeneralObjectInfo>().isActive = true;
                        obj.SetActive(true);

                        ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
                    }
                }
        }
    }


        IEnumerator PlayOnPath(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                obj.GetComponentInChildren<SplineMoveEvents>().reverse = false;
                StartCoroutine(obj.GetComponentInChildren<SplineMoveEvents>().PlayOnPath(e.doParameter));
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }
        IEnumerator PlayOnPath_Reverse(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                obj.GetComponentInChildren<SplineMoveEvents>().reverse = true;
                StartCoroutine(obj.GetComponentInChildren<SplineMoveEvents>().PlayOnPath(e.doParameter));
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }

    IEnumerator PlayOnPath_Additive(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                obj.GetComponentInChildren<SplineMoveEvents>().reverse = false;
                StartCoroutine(obj.GetComponentInChildren<SplineMoveEvents>().PlayOnPath_Additive(e.doParameter));
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }
        IEnumerator PlayOnPath_Additive_Reverse(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                obj.GetComponentInChildren<SplineMoveEvents>().reverse = true;
                StartCoroutine(obj.GetComponentInChildren<SplineMoveEvents>().PlayOnPath_Additive(e.doParameter));
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }

        IEnumerator PlayOnPath_Additive_RotationOnly(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                obj.GetComponentInChildren<SplineMoveEvents>().reverse = false;
                StartCoroutine(obj.GetComponentInChildren<SplineMoveEvents>().PlayOnPath_Additive_RotationOnly(e.doParameter));
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }

        IEnumerator PlayOnPath_Additive_RotationOnly_Reverse(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                obj.GetComponentInChildren<SplineMoveEvents>().reverse = false;
                StartCoroutine(obj.GetComponentInChildren<SplineMoveEvents>().PlayOnPath_Additive_RotationOnly_Reverse(e.doParameter));
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }
    
    public string GetParameterIndex(string parameter, int index)
    {
        string[] splitArray =  parameter.Split(char.Parse(","));

        return splitArray[index];
    }




        IEnumerator ViewPoster(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            float parsedValue;
            bool successfulParse = float.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(e.doParameter), out parsedValue);
            
            if(successfulParse)
            {
                PlayerObjectInteractionStateMachine.Instance.ViewPoster(obj, parsedValue);
            }
            else
            {
                PlayerObjectInteractionStateMachine.Instance.ViewPoster(obj, 0f);
            }

        }
    }



        IEnumerator ViewPoster_ClickThrough(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            float parsedValue;
            bool successfulParse = float.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(e.doParameter), out parsedValue);
            
            if(successfulParse)
            {
                PlayerObjectInteractionStateMachine.Instance.ViewPoster(obj, parsedValue, true);
            }
            else
            {
                PlayerObjectInteractionStateMachine.Instance.ViewPoster(obj, 0f);
            }

        }
    }


        IEnumerator SetPosterAsScreenOverlay(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                ScreenOverlayManager.Instance.AddPosterToOverlay(obj, false);
        }
    }

        IEnumerator SetPosterAsScreenOverlay_Skybox(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                ScreenOverlayManager.Instance.AddPosterToOverlay(obj, true);
        }
    }

        IEnumerator SetPosterAsSkyboxMedia(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                LevelGlobalMediaManager.Instance.SwitchMedia(obj);
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }



        IEnumerator SwitchPosterMedia_Texture(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            //gather block data
                PosterMeshCreator[] allPosters = FindObjectsOfType(typeof(PosterMeshCreator), true) as PosterMeshCreator[];
                for(int i = 0; i < allPosters.Length; i++)
                {
                    if(allPosters[i].name == e.doParameter)
                    {
                        StartCoroutine(obj.GetComponentInChildren<PosterEventComponent>().SwitchPosterMedia(allPosters[i].gameObject));
                        ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
                        break;
                    }
                }

        }
    }

    IEnumerator SaveMediaOnPlayerPC(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                StartCoroutine(obj.GetComponent<PosterEventComponent>().SaveMediaOnPlayerPC());
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }


        IEnumerator ResetMedia(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                obj.GetComponentInChildren<PosterEventComponent>().ResetMedia();
        }
    }



    IEnumerator PlayPosterVideo(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                if(obj.GetComponent<VLC_MediaPlayer>())
                {
                    if(obj.GetComponent<PosterMeshCreator>().isVideo)
                    {
                    obj.GetComponent<PosterMeshCreator>().canVideoSeek = false;
                    obj.GetComponent<VLC_MediaPlayer>().PlayVideo(obj);
                    }
                }
        }

    }

    IEnumerator PlayPosterVideo_EnableControlls(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                if(obj.GetComponent<VLC_MediaPlayer>())
                {
                    obj.GetComponent<PosterMeshCreator>().canVideoSeek = true;
                    obj.GetComponent<VLC_MediaPlayer>().PlayVideo(obj);
                }
        }

    }

    IEnumerator PausePosterVideo(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                if(obj.GetComponent<VLC_MediaPlayer>())
                {
                    obj.GetComponent<VLC_MediaPlayer>().Pause();
                }        }

    }


    IEnumerator StopPosterVideo(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                if(obj.GetComponent<VLC_MediaPlayer>())
                {
                    obj.GetComponent<VLC_MediaPlayer>().StopVideo();
                }        }

    }


    IEnumerator SetPosterVideoVolume(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                if(obj.GetComponent<VLC_MediaPlayer>())
                {
                    int parsedValue;
                    bool successfulParse = int.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(e.doParameter), out parsedValue);
                    
                    if(successfulParse)
                    {
                    //make sure to clamp! prevent loud audio!
                    if(parsedValue > 100)
                    {
                        parsedValue = 100;
                    }
                    else if(parsedValue < 0)
                    {
                        parsedValue = 0;
                    }

                    obj.GetComponent<VLC_MediaPlayer>().SetVolume(parsedValue);
                    }
                }     
        }

    }





    IEnumerator SetPosterVideoTime(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                if(obj.GetComponent<VLC_MediaPlayer>())
                {
                    float parsedValue;
                    bool successfulParse = float.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(e.doParameter), out parsedValue);
                    
                    
                    obj.GetComponent<VLC_MediaPlayer>().SetTime(Convert.ToInt64(parsedValue * 1000));
                }        }

    }



    IEnumerator RemovePosterFromOverlay(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            List<GameObject> list = new List<GameObject>();
            list.Add(obj);
            ScreenOverlayManager.Instance.RemovePostersFromOverlay(list);
        }

    }






    IEnumerator LoadMap(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GlobalUtilityFunctions.OpenMap(GlobalUtilityFunctions.InsertVariableValuesInText(e.doParameter), false);
    }

    IEnumerator LoadMap_Additive(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        string path = GlobalUtilityFunctions.CheckIfMapFileExistsInCurrentOpenedProject(e.doParameter);

        if(path.EndsWith(".zip") || path.EndsWith(".otaku"))
        {
            //zip file manager script
            ZipFileHandler.Instance.AttemptToOpenZipFile(path, "", true, false);
        }
        else if(path.EndsWith(".json"))
        {
            StartCoroutine(LoadMapAdditive.Instance.LoadMap_Additive(GlobalUtilityFunctions.InsertVariableValuesInText(path)));
        }



    }



    IEnumerator AddDialogue(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                StartCoroutine(obj.GetComponentInChildren<DialogueEventComponent>().AddDialogue());
        }
    }

    IEnumerator AddDialogue_FrontOfQue(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                StartCoroutine(obj.GetComponentInChildren<DialogueEventComponent>().AddDialogue_FrontOfQue());
        }
    }

    IEnumerator PlayAudio(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            float parsedValue;
            bool successfulParse = float.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(e.doParameter), out parsedValue);
            
            if(successfulParse)
            {
                StartCoroutine(obj.GetComponentInChildren<AudioSourceEventComponent>().PlayAudio(parsedValue));
            }
            else
            {
                StartCoroutine(obj.GetComponentInChildren<AudioSourceEventComponent>().PlayAudio());
            }
        }
    }

    IEnumerator PlayAudio_OneShot(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                StartCoroutine(obj.GetComponentInChildren<AudioSourceEventComponent>().PlayAudio_OneShot());
        }
    }

    IEnumerator PauseAudio(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                StartCoroutine(obj.GetComponentInChildren<AudioSourceEventComponent>().PauseAudio());
        }
    }
    IEnumerator ResumeAudio(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                StartCoroutine(obj.GetComponentInChildren<AudioSourceEventComponent>().ResumeAudio());
        }
    }
        IEnumerator StopAudio(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            float parsedValue;
            bool successfulParse = float.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(e.doParameter), out parsedValue);
            
            if(successfulParse)
            {
                StartCoroutine(obj.GetComponentInChildren<AudioSourceEventComponent>().StopAudio(parsedValue));
            }
            else
            {
                StartCoroutine(obj.GetComponentInChildren<AudioSourceEventComponent>().StopAudio());
            }

        }
    }
        IEnumerator SetPitch(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            float parsedValue;
            bool successfulParse = float.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(e.doParameter), out parsedValue);
            
            if(successfulParse)
            {
                StartCoroutine(obj.GetComponentInChildren<AudioSourceEventComponent>().SetPitch(parsedValue));
            }
            else
            {
                StartCoroutine(obj.GetComponentInChildren<AudioSourceEventComponent>().SetPitch());
            }

        }
    }

    IEnumerator SetPitch_RandomRange(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            // Split the input string by comma
            string[] numbersAsStrings = e.doParameter.Split(',');

            if(numbersAsStrings.Length == 2) //make sure there are only 2 parsed values
            {

                float parsedMinRange;
                float parsedMaxRange;
                bool successfulParse1 = float.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(numbersAsStrings[0]), NumberStyles.Any, CultureInfo.InvariantCulture, out parsedMinRange);
                bool successfulParse2 = float.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(numbersAsStrings[1]), NumberStyles.Any, CultureInfo.InvariantCulture, out parsedMaxRange);

                //if either value fails to parse... do not execute the action
                if(successfulParse1 == false || successfulParse2 == false)
                {
                    UINotificationHandler.Instance.SpawnNotification("<color=red>Error: Could not parse value");
                }
                else
                {
                    StartCoroutine(obj.GetComponentInChildren<AudioSourceEventComponent>().SetPitch_RandomRange(parsedMinRange, parsedMaxRange));
                }
            }
            else
            {
                UINotificationHandler.Instance.SpawnNotification("<color=red>Error: RandomRange failed. Make sure you have two numbers seperated by a comma");
            }
        }
    }


        IEnumerator IncrementCounter(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                obj.GetComponentInChildren<CounterEventComponent>().Increment();
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }

        IEnumerator AddToCounter(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                int parsedValue;
                bool successfulParse = int.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(e.doParameter), out parsedValue);
                if(successfulParse)
                {
                    obj.GetComponentInChildren<CounterEventComponent>().Add((parsedValue));
                    ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
                }
                else
                {
                    UINotificationHandler.Instance.SpawnNotification("<color=red>Error: Could not parse value. Make sure its a whole number");
                }
        }
    }
        IEnumerator SubtractFromCounter(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {  
                int parsedValue;
                bool successfulParse = int.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(e.doParameter), out parsedValue);
                if(successfulParse)
                {
                    obj.GetComponentInChildren<CounterEventComponent>().Substract((parsedValue));
                    ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
                }
                else
                {
                    UINotificationHandler.Instance.SpawnNotification("<color=red>Error: Could not parse value. Make sure its a whole number");
                }

        }
    }
            IEnumerator SetCounter(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                int parsedValue;
                bool successfulParse = int.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(e.doParameter), out parsedValue);
                if(successfulParse)
                {
                    obj.GetComponentInChildren<CounterEventComponent>().SetCount((parsedValue));
                    ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
                }
                else
                {
                    UINotificationHandler.Instance.SpawnNotification("<color=red>Error: Could not parse value. Make sure its a whole number");
                }
        }
    }
    IEnumerator SetCounter_RandomRange(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                // Split the input string by comma
                string[] numbersAsStrings = e.doParameter.Split(',');

            if(numbersAsStrings.Length == 2) //make sure there are only 2 parsed values
            {

                int parsedMinRange;
                int parsedMaxRange;
                bool successfulParse1 = int.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(numbersAsStrings[0]), NumberStyles.Any, CultureInfo.InvariantCulture, out parsedMinRange);
                bool successfulParse2 = int.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(numbersAsStrings[1]), NumberStyles.Any, CultureInfo.InvariantCulture, out parsedMaxRange);

                //if either value fails to parse... do not execute the action
                if(successfulParse1 == false || successfulParse2 == false)
                {
                    UINotificationHandler.Instance.SpawnNotification("<color=red>Error: Could not parse value. Make sure its a whole number");
                }
                else
                {
                obj.GetComponentInChildren<CounterEventComponent>().SetCount_RandomRange(parsedMinRange, parsedMaxRange);
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
                }
            }
            else
            {
                UINotificationHandler.Instance.SpawnNotification("<color=red>Error: RandomRange failed. Make sure you have two whole numbers seperated by a comma");
            }
        }
    }





    IEnumerator ResetCounterToDefaultValue(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                obj.GetComponentInChildren<CounterEventComponent>().ResetToDefault();
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }


        IEnumerator CheckAndTriggerLogic(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                obj.GetComponent<CounterEventComponent>().CheckAndTriggerLogic(FindAndReturnGameObjectOfEventID_Pure(e.id));
        }
    }

        IEnumerator OpenCounterInputWindow(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                InputEventManager_Counter.Instance.OpenInputWindow(FindAndReturnGameObjectOfEventID_Pure(e.id));
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }

        IEnumerator CheckAndTriggerStringLogic(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                obj.GetComponent<StringEventComponent>().CheckAndTriggerStringLogic(FindAndReturnGameObjectOfEventID_Pure(e.id));
        }
    }

            IEnumerator SetString(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                    obj.GetComponentInChildren<StringEventComponent>().SetString(e.doParameter);
                    ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }

    IEnumerator OpenStringInputWindow(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                InputEventManager_String.Instance.OpenInputWindow(FindAndReturnGameObjectOfEventID_Pure(e.id));
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }


        IEnumerator SetState(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                StartCoroutine(obj.GetComponentInChildren<StateEventComponent>().SetState(e));
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }


        IEnumerator SetNextState(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                StartCoroutine(obj.GetComponentInChildren<StateEventComponent>().SetNextState());
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }


        IEnumerator SetPreviousState(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                StartCoroutine(obj.GetComponentInChildren<StateEventComponent>().SetPreviousState());
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }


        IEnumerator SetRandomState(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                StartCoroutine(obj.GetComponentInChildren<StateEventComponent>().SetRandomState());
                ResetLevelParametersManager.Instance.AddEntityChangeToList(obj, e);
        }
    }

    IEnumerator OpenDialogueChoiceBox(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            DialogueChoiceManager.Instance.CreateListOfDialogeChoicesFromStateObject(obj);
        }
    }


        IEnumerator TriggerBasedOnState(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                StartCoroutine(obj.GetComponentInChildren<StateEventComponent>().StateTrigger(FindAndReturnGameObjectOfEventID_Pure(e.id), e));
        }
    }



    IEnumerator StopPath(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            if(obj.GetComponent<PathEmulatorList>())
            {
                StartCoroutine(obj.GetComponentInChildren<PathEmulatorList>().StopPath());
            }
        }
    }


    IEnumerator PausePath(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            if(obj.GetComponent<PathEmulatorList>())
            {
                StartCoroutine(obj.GetComponentInChildren<PathEmulatorList>().PausePath());
            }
        }
    }




    IEnumerator ResumePath(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            if(obj.GetComponent<PathEmulatorList>())
            {
                StartCoroutine(obj.GetComponentInChildren<PathEmulatorList>().ResumePath());
            }
        }
    }

    IEnumerator ReverseDirection(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            if(obj.GetComponent<PathEmulatorList>())
            {
                StartCoroutine(obj.GetComponentInChildren<PathEmulatorList>().ReverseDirection());
            }
        }
    }

    IEnumerator AddPlayerVelocity(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                StartCoroutine(obj.GetComponentInChildren<PlayerMoverEventComponent>().AddPlayerVelocity(e));
        }
    }

        IEnumerator SetPlayerVelocity(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                StartCoroutine(obj.GetComponentInChildren<PlayerMoverEventComponent>().SetPlayerVelocity(e));
        }
    }

        IEnumerator SetPlayerVelocity_Launch(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            StartCoroutine(obj.GetComponentInChildren<PlayerMoverEventComponent>().SetPlayerVelocity_Launch(e));
        }
    }


        IEnumerator SetPlayerRotation(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            StartCoroutine(obj.GetComponentInChildren<PlayerMoverEventComponent>().SetPlayerRotation());
        }
    }


        IEnumerator SetPlayerPosition(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            StartCoroutine(obj.GetComponentInChildren<PlayerMoverEventComponent>().SetPlayerPosition());
        }
    }

        IEnumerator CheckAndTriggerDateLogic(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            obj.GetComponent<DateEventComponent>().CheckAndTriggerDateLogic(FindAndReturnGameObjectOfEventID_Pure(e.id));
        }
    }



        IEnumerator SetToCurrentDateAndTime(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            obj.GetComponent<DateEventComponent>().SetToCurrentDateAndTime();
        }
    }

    //prefab entity
    IEnumerator LoadPrefab(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            obj.GetComponent<PrefabEntity>().LoadPrefab();
        }
    }
    IEnumerator ClearPrefab(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
            obj.GetComponent<PrefabEntity>().ClearPrefab();
        }
    }


    //Global Events

    //ON

    IEnumerator OnLevelStart(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }



    IEnumerator ClearAllDialogue(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        DialogueBoxStateMachine.Instance.ClearAllCurrentDialogue();
    }



        IEnumerator ShakeScreen(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

            float parsedValue;
            bool successfulParse = float.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(e.doParameter), out parsedValue);
            
            if(successfulParse)
            {
                ScreenShake2.Instance.Shake(parsedValue);
            }
            else
            {
                ScreenShake2.Instance.Shake(0.4f);
            }






        
    }

        IEnumerator ResetPlayerJump(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        PlayerMovementBasic.Instance.ResetPlayerJump();
    }

        IEnumerator SetPlayerPosition_PlayerMover(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        foreach(GameObject obj in SaveAndLoadLevel.Instance.allLoadedGameObjects)
        {
            if(obj.GetComponent<PlayerMoverEventComponent>() && obj.GetComponent<Note>().note == GlobalUtilityFunctions.InsertVariableValuesInText(e.doParameter))
            {
                PlayerMovementBasic.Instance.SetVelocity(obj.transform.forward * 0);
                PlayerMovementBasic.Instance.transform.position = obj.transform.position + (obj.transform.up * ConfigMenuUIEvents.playerPositioningOffset);
                PlayerMovementBasic.Instance.transform.rotation = obj.transform.rotation;

            }
        }
    }


        IEnumerator ResetPlayerView(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        PlayerObjectInteractionStateMachine.Instance.LeavePosterViewingMode();
    }

    

        IEnumerator ResetPlayerScreenOverlay(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        ScreenOverlayManager.Instance.RemovePostersFromOverlay(ScreenOverlayManager.Instance.posterOverlayCurrentList);
    }

    
        IEnumerator ParentPlayerToBlock(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);
 
        GameObject obj = FindAndReturnGameObjectOfEventID(e);
        if(obj != null)
        {
                PlayerMovementBasic.Instance.parentedMechanic_currentParentBlock = obj;
                PlayerMovementBasic.Instance.transform.SetParent(obj.transform);
        }
    }

        IEnumerator UnparentPlayer(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        PlayerMovementBasic.Instance.parentedMechanic_currentParentBlock = null;
        PlayerMovementBasic.Instance.transform.SetParent(null, true);
    }




        IEnumerator ResetSkyBoxMedia(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        LevelGlobalMediaManager.Instance.ResetMedia();
    }

        IEnumerator PopGlobalEvent(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);
        GlobalEventEntityComponent.Instance.PopGlobalEvent(e.doParameter);
    }






    //OBJECT FINDING HELPER FUNCTIONS

    public GameObject FindAndReturnGameObjectOfEventID(SaveAndLoadLevel.Event e)
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if(go.name == e.id)
            {
                //if the object is a pointer... then return the object it is pointing to
                if(go.GetComponent<GlobalParameterPointerEntity>())
                {
                    return go.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity();
                }
                else
                {
                    return go;
                }
                break;
            }
        }

        return null;
    }



    public static GameObject FindAndReturnGameObjectOfEventID_Pure(string id)
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if(go.name == id)
            {
                    return go;
            }
        }

        return null;
    }



}
