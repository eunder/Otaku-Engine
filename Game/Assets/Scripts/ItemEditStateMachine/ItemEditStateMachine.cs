using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;
using TMPro;
using System.Linq;
public class ItemEditStateMachine : MonoBehaviour
{
    private static ItemEditStateMachine _instance;
    public static ItemEditStateMachine Instance { get { return _instance; } }

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
    public IItemEditStateMachine currentState;

    public ItemEditStateMachineState_Idle ItemEditStateMachineStateIdle = new ItemEditStateMachineState_Idle();
    public ItemEditStateMachineState_Block ItemEditStateMachineStateBlock = new ItemEditStateMachineState_Block();
    public ItemEditStateMachineState_Block_SetEndPosition ItemEditStateMachineStateBlockSetEndPosition = new ItemEditStateMachineState_Block_SetEndPosition();
    public ItemEditStateMachineState_Poster ItemEditStateMachineStatePoster = new ItemEditStateMachineState_Poster();
    public ItemEditStateMachineState_Poster_SetDepthLayerPosterID ItemEditStateMachineStatePosterSetDepthLayerPosterID = new ItemEditStateMachineState_Poster_SetDepthLayerPosterID();
    public ItemEditStateMachineState_Door ItemEditStateMachineStateDoor = new ItemEditStateMachineState_Door();
    public ItemEditStateMachineState_Dialogue ItemEditStateMachineStateDialogue = new ItemEditStateMachineState_Dialogue();
    public ItemEditStateMachineState_AudioSource ItemEditStateMachineStateAudioSource = new ItemEditStateMachineState_AudioSource();
    public ItemEditStateMachineState_Counter ItemEditStateMachineStateCounter = new ItemEditStateMachineState_Counter();
    public ItemEditStateMachineState_PlayerMover ItemEditStateMachineStatePlayerMover = new ItemEditStateMachineState_PlayerMover();
    public ItemEditStateMachineState_String ItemEditStateMachineStateString = new ItemEditStateMachineState_String();
    public ItemEditStateMachineState_State ItemEditStateMachineStateState = new ItemEditStateMachineState_State();
    public ItemEditStateMachineState_Path ItemEditStateMachineStatePath = new ItemEditStateMachineState_Path();
    public ItemEditStateMachineState_GlobalEntityPointer ItemEditStateMachineStateGlobalEntityPointer = new ItemEditStateMachineState_GlobalEntityPointer();
    public ItemEditStateMachineState_Date ItemEditStateMachineStateDate = new ItemEditStateMachineState_Date();
    public ItemEditStateMachineState_Light ItemEditStateMachineStateLight = new ItemEditStateMachineState_Light();
    public ItemEditStateMachineState_Prefab ItemEditStateMachineStatePrefab = new ItemEditStateMachineState_Prefab();

    public GameObject playerGameObject;
    public SimpleSmoothMouseLook mouseLooker;
    public PlayerObjectInteractionStateMachine playerInteractionStateMachine;
    public GameObject noteCANVAS;
    public TMP_InputField noteInputField;
    public GameObject editor_CAMERA;
    public TMP_InputField width_InputField;
    public TMP_InputField height_InputField;
    public Toggle billboard_Toggle;
    public Toggle billboard_Character_Toggle;

    //tool tip
    public TextMeshProUGUI toolTipText;

    public GameObject posterAreaResizePreview;

        //color picker current color;
        public Color color;


    [Header("Block Handling")]
    public GameObject blockEditor_CANVAS;
    public Toggle blockEditor_isTrigger_TOGGLE;
    public Toggle blockIsScannable_Toggle;
    public Toggle blockIgnorePlayer_Toggle;
    public Toggle blockIgnorePlayerClick_Toggle;


    [Header("Frame Poster Handling")]
    public GameObject posterEditor_CANVAS;
    public TMP_InputField posterURL;


    //frames
    public GameObject frameProperties_Window;
    public List<GameObject> frameLayerListOfLayersCreated = new List<GameObject>(); // holds the list of UI frame buttons created on open and/or when frame added
    public GameObject frameLayer_currentlySelected;
    public GameObject frameLayerButton_currentlySelected;

    public GameObject framePreset;
    public GameObject frameLayerPrefab;
    public Transform frameLayerList;
    public Slider frame_widthSlider;
    public Slider frame_heightSlider;
    public Slider frame_depthSlider;
    public RectTransform colorPicker;
    public Texture2D colorPickerTexture;
    public Image outeredge_buttonImage;
    public Image inneredge_buttonImage;
    public Slider frame_luminanceSlider_outer;
    public Slider frame_luminanceSlider_inner;


    //depth layers
    public GameObject depthLayerProperties_Window;

    public GameObject posterEditorSetDepthLayerPosterID_CANVAS;
    public GameObject depthLayersBeingUsed_Blocker;

    public TMP_InputField depthLayer_Depth_InputField;
    public TMP_InputField depthLayer_Size_InputField;
    public TMP_InputField depthLayer_CurveX_InputField;
    public TMP_InputField depthLayer_CurveY_InputField;



    public List<GameObject> depthLayerListOfLayersCreated = new List<GameObject>(); // holds the list of UI depth layer buttons created on open and/or when layer added 
    public GameObject depthLayer_currentlySelected;
    public GameObject depthLayerButton_currentlySelected;
    public GameObject depthLayerButtonPreset;
    public GameObject depthLayerPrefab;
    public Transform depthLayerList;


    //poster view settings
    public TMP_Dropdown posterViewSettings_ScrollingMode_DropDown;
    public TMP_Dropdown posterViewSettings_AlignmentMode_DropDown;
    public Slider posterViewSettings_ZoomForcedOffset_Slider;
    public Slider posterViewSettings_RotationEffectAmount_Slider;
    public Toggle posterViewSettings_CanZoom_Toggle;
    public Slider posterViewSettings_ExtraBorder_Slider;
    public Toggle posterViewSettings_InverseLook_Toggle;


    //poster color key settings
    public Image posterColorKeySettings_ColorKey_Image;
    public Slider posterColorKeySettings_Threshold_Slider;
    public Slider posterColorKeySettings_transparencyThreshold_Slider;
    public Slider posterColorKeySettings_spillCorrection_Slider;
    public Slider posterColorKeySettings_HueRange_Slider;

    //poster shader settings
    public TMP_Dropdown posterShaderSettings_shaderName_DropDown;


    //poster texture filtering settings
    public Toggle posterTextureFilteringSettings_enableTextureFiltering_Toggle;

    //texture scrolling setttings
    public TMP_InputField posterTextureScrollingX_InputField;
    public TMP_InputField posterTextureScrollingY_InputField;


    //footstep settings
    public TMP_InputField posterFootstepPath_InputField;

    //xray settings
    public TMP_InputField posterXraySize_InputField;
    public TMP_InputField posterXrayTransparency_InputField;
    public TMP_InputField posterXraySoftness_InputField;
    public Toggle posterXrayIsScannable_Toggle;



    public GameObject currentObjectEditing;
    public CustomeItemType.TypeOfItem currentItemType;
    public SuccessfulImageUploadEvent successfullImageUploadEventHandler;


    public TMP_Dropdown colorEdge_DropDown;


    public AudioSource addFrame_AudioSource;
    public AudioSource removeFrame_AudioSource;




    //audio
    public AudioSource openAndCloseMenu_audioSource;
    public AudioClip openMenu_audioClip;
    public AudioClip closeMenu_audioClip;


    //Update the values of the pop-up window when a frame object is clicked
    public void RecalculateSliderValues()
    {
        if(frameLayer_currentlySelected)
        {
            frame_widthSlider.value = frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().frame_width;
            frame_heightSlider.value = frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().frame_height;
            frame_depthSlider.value = frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().heightDepth;

            frame_luminanceSlider_outer.value = frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().rimLuminance[0];

            frame_luminanceSlider_inner.value = frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().rimLuminance[1];
        }

        if(depthLayer_currentlySelected)
        {
            depthLayer_Depth_InputField.text = depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().depth.ToString();
            depthLayer_Size_InputField.text = depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().size.ToString();
            depthLayer_CurveX_InputField.text = depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().shapeKey_CurveX.ToString();
            depthLayer_CurveY_InputField.text = depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().shapeKey_CurveY.ToString();
        }

    }

    public void UpdateButtonColors()
    {
        outeredge_buttonImage.color = frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().rimColors[0];
        inneredge_buttonImage.color = frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().rimColors[1];
    }

 public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue){
     
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
     
        return(NewValue);
    }

    public void SetCurrentItemTypeToEmpty()
    {
        currentItemType = CustomeItemType.TypeOfItem.empty;
        currentObjectEditing = null;

 
    } 

    [Header("Video Case Handling")]
    public GameObject videoEditor_CANVAS;
    public Toggle isLooping_TOGGLE;
    public Slider case_UVScaleSlider;
    public Slider case_UVOffsetXSlider;
    public Slider case_UVOffsetYSlider;


    [Header("Door Handling")]
    public GameObject doorEditor_CANVAS;
    public TMP_InputField door_Path;




    [Header("Dialogue")]
    public GameObject dialogueEditor_CANVAS;
    public GameObject dialogueEditorSetPosterToMove_CANVAS;
    public TMP_InputField dialogue_dialogue_InputField;
    public TMP_InputField dialogue_voicePath_InputField;
    public TMP_InputField dialogue_pitch_InputField;
    public Toggle dialogue_clearPreviousDialogue_Toggle;

    [Header("AudioSource")]
    public GameObject audioSourceEditor_CANVAS;
    public TMP_InputField audioSource_audioPath_InputField;
    public TMP_InputField audioSource_pitch_InputField;
    public Slider audioSource_spatialBlend_Slider;
    public TMP_InputField audioSource_minDistance_InputField;
    public TMP_InputField audioSource_maxDistance_InputField;
    public Toggle audioSource_repeat_Toggle;

    public GameObject audioSource_minDistance_VisualizationGameObject;
    public GameObject audioSource_maxDistance_VisualizationGameObject;


    public Button audioSource_Play_Button;
    public Button audioSource_Stop_Button;


    [Header("Counter")]
    public GameObject counterEditor_CANVAS;
    public TMP_InputField counter_defaultCount_InputField;
    public TMP_InputField counter_currentCount_InputField;

    [Header("String")]
    public GameObject stringEditor_CANVAS;
    public TMP_InputField string_defaultString_InputField;
    public TMP_InputField string_currentString_InputField;


    [Header("Player Mover")]
    public GameObject playerMover_CANVAS;


    [Header("State Entity")]
    public GameObject stateEditor_CANVAS;
    public GameObject stateEditor_stateList_GameObject;
    public List<GameObject> stateEditor_stateElementList = new List<GameObject>();
    public GameObject stateEditor_stateListElement_Prefab;
    public TMP_InputField stateEditor_defaultState_InputField;
    public TMP_InputField stateEditor_currentState_InputField;
    public TMP_InputField stateEditor_stateToAdd_InputField;
    public TMP_InputField stateEditor_timeUntilChoiceBoxCloses_InputField;


    [Header("Path Entity")]
    public GameObject pathEditor_CANVAS;
    public GameObject pathEditorSetObjectToMove_CANVAS;

    public TMP_InputField pathEditor_time_InputField;
    public TMP_Dropdown pathEditor_localType_Dropdown;

    public TMP_Dropdown pathEditor_pathObjectRotationType_Dropdown;
    public Slider pathEditor_lookAhead_Slider;

    public TMP_Dropdown pathEditor_loopType_Dropdown;
    public Toggle pathEditor_closeLoop_Toggle;

    public TMP_Dropdown pathEditor_easeType_Dropdown;
    public TMP_Dropdown pathEditor_pathType_Dropdown;
    public Toggle pathEditor_wayPointRotation_Toggle;


    public Toggle pathEditor_moveToPath_Toggle;


    [Header("Global Entity Pointer")]
    public GameObject globalEntityEditor_CANVAS;
    public List<GameObject> globalEntityEditor_UIentityList = new List<GameObject>();
    public GameObject globalEntityEditor_UIEntityHolderContainer;


    public List<GameObject> localEntityEditor_UIentityList = new List<GameObject>();
    public GameObject localEntityEditor_UIEntityHolderContainer;


    public GameObject currentlyHighlightedEntityUIObject;
    public GameObject UIEntityHolderPrefab;



    [Header("Date Entity")]
    public GameObject DateEditor_CANVAS;
    public TMP_InputField DateEditor_date_InputField;




    [Header("Light Entity")]
    public GameObject lightEditor_CANVAS;
    public TMP_Dropdown lightEditor_lightType_Dropdown;
    public TMP_InputField lightEditor_strength_InputField;
    public Image lightEditor_color_Image;

    public TMP_InputField lightEditor_range_InputField;
    public TMP_InputField lightEditor_spotAngle_InputField;
    public TMP_Dropdown lightEditor_shadowType_Dropdown;


    [Header("Light Entity")]
     public GameObject prefab_CANVAS;
    public TMP_InputField prefab_mapToLoad_InputField;
    public Toggle prefab_loadOnStart_Toggle;



    [Header("Shader Picking Window")]
    public GameObject shaderPickingWindow;




    void OnEnable()
    {
        currentState = ItemEditStateMachineStateIdle;
    }
    void Start()
    {
        colorPickerTexture = colorPicker.GetComponent<Image>().mainTexture as Texture2D;
    }



    //FOR COLOR BUTTON EDITING (MESSY)

        [Header("Color Button Editing")]

        public Image buttonImage = null;
        public UIColorPickerLogic colorpicker;
        public int posterEdgeIndex = 0;
        public void SetOuterEdgeColor(Image buttImage)
        {
            posterEdgeIndex = 0;
            buttonImage = buttImage;
            OpenColorWheel();
        }
        public void SetInnerEdgeColor(Image buttImage)
        {
            posterEdgeIndex = 1;
            buttonImage = buttImage;
            OpenColorWheel();
        }
        public void SetColorKeyColor(Image buttImage)
        {
            buttonImage = buttImage;
            
            colorpicker.InitialColor = currentObjectEditing.GetComponent<PosterColorKeySettings>().colorKey;
            colorpicker.color = currentObjectEditing.GetComponent<PosterColorKeySettings>().colorKey;
            colorpicker.ActivateColorPicker();

        }
        public void OpenColorWheel()
        {
            colorpicker.InitialColor = frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().rimColors[posterEdgeIndex];
            colorpicker.color = frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().rimColors[posterEdgeIndex];
            colorpicker.ActivateColorPicker();
        }


    void Update()
    {
        currentState = currentState.DoState(this);
        currentStateName = currentState.ToString();



                //FOR COLOR BUTTON EDITING (MESSY)
                if(Input.GetMouseButtonUp(0)) //bootleg soltion, find better logical way
                {
                    buttonImage = null;
                }

                if(buttonImage)
                {
                    buttonImage.color = colorpicker.color;
                }
    }

    

          [Header("Camera Orbit Settings")]
          public Transform target;
         public float distance = 2.0f;
         public float xSpeed = 20.0f;
         public float ySpeed = 20.0f;
         public float yMinLimit = -90f;
         public float yMaxLimit = 90f;

         public float distanceMin = 10f;
         public float distanceMax = 10f;
         public float distnaceIncrement = 0.4f;
         public float smoothTime = 2f;
         float rotationYAxis = 0.0f;
         float rotationXAxis = 0.0f;
         float velocityX = 0.0f;
         float velocityY = 0.0f;
         public float mouseScroll = 1.0f;

         public Transform cameraCurrentItemEditing;
        
         private bool isRotateAreaDown = false;

         public void setRotateAreaBool(bool isPointerDown)
         {
             isRotateAreaDown = isPointerDown;
         } 




          void LateUpdate()
         {

            if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
            {
                distance -= distnaceIncrement;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
            {
                distance += distnaceIncrement;
            }

            if(distance > distanceMax)
            {
                distance = distanceMax;
            }
            if(distance < distanceMin)
            {
                distance = distanceMin;
            }



             if (target)
             {
                 if (Input.GetMouseButton(1))
                 {
                     velocityX += xSpeed * Input.GetAxis("Mouse X") * 0.02f;
                     velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
                 }
                 rotationYAxis += velocityX;
                 rotationXAxis -= velocityY;
                 rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
                 Quaternion fromRotation = Quaternion.Euler(cameraCurrentItemEditing.rotation.eulerAngles.x, cameraCurrentItemEditing.rotation.eulerAngles.y, 0);
                 Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
                 Quaternion rotation = toRotation;
     
                // distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
                 //RaycastHit hit;
                 //if (Physics.Linecast(target.position, transform.position, out hit))
                 //{
                //     distance -= hit.distance;
              //   }
                 Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                 Vector3 position = rotation * negDistance + target.position;
     
                 cameraCurrentItemEditing.rotation = rotation;
                 cameraCurrentItemEditing.position = position;
                 velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
                 velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
             }
         }
         public static float ClampAngle(float angle, float min, float max)
         {
             if (angle < -360F)
                 angle += 360F;
             if (angle > 360F)
                 angle -= 360F;
             return Mathf.Clamp(angle, min, max);
         }





        //POSTER
        public void UpdatePosterAfterChangingDimensions()
        {
            currentObjectEditing.GetComponent<PosterMeshCreator>().RebuildMeshCollider();
        }




        //FRAMES
         public void AddFrameToPoster()
         {
            //create frame
            GameObject newFrame = Instantiate(framePreset, currentObjectEditing.transform.position, currentObjectEditing.transform.rotation);

            //make it a child of poster
            newFrame.transform.parent = currentObjectEditing.transform;
            newFrame.transform.localScale = Vector3.one;

            //apply mesh
            if(currentObjectEditing.GetComponent<PosterFrameList>().posterFrameList.Count == 0)
            {
            newFrame.GetComponent<PosterMeshCreator_BorderFrame>().meshOfObjectToDrawFrameAround = currentObjectEditing.GetComponent<MeshFilter>();
            }
            else
            {
            newFrame.GetComponent<PosterMeshCreator_BorderFrame>().meshOfObjectToDrawFrameAround = currentObjectEditing.GetComponent<PosterFrameList>().posterFrameList.Last().GetComponent<MeshFilter>();
            }

            //add newly create frame to the frame list of the object
            currentObjectEditing.GetComponent<PosterFrameList>().posterFrameList.Add(newFrame);

            //play add sound
            addFrame_AudioSource.Play();

            UpdateFrameList();
         }



                  public void RemoveFrameFromPoster()  //last one
         {
            
            if(frameLayer_currentlySelected)
            {
             GameObject.Destroy(frameLayer_currentlySelected);
            currentObjectEditing.GetComponent<PosterFrameList>().posterFrameList.Remove(frameLayer_currentlySelected);


            //play remove sound
            removeFrame_AudioSource.Play();

             UpdateFrameList();
            }
                        else
            {
                UINotificationHandler.Instance.SpawnNotification("<color=red>Error: No layer selected to delete");
            } 

         }

         public void UpdateFrameList()
         {
                             //delete frame buttons objects
                foreach(GameObject frameObj in frameLayerListOfLayersCreated)
                {
                    GameObject.Destroy(frameObj);
                }
                //clear the list
                frameLayerListOfLayersCreated.Clear();


                         //create buttons depending on the count of frames the poster has
            for(int i = 0;  i < currentObjectEditing.GetComponent<PosterFrameList>().posterFrameList.Count; i++)
            {
                GameObject frameButton = GameObject.Instantiate(frameLayerPrefab);
                frameButton.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
                frameButton.transform.SetParent(frameLayerList, false);

                frameButton.GetComponent<FrameUIButton_FrameHolder>().currentAssignedFrame = currentObjectEditing.GetComponent<PosterFrameList>().posterFrameList[i];
            
                frameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Frame: " + (i + 1);

                frameLayerListOfLayersCreated.Add(frameButton);
            }

        if(currentObjectEditing.GetComponent<PosterFrameList>().posterFrameList.Count > 0)
        {
            //assigns the last frame to have the appropriate bool checked
                    for(int i = 0; i < currentObjectEditing.GetComponent<PosterFrameList>().posterFrameList.Count - 1 ;i++)
        {
            currentObjectEditing.GetComponent<PosterFrameList>().posterFrameList[i].GetComponent<PosterMeshCreator_BorderFrame>().isLastFrame = false;
        }
            currentObjectEditing.GetComponent<PosterFrameList>().posterFrameList[currentObjectEditing.GetComponent<PosterFrameList>().posterFrameList.Count - 1].GetComponent<PosterMeshCreator_BorderFrame>().isLastFrame = true;
        }

        Highlight_TheCurrentlySelectedFrameLayer();
         }


    public void Highlight_TheCurrentlySelectedFrameLayer()
    {
        foreach(GameObject obj in frameLayerListOfLayersCreated)
        {
            obj.GetComponent<CanvasGroup>().alpha = 0.2f;
        }
        
        if(frameLayerButton_currentlySelected)
        frameLayerButton_currentlySelected.GetComponent<CanvasGroup>().alpha = 1.0f;
    }



        public void UpdateBorderWidth(float sliderValue)
        {
            frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().frame_width = frame_widthSlider.value;
            frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().UpdateFrame();
        }
        public void UpdateBorderHeight(float sliderValue)
        {
            frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().frame_height = frame_heightSlider.value;
            frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().UpdateFrame();
        }
        public void UpdateBorderDepth(float sliderValue)
        {
            frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().heightDepth = frame_depthSlider.value;
            frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().UpdateFrame();
        }

        public void UpdateAllFrameBorderValues()
        {
            foreach (GameObject frame in currentObjectEditing.GetComponent<PosterFrameList>().posterFrameList)
            {
                frame.GetComponent<PosterMeshCreator_BorderFrame>().UpdateFrame();
            } 
        }

        public void UpdateBorderFrameValues()
        {
            frameLayer_currentlySelected.GetComponent<PosterMeshCreator_BorderFrame>().UpdateFrame();
        }

        public void UpdateMeshCollider()
        {
            if(currentObjectEditing)
            currentObjectEditing.GetComponent<PosterMeshCreator>().RebuildMeshCollider();
        }






    //DEPTH LAYERS
  public void AddDepthLayerToPoster()
         {
            //create depth layer
            GameObject newDepthLayer = Instantiate(depthLayerPrefab, currentObjectEditing.transform.position, currentObjectEditing.transform.rotation);

            //make it a child of poster
            newDepthLayer.transform.parent = currentObjectEditing.transform;
            newDepthLayer.transform.localScale = Vector3.one;

            //add newly create frame to the frame list of the object
            currentObjectEditing.GetComponent<PosterDepthLayerList>().posterDepthLayerList.Add(newDepthLayer);


            newDepthLayer.GetComponent<Poster_DepthStencilFrame>().AssignStencilMatSettings();

            //play add sound
            addFrame_AudioSource.Play();

            UpdateDepthLayerList();

            CheckIfThereAreDepthLayersInPoster(); //use it here instead of UpdateDepthLayerList or else the image will always be re-loded on poster edit
         }



                  public void RemoveDepthLayerFromPoster()  //currently selected one
         {

            if(depthLayer_currentlySelected)
            {
             GameObject.Destroy(depthLayer_currentlySelected);
             currentObjectEditing.GetComponent<PosterDepthLayerList>().posterDepthLayerList.Remove(depthLayer_currentlySelected);

            //play remove sound
            removeFrame_AudioSource.Play();

             UpdateDepthLayerList();

             CheckIfThereAreDepthLayersInPoster(); //use it here instead of UpdateDepthLayerList or else the image will always be re-loded on poster edit
            }
            else
            {
                UINotificationHandler.Instance.SpawnNotification("<color=red>Error: No layer selected to delete");
            } 
         }





         public void UpdateDepthLayerList()
         {
                             //delete frame buttons objects
                foreach(GameObject depthLayerObj in depthLayerListOfLayersCreated)
                {
                    GameObject.Destroy(depthLayerObj);
                }
                //clear the list
                depthLayerListOfLayersCreated.Clear();


            //create buttons depending on the count of frames the poster has
            for(int i = 0;  i < currentObjectEditing.GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count; i++)
            {
                GameObject depthLayerButton = GameObject.Instantiate(depthLayerButtonPreset);
                depthLayerButton.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
                depthLayerButton.transform.SetParent(depthLayerList, false);

                depthLayerButton.GetComponent<StencilDepthLayerUIButton_Holder>().currentAssignedDepthLayer = currentObjectEditing.GetComponent<PosterDepthLayerList>().posterDepthLayerList[i];

                //long-ass get... in the end. it assigns the poster reference media name as the layer name    
                //check if there is poster
                if(currentObjectEditing.GetComponent<PosterDepthLayerList>().posterDepthLayerList[i].GetComponent<Poster_DepthStencilFrame>().posterToReference)
                {
                    depthLayerButton.GetComponentInChildren<TextMeshProUGUI>().text = currentObjectEditing.GetComponent<PosterDepthLayerList>().posterDepthLayerList[i].GetComponent<Poster_DepthStencilFrame>().posterToReference.GetComponent<PosterMeshCreator>().ImageName(false);
                }
                else
                {
                    depthLayerButton.GetComponentInChildren<TextMeshProUGUI>().text = "[Empty]";
                }

                //setting the render que (starting from 3001)
                currentObjectEditing.GetComponent<PosterDepthLayerList>().posterDepthLayerList[i].GetComponent<Poster_DepthStencilFrame>().UpdateGeneralValues();


                depthLayerListOfLayersCreated.Add(depthLayerButton);
            }

                Highlight_TheCurrentlySelectedDepthLayer();
         }


    public void Highlight_TheCurrentlySelectedDepthLayer()
    {
        foreach(GameObject obj in depthLayerListOfLayersCreated)
        {
            obj.GetComponent<CanvasGroup>().alpha = 0.2f;
        }
        
        if(depthLayerButton_currentlySelected)
        depthLayerButton_currentlySelected.GetComponent<CanvasGroup>().alpha = 1.0f;
    }


    //used to make sure when one thing is highlighted. the thing in the other group gets unhighlighted
    public void Highlight_GeneralLayers()
    {
                Highlight_TheCurrentlySelectedDepthLayer();
                Highlight_TheCurrentlySelectedFrameLayer();
    }
    


         
        //disable the loading of the poster media if there is one depth layer or more. This also makes it so that the media is loaded again if the last depth layer was removed
        void CheckIfThereAreDepthLayersInPoster() 
        {
            // configure the poster (the poster will check itself to see if it has any depth layers attached)
            StartCoroutine(currentObjectEditing.GetComponent<PosterMeshCreator>().LoadImage(posterURL.text));

            //  enable/disable media blocker
            if(currentObjectEditing.GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count <= 0)
            {
                depthLayersBeingUsed_Blocker.SetActive(false);
            }
            else
            {
                currentObjectEditing.GetComponent<PosterMeshCreator>().ChangeShaderOfPoster();
                depthLayersBeingUsed_Blocker.SetActive(true);
            }

            PosterDepthLayerStencilRefManager.Instance.AssignCorrectStencilRefsToAllPostersInScene();
        }


        public void Poster_OnButtonPressedToSetDepthLayerPosterID()
    {
        currentState = ItemEditStateMachineStatePosterSetDepthLayerPosterID;
    }




        //FOR SOME MOTHER-FUCKING REASON!... YOU HAVE TO USE DYNAMIC FUNCTIONS TO SET THESE VALUES ON SLIDER CHANGE.... OR ELSE IT WILL FUCK UP!


        public void UpdateDepthLayerDepth(string value)
        {
            depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().depth = float.Parse(value);
            depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().UpdateGeneralValues();
        }
         public void UpdateDepthLayerSize(string value)
        {
            depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().size = float.Parse(value);
            depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().UpdateGeneralValues();
        }

        public void UpdateDepthLayerShapeKeyCurveX(string value)
        {
            depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().shapeKey_CurveX = float.Parse(value);
            depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().UpdateGeneralValues();
        }
        public void UpdateDepthLayerShapeKeyCurveY(string value)
        {
            depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().shapeKey_CurveY = float.Parse(value);
            depthLayer_currentlySelected.GetComponent<Poster_DepthStencilFrame>().UpdateGeneralValues();
        }


        //poster preview mode button toggle
        public void EnableDisableViewPreview()
        {
            if(playerInteractionStateMachine.currentState == playerInteractionStateMachine.PlayerObjectInteractionStateViewingFrame)
            {
                playerInteractionStateMachine.fadeToBlack_Duration = 0f;
                playerInteractionStateMachine.LeavePosterViewingMode();
                                mouseLooker.wheelPickerIsTurnedOn = false;
                Invoke("DelayEnableView", 0.05f);
            }
            else
            {
                playerInteractionStateMachine.ViewPoster(currentObjectEditing, 0f);
            }
        }

        //used to make the player go back into "editing" mode after exiting preview
        void DelayEnableView()
        {
            PlayerMovementBasic.Instance.enabled = false;   
            mouseLooker.wheelPickerIsTurnedOn = true;
        }


    public void UpdateColorKeyButtonColor(Color color)
    {
        posterColorKeySettings_ColorKey_Image.color = currentObjectEditing.GetComponent<PosterColorKeySettings>().colorKey;
    }

    public void UpdatePosterShaderOnDropDownChange()
    {
         currentObjectEditing.GetComponent<PosterMeshCreator>().ChangeShaderOfPoster(posterShaderSettings_shaderName_DropDown.options[posterShaderSettings_shaderName_DropDown.value].text);
    }

    public void UpdateTextureFilteringOnToggle(bool value)
    {
        currentObjectEditing.GetComponent<PosterMeshCreator>().textureFiltering = value;
        currentObjectEditing.GetComponent<PosterMeshCreator>().UpdateTextureFiltering();
    }


    public void UpdateTextureScrollXOnInputField(string value)
    {
        currentObjectEditing.GetComponent<PosterTextureScroll>().scrollSpeed.x = float.Parse(value);
    }
    public void UpdateTextureScrollYOnInputField(string value)
    {
        currentObjectEditing.GetComponent<PosterTextureScroll>().scrollSpeed.y = float.Parse(value);
    }

    public void OnFootStepAudioInputFieldFinishedEditing(string value)
    {
       StartCoroutine(currentObjectEditing.GetComponent<PosterFootstepSound>().LoadFootStepSound(value));
    }


    public void UpdateXraySizeOnInputField(string value)
    {
        currentObjectEditing.GetComponent<PosterXraySettings>().xray_Size = float.Parse(value);
        currentObjectEditing.GetComponent<PosterXraySettings>().UpdateShaderWithXraySettings();
    }
    public void UpdateXrayTransparencyOnInputField(string value)
    {
        currentObjectEditing.GetComponent<PosterXraySettings>().xray_Transparency = float.Parse(value);
        currentObjectEditing.GetComponent<PosterXraySettings>().UpdateShaderWithXraySettings();
    }
    public void UpdateXraySoftnessOnInputField(string value)
    {
        currentObjectEditing.GetComponent<PosterXraySettings>().xray_Softness = float.Parse(value);
        currentObjectEditing.GetComponent<PosterXraySettings>().UpdateShaderWithXraySettings();
    }

/*
        //DOOR EDITING FUNCTIONS
        public void onRoomURLInputFieldFinishEditing()
    {
       currentObjectEditing.GetComponent<DoorGenerateDisplayInfo>().LoadRoomPreview(door_Path.text);
       currentObjectEditing.GetComponent<DoorInfo>().pathFileToLoad = door_Path.text;
    }
*/


        //Dialogue editing functions
    public void OnDialogueVoiceInputFieldFinishEditing(string path)
    {
       StartCoroutine(currentObjectEditing.GetComponent<DialogueContentObject>().LoadAudio(path));
    }

    public void OnAudioSourceInputFieldFinishEditing(string path)
    {
       StartCoroutine(currentObjectEditing.GetComponent<AudioSourceObject>().LoadAudio(path));
    }

    public void PreviewDialogueButton()
    {
        //create a fresh dialogue object for preview (because if you use the source... the events might get popped)
        
        // Create a temporary GameObject
        GameObject tempObject = new GameObject("PreviewDialogue");

        // Add a temporary component to the GameObject
        DialogueContentObject previewDialogue = tempObject.AddComponent<DialogueContentObject>();
        previewDialogue.dialogue = currentObjectEditing.GetComponent<DialogueContentObject>().dialogue;
        previewDialogue.voice_AudioClip = currentObjectEditing.GetComponent<DialogueContentObject>().voice_AudioClip;
        previewDialogue.pitch = currentObjectEditing.GetComponent<DialogueContentObject>().pitch;

        previewDialogue.clearPreviousDialogue = true; //force it to be true for preview dialogue

        DialogueBoxStateMachine.Instance.AddDialogue(tempObject.GetComponent<DialogueContentObject>());
    }





    //state editing functions
    public void OnButtonPressedToAddStateToList()
    {
        if(!IsStringIsAlreadyInStateList(stateEditor_stateToAdd_InputField.text))
        {
            //add the state to the entity state list
            currentObjectEditing.GetComponent<State>().states.Add(stateEditor_stateToAdd_InputField.text);
            
            //refresh
            PopulateStateList();
        }
        
    }
    
    public void PopulateStateList()
    {

            //clear list
            foreach(GameObject obj in stateEditor_stateElementList)
            {
                GameObject.Destroy(obj);
            }
            stateEditor_stateElementList.Clear();


            //fill list with current object's states
            foreach(string state in currentObjectEditing.GetComponent<State>().states)
            {
                GameObject stateElement = GameObject.Instantiate(stateEditor_stateListElement_Prefab, Vector3.zero, Quaternion.identity);

                stateElement.GetComponent<TMP_InputField>().text = state;
                stateElement.transform.SetParent(stateEditor_stateList_GameObject.transform, false);
                stateEditor_stateElementList.Add(stateElement);
            }
    }


    bool IsStringIsAlreadyInStateList(string text)
    {
            bool isAlreadyInList = false;

            foreach(string state in currentObjectEditing.GetComponent<State>().states)
            {
                if(text == state)
                {
                    isAlreadyInList = true;
                    UINotificationHandler.Instance.SpawnNotification("<color=red>Error: cannot use duplicates");
                    break;
                }
            }

            return isAlreadyInList;
    }


    public void UpdateStateString(GameObject obj)
    {
        if(!IsStringIsAlreadyInStateList(obj.GetComponent<TMP_InputField>().text))
        {
            for(int i = 0; i <  stateEditor_stateList_GameObject.transform.childCount; i++)
            {
                if(stateEditor_stateList_GameObject.transform.GetChild(i).gameObject == obj)
                {
                    currentObjectEditing.GetComponent<State>().states[i] = obj.GetComponent<TMP_InputField>().text;
                }
            }
        }
    }

    public void DeleteStateFromList(GameObject obj)
    {
            //remove from state element list(gameobject), remove from state list(string), and then destroy
            stateEditor_stateElementList.Remove(obj);
            currentObjectEditing.GetComponent<State>().states.Remove(obj.GetComponent<TMP_InputField>().text);

            Destroy(obj);

    }


    public void MoveStateLayerUp(GameObject obj)
    {
        for(int i = 0; i <  stateEditor_stateList_GameObject.transform.childCount; i++)
            {
                if(stateEditor_stateList_GameObject.transform.GetChild(i).gameObject == obj)
                {
                    GlobalUtilityFunctions.MoveStringListElementUp(currentObjectEditing.GetComponent<State>().states, i);
                }
            }

        PopulateStateList();
    }

    public void MoveStateLayerDown(GameObject obj)
    {
        for(int i = 0; i <  stateEditor_stateList_GameObject.transform.childCount; i++)
            {
                if(stateEditor_stateList_GameObject.transform.GetChild(i).gameObject == obj)
                {
                    GlobalUtilityFunctions.MoveStringListElementDown(currentObjectEditing.GetComponent<State>().states, i);
                }
            }

        PopulateStateList();
    }

    //audiosource editing functions

    public void PlayAudioSource()
    {
        currentObjectEditing.GetComponent<AudioSource>().Play();
    }

    public void StopAudioSource()
    {
        currentObjectEditing.GetComponent<AudioSource>().Stop();
    }





    //entity pointer editing functions

    public void AssignCurrentlySelectedGlobalEntity(GameObject obj)
    {
        currentlyHighlightedEntityUIObject = obj;


        currentObjectEditing.GetComponent<GlobalParameterPointerEntity>().idOfGlobalEntityToPointTo = currentlyHighlightedEntityUIObject.GetComponent<GlobalParameterUIHolder>().idOfGlobalEntityToPointTo;


        //change the sprite of the global pointer to whatever gameobject its pointing to
        currentObjectEditing.GetComponent<GlobalParameterPointerEntity>().AssignImageOfEntityToPointTo();



        RefreshGlobalEntityList();
    }


    public void PopulateGlobalAndLocalEntityList()
    {
        GlobalParameterManager.Instance.LoadGlobalEntities();

        //clear groups
        foreach(GameObject obj in globalEntityEditor_UIentityList)
        {
            Destroy(obj);
        }
        globalEntityEditor_UIentityList.Clear();

        foreach(GameObject obj in localEntityEditor_UIentityList)
        {
            Destroy(obj);
        }
        localEntityEditor_UIentityList.Clear();




        //fill groups
        foreach(GameObject obj in GlobalParameterManager.Instance.ReturnObjectListOfGlobalEntitites())
        {
        GameObject newUiEntity = Instantiate(UIEntityHolderPrefab);
        newUiEntity.transform.SetParent(globalEntityEditor_UIEntityHolderContainer.transform, false);

        newUiEntity.GetComponent<GlobalParameterUIHolder>().idOfGlobalEntityToPointTo = obj.GetComponent<GeneralObjectInfo>().id;
        newUiEntity.GetComponent<GlobalParameterUIHolder>().gameObjectEntity = obj;
        newUiEntity.GetComponent<GlobalParameterUIHolder>().UpdateUIInfo();

        globalEntityEditor_UIentityList.Add(newUiEntity);
        }

        foreach(GameObject obj in GlobalParameterManager.Instance.ReturnObjectListOfLocalVariables())
        {
        GameObject newUiEntity = Instantiate(UIEntityHolderPrefab);
        newUiEntity.transform.SetParent(localEntityEditor_UIEntityHolderContainer.transform, false);

        newUiEntity.GetComponent<GlobalParameterUIHolder>().idOfGlobalEntityToPointTo = obj.GetComponent<GeneralObjectInfo>().id;
        newUiEntity.GetComponent<GlobalParameterUIHolder>().gameObjectEntity = obj;
        newUiEntity.GetComponent<GlobalParameterUIHolder>().UpdateUIInfo();

        localEntityEditor_UIentityList.Add(newUiEntity);
        }





        //for highlighting the current assigned entity on the entity list
        foreach(GameObject obj in globalEntityEditor_UIentityList)
        {
            if(obj.GetComponent<GlobalParameterUIHolder>().idOfGlobalEntityToPointTo == currentObjectEditing.GetComponent<GlobalParameterPointerEntity>().idOfGlobalEntityToPointTo)
            currentlyHighlightedEntityUIObject = obj;
        }

        foreach(GameObject obj in localEntityEditor_UIentityList)
        {
            if(obj.GetComponent<GlobalParameterUIHolder>().idOfGlobalEntityToPointTo == currentObjectEditing.GetComponent<GlobalParameterPointerEntity>().idOfGlobalEntityToPointTo)
            currentlyHighlightedEntityUIObject = obj;
        }



        RefreshGlobalEntityList();
        
    }

    public void RefreshGlobalEntityList()
    {
        //highlight the currently selected entity (by setting the alpha of the group)
        foreach(GameObject obj in globalEntityEditor_UIentityList)
        {
            obj.GetComponent<CanvasGroup>().alpha = 0.25f;
        }
        foreach(GameObject obj in localEntityEditor_UIentityList)
        {
            obj.GetComponent<CanvasGroup>().alpha = 0.25f;
        }


        if(currentlyHighlightedEntityUIObject != null)
        {
        currentlyHighlightedEntityUIObject.GetComponent<CanvasGroup>().alpha = 1.0f;
        }

    }


    //date editing functions

    public void Date_AssignEmptyTimeTemplateDefault()
    {
        currentObjectEditing.GetComponent<DateEventComponent>().ResetToDefault();
        DateEditor_date_InputField.text = currentObjectEditing.GetComponent<Date>().date.ToString();
    }

    public void Date_AssignCurrentTime()
    {
        currentObjectEditing.GetComponent<DateEventComponent>().SetToCurrentDateAndTime();
        DateEditor_date_InputField.text = currentObjectEditing.GetComponent<Date>().date.ToString();
    }


    //Light editing functions
    public void SetLightColor(Image buttImage)
    {
        buttonImage = buttImage;
        
        colorpicker.InitialColor = currentObjectEditing.GetComponent<LightEntity>().color;
        colorpicker.color = currentObjectEditing.GetComponent<LightEntity>().color;
        colorpicker.ActivateColorPicker();

    }



    //shader picking functions
    public void OpenShaderPickingWindow()
    {
        shaderPickingWindow.SetActive(true);
        ShaderPickingManager.Instance.HighlightCurrentSelectedShader();
    }



}
