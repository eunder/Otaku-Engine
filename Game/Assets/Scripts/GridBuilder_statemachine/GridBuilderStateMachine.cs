using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;
using TMPro;
using System.Linq;

public class GridBuilderStateMachine : MonoBehaviour
{
   private static GridBuilderStateMachine _instance;
    public static GridBuilderStateMachine Instance { get { return _instance; } }

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
    public IGridBuilderState currentState;
    public GridBuilderState_Idle GridBuilderStateIdle = new GridBuilderState_Idle(); 
    public GridBuilderState_Selection GridBuilderStateSelection = new GridBuilderState_Selection(); 
    public GridBuilderState_CubeCreator GridBuilderStateCubeCreator = new GridBuilderState_CubeCreator(); 
    public GridBuilderState_CubeDeleter GridBuilderStateCubeDeleter = new GridBuilderState_CubeDeleter(); 

    public GridBuilderState_CubeResizer GridBuilderStateCubeResizer = new GridBuilderState_CubeResizer(); 
    public GridBuilderState_CubeEdgeEditor GridBuilderStateCubeEdgeEditor = new GridBuilderState_CubeEdgeEditor(); 
    public bool newStateOnceEvent = false;

    public int i = 0;

    public PlayerObjectInteractionStateMachine playerObjectInteractionStateMachine; //makes sure to enable player interaction state machine when finished editing...

    public GameObject EditorCube; //parent with child cube in it
    public GameObject cubeMesh; //child of parent
    public TransformArrow_PlaneFacePlayer_OneAxisRotation planeThatFacesPlayerOneAxis;
    public Vector3 wallHitnormal;
    public Vector3 wallHitnormal_start; //help the plane that faces player orienttate depending...
    public Plane m_Plane;
    public Camera mainCam;
    public GameObject pointOnPlane; //ticker
    public Vector3 PosrelativeToCenterPlane;
    public GameObject customeCubePrefab; //apply cubeToScale world position to this object.
    public GridDrawer grid;
    public GridSizeManager gridManager;
    public SimpleSmoothMouseLook mouseLooker; // to disable and enable mouse while placing
    public LayerMask collidableLayers_layerMask;
    public AnimationCurve createShakeAmount_Curve;

    public GameObject BluePrint_Model;
    public GameObject BluePrintPencil_Model;
    public TextMeshProUGUI bluePrintGridSize_Text;
    public TextMeshProUGUI bluePrintTotalBlocks_Text;
    public TextMeshProUGUI bluePrintDimensions_Text;

    //debugging
    public GameObject closestCorner_Model;
    public GameObject secondClosestCorner_Model;


    public GameObject closestEdge_Model;

    public GameObject closestPointOnEdge_Model;



    //selection
    public enum SelectionGymbalPosition{Center, Pivot};
    public enum SelectionGymbalOrientation{Global, Local};
    public SelectionGymbalPosition selectionGymbalPosition;
    public SelectionGymbalOrientation selectionGymbalOrientation;

    public TMP_Dropdown selectionGymbal_Position_DropDown;
    public TMP_Dropdown selectionGymbal_Orientation_DropDown;



    public TMP_InputField selectedObjectProperties_posX_InputField;
    public TMP_InputField selectedObjectProperties_posY_InputField;
    public TMP_InputField selectedObjectProperties_posZ_InputField;

    public TMP_InputField selectedObjectProperties_rotX_InputField;
    public TMP_InputField selectedObjectProperties_rotY_InputField;
    public TMP_InputField selectedObjectProperties_rotZ_InputField;



    public GameObject selectionLowLevelPanelHolder_Canvas;



    public List<GameObject> selectedObjects = new List<GameObject>();

    public LayerMask selectionTool_LayerMask;


    public Transform gymbalArrowsContainer;
    public Transform gymbalThreeArrowWidget;


    public Vector3 gimbalParentStartPos; //for resetting the position on cancel
    public Quaternion gimbalParentStartRot; //for resetting the rotation on cancel




    public Color selectionBoxAdd_Color;
    public Color selectionBoxRemove_Color;

    public GameObject selectionInfo_Canvas;
    public TextMeshProUGUI selectionCount_Text;
    public Transform positionerTicker;
    public GameObject positionerContainer;

    public RectTransform selectionBox;

    public GameObject addingIcon;
    public GameObject removingIcon;

    //cube resizing
    public GameObject rescaleEightArrowGimbal;
    public GameObject cubeResizeArrow_Y_front;
    public GameObject cubeResizeArrow_Y_back;
    public GameObject cubeResizeArrow_X_front;
    public GameObject cubeResizeArrow_X_back;
    public GameObject cubeResizeArrow_Z_front;
    public GameObject cubeResizeArrow_Z_back;
    public Transform cubeResizeGimbalContainer;

    public GameObject currentCubeToResize;

    //edge edit
    public Transform edgeEditGimbal;
    public Transform edgeEditGimbalContainer;

    public GameObject selectedCornerSetterPrefab;

    public enum EditMode {corner, edge, face};

    public EditMode editMode;

    public List<SaveAndLoadLevel.Corner> selectedCorners = new List<SaveAndLoadLevel.Corner>(); // list of corners

    public List<GameObject> listOfSelectedCornerObjects = new List<GameObject>(); //the list of prefabs that modify the corners

    public GameObject selectedCornersLineRenderer_GameObject;


    public int gridIncrementIndex = 0;

    //the material(s) that will be used to "highlight" cubes
    public Material[] edgeEditor_HighLightMatList;
 

    public GameObject edgeEditModel_Head;
    public GameObject edgeEditModel_Eyes;
    public GameObject edgeEditModel_modeCorner;
    public GameObject edgeEditModel_modeEdge;
    public GameObject edgeEditModel_modeFace;


    public AnimationCurve incrementSizeToParticleSize_AnimationCurve;
    public AnimationCurve incrementSizeToPitch_AnimationCurve;
    public AnimationCurve incrementSizeToVolume_AnimationCurve;

    public AudioSource edgeEditIncrement_AudioSource;
    public AudioSource edgeEditChangeMode_AudioSource;
    public GameObject edgeEditParticle_Prefab;

    //texturing
    public GridBuilderState_TextureFace_Fit GridBuilderState_TextureFaceFit = new GridBuilderState_TextureFace_Fit();
    public GridBuilderState_TextureFace_Scale GridBuilderState_TextureFaceScale = new GridBuilderState_TextureFace_Scale();
    public GridBuilderState_TextureFace_Offset GridBuilderState_TextureFaceOffset = new GridBuilderState_TextureFace_Offset();
    public GridBuilderState_TextureFace_Rotation GridBuilderState_TextureFaceRotation = new GridBuilderState_TextureFace_Rotation();

    public GridBlockQuadHighlighter blockQuadHighLighter;

    public Color previewCube_error;
    public Color previewCube_correct;

    public AudioSource UVScale_AudioSource;

    //material "painting"
    public GridBuilderState_BlockMaterialPaint GridBuilderStateBlockMaterialPaint = new GridBuilderState_BlockMaterialPaint();
    public Material materialToApply;
    public string materialNameToApply;
    //sounds
    public AudioSource gridBuilderAudioSource_Build;
    public AudioSource gridBuilderAudioSource_StepBack;
    public AudioSource gridBuilderAudioSource_Create;
    public AudioClip gridBuierAudioClip_confirm1;
    public AudioClip gridBuierAudioClip_finalize1;
    public AudioClip gridBuierAudioClip_gridsnap1;
    public AudioClip gridBuierAudioClip_error;

    public AudioClip gridBuilderPlacePoint1_AudioClip;
    public AudioClip gridBuilderPlacePoint2_AudioClip;
    public AudioClip gridBuilderPlacePoint3_AudioClip;

    public AudioClip gridBuilderErasePoint1_AudioClip;

    
    public AnimationCurve pitchCreate_Curve;


    public GameObject Hammer_Model;
    public Animator Hammer_anim;

    public AudioSource gridBuilderDestroyer_AudioSource;
    public AudioClip gridBuilderDestroyerGlass_AudioClip;
    public AudioClip gridBuilderDestroyerHit_AudioClip;

    public AnimationCurve pitchCurve;

    //deletion
    public GameObject deletion_Particle;

 
     //tool tip
    public TextMeshProUGUI toolTipText;

    public GameObject AIMonster;
 
    public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {
     
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
     
        return(NewValue);
    }


    void OnEnable()
    {
        currentState = GridBuilderStateIdle;
    }
    void Update()
    {


    
        currentState = currentState.DoState(this);

        if(currentStateName != currentState.ToString()) // makes sure the enter event happens by checking if the state name has changed
        {
            newStateOnceEvent = false;
        }

        currentStateName = currentState.ToString();


    }



    bool selectedObjctProperties_allowToCallOnValueChangeFunction = false;

    public void SetObjectPropertyGUIElements()
    {   
        selectedObjctProperties_allowToCallOnValueChangeFunction = false;

        List<Transform> selectedObjectList = new List<Transform>();            

        //unparent the selected objects from container or else the values will be based on the container parent!
        foreach(Transform selectedThing in gymbalArrowsContainer)
        {
            selectedObjectList.Add(selectedThing);
        }
        foreach(Transform selectedThing in gymbalArrowsContainer)
        {
            selectedThing.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
        }


        //actual value setting...
        selectedObjectProperties_posX_InputField.text = selectedObjects.Last().transform.localPosition.x.ToString();
        selectedObjectProperties_posY_InputField.text = selectedObjects.Last().transform.localPosition.y.ToString();
        selectedObjectProperties_posZ_InputField.text = selectedObjects.Last().transform.localPosition.z.ToString();
    
        selectedObjectProperties_rotX_InputField.text = selectedObjects.Last().transform.localEulerAngles.x.ToString();
        selectedObjectProperties_rotY_InputField.text = selectedObjects.Last().transform.localEulerAngles.y.ToString();
        selectedObjectProperties_rotZ_InputField.text = selectedObjects.Last().transform.localEulerAngles.z.ToString();
    

        //set back the parented objects
        foreach(Transform selectedThing in selectedObjectList)
        {
            selectedThing.SetParent(gymbalArrowsContainer);
        }


        selectedObjctProperties_allowToCallOnValueChangeFunction = true;
    }

    public void SetObjectTransformSettingsOnInputFieldValueChange()
    {
        if(selectedObjctProperties_allowToCallOnValueChangeFunction)
        {
            List<Transform> selectedObjectList = new List<Transform>();            

            //unparent the selected objects from container or else the values will be based on the container parent!
            foreach(Transform selectedThing in gymbalArrowsContainer)
            {
                selectedObjectList.Add(selectedThing);
            }
            foreach(Transform selectedThing in gymbalArrowsContainer)
            {
            selectedThing.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
            }



            //actual adjusting...
            float posX = float.TryParse(selectedObjectProperties_posX_InputField.text, out float result_posX) ? result_posX : 0f;
            float posY = float.TryParse(selectedObjectProperties_posY_InputField.text, out float result_posY) ? result_posY : 0f;
            float posZ = float.TryParse(selectedObjectProperties_posZ_InputField.text, out float result_posZ) ? result_posZ : 0f;

            float rotX = float.TryParse(selectedObjectProperties_rotX_InputField.text, out float result_rotX) ? result_rotX : 0f;
            float rotY = float.TryParse(selectedObjectProperties_rotY_InputField.text, out float result_rotY) ? result_rotY : 0f;
            float rotZ = float.TryParse(selectedObjectProperties_rotZ_InputField.text, out float result_rotZ) ? result_rotZ : 0f;

            selectedObjects.Last().transform.localPosition = new Vector3(posX, posY, posZ);
            selectedObjects.Last().transform.localEulerAngles = new Vector3(rotX, rotY, rotZ);



            //set back the parented objects
            foreach(Transform selectedThing in selectedObjectList)
            {
                selectedThing.SetParent(gymbalArrowsContainer);
            }

            ResetGimbalOrientation();
        }
    }


    public void SetGymbalOrientationsFromDropDown()
    {
        if(selectionGymbal_Position_DropDown.options[selectionGymbal_Position_DropDown.value].text == "Center")
        {
            selectionGymbalPosition = SelectionGymbalPosition.Center;
        }
        else
        {
            selectionGymbalPosition = SelectionGymbalPosition.Pivot;
        }

        if(selectionGymbal_Orientation_DropDown.options[selectionGymbal_Orientation_DropDown.value].text == "Global")
        {
            selectionGymbalOrientation = SelectionGymbalOrientation.Global;
        }
        else
        {
            selectionGymbalOrientation = SelectionGymbalOrientation.Local;
        }


        ResetGimbalOrientation();
    }


    public void ResetGimbalOrientation()
    {
        gymbalArrowsContainer.parent = null;

        if(selectedObjects.Count >= 1)
        {
            if(selectionGymbalPosition == SelectionGymbalPosition.Center)
            {
                gymbalThreeArrowWidget.transform.position = GlobalUtilityFunctions.CalculateAverageVectorPositionFromListOfGameObjects(selectedObjects);
            }
            else
            {
                gymbalThreeArrowWidget.transform.position = selectedObjects.Last().transform.position;
            }

            if(selectionGymbalOrientation == SelectionGymbalOrientation.Global)
            {
                gymbalThreeArrowWidget.transform.rotation = Quaternion.identity;
            }
            else
            {
                gymbalThreeArrowWidget.transform.rotation = selectedObjects.Last().transform.rotation;
            }

        }




        gymbalArrowsContainer.parent = gymbalThreeArrowWidget.transform;
                                            
        gimbalParentStartPos = gymbalThreeArrowWidget.transform.position; //save position in case of reset later
        gimbalParentStartRot = gymbalThreeArrowWidget.transform.rotation; //save rotation in case of reset later

    }





}
