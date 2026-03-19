using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;


public class ObjectVisibilityViewManager : MonoBehaviour
{
    private static ObjectVisibilityViewManager _instance;
    public static ObjectVisibilityViewManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }





    public bool gizmoViewIsOn = false;


    public GameObject noteCanvas;
    public TextMeshProUGUI noteText;

    RaycastHit noteFinderRayCastHit;

    public LayerMask noteFinderCollidableLayers;
    public GameObject currentGameObjectHovering;

    //ticker
    public GameObject currentGameObjectHoveringTicker;
    public GameObject currentGameObjectHovering_HighlighterObject; //copies the mesh and position to create a "highlight" effect on the current hovering object




    //hierchy
    public GameObject hierchyCanvas;
    public TextMeshProUGUI selectionChildCount_Text;
    public TextMeshProUGUI selectionParentCount_Text;

    public GameObject hierchy_parent_LineRenderer;
    public List<GameObject> hierchyLineRenderers_Parent = new List<GameObject>();
    public List<GameObject> hierchyLineRenderers_Children = new List<GameObject>();

    public Color parentHierchyLine_Color;
    public Color childHierchyLine_Color;

    //SFX
    public GameObject xrayModeCanvas;
    public TextMeshProUGUI xrayMode_currentMode_Text;
 
    public AudioSource xrayMode_AudioSource;
    public AudioClip xrayMode_on_AudioClip;
    public AudioClip xrayMode_off_AudioClip;



    //mostly used for showing notes
    void Update()
    {
                 if (Physics.Raycast(PlayerObjectInteractionStateMachine.Instance.transform.position, PlayerObjectInteractionStateMachine.Instance.transform.forward, out noteFinderRayCastHit, Mathf.Infinity, noteFinderCollidableLayers)
                  && !WheelPickerHandler.Instance.wheelPickerIsOpen
                  && ItemEditStateMachine.Instance.currentStateName == "ItemEditStateMachineState_Idle"
                  && currentViewMode > 0)
                {
                 //if it is an entity.. (this is use so that when the player cursor is over transform arrows... the ticker and stuff gets disabled...)
                 if(noteFinderRayCastHit.transform.GetComponent<GeneralObjectInfo>())
                 {
                    if(currentGameObjectHovering != noteFinderRayCastHit.transform.gameObject)
                    {
                        currentGameObjectHovering = noteFinderRayCastHit.transform.gameObject;

                        currentGameObjectHoveringTicker.SetActive(true);
                        currentGameObjectHoveringTicker.transform.position = currentGameObjectHovering.transform.position;

                        if(currentGameObjectHovering.GetComponent<BlockFaceTextureUVProperties>())
                        {
//                        currentGameObjectHovering_HighlighterObject.SetActive(true);
                        currentGameObjectHovering_HighlighterObject.transform.position = currentGameObjectHovering.transform.position;
                        currentGameObjectHovering_HighlighterObject.transform.rotation = currentGameObjectHovering.transform.rotation;
                        currentGameObjectHovering_HighlighterObject.GetComponent<MeshFilter>().mesh = currentGameObjectHovering.GetComponent<MeshFilter>().mesh;
                        }
                        else
                        {
                        currentGameObjectHovering_HighlighterObject.SetActive(false);
                        }
                        


                            //the actual displaying of notes
                            if(currentGameObjectHovering.GetComponent<Note>())
                            {
                                //show the notes if it isnt empty... other wise hide the box
                                if(currentGameObjectHovering.GetComponent<Note>().note.Length >= 1)
                                {
                                noteCanvas.SetActive(true);
                                noteText.text = currentGameObjectHovering.GetComponent<Note>().note;
                                }
                                else
                                {
                                    noteCanvas.SetActive(false);
                                }
                            }
                            else
                            {
                                noteCanvas.SetActive(false);
                            }


                            //drawing parent hiechery (red line)
                            if(currentGameObjectHovering.GetComponent<GeneralObjectInfo>().parentObject != null)
                            {
                                    selectionParentCount_Text.text = "is parented: yes";
                                    hierchyCanvas.SetActive(true);

                                    //clear list if any
                                    foreach(GameObject obj in hierchyLineRenderers_Parent)
                                    {
                                        if(obj)
                                        {
                                            Destroy(obj);
                                        }
                                    }
                                    hierchyLineRenderers_Parent.Clear();


                                    GameObject lineRenderer = Instantiate(hierchy_parent_LineRenderer);
                                    //styling
                                    lineRenderer.GetComponent<LineRenderer>().startColor = parentHierchyLine_Color;
                                    lineRenderer.GetComponent<LineRenderer>().endColor = parentHierchyLine_Color;
                                    lineRenderer.GetComponent<LineRenderer>().SetPosition(0, currentGameObjectHovering.transform.position);
                                    lineRenderer.GetComponent<LineRenderer>().SetPosition(1, currentGameObjectHovering.GetComponent<GeneralObjectInfo>().parentObject.transform.position);                        

                                    hierchyLineRenderers_Parent.Add(lineRenderer);
                         
                            }
                            else
                            {
                                    selectionParentCount_Text.text = "is parented: no";

                                    //clear list if any
                                    foreach(GameObject obj in hierchyLineRenderers_Parent)
                                    {
                                        if(obj)
                                        {
                                            Destroy(obj);
                                        }
                                    }
                                    hierchyLineRenderers_Parent.Clear(); 
                            }
                            


                            //drawing child hiechery (blue lines)
                            if(currentGameObjectHovering.GetComponent<GeneralObjectInfo>().children.Count >= 1)
                            {
                                    selectionChildCount_Text.text = "Child Count: " + currentGameObjectHovering.GetComponent<GeneralObjectInfo>().children.Count;
                                    hierchyCanvas.SetActive(true);


                                    //clear list if any
                                    foreach(GameObject obj in hierchyLineRenderers_Children)
                                    {
                                        if(obj)
                                        {
                                            Destroy(obj);
                                        }
                                    }
                                    hierchyLineRenderers_Children.Clear();

                                    foreach(GameObject child in currentGameObjectHovering.GetComponent<GeneralObjectInfo>().childrenObjects)
                                    {
                                        if(child != null)
                                        {
                                    GameObject lineRenderer = Instantiate(hierchy_parent_LineRenderer);
                                    //styling
                                    lineRenderer.GetComponent<LineRenderer>().startColor = childHierchyLine_Color;
                                    lineRenderer.GetComponent<LineRenderer>().endColor = childHierchyLine_Color;
                                    lineRenderer.GetComponent<LineRenderer>().SetPosition(0, child.transform.position );
                                    lineRenderer.GetComponent<LineRenderer>().SetPosition(1, currentGameObjectHovering.transform.position);                        

                                    hierchyLineRenderers_Children.Add(lineRenderer);
                                        }
                                    }
                            }
                            else
                            {
                                    selectionChildCount_Text.text = "Child Count: 0";


                                    //clear list if any
                                    foreach(GameObject obj in hierchyLineRenderers_Children)
                                    {
                                        if(obj)
                                        {
                                            Destroy(obj);
                                        }
                                    }
                                    hierchyLineRenderers_Children.Clear(); 
                            }

                            //if no parent or children, hide the hierchy window
                            if(currentGameObjectHovering.GetComponent<GeneralObjectInfo>().children.Count <= 0 && currentGameObjectHovering.GetComponent<GeneralObjectInfo>().parentObject == null)
                            {
                            hierchyCanvas.SetActive(false);
                            }
                    }
                 }
                 else //when looking at an object that isnt an entity
                 {
                        currentGameObjectHovering = null;
                        currentGameObjectHoveringTicker.SetActive(false);
                        currentGameObjectHovering_HighlighterObject.SetActive(false);

                        noteCanvas.SetActive(false);
                        hierchyCanvas.SetActive(false);
                 }


                }
                else //when looking at skybox
                {

                        currentGameObjectHovering = null;
                        currentGameObjectHoveringTicker.SetActive(false);
                        currentGameObjectHovering_HighlighterObject.SetActive(false);

                        noteCanvas.SetActive(false);
                        hierchyCanvas.SetActive(false);
                }
   
            //clear all line renderers if view mode is 0 (normal view mode is 0...)
               if(currentViewMode == 0)
            {
                currentGameObjectHoveringTicker.SetActive(false);
                currentGameObjectHovering_HighlighterObject.SetActive(false);

                ClearHierchyLineRendererLists();
            }
   
   
             //Toggle visibility of hidden objects
          if(Input.GetKeyDown(KeyCode.G) && Input.GetKey(KeyCode.LeftShift) == false && Input.GetKey(KeyCode.LeftAlt) == false && EditModeStaticParameter.isInEditMode
           && SaveAndLoadLevel.Instance.isLevelLoaded == true
           && ZipFileHandler.Instance.password_Window.activeSelf == false
           && InputEventManager_String.Instance.InputEventManagerWindow.activeSelf == false
           && InputEventManager_Counter.Instance.InputEventManagerWindow.activeSelf == false
           && GlobalUtilityFunctions.CheckIfPlayerIsEditingInputField() == false)
          {
            currentViewMode++;
            if(currentViewMode == 3)
            currentViewMode = 0;

            TriggerCurrentViewModeFunctions();
          }
   
   
    }


        public void TriggerCurrentViewModeFunctions()
        {
            if(currentViewMode == 0)
            {
             TurnOff_GizmoInteractionAndView();
             HideHiddenGameObjects();
             DisableSeeThroughNormalBlocks();
             RestorePosterOpaque();
             DisableWidgetCanvasComponents();

             EnablePostProcessing();
             RenderSettings.fog = GlobalMapSettingsManager.Instance.fog_isOn;


            //sfx
             xrayModeCanvas.SetActive(false);
             xrayMode_currentMode_Text.text = currentViewMode.ToString();
             xrayMode_AudioSource.clip = xrayMode_off_AudioClip;
             xrayMode_AudioSource.Play();
            }
            else if(currentViewMode == 1)
            {
              DisableSeeThroughNormalBlocks();
              RestorePosterOpaque();


              TurnOn_GizmoInteractionAndView();
              ShowHiddenGameObjects();
              EnableWidgetCanvasComponents();

              EnablePostProcessing();

              RenderSettings.fog = GlobalMapSettingsManager.Instance.fog_isOn;


            //sfx
             xrayModeCanvas.SetActive(true);
             xrayMode_currentMode_Text.text = currentViewMode.ToString();
             xrayMode_AudioSource.clip = xrayMode_on_AudioClip;
             xrayMode_AudioSource.Play();
            }
            else if(currentViewMode == 2)
            {
              //important, this needs to be turned on and off in case the player toggles a trigger of a block off  during view mode one. other wise the game will set the trigger material as the actual saved block's material...
              TurnOff_GizmoInteractionAndView();
              TurnOn_GizmoInteractionAndView();

              ShowHiddenGameObjects_WithSpecialMat();


             EnableSeeThroughNormalBlocks();
             MakePostersOpaque();

              DisablePostProcessing();

            RenderSettings.fog = false;

            //sfx
             xrayModeCanvas.SetActive(true);
             xrayMode_currentMode_Text.text = currentViewMode.ToString();
             xrayMode_AudioSource.clip = xrayMode_on_AudioClip;
             xrayMode_AudioSource.Play();
            }
        }











        public void ClearHierchyLineRendererLists()
        {
                //clear list if any
                foreach(GameObject obj in hierchyLineRenderers_Children)
                {
                    if(obj)
                    {
                        Destroy(obj);
                    }
                }
                hierchyLineRenderers_Children.Clear(); 

                //clear list if any
                foreach(GameObject obj in hierchyLineRenderers_Parent)
                {
                    if(obj)
                    {
                        Destroy(obj);
                    }
                }
                hierchyLineRenderers_Parent.Clear();
        }






        public int currentViewMode = 0; //0 = off, 1 = widgets/hidden blocks/triggers, 2 = + xray mode 
        bool showingHiddenObjects = false;


    //gizmo view
    public void TurnOn_GizmoInteractionAndView()
    {

        //show the gizmo layer
        PlayerObjectInteractionStateMachine.Instance.playerCamera.cullingMask |= 1 << LayerMask.NameToLayer("Gizmo");
        PlayerObjectInteractionStateMachine.Instance.playerCamera.cullingMask |= 1 << LayerMask.NameToLayer("Trigger");

        //enable raycast click interaction with gizmo layer
        PlayerObjectInteractionStateMachine.Instance.collidableLayers_layerMask |= 1 << LayerMask.NameToLayer("Gizmo");
        PlayerObjectInteractionStateMachine.Instance.collidableLayers_layerMask |= 1 << LayerMask.NameToLayer("Trigger");

        //enable picking up gizmos
        PlayerObjectInteractionStateMachine.Instance.holding_collidableLayers_layerMask |= 1 << LayerMask.NameToLayer("Gizmo");



        //also enable click interaction with blocks that have "ingorePlayerClick" bool on
        PlayerObjectInteractionStateMachine.Instance.collidableLayers_layerMask |= 1 << LayerMask.NameToLayer("IgnorePlayerClick");




        ShowTriggerBlocks();

        //update the colors of the light entities
        LightEntity[] allLightEntities = FindObjectsOfType(typeof(LightEntity), true) as LightEntity[];
        foreach(LightEntity c in allLightEntities)
        {
            c.UpdateColorOfWidget();
        }



        gizmoViewIsOn = true;
    }

    public void TurnOff_GizmoInteractionAndView()
    {

        //Hide the gizmo layer
        PlayerObjectInteractionStateMachine.Instance.playerCamera.cullingMask &=  ~(1 << LayerMask.NameToLayer("Gizmo"));
        PlayerObjectInteractionStateMachine.Instance.playerCamera.cullingMask &=  ~(1 << LayerMask.NameToLayer("Trigger"));
   
        //disable raycast click interaction with gizmo layer
        PlayerObjectInteractionStateMachine.Instance.collidableLayers_layerMask &=  ~(1 << LayerMask.NameToLayer("Gizmo"));
        PlayerObjectInteractionStateMachine.Instance.collidableLayers_layerMask &=  ~(1 << LayerMask.NameToLayer("Trigger"));

        //disable picking up gizmos
        PlayerObjectInteractionStateMachine.Instance.holding_collidableLayers_layerMask &=  ~(1 << LayerMask.NameToLayer("Gizmo"));

        //also disable click interaction with blocks that have "ingorePlayerClick" bool on
        PlayerObjectInteractionStateMachine.Instance.collidableLayers_layerMask &=  ~(1 << LayerMask.NameToLayer("IgnorePlayerClick"));


        HideTriggerBlocks();
        gizmoViewIsOn = false;
    }

    public Material[] hiddenObjectMatList_Triggers;
    public List<GameObject> triggersInMap = new List<GameObject>();

    public void ShowTriggerBlocks()
    {
        Block[] allCurrentBlocksInScene = FindObjectsOfType(typeof(Block), true) as Block[];
        for(int i = 0; i < allCurrentBlocksInScene.Length; i++)
        {
            if(allCurrentBlocksInScene[i].GetComponentInChildren<Collider>().isTrigger)
            {
                if(!IsSelectedWithSelectionTool(allCurrentBlocksInScene[i].gameObject))
                {
                    triggersInMap.Add(allCurrentBlocksInScene[i].gameObject);

                    //change all materials on each material of the block
                    allCurrentBlocksInScene[i].GetComponentInChildren<BlockFaceTextureUVProperties>()._renderer.sharedMaterials = hiddenObjectMatList_Triggers;
                }
            }
        }
    }


    public void HideTriggerBlocks() //AND also set back their textures...
    {
                foreach(GameObject bl in triggersInMap)
        {
            if(bl)
            {
                bl.GetComponent<GeneralObjectInfo>().ResetMaterials();
            }
        }

                triggersInMap.Clear(); // DOES THIS CLEAR ALL THE MEMORY?

    }

    //hidden block view
    public Material[] hiddenObjectMatList;

    public void ShowHiddenGameObjects()
    {
       foreach(GeneralObjectInfo e in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
        {
                if(e.isActive_original == false)
                {
                    e.transform.gameObject.SetActive(true);
                }
        }

    }

    public void ShowHiddenGameObjects_WithSpecialMat()
    {
        GeneralObjectInfo[] allCurrentGameObjectsInScene_t = FindObjectsOfType(typeof(GeneralObjectInfo), true) as GeneralObjectInfo[];
        List<GameObject> allCurrentGameObjectsInScene = new List<GameObject>();

        foreach (GeneralObjectInfo g in allCurrentGameObjectsInScene_t)
        {
            allCurrentGameObjectsInScene.Add(g.gameObject);
        }


        for(int i = 0; i < allCurrentGameObjectsInScene.Count; i++)
        {                                                                                                                           
            if(allCurrentGameObjectsInScene[i].GetComponent<GeneralObjectInfo>().isActive_original == false && allCurrentGameObjectsInScene[i].GetComponent<GeneralObjectInfo>().canBeDeactivatedByEngine)
            {
                if(!IsSelectedWithSelectionTool(allCurrentGameObjectsInScene[i].gameObject))
                {
                    //activate the block (if hidden)
                    allCurrentGameObjectsInScene[i].SetActive(true);


                    //manipulate the materials
                    //change all materials on each material of the block
                    allCurrentGameObjectsInScene[i].GetComponentInChildren<Renderer>().sharedMaterials = hiddenObjectMatList;
                }
            }
        } 


        //update path renderers
        UpdateAllPathLineRenderers();
    }


    public void HideHiddenGameObjects()
    {

        foreach(GeneralObjectInfo e in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
        {
            if(e.isActive_original == false)
            {
                e.transform.gameObject.GetComponent<GeneralObjectInfo>().ResetMaterials();
                e.transform.gameObject.transform.GetComponent<GeneralObjectInfo>().ResetVisibility();
            }
        }





    }



    //XRAY MODE
    public List<GameObject> normalBlocksInMap = new List<GameObject>();

    public Material[] normalBlockMatList;

    Material storedSkyBox_Material; //store the skybore to restore it

    public Material xrayModeSkybox_Material;

    public void EnableSeeThroughNormalBlocks()
    {
        //store and replace skybox
        storedSkyBox_Material = RenderSettings.skybox;
        RenderSettings.skybox = xrayModeSkybox_Material;



           Block[] allCurrentBlocksInScene = FindObjectsOfType(typeof(Block), true) as Block[];
        for(int i = 0; i < allCurrentBlocksInScene.Length; i++)
        {
            //if is active AND is not a trigger
            if(allCurrentBlocksInScene[i].GetComponentInChildren<GeneralObjectInfo>().isActive_original == true && allCurrentBlocksInScene[i].GetComponentInChildren<Collider>().isTrigger == false)
            {
                if(!IsSelectedWithSelectionTool(allCurrentBlocksInScene[i].gameObject))
                {
                    normalBlocksInMap.Add(allCurrentBlocksInScene[i].gameObject);

                    //change all materials on each material of the block
                    allCurrentBlocksInScene[i].GetComponentInChildren<BlockFaceTextureUVProperties>()._renderer.sharedMaterials = normalBlockMatList;
                }
            }
        } 
    }



    public void DisableSeeThroughNormalBlocks()
    {
        //restore skybox
        if(storedSkyBox_Material)
        {
            RenderSettings.skybox = storedSkyBox_Material;
        }


        storedSkyBox_Material = null;

           foreach(GameObject bl in normalBlocksInMap)
        {
            if(bl)
            {
                if(!IsSelectedWithSelectionTool(bl))
                {

                    bl.GetComponentInChildren<GeneralObjectInfo>().ResetMaterials();
                    
                }
            }
        }
        normalBlocksInMap.Clear(); // DOES THIS CLEAR ALL THE MEMORY?
    }



    //Make Posters Red
    public Material[] posterMatList;

    public void MakePostersOpaque()
    {
        foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
        {
                if(poster != null)
                {
                //if is active and is not a trigger...
                if(poster.GetComponentInChildren<GeneralObjectInfo>().isActive_original == true)
                {
                    if(!IsSelectedWithSelectionTool(poster))
                    {
                        //change all materials on each material of the block
                        poster.GetComponent<Renderer>().sharedMaterials = posterMatList;
            
                        if(poster.GetComponent<Renderer>().material != null && poster.GetComponent<GeneralObjectInfo>().baseMatList[0] != null )
                        {
                            //set the poster texture as the xray texture. (dont use this line if youre going with the simpler poster xray material)
                            poster.GetComponent<Renderer>().material.SetTexture("_MainTex", poster.GetComponent<GeneralObjectInfo>().baseMatList[0].GetTexture("_MainTex") );

                            poster.GetComponent<PosterMeshCreator>().NullfiySetMaterialProperties(); //because material property blocks will override the following color changes

                            //if it is a reference to another material... color it green. otherwise, if it is a source material... color it red
                            System.Guid guid;
                            if(System.Guid.TryParse(poster.GetComponent<PosterMeshCreator>().urlFilePath, out guid))
                            {
                                poster.GetComponent<Renderer>().material.SetColor("_Color", new Color(0f, 1, 0f, 0f)); //0.1f
                            }
                            else
                            {
                                poster.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.3f, 1.0f, 0.4f, 0.7f)); //red: 1.0f, 0.4f, 0.4f, 0.7F  
                            }
                        }
                    }
                }
            }
        } 

    }

    public void RestorePosterOpaque()
    {
         foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
        {
            if(poster)
            {
                if(!IsSelectedWithSelectionTool(poster))
                {
                poster.GetComponent<PosterMeshCreator>().SetMaterialProperties(); //because material property blocks will override the following color changes
            
                //set shared materials back
                poster.GetComponent<GeneralObjectInfo>().ResetMaterials();
                }
            }
        }
    }


    public void UpdateAllPathLineRenderers()
    {
                PathNode_WaypointLineRenderer[] allCurrentPathRenderersInScene = FindObjectsOfType(typeof(PathNode_WaypointLineRenderer), true) as PathNode_WaypointLineRenderer[];
        for(int i = 0; i < allCurrentPathRenderersInScene.Length; i++)
        {
            allCurrentPathRenderersInScene[i].UpdateLineRenderer();
        }
       
    }




    public bool IsSelectedWithSelectionTool(GameObject obj)
    {
        if(GridBuilderStateMachine.Instance.selectedObjects.Contains(obj))
        {
            return true;
        }
        else
        {
            return false;
        }
    }




    public void EnableWidgetCanvasComponents()
    {
        CounterCanvasUpdater[] allCounterCanvasUpdaters = FindObjectsOfType(typeof(CounterCanvasUpdater), true) as CounterCanvasUpdater[];
        foreach(CounterCanvasUpdater c in allCounterCanvasUpdaters)
        {
            c.gameObject.SetActive(true);
        }

        DialogueCanvasUpdater[] allDialogueCanvasUpdaters = FindObjectsOfType(typeof(DialogueCanvasUpdater), true) as DialogueCanvasUpdater[];
        foreach(DialogueCanvasUpdater c in allDialogueCanvasUpdaters)
        {
            c.gameObject.SetActive(true);
        }

        NoteWorldCanvasUpdater[] allNoteCanvasUpdaters = FindObjectsOfType(typeof(NoteWorldCanvasUpdater), true) as NoteWorldCanvasUpdater[];
        foreach(NoteWorldCanvasUpdater c in allNoteCanvasUpdaters)
        {
            c.gameObject.SetActive(true);
        }
    }


    public void DisableWidgetCanvasComponents()
    {
        CounterCanvasUpdater[] allCounterCanvasUpdaters = FindObjectsOfType(typeof(CounterCanvasUpdater), true) as CounterCanvasUpdater[];
        foreach(CounterCanvasUpdater c in allCounterCanvasUpdaters)
        {
            c.gameObject.SetActive(false);
        }

        DialogueCanvasUpdater[] allDialogueCanvasUpdaters = FindObjectsOfType(typeof(DialogueCanvasUpdater), true) as DialogueCanvasUpdater[];
        foreach(DialogueCanvasUpdater c in allDialogueCanvasUpdaters)
        {
            c.gameObject.SetActive(false);
        }
        
        NoteWorldCanvasUpdater[] allNoteCanvasUpdaters = FindObjectsOfType(typeof(NoteWorldCanvasUpdater), true) as NoteWorldCanvasUpdater[];
        foreach(NoteWorldCanvasUpdater c in allNoteCanvasUpdaters)
        {
            c.gameObject.SetActive(false);
        }
    }



    public void EnablePostProcessing()
    {
        SimpleSmoothMouseLook.Instance.transform.GetComponent<PostProcessVolume>().enabled = true;
    }
    public void DisablePostProcessing()
    {
        SimpleSmoothMouseLook.Instance.transform.GetComponent<PostProcessVolume>().enabled = false;
    }







/*

    public void HideLayer(string layerName)
    {
        PlayerObjectInteractionStateMachine.Instance.playerCamera.cullingMask &=  ~(1 << LayerMask.NameToLayer(layerName));
        
        //disable raycast click interaction
        PlayerObjectInteractionStateMachine.Instance.collidableLayers_layerMask &=  ~(1 << LayerMask.NameToLayer(layerName));
    }

    public void ShowLayer(string layerName)
    {
        PlayerObjectInteractionStateMachine.Instance.playerCamera.cullingMask |= 1 << LayerMask.NameToLayer(layerName);

        PlayerObjectInteractionStateMachine.Instance.collidableLayers_layerMask |= 1 << LayerMask.NameToLayer(layerName);
    }

*/


}
