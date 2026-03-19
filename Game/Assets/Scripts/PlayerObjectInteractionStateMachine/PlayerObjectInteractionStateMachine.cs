using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;
using TMPro;
using DG.Tweening;

public class PlayerObjectInteractionStateMachine : MonoBehaviour
{
    private static PlayerObjectInteractionStateMachine _instance;
    public static PlayerObjectInteractionStateMachine Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    [SerializeField]
    public string currentStateName;
    public IPlayerObjectInteractionState currentState;

    public PlayerObjectInteractionState_Idle PlayerObjectInteractionStateIdle = new PlayerObjectInteractionState_Idle(); 
    public PlayerObjectInteractionState_Holding PlayerObjectInteractionStateHolding = new PlayerObjectInteractionState_Holding(); 
    public PlayerObjectInteractionState_ViewingFrame PlayerObjectInteractionStateViewingFrame = new PlayerObjectInteractionState_ViewingFrame(); 
    public PlayerObjectInteractionState_ClickedOnDoor PlayerObjectInteractionStateClickedOnDoor = new PlayerObjectInteractionState_ClickedOnDoor(); 
    public PlayerObjectInteractionState_Equipped_Scanner PlayerObjectInteractionStateEquippedScanner = new PlayerObjectInteractionState_Equipped_Scanner(); 
    public PlayerObjectInteractionState_Equipped_Painter PlayerObjectInteractionStateEquippedPainter = new PlayerObjectInteractionState_Equipped_Painter(); 
    public PlayerObjectInteractionState_GlobalSettings PlayerObjectInteractionStateGlobalSettings = new PlayerObjectInteractionState_GlobalSettings(); 
    public PlayerObjectInteractionState_WorkshopPicture PlayerObjectInteractionStateWorkshopPicture = new PlayerObjectInteractionState_WorkshopPicture(); 
    public PlayerObjectInteractionState_WorkshopPictureMission PlayerObjectInteractionStateWorkshopPictureMission = new PlayerObjectInteractionState_WorkshopPictureMission(); 
    public PlayerObjectInteractionState_Equipped_GlueGun PlayerObjectInteractionStateEquippedGlueGun = new PlayerObjectInteractionState_Equipped_GlueGun(); 
    public PlayerObjectInteractionState_WiringTool PlayerObjectInteractionStateWiringTool = new PlayerObjectInteractionState_WiringTool();
    public GameObject currentlyHeldObject;
    public Vector3 holdingObject_PickedUpPosition;
    public Quaternion holdingObject_PickedUpRotation;

    public bool isCurrentlyHoldingObject = false;

    public bool newlyBoughtObjectHasBeenPlaced = true;  //for make sure the object is deleted on cancel (set to false in the wheel picker spawn object class)

    public RaycastHit objectHitInfo;
    public LayerMask collidableLayers_layerMask;
    public LayerMask holding_collidableLayers_layerMask; //needs a special layer mask because of the unique situtations where blocks with lighting shaders
    public float rotationOffset = 0f;
    public Collider[] allObjColliders;
    public float wallOffset = 0.01f;
    public bool currentlyHeldObject_isPlaceable = false;
    public Color placeable_Color;
    public Color notPlaceable_Color;
    public Transform item_position;

    public ItemEditStateMachine itemEditorStateMachine;

    public GameObject centerDot_Canvas;


    //clicker notifer thing
    public GameObject ClickerNotifier_gameObject;




    public GameObject itemPickUp_Particle;


    //
    public GameObject ViewingFrameEmulator_StartPositionOnlyPrefab;




    //for poster viewing state
    public int posterViewingStateIndex; //0 = nothing, 1 = fading to black , 2 = calculating distance, 3 = fading out of black, 4 = view controlls
    public Camera playerCamera;
    public Transform playerCameraParent; //for setting the camera back

    public Transform playerCameraBase;

    public GameObject currentGameObjectLookingAt;
    public GameObject currentGameObjectLookingAt_newer; //used to make the game make sure the player can move while fading out onto a new poster (if they are currently viewing a poster already)
    public bool posterViewer_continueViewing = false; //used to make the game make sure the player can move while fading out onto a new poster (if they are currently viewing a poster already)
    public SimpleSmoothMouseLook mouseLooker;
    public PlayerMovementBasic playerMover;

    public RawImage fadeToBlack_Image;
    public float fadeToBlack_Duration = 0f;
    public bool clickThrough_enabled = false;


    public string screenEdgeToMatch;
    public float panXPos = 0.5f; // 0 to 1
    public float panYPos = 0.5f; // 0 to 1

    public Vector3 imageViewing_rotationOffset;

    public bool pickedUpObject = false;

    public GridBlockQuadHighlighter blockQuadHighLighter;


    //for video screen interaction
    public VLC_MediaPlayer videoPlayer;

    //for door
    public ExitRoomEvent exitRoomEvent;

    //public GameObject doorInfo_CANVAS;


    //for scanner
    public GameObject objectCurrentlyScanning;
    public GameObject scanner_model;
    public GameObject scanner_spinner;
   // public GameObject UpperLeft_CornerHUD;
   // public GameObject UpperRight_CornerHUD;
   // public GameObject LowerLeft_CornerHUD;
   // public GameObject LowerRight_CornerHUD;
    public GameObject ScanInfo_HUD;
    public TextMeshProUGUI ScanInfo_text;
    public TextMeshProUGUI ScanAmount_Text;

    public AudioSource scannerScan_AudioSource;
    public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {
    
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
     
        return(NewValue);
    }

    //for painter

    public LayerMask painter_collidableLayers_layerMask; //needs a special layer mask because of the unique situtations where blocks with lighting shaders

    public UIColorPickerLogic colorpicker;


    public GameObject materialMenuItem_prefab;
    public GameObject customeMaterialList;


    //material properties
    public Color painterColor;
    public Slider alphaSlider;

    public TMP_Dropdown painter_PaintingSettings_DropDown;


    public GameObject painter_model;
    public Renderer painter_head_renderer;
    public GameObject painterSettings_Canvas;

    public GameObject painter_modeGUI_face;
    public GameObject painter_modeGUI_whole;


    public Material painterCurrentMaterial;

    public TextMeshProUGUI painter_materialDataGUI_materialName;
    public TextMeshProUGUI painter_materialDataGUI_totalFaceCount;


    public PainterChangeColor painterChangeColor_MenuFunctions;
    public Image buttonImage;

    public ParticleSystem copySplatter_Particle;
    public Animator paintRoll_Anim;
    public GameObject painterRollPaint_Particle;

    public AudioSource paintRollerDip_AudioSource;
    public AudioSource paintRollerPaint_AudioSource;

    public AnimationCurve painterParticleSize_AnimationCurve;

    public Vector2 UVOffSet_Store = new Vector2(0f,0f);
    public Vector2 UVScale_Store = new Vector2(1f,1f);
    public float UVRotation_Store;



    //audio
    public AudioSource objectPlacementAudioSource;
    public AudioClip objectRotateSnapAudioClip;
    public AudioClip objectPlaceClip;
    public float objectPlaceClipPitch_min = 0.9f;
    public float objectPlaceClipPitch_max = 1.2f;

    public AudioClip objectPickUpClip;
    public float objectPickUpClipPitch_min = 0.9f;
    public float objectPickUpClipPitch_max = 1.2f;

    public AudioClip objectDestroyClip;
    public float objectDestroyClipPitch_min = 0.9f;
    public float objectDestroyClipPitch_max = 1.2f;

    //tool tip
    public TextMeshProUGUI toolTipText;


    //ai wife
    public Animator aiWife_anim;


    //workshop picture taking
    public GameObject canvasRenderTexture_GameObject;
    public RenderTexture screenShot_RenderTexture;
    public AudioSource screenShot_AudioSource;

    //achievements
    public AchievementManager achievementManager;

    //global settings
    public GameObject globalMapSettingsCanvas;
    public bool pressedToExitGlobalMapSettings = false;
    public MusicHandler musicHandler;
    public LevelGlobalMediaManager levelGlobalMediaManager;
    public Material linearColorskybox_Mat;

    //glue gun
    public GameObject glueGun_GameObject;
    public ParticleSystem glueGun_ParticleSystem;
    public ParticleSystem glueGun_ParticleSystem_Collision; //the one used for detecting the collision event

    public AudioSource glueGun_AudioSource;


    //wiring tool

    public int wiringToolState = 0;

    public GameObject wiringTool_wheelPickerEventList_OnGlobal;

    public GameObject wiringTool_HUD;

  //  public GameObject wiringTool_model;
  //  public GameObject wiringTool_modelImage;

    public GameObject blockEventOnList;
    public GameObject blockEventDoList;

    public GameObject blockEventDoList_Global;

    public GameObject currentWiringObject;
    public GameObject wiringTool_LineRendererPrefab;
    public Transform wiringTool_LineRendererContainer;
    public string onEvent;
    public string onParameter;
    public string id;
    public float delay;
    public string doEvent;
    public string doParameter;
    
    public bool editModeOpen = false;

    //used to make sure the player dosnt proceed before setting the parameters. The parameters will set this value to true when they are picked
    public bool canPickNextEventObject = false;

    void OnEnable()
    {
        currentState = PlayerObjectInteractionStateIdle;
    }
    void Update()
    {
        currentState = currentState.DoState(this);
        currentStateName = currentState.ToString();
    }
 

    public void ResetEventValues()
    {
        onEvent = null;
        onParameter = null;
        id = null;
        delay = 0;
        doEvent = null;
        doParameter = null;
    }


    //painter tool
    
    public void UpdatePainterCurrentMaterialHelpfulData()
    {
        //check if painter has material
        if(painterCurrentMaterial == null)
        {
            painter_materialDataGUI_materialName.text = "<color=red> [No material assigned]";
            painter_materialDataGUI_totalFaceCount.text = "Used on: 0 faces";
            return;
        }



        //asign mat name
        foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
        {
            if(poster.name == painterCurrentMaterial.name)
            {
                painter_materialDataGUI_materialName.text = "//" + poster.GetComponent<Note>().note;
            }
        }

        //assign total face count being used by mat
        painter_materialDataGUI_totalFaceCount.text = "Used on: " + GlobalUtilityFunctions.GetTotalNumberOfFacesUsingPosterMaterial(painterCurrentMaterial.name).ToString() + " faces";
    }











    //wiring tool
    public void AddEventToCurrentWiringObject()
    {
        SaveAndLoadLevel.Event e = new SaveAndLoadLevel.Event();
        
        e.onAction = onEvent;
        e.onParamater = onParameter;
        e.id = id;
        e.delay = delay;
        e.doAction = doEvent;
        e.doParameter = doParameter;

        currentWiringObject.GetComponent<EventHolderList>().events.Add(e);

        GlobalParameterManager.Instance.SaveGlobalEntities();


        ResetEventValues();

        UINotificationHandler.Instance.SpawnNotification("<color=green>Event Created!");
    }


    public AudioSource wiringToolSFX_audioSource;
    public AudioClip WiringToolSFX_audioClip_Typing1;
    public AudioClip wiringToolSFX_audioClip_Enter1;
    public AudioClip wiringToolSFX_audioClip_Return1;


/*
    public AudioSource wiringToolSFX_RandomEvent_audioSource;
    public AudioClip wiringToolSFX_RandomEvent_audioClip;
*/
    public void wiringToolSFX_soundType()
    {
        wiringToolSFX_audioSource.clip = WiringToolSFX_audioClip_Typing1;
        wiringToolSFX_audioSource.Play();
    }

    public void wiringToolSFX_soundFinished()
    {
        wiringToolSFX_audioSource.clip = wiringToolSFX_audioClip_Enter1;
        wiringToolSFX_audioSource.Play();
    }
    public void wiringToolSFX_soundReturn()
    {
        wiringToolSFX_audioSource.clip = wiringToolSFX_audioClip_Return1;
        wiringToolSFX_audioSource.Play();
    }

/*
    public void wiringToolSFX_RandomEventImage()
    {
        wiringToolSFX_RandomEvent_audioSource.clip = wiringToolSFX_RandomEvent_audioClip;
        wiringToolSFX_audioSource.Play();
    }
*/


    public void ViewPoster(GameObject poster, float fadeTime = 0.0f, bool clickThrough = false)
    {
        HardcodedExit_RemoveAllToolOverlay();

        fadeToBlack_Duration = fadeTime;
        clickThrough_enabled = clickThrough;

        //disable vertex snapping for poster and its frames
        poster.GetComponent<Renderer>().material.SetFloat("_DisableVertexSnapping", 1);

        foreach(GameObject frame in poster.GetComponent<PosterFrameList>().posterFrameList)
        {
            frame.GetComponent<Renderer>().materials[0].SetFloat("_DisableVertexSnapping", 1);
            frame.GetComponent<Renderer>().materials[1].SetFloat("_DisableVertexSnapping", 1);
        }


        //this check happens just to make sure player can move around while fading-into another poster (when already looking at one)
        if(currentGameObjectLookingAt != null)
        {
            //make sure to assign the layer back to layer 8 if theres already a poster being viewed
            currentGameObjectLookingAt.layer = 8;

            currentGameObjectLookingAt_newer = poster;
            posterViewer_continueViewing = true;
        }
        else
        {
            currentGameObjectLookingAt = poster;
        }

        currentState = PlayerObjectInteractionStateViewingFrame;
        posterViewingStateIndex = 0;
    }

    //function that fades to black and gives control back to player
    public void LeavePosterViewingMode()
    {
        
        if(currentState == PlayerObjectInteractionStateViewingFrame)
        {
            if(currentGameObjectLookingAt != null)
            {

            //enable enable snapping for poster and its frames
            currentGameObjectLookingAt.GetComponent<Renderer>().material.SetFloat("_DisableVertexSnapping", 0);


            currentGameObjectLookingAt.layer = 8;
            foreach (Transform child in currentGameObjectLookingAt.transform)
            {
                //but make sure it dosnt set gizmos to the layer!
                if(child.gameObject.layer == 8)
                {
                    child.gameObject.layer = 8;
                }
            }




            foreach(GameObject frame in currentGameObjectLookingAt.GetComponent<PosterFrameList>().posterFrameList)
            {
                frame.GetComponent<Renderer>().materials[0].SetFloat("_DisableVertexSnapping", 0);
                frame.GetComponent<Renderer>().materials[1].SetFloat("_DisableVertexSnapping", 0);
            }
     
     
            XrayRaycast.Instance.enabled = false;
            currentGameObjectLookingAt.layer = 8;



            fadeToBlack_Image.DORewind();
            fadeToBlack_Image.DOFade(1.0f, fadeToBlack_Duration).OnComplete(() =>FadeOutOfBlackComplete()).SetEase(Ease.Linear);
            }
      
      
        if(ItemEditStateMachine.Instance.currentState == ItemEditStateMachine.Instance.ItemEditStateMachineStateIdle)
        {   
            //to prevent poster event from messing with player mouse while they are in the toolbar menu
            if(EscapeToggleToolBar.toolBarisOpened == false)
            {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;
            }
        }
        else
        {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //enable xray (if player is editing poster, make sure the xray is still turned on when leaving poster view mode)
        XrayRaycast.Instance.enabled = true;
        }
      
      
        }
    }


    public void FadeOutOfBlackComplete()
    {
                currentState = PlayerObjectInteractionStateIdle;

                fadeToBlack_Image.DOFade(0.0f, fadeToBlack_Duration).SetEase(Ease.Linear);
                playerCameraBase.transform.SetParent(playerCameraParent);
                playerCameraBase.transform.localPosition = new Vector3(0,1.4f,0); //set back correct player height position
                playerCameraBase.transform.localRotation = Quaternion.identity;


                mouseLooker.enabled = true;
                mouseLooker.transform.localPosition = new Vector3(0.0f,0.0f,0.0f); //set position back to zero or else it gets messed up
                playerMover.enabled = true;   

                currentGameObjectLookingAt.layer = 8;
                foreach (Transform child in currentGameObjectLookingAt.transform)
                {
                    //but make sure it dosnt set gizmos to the layer!
                    if(child.gameObject.layer == 8)
                    {
                        child.gameObject.layer = 8;
                    }
                }

        if(ItemEditStateMachine.Instance.currentState == ItemEditStateMachine.Instance.ItemEditStateMachineStateIdle)
        {
                currentGameObjectLookingAt.GetComponent<GeneralObjectInfo>().ResetVisibility(); //do this late or else the player camera will also be disabled
        }

                currentGameObjectLookingAt = null;
                currentGameObjectLookingAt_newer = null;
                posterViewer_continueViewing = false;

    }


    //used when the game manually sets the state... (preventing the "OnExit" of whatever state they were in to be called...)
    public void HardcodedExit_RemoveAllToolOverlay()
    {
                ClickerNotifier_gameObject.SetActive(false);


                //wiring
                wiringTool_HUD.SetActive(false);
                pickedUpObject = false;
                EventsUI_EventDropDown_Manager.Instance.close_Button.SetActive(false);
                EventsUI_EventDropDown_Manager.Instance.EraseList();

                //painter
                blockQuadHighLighter.gameObject.SetActive(false);
                painterSettings_Canvas.SetActive(false);
                painter_model.SetActive(false);

                //glue gun
                glueGun_ParticleSystem.Stop();
                glueGun_ParticleSystem_Collision.Stop();
                glueGun_AudioSource.Stop();


                //hard drive gun
                scannerScan_AudioSource.Stop();
                scanner_model.SetActive(false);

    }




    }
