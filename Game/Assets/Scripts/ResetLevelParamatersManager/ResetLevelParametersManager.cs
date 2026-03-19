using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;



//THIS CLASS IS MEANT TO MAKE IT EASIER FOR THE PLAYER TO SEE WHAT CHANGES HAPPEN IN THE MAP, AND THE OPTION TO RESTART THESE CHANGED PARAMETERS



public class ResetLevelParametersManager : MonoBehaviour
{

    private static ResetLevelParametersManager _instance;
    public static ResetLevelParametersManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public bool levelRestartEventPopped = false;

    public List<ObjectChange> objectChangeList = new List<ObjectChange>();


     [System.Serializable]
    public class ObjectChange
    {
        public GameObject objectThatChanged;
        public SaveAndLoadLevel.Event eventInfo;
        public string info;
    }

    //in-game event system mechanic
    public void AddEntityChangeToList(GameObject objThatChanged ,SaveAndLoadLevel.Event e)
    {
        //if the object id of the event was a pointer... dont add it to change list
        if(EventActionManager.FindAndReturnGameObjectOfEventID_Pure(e.id).GetComponent<GlobalParameterPointerEntity>())
        return;


        ObjectChange objChange = new ObjectChange();
        objChange.objectThatChanged = objThatChanged;
        objChange.eventInfo = e;

        if(ContainsGameObject(objectChangeList, objThatChanged) == false)
        {
        objectChangeList.Add(objChange);
      //  UpdateUILogList();
        }
    }

    //hard coded events in code "info". this leaves the "evenInfo" parameter alone. later on the game checks if it is null or not to see if it should print "eventInfo" or "info"
    public void AddEntityChangeToList(GameObject objThatChanged , string info)
    {
        ObjectChange objChange = new ObjectChange();
        objChange.objectThatChanged = objThatChanged;
        objChange.info = info;

        if(ContainsGameObject(objectChangeList, objThatChanged) == false)
        {
        objectChangeList.Add(objChange);
      //  UpdateUILogList();
        }
    }


    public void AddInfoToLogScreen(string info)
    {
        changedObjectLog.text += info;
    }



public bool ContainsGameObject(List<ObjectChange> list, GameObject targetGameObject)
{
    foreach (ObjectChange customClass in list)
    {
        if (customClass.objectThatChanged == targetGameObject)
        {
            return true;
        }
    }
    return false;
}




    void Update()
    {
    if(EditModeStaticParameter.isInEditMode)
    {
        if(string.IsNullOrEmpty(changedObjectLog.text) == false)
        {
            objectChangeCanvas.SetActive(true);
        }
        else
        {
            objectChangeCanvas.SetActive(false);
        }
    

        if(Input.GetKeyDown(KeyCode.R) && SaveAndLoadLevel.Instance.isLevelLoaded == true && ItemEditStateMachine.Instance.currentState == ItemEditStateMachine.Instance.ItemEditStateMachineStateIdle
        && ZipFileHandler.Instance.password_Window.activeSelf == false
        && InputEventManager_String.Instance.InputEventManagerWindow.activeSelf == false
        && InputEventManager_Counter.Instance.InputEventManagerWindow.activeSelf == false)
        {
            ResetAllLevelObjectsThatHaveChanged();
        }
    }
    }


    public void ResetAllLevelObjectsThatHaveChanged()
    {
        if(SaveAndLoadLevel.Instance.totalAmountOfCoroutinesBeingUsedForLoading > 0)
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>Level Reset Canceled: try fly mode");
            return;
        }
        else
        {
            UINotificationHandler.Instance.SpawnNotification("Level Reset!", UINotificationHandler.NotificationStateType.ping);
        }

        PlayerMovementBasic.Instance.RemovePlayerFromPlatform();
        PlayerMovementBasic.Instance.transform.SetParent(null, true); //"true" second argument so that the player scale is respected

        //reset the player viewing poster
        PlayerObjectInteractionStateMachine.Instance.fadeToBlack_Duration = 0f; 
        PlayerObjectInteractionStateMachine.Instance.LeavePosterViewingMode();

        //ONLY change it to idle if its in viewing mode... (so that it dosnt change other stuff like wiring tool and painter tool...)
        if(PlayerObjectInteractionStateMachine.Instance.currentState == PlayerObjectInteractionStateMachine.Instance.PlayerObjectInteractionStateViewingFrame)
        {
            PlayerObjectInteractionStateMachine.Instance.currentState = PlayerObjectInteractionStateMachine.Instance.PlayerObjectInteractionStateIdle;
        }
        PlayerObjectInteractionStateMachine.Instance.playerCameraBase.transform.SetParent(PlayerObjectInteractionStateMachine.Instance.playerCameraParent);



        foreach(GameObject obj in SaveAndLoadLevel.Instance.allLoadedGameObjects)
        {
            if(obj != null)
            {
                obj.transform.GetComponent<GeneralObjectInfo>().ResetVisibility();
            }
        }







        //remove all selected items
        HighLightManager.Instance.UnHighLightObjects(GridBuilderStateMachine.Instance.selectedObjects);
        foreach(GameObject selectedItem in GridBuilderStateMachine.Instance.selectedObjects)
        {
            if(selectedItem.GetComponent<GeneralObjectInfo>())
            {
            selectedItem.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
            }   
        }

        //update positions of selected objects
        foreach(GameObject selectedItem in GridBuilderStateMachine.Instance.selectedObjects)
        {
            if(selectedItem.GetComponent<GeneralObjectInfo>())
            selectedItem.GetComponent<GeneralObjectInfo>().UpdatePosition();
        }

        GridBuilderStateMachine.Instance.selectedObjects.Clear();







        //reset local variables
        Counter[] allCurrentCountersInScene = FindObjectsOfType(typeof(Counter), true) as Counter[];
        foreach(Counter counter in allCurrentCountersInScene)
        {
            //SHITTY TEMPORARY FIX REMOVE LATER. Makes sure the game does not reset global entities
            if(GlobalParameterManager.Instance.allLoadedGlobalEntities.Contains(counter.gameObject) == false)
            {
                counter.GetComponent<GeneralObjectInfo>().ResetAllObjectParameters();
            }
        }
        Date[] allCurrentDatesInScene = FindObjectsOfType(typeof(Date), true) as Date[];
        foreach(Date date in allCurrentDatesInScene)
        {
            //SHITTY TEMPORARY FIX REMOVE LATER. Makes sure the game does not reset global entities
            if(GlobalParameterManager.Instance.allLoadedGlobalEntities.Contains(date.gameObject) == false)
            {
                date.GetComponent<GeneralObjectInfo>().ResetAllObjectParameters();
            }
        }



        levelRestartEventPopped = false;


        GeneralObjectInfo[] allCurrentEntitiesInScene = FindObjectsOfType(typeof(GeneralObjectInfo), true) as GeneralObjectInfo[];

        foreach(GeneralObjectInfo e in allCurrentEntitiesInScene)
        {
                        //SHITTY TEMPORARY FIX REMOVE LATER. Makes sure the game does not reset global entities
            if(GlobalParameterManager.Instance.allLoadedGlobalEntities.Contains(e.gameObject) == false)
            {
                e.ResetAllObjectParameters();


                //remove pathEmulator components(if the object has any)
                if(e.GetComponent<PathEmulatorList>() != null)
                {
                    Destroy(e.GetComponent<PathEmulatorList>());
                }
            }
        }



        //stop all paths playing (path emulators) 
        PathEmulator[] allCurrentPathsEmulatorsInScene = FindObjectsOfType(typeof(PathEmulator), true) as PathEmulator[];

        foreach(PathEmulator e in allCurrentPathsEmulatorsInScene)
        {
            Destroy(e.gameObject);
        }


        //shitty optimization that only happens on level restart (removes rigidbodies from blocks that have them due to the path stuff)
        Block[] allCurrentBlocksInScene = FindObjectsOfType(typeof(Block), true) as Block[];

        foreach(Block e in allCurrentBlocksInScene)
        {
            if(e.GetComponent<Rigidbody>() != null)
            {
                Destroy(e.GetComponent<Rigidbody>());
            }               
        }







        //loop through all posters and set the correct shared material based on id
        foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
        {
            if(poster != null)
            {
                poster.GetComponent<PosterMeshCreator>().AssignSharedMaterialBasedOnGUID();
            }
        }
        //loop through all posters and set the correct shared material based on id
        foreach(GameObject block in SaveAndLoadLevel.Instance.allLoadedBlocks)
        {
            if(block != null)
            {
                block.GetComponent<BlockFaceTextureUVProperties>().AssignSharedMaterialBasedOnGUID();
            }
        }




        //Set all "_DisableVertexSnapping" values back to default
        foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
        {
            if(poster != null)
            {
            poster.transform.GetComponent<Renderer>().material.SetFloat("_DisableVertexSnapping", 0);
            foreach(GameObject frame in poster.GetComponent<PosterFrameList>().posterFrameList)
            {
                frame.GetComponent<Renderer>().materials[0].SetFloat("_DisableVertexSnapping", 0);
                frame.GetComponent<Renderer>().materials[1].SetFloat("_DisableVertexSnapping", 0);
            }

            //reset poster materials
            poster.transform.GetComponent<GeneralObjectInfo>().ResetMaterials();


            //apply a template video playing image to let players know it is a video
            if(poster.transform.GetComponent<PosterMeshCreator>().isVideo)
            {
                poster.transform.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial.SetTexture("_MainTex",poster.transform.GetComponent<PosterMeshCreator>().videoTemplateTexture);
            }

          //  poster.GetComponent<PosterMeshCreator>().ChangeShaderOfPoster();//     legacy... may be needed back if something breaks...
            }
        }





        //reset global stuff

  


        ScreenOverlayManager.Instance.RemovePostersFromOverlay(ScreenOverlayManager.Instance.posterOverlayCurrentList);
        LevelGlobalMediaManager.Instance.ResetMedia();
        PosterDepthLayerStencilRefManager.Instance.AssignCorrectStencilRefsToAllPostersInScene();

        ResetAllVideoPlayers();

        //clear all dialogue que
        DialogueBoxStateMachine.Instance.ClearAllCurrentDialogue();

        //close dialogue choice screen
        DialogueChoiceManager.Instance.dialogueChoice_Canvas.SetActive(false);
        DialogueChoiceManager.Instance.dialogueChoiceBoxIsActive = false;

        //enable player
        SimpleSmoothMouseLook.Instance.enabled = true;
        SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;

        PlayerMovementTypeKeySwitcher.Instance.EnableMovementBasedOnCurrentMovement();
        //stop all events
        EventActionManager.Instance.StopAllEvents();

        objectChangeList.Clear();
        changedObjectLog.text = "";






        //update child index
        foreach(GeneralObjectInfo e in allCurrentEntitiesInScene)
        {
            //SHITTY TEMPORARY FIX REMOVE LATER. Makes sure the game does not reset global entities
            if(GlobalParameterManager.Instance.allLoadedGlobalEntities.Contains(e.gameObject) == false)
            {
                e.UpdateChildIndex();
            }
        }



        //reset positions
        foreach(GeneralObjectInfo e in allCurrentEntitiesInScene)
        {
            //SHITTY TEMPORARY FIX REMOVE LATER. Makes sure the game does not reset global entities
            if(GlobalParameterManager.Instance.allLoadedGlobalEntities.Contains(e.gameObject) == false)
            {
                e.ResetPosition();
            }
        }








        //stop all audio
        AudioSourceObject[] allCurrentAudioSourcesInScne = FindObjectsOfType(typeof(AudioSourceObject), true) as AudioSourceObject[];

        foreach(AudioSourceObject a in allCurrentAudioSourcesInScne)
        {
            a.transform.GetComponent<AudioSourceObject>().SetValuesOnAudioSource();
            a.transform.GetComponent<AudioSource>().Stop();
            a.transform.GetComponent<AudioSourceObject>().OnAudioStop();

        }

        //set volume back to 1 (default for now)
        foreach(AudioSourceObject a in allCurrentAudioSourcesInScne)
        {
            a.transform.GetComponent<AudioSource>().DORewind();
            a.transform.GetComponent<AudioSource>().volume = 1f;
        }



        //remove objects that were loaded additively

        List<GameObject> additiveObjectsToRemove = new List<GameObject>();


        foreach(GameObject obj in SaveAndLoadLevel.Instance.allLoadedGameObjects)
        {
            if(obj != null)
            {
                if(obj.GetComponent<GeneralObjectInfo>().wasLoadedAdditive)
                {
                    additiveObjectsToRemove.Add(obj);
                }
            }
        }


        BlockBaseCubePositionFinder_Singleton.Instance.RemoveFromCube();
        
        foreach(GameObject obj in additiveObjectsToRemove)
        {
            SaveAndLoadLevel.Instance.allLoadedGameObjects.Remove(obj);
            Destroy(obj);
        }


        //reset additive global events
        SaveAndLoadLevel.Instance.additiveGlobalEvents.Clear();

        //level start event
        if(PlayerMovementTypeKeySwitcher.Instance.isInFlyMode == false)
        {
                PlayLevelStartEvents();
        }
    }


    void ResetAllVideoPlayers()
    {
        //stop all videos from playing
        VLC_GlobalMediaPlayer.Instance.Open();



        //stop all poster videos
        PosterMeshCreator[] allCurrentPostersInScene = FindObjectsOfType(typeof(PosterMeshCreator), true) as PosterMeshCreator[];

        foreach(PosterMeshCreator e in allCurrentPostersInScene)
        {
            e.transform.GetComponent<VLC_MediaPlayer>().Stop();
        }

    }


    public void PlayLevelStartEvents()
    {
 
            EventActionManager.Instance.TryPlayEvent_Global("OnLevelStart");




        //call this AFTER calling the global OnLevelStart event thing.... there is a bug i am completely unaware how to solve... if this statement below is called before TryPlay_Global"OnLevelStart"... for some reason the events dont happen AT ALL!!! even the debug messages show up... but the events dont play!
       if(levelRestartEventPopped == true)
        {
            return;
        }

            levelRestartEventPopped = true;


        PrefabEntity[] allCurrentPrefabEntitiesInScene = FindObjectsOfType(typeof(PrefabEntity), true) as PrefabEntity[];

        foreach(PrefabEntity prefab in allCurrentPrefabEntitiesInScene)
        {
            if(prefab.loadOnStart)
            {
                prefab.LoadPrefab();
            }
        }


        //play the global evnets of the additive prefabs
            foreach(SaveAndLoadLevel.Event e in SaveAndLoadLevel.Instance.additiveGlobalEvents)
            {
                Debug.Log("EVENT:" + e);
                EventActionManager.Instance.TryPlayEvent_Single(e);
            }
    }







    public GameObject objectChangeCanvas;
    public TextMeshProUGUI changedObjectLog;

    public void UpdateUILogList()
    {
        return;
                changedObjectLog.text = "";

                    foreach(ObjectChange obj in objectChangeList)
                    {
                        if(obj.eventInfo != null)
                        {
                        changedObjectLog.text += 
                        "* " + obj.eventInfo.onAction + " -> " + obj.objectThatChanged.GetComponent<GeneralObjectInfo>().entityName+"(" + obj.eventInfo.doAction + ")<br>";

                        }
                        else //has no event info (hard coded event)
                        {
                        changedObjectLog.text += 
                        "* " + obj.info + "</color> <br>";
                        }
                    }
    }

}
