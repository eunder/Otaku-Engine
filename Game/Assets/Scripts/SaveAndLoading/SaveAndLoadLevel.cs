using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Globalization;

public class SaveAndLoadLevel : MonoBehaviour
{  

    private static SaveAndLoadLevel _instance;
    public static SaveAndLoadLevel Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }






    public bool isLevelLoaded = false;


    public RawImage blackFadeImage;
    public GameObject playerObject;
    
    public GameObject toolTipsCanvas;

    public GameObject loadingScreenCanvas;

    public MusicHandler musicHandler;

    public TextMeshProUGUI loadingText;

    //public PosterMeshCreator globalSkyboxMedia;
    public LevelGlobalMediaManager globalSkyboxMedia;

    public Material[] defaultMat;

      void Start()
    {
       StartCoroutine(InitializeLevel());
    }

    IEnumerator InitializeLevel()
    {

        if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp == null || GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp == "")
        {
         yield return new WaitForSeconds(0.5f);

         blackFadeImage.DOColor(Color.clear, 1.55f);
        }
        else if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.StartsWith("http") && GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.EndsWith(".json"))
        {
            yield return GetLevelURLData(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp);

        }
        else
        {
            if (System.IO.File.Exists(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp))
            {
                try
                {
                    if(!GlobalUtilityFunctions.IsPathSafe(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp))
                    {
                        throw new UnauthorizedAccessException($"Invalid path: {GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp}");
                    }

                    string json = File.ReadAllText(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp);

                    if(!GlobalUtilityFunctions.IsJsonFileSafe(json))
                    {
                        yield break; // Exit the coroutine early if an exception occurs
                    }
                    
                    fileLevelData = JsonUtility.FromJson<LevelData>(json);


                }
                catch(Exception ex)
                {
                    UINotificationHandler.Instance.SpawnNotification("<color=red>Level Init: " + ex.Message);
                    yield break; // Exit the coroutine early if an exception occurs
                }

                    yield return LoadLevel();
            }
            else
            {
                    UINotificationHandler.Instance.SpawnNotification("<color=red>Error! Could not find file!", UINotificationHandler.NotificationStateType.error);
            }
        }

    }

  public LevelData fileLevelData;


  public GameObject customeMaterialPrefab;

  public GameObject blockPrefab;
  public GameObject posterPrefab;
  public GameObject framePrefab;
  public GameObject depthLayerPrefab;
  public GameObject doorPrefab;

  public GameObject dialoguePrefab;
  public GameObject audioSourcePrefab;
  public GameObject counterPrefab;
  public GameObject stringPrefab;
  public GameObject statePrefab;
  public GameObject pathPrefab;
  public GameObject pathChildPrefab;
  public GameObject playerMoverPrefab;
  public GameObject globalEntityPointerPrefab;
  public GameObject DatePrefab;
  public GameObject lightPrefab;
  public GameObject prefabPrefab;

[System.Serializable]
    public class LevelData
    {
    public string gameVersion;


    //global map settings
    public float shadowIntensity = -10f;
    public float lightIntensity = 1.7f;
    public Color shadowColor = new Color(0.0f,0.0f,0.0f,1);
    public Color lightColor = new Color(1,1,1,1);

    public Color skyboxTopColor = new Color(0,0,0,1);
    public Color skyboxMidColor = new Color(0,0,0,1);
    public Color skyboxBotColor = new Color(0,0,0,1);
    public float skyBoxExp = 0.1f;

    public string musicTrackPath = "";

    public string globalSkyboxMediaPath = "";
    public float globalSkyboxMedia_UVScaleX = 1.0f;
    public float globalSkyboxMedia_UVScaleY = 1.0f;
    public float globalSkyboxMedia_UVOffsetX = 0.0f;
    public float globalSkyboxMedia_UVOffsetY = 0.0f;
    public float globalSkyboxMedia_UVOffsetScrollX = 0.0f;
    public float globalSkyboxMedia_UVOffsetScrollY = 0.0f;

    public bool fog_isOn = false;
    public Color fog_Color = new Color(1,1,1,1);
    public float fog_Start = 5;
    public float fog_End = 50;


    //post processing
    public bool bloom_isOn = false;
    public float bloom_intensity = 10.48f;
    public float bloom_threshold = 1.13f;
    public Color bloom_color = Color.white;
    public float bloom_softKnee = 0.5f;
    public float bloom_diffusion = 7;

    public bool dof_isOn = false;
    public float dof_maxFocusDistance = 20f;

    public bool ao_isOn = false;





    public bool vertexSnapping = false;

    public string AIMonsterMediaPath = "SA:monster.png";

    //materials
    public List<MaterialData> allLevelMaterials = new List<MaterialData>();

    //objects
    public List<BlockData> allLevelBlocks = new List<BlockData>();
    public List<PosterData> allLevelPosters = new List<PosterData>();
    public List<DoorData> allLevelDoors = new List<DoorData>();
    public List<BasicItem> allLevelBasicItems = new List<BasicItem>();
    public List<DialogueData> allLevelDialogue = new List<DialogueData>();
    public List<AudioSourceData> allLevelAudioSources = new List<AudioSourceData>();
    public List<CounterData> allLevelCounters = new List<CounterData>();
    public List<StringData> allLevelStrings = new List<StringData>();
    public List<StateData> allLevelStates = new List<StateData>();
    public List<PathData> allLevelPaths = new List<PathData>();
    public List<PathChildData> allLevelChildPaths = new List<PathChildData>();
    public List<PlayerMoverData> allPlayerMovers = new List<PlayerMoverData>();
    public List<GlobalEntityPointerData> allGlobalEntityPointers = new List<GlobalEntityPointerData>();
    public List<DateData> allLevelDates = new List<DateData>();
    public List<LightData> allLevelLights = new List<LightData>();
    public List<PrefabData> allLevelPrefabs = new List<PrefabData>();

    public List<Event> globalEvents = new List<Event>();
    }

    //used to make sure "OnLevelStart" events of loaded additive maps get called when the player unpauses or something
    public List<Event> additiveGlobalEvents;

//saving level blocks
[System.Serializable]
    public class BlockData
    {
        public string id;
        public string note;
        public bool isActive = true;
        public Vector3 blockPos;
        public Vector3 originalPivot;
        public Vector3 blockScale;
        public Quaternion blockRotation;

        public string idOfParent;
        public int childIndex;

        public List<string> children = new List<string>();

        public bool isTrigger = false;
        public bool isScannable = false;
        public bool ignorePlayer = false;
        public bool ignorePlayerClick = false;

        public Color materialColor_y = new Color(1,1,1,1);
        public Color materialColor_yneg = new Color(1,1,1,1);
        public Color materialColor_x = new Color(1,1,1,1);
        public Color materialColor_xneg = new Color(1,1,1,1);
        public Color materialColor_z = new Color(1,1,1,1);
        public Color materialColor_zneg = new Color(1,1,1,1);
        
        public Color materialColor; //LEGACY!!! ONLY USED TO PREVENT PLAYER'S OLDER MAPS FROM NOT WORKING

        public string materialName_y;
        public string materialName_yneg;
        public string materialName_x;
        public string materialName_xneg;
        public string materialName_z;
        public string materialName_zneg;

        public string materialName; //LEGACY!!! ONLY USED TO PREVENT PLAYER'S OLDER MAPS FROM NOT WORKING


        //top
        public Corner corner_Y_X_Z ;
        public Corner corner_Y_X_Zneg ;
        public Corner corner_Y_Xneg_Z ;
        public Corner corner_Y_Xneg_Zneg ;

        //bottom
        public Corner corner_Yneg_X_Z;
        public Corner corner_Yneg_X_Zneg ;
        public Corner corner_Yneg_Xneg_Z ;
        public Corner corner_Yneg_Xneg_Zneg;


        public Vector2[] UVScale = new Vector2[6]{
            new Vector2 (1.0f,1.0f), //top
            new Vector2 (1.0f,1.0f), //x+ side
            new Vector2 (1.0f,1.0f), //bottom
            new Vector2 (1.0f,1.0f), //x- side
            new Vector2 (1.0f,1.0f), //z- side
            new Vector2 (1.0f,1.0f)  //z+ side
                                                }; 
            public Vector2[] UVOffset = new Vector2[6]{
            new Vector2 (0.0f,0.0f), //top
            new Vector2 (0.0f,0.0f), //x+ side
            new Vector2 (0.0f,0.0f), //bottom
            new Vector2 (0.0f,0.0f), //x- side
            new Vector2 (0.0f,0.0f), //z- side
            new Vector2 (0.0f,0.0f)  //z+ side
                                                }; 
            public float[] UVRotation = new float[6]{
            0f, //top
            0f, //x+ side
            0f, //bottom
            0f, //x- side
            0f, //z- side
            0f  //z+ side
                                                }; 

        public List<Event> events = new List<Event>();

    }


    [System.Serializable]
    public class Event
    {
        public string onAction;
        public string onParamater;
        public string id; // the id of the object to affect 
        public float delay;
        public string doAction;
        public string doParameter;
        public bool happenOnce = false;

        //used to tell the game if the event happened or not... this shouldnt be saved, but It will be...
        public bool hasTriggered = false;
    }


    //create a "deep" clone... because c# likes to pass references...
    public List<SaveAndLoadLevel.Event> CreateEventListDeepClone(List<SaveAndLoadLevel.Event> eventlist)
    {
        List<SaveAndLoadLevel.Event> events = new List<SaveAndLoadLevel.Event>();
        foreach(SaveAndLoadLevel.Event e in eventlist)
        {
            SaveAndLoadLevel.Event newEventCopy = new SaveAndLoadLevel.Event();
            newEventCopy.onAction = e.onAction;
            newEventCopy.onParamater = e.onParamater;
            newEventCopy.id = e.id;
            newEventCopy.delay = e.delay;
            newEventCopy.doAction = e.doAction;
            newEventCopy.doParameter = e.doParameter;
            newEventCopy.happenOnce = e.happenOnce;
            events.Add(newEventCopy);
        }
        return events;
    }


    [System.Serializable]
    public class Corner
    {
        public Vector3 corner_Pos;
    }

    [System.Serializable]
    public class MaterialData
    {
        public string materialPath;
        public string shaderName;
    }


[System.Serializable]
    public class PosterData
    {
        public string id;
        public string note;
        public string idOfParent;
        public int childIndex;
        public List<string> children = new List<string>();
        public bool isActive = true;
        public string imageUrl;
        public Color _color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public float areaWidth;
        public float areaHeight;
        public Vector3 posterPos;
        public Quaternion posterRot;
        public List<FrameData> allPosterFrames = new List<FrameData>();
        public List<DepthLayerData> allPosterDepthLayers = new List<DepthLayerData>();
        public bool isBillboard = false;
        public bool isCharacterBillboard = false;

        public string scrollingMode = "locked";
        public string alignmentMode;
        public float zoomForcedOffset; 
        public float rotationEffectAmount;
        public bool canZoom = false;
        public float extraBorder = 0f;
        public bool inverseLook = false; 

        public Color colorKey = new Color(0,1,0,1);
        public float colorKey_threshold = 0;
        public float transparencyThreshold = 0;
        public float spillCorrection = 0;
        public string shaderName = "Shader Graphs/UnlitTransparentDither";
        public bool textureFiltering = true;
        public Vector2 scrollSpeed = new Vector2(0,0);
        public string footstepSoundPath;

        public float xray_Size = 0;
        public float xray_Transparency = 1;
        public float xray_Softness = 0;
        public bool isScannable = false;

        public List<Event> events = new List<Event>();

    }

[System.Serializable]
    public class FrameData
    {
        public float frameWidth;
        public float frameHeight;
        public float frameDepth;
        public Color frameOuterColor;
        public float frameOuterLuminance;
        public Color frameInnerColor;
        public float frameInnerLuminance;
    }
    
[System.Serializable]
    public class DepthLayerData
    {
    public string IdOfPosterToReference;
    public float depth;
    public float size = 1;
    public float shapeKey_CurveX;
    public float shapeKey_CurveY;
    }


    [System.Serializable]
    public class DoorData
    {
        public string id;
        public string note;
        public string idOfParent;
        public int childIndex;
        public List<string> children = new List<string>();
        public bool isActive = true;
        public Vector3 doorPos;
        public Quaternion doorRot;
        public string doorUrl;
    }
    
    [System.Serializable]
    public class DialogueData
    {
        public string id;
        public string note;
        public string idOfParent;
        public int childIndex;
        public List<string> children = new List<string>();
        public Vector3 position;
        public Quaternion rotation;

        public string dialogue;
        public string voicePath;
        public float pitch;
        public bool clearPreviousDialogue;

        public List<Event> events = new List<Event>();

    }

    
    [System.Serializable]
    public class AudioSourceData
    {
        public string id;
        public string note;
        public string idOfParent;
        public int childIndex;
        public List<string> children = new List<string>();
        public Vector3 position;
        public Quaternion rotation;

        public string audioPath;
        public float pitch;
        public float spatialBlend;
        public float minDistance;
        public float maxDistance;
        public bool repeat;

        public List<Event> events = new List<Event>();

    }

    [System.Serializable]
    public class CounterData
    {
        public string id;
        public string note;
        public string idOfParent;
        public int childIndex;
        public List<string> children = new List<string>();
        public Vector3 position;
        public Quaternion rotation;

        public int currentCount_default;
        public int currentCount;

        public List<Event> events = new List<Event>();

    }

    [System.Serializable]
    public class StringData
    {
        public string id;
        public string note;
        public string idOfParent;
        public int childIndex;
        public List<string> children = new List<string>();
        public Vector3 position;
        public Quaternion rotation;

        public string currentString_default;
        public string currentString;

        public List<Event> events = new List<Event>();

    }

    [System.Serializable]
    public class StateData
    {
        public string id;
        public string note;
        public string idOfParent;
        public int childIndex;
        public List<string> children = new List<string>();
        public Vector3 position;
        public Quaternion rotation;
        public List<string> states = new List<string>();
        public float timeUntilChoiceBoxCloses = 0f;
        public List<Event> events = new List<Event>();
    }



    [System.Serializable]
    public class PathData
    {
        public string id;
        public string note;
        public string idOfParent;
        public int childIndex;
        public List<string> children = new List<string>();
        public Vector3 position;
        public Quaternion rotation;
        public string idOfObjectToMove;
        public bool moveToPath = false;

        public float time = 1.0f;
        public string loopType = "Linear";
        public bool closeLoop = false;
        public string pathType = "Linear";
        public bool wayPointRotation = false;

        public string easeType = "Linear";

        public List<Event> events = new List<Event>();
    }



    [System.Serializable]
    public class PathChildData
    {
        public string id;
        public string note;
        public Vector3 position;
        public Quaternion rotation;

        public string idOfParent;
        public int childIndex;
    }



   [System.Serializable]
    public class PlayerMoverData
    {
        public string id;
        public string note;
        public string idOfParent;
        public int childIndex;
        public List<string> children = new List<string>();
        public Vector3 position;
        public Quaternion rotation;

        public List<Event> events = new List<Event>();

    }

   [System.Serializable]
    public class GlobalEntityPointerData
    {
        public string id;
        public string note;
        public string idOfParent;
        public int childIndex;
        public List<string> children = new List<string>();
        public Vector3 position;
        public Quaternion rotation;


        public string idOfGlobalEntityToPointTo;


        public List<Event> events = new List<Event>();

    }


   [System.Serializable]
    public class DateData
    {
        public string id;
        public string note;
        public string idOfParent;
        public int childIndex;
        public List<string> children = new List<string>();
        public Vector3 position;
        public Quaternion rotation;

        public string date;

        public List<Event> events = new List<Event>();

    }

   [System.Serializable]
    public class LightData
    {
        public string id;
        public string note;
        public string idOfParent;
        public int childIndex;
        public List<string> children = new List<string>();
        public Vector3 position;
        public Quaternion rotation;
        public bool isActive = true;

        public string lightType = "point";

        public float range = 10.0f;
        public float strength = 1.0f;
        public Color color = Color.white;
        public float spotAngle = 30.0f;
        public string shadowType = "none";

        public List<Event> events = new List<Event>();

    }

   [System.Serializable]
    public class PrefabData
    {
        public string id;
        public string note;
        public string idOfParent;
        public int childIndex;
        public List<string> children = new List<string>();
        public bool isActive = true;
        public Vector3 position;
        public Quaternion rotation;
        public string mapToLoad;
        public bool loadOnStart = false;

        public List<Event> events = new List<Event>();
    }





    [System.Serializable]
    public class BasicItem
    {
        public Vector3 itemPos;
        public Quaternion itemRot;
        public int itemPrefabIDIndex;
    }



    //store all the loaded objects because later on you will need to refernce them again to assign certain references...

    public List<GameObject> allLoadedGameObjects = new List<GameObject>(); //NOTE... if something in the hierchy breaks... check the usage of this list... it seems slow...
    public List<GameObject> allLoadedBlocks = new List<GameObject>();
    public List<GameObject> allLoadedPosters = new List<GameObject>();

    public List<string> allLoadedGUIDs = new List<string>();


    public List<GameObject> recentlyLoadedGameObjectList = new List<GameObject>();  


    IEnumerator LoadLevel()
    {
        yield return LoadGlobalData(fileLevelData);
        yield return LoadEntities(fileLevelData);
        yield return SetHierchies();

        yield return PostLoadData(fileLevelData);
    }


    IEnumerator LoadGlobalData(SaveAndLoadLevel.LevelData levelData)
    {
        Debug.Log("Building Level...");
        Debug.Log("Loading Level Global Settings");

        Shader.SetGlobalFloat("_HighIntensity", levelData.lightIntensity);
        Shader.SetGlobalColor("_HighColor", levelData.lightColor);
        
        float factor = Mathf.Pow(2,levelData.shadowIntensity);
        RenderSettings.ambientLight = levelData.shadowColor * factor;


        GlobalMapSettingsManager.Instance.lightingIntensity_shadow = levelData.shadowIntensity;
        GlobalMapSettingsManager.Instance.lightingColor_shadow = levelData.shadowColor;


        Shader.SetGlobalColor("_AmbientLightColor", RenderSettings.ambientLight);
        Shader.SetGlobalFloat("_AmbientLightIntensity", GlobalMapSettingsManager.Instance.lightingIntensity_shadow);

        GlobalMapSettingsManager.Instance.skybox_Color_Top = levelData.skyboxTopColor;
        GlobalMapSettingsManager.Instance.skybox_Color_Top_original = levelData.skyboxTopColor;
        GlobalMapSettingsManager.Instance.skybox_Color_Middle = levelData.skyboxMidColor;
        GlobalMapSettingsManager.Instance.skybox_Color_Middle_original = levelData.skyboxMidColor;
        GlobalMapSettingsManager.Instance.skybox_Color_Bottom = levelData.skyboxBotColor;
        GlobalMapSettingsManager.Instance.skybox_Color_Bottom_original = levelData.skyboxBotColor;
        GlobalMapSettingsManager.Instance.skybox_Color_Exp = levelData.skyBoxExp;
        GlobalMapSettingsManager.Instance.skybox_Color_Exp_original = levelData.skyBoxExp;


        GlobalMapSettingsManager.Instance.linearColorskybox_Mat.SetFloat("_Exp", levelData.skyBoxExp);
     
        GlobalMapSettingsManager.Instance.UpdateSkyBoxColors();


        GlobalMapSettingsManager.Instance.fog_isOn = levelData.fog_isOn;
        GlobalMapSettingsManager.Instance.fog_Color = levelData.fog_Color;
        GlobalMapSettingsManager.Instance.fog_Start = levelData.fog_Start;
        GlobalMapSettingsManager.Instance.fog_End = levelData.fog_End;
        Shader.SetGlobalFloat("_EnableFog", levelData.fog_isOn ? 2f:0f); //have to do this because of shadergraph not having an option to turn off fog...


        //post processing
        PostProcessingManager.Instance.bloom_isOn = levelData.bloom_isOn;
        PostProcessingManager.Instance.bloom_intensity = levelData.bloom_intensity;
        PostProcessingManager.Instance.bloom_threshold = levelData.bloom_threshold;
        PostProcessingManager.Instance.bloom_color = levelData.bloom_color;
        PostProcessingManager.Instance.bloom_softKnee = levelData.bloom_softKnee;
        PostProcessingManager.Instance.bloom_diffusion = levelData.bloom_diffusion;

        PostProcessingManager.Instance.dof_isOn = levelData.dof_isOn;
        PostProcessingManager.Instance.dof_maxFocusDistance = levelData.dof_maxFocusDistance;

        PostProcessingManager.Instance.ao_isOn = levelData.ao_isOn;


        //LEGACY, try to keep old post processing style on old maps
        if(levelData.gameVersion == default(string))
        {
                    PostProcessingManager.Instance.bloom_intensity = 2f;
                    PostProcessingManager.Instance.bloom_isOn = true;
                    PostProcessingManager.Instance.ao_isOn = true;
        }





        PostProcessingManager.Instance.UpdatePostProcessingSettings();




        GlobalMapSettingsManager.Instance.vertexSnapping = levelData.vertexSnapping;
        Shader.SetGlobalFloat("_EnableVertexSnapping", levelData.vertexSnapping ? 2f:0f);

        GlobalMapSettingsManager.Instance.UpdateFogProperties();

        yield return null;
    }

        //load types: 0 = normal, 1 = additive, 2 = copy and pasted.
        //2 is used to prevent global stuff from being modified when copy and pasting...

    public int totalAmountOfCoroutinesBeingUsedForLoading = 0; //mostly used to prevent the player from spamming reset while prefabs are being loaded on level start...

    public IEnumerator LoadEntities(SaveAndLoadLevel.LevelData levelData, int loadType = 0, string zipJsonPath = "", Transform objToParentTo = null)
    {
        totalAmountOfCoroutinesBeingUsedForLoading++;

        recentlyLoadedGameObjectList.Clear();

        //primarly used to keep track of objects in THIS corutine... (to make multiple coroutines happening at the same time possible)
        List<GameObject> listOfLoadedObjectsInThisCoroutine = new List<GameObject>();




        Debug.Log("Loading Level Posters...");

        //load posters
       for(int i = 0; i < levelData.allLevelPosters.Count; i++)
        {
            //this is purely added so that when an additive map with default "dev" textures is loaded... it dosnt mess up the map on reset

            GameObject newPoster = GameObject.Instantiate(posterPrefab, levelData.allLevelPosters[i].posterPos, levelData.allLevelPosters[i].posterRot);
            newPoster.GetComponent<Note>().note = levelData.allLevelPosters[i].note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(levelData.allLevelPosters[i].id, out guid))
            {
                newPoster.name = levelData.allLevelPosters[i].id;
                newPoster.transform.GetComponent<GeneralObjectInfo>().id = levelData.allLevelPosters[i].id;
            }
            else
            {
                newPoster.name = System.Guid.NewGuid().ToString();
            }



            //general object info
            newPoster.transform.GetComponent<GeneralObjectInfo>().isActive = levelData.allLevelPosters[i].isActive;
            newPoster.transform.GetComponent<GeneralObjectInfo>().isActive_original = levelData.allLevelPosters[i].isActive;
            newPoster.transform.GetComponent<GeneralObjectInfo>().position = levelData.allLevelPosters[i].posterPos;
            newPoster.transform.GetComponent<GeneralObjectInfo>().position_original = levelData.allLevelPosters[i].posterPos;
            newPoster.transform.GetComponent<GeneralObjectInfo>().rotation = levelData.allLevelPosters[i].posterRot;
            newPoster.transform.GetComponent<GeneralObjectInfo>().rotation_original = levelData.allLevelPosters[i].posterRot;
            newPoster.transform.GetComponent<GeneralObjectInfo>().idOfParent = levelData.allLevelPosters[i].idOfParent;
            newPoster.transform.GetComponent<GeneralObjectInfo>().childIndex = levelData.allLevelPosters[i].childIndex;
            newPoster.transform.GetComponent<GeneralObjectInfo>().children = levelData.allLevelPosters[i].children;



              //depth layers
              //Assign these first because the poster will check if there are depth layers or not after this...
            for(int i2 = 0; i2 < levelData.allLevelPosters[i].allPosterDepthLayers.Count; i2++)// a for loop is required due to complexity of algorithm
            {
            GameObject newDepthLayer = GameObject.Instantiate(depthLayerPrefab, Vector3.zero, Quaternion.identity, newPoster.transform);
            newDepthLayer.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
            newDepthLayer.transform.localRotation = Quaternion.Euler(0, 0, 0);


            newDepthLayer.GetComponent<Poster_DepthStencilFrame>().IdOfPosterToReference = levelData.allLevelPosters[i].allPosterDepthLayers[i2].IdOfPosterToReference;
            newDepthLayer.GetComponent<Poster_DepthStencilFrame>().depth = levelData.allLevelPosters[i].allPosterDepthLayers[i2].depth;
            newDepthLayer.GetComponent<Poster_DepthStencilFrame>().size = levelData.allLevelPosters[i].allPosterDepthLayers[i2].size;
            newDepthLayer.GetComponent<Poster_DepthStencilFrame>().shapeKey_CurveX = levelData.allLevelPosters[i].allPosterDepthLayers[i2].shapeKey_CurveX;
            newDepthLayer.GetComponent<Poster_DepthStencilFrame>().shapeKey_CurveY = levelData.allLevelPosters[i].allPosterDepthLayers[i2].shapeKey_CurveY;


            newPoster.GetComponent<PosterDepthLayerList>().posterDepthLayerList.Add(newDepthLayer);
          
          
          
            }





            newPoster.GetComponent<PosterMeshCreator>()._color = levelData.allLevelPosters[i]._color;
            newPoster.GetComponent<PosterMeshCreator>().box.x = levelData.allLevelPosters[i].areaWidth;
            newPoster.GetComponent<PosterMeshCreator>().box.y = levelData.allLevelPosters[i].areaHeight;
            newPoster.GetComponent<PosterMeshCreator>().shaderName = levelData.allLevelPosters[i].shaderName;
            newPoster.GetComponent<PosterMeshCreator>().textureFiltering = levelData.allLevelPosters[i].textureFiltering;
            newPoster.GetComponent<PosterTextureScroll>().scrollSpeed = levelData.allLevelPosters[i].scrollSpeed;
            newPoster.GetComponent<PosterFootstepSound>().footstepSoundPath = levelData.allLevelPosters[i].footstepSoundPath;
            newPoster.GetComponent<PosterBillboard>().useBillboard = levelData.allLevelPosters[i].isBillboard;
            newPoster.GetComponent<PosterBillboard>().useCharacterBillboard = levelData.allLevelPosters[i].isCharacterBillboard;
            newPoster.GetComponent<EventHolderList>().events = CreateEventListDeepClone(levelData.allLevelPosters[i].events);


            //this makes sure there is no "ArgumentException: Path is empty System.IO.FileStream" error when there is not url path on the saved item.
            if(levelData.allLevelPosters[i].imageUrl.Length > 0)
            {
            newPoster.GetComponent<PosterMeshCreator>().urlFilePath = levelData.allLevelPosters[i].imageUrl;
            }

            loadingText.text = "Loading: Media " + (i+1) + " of " + levelData.allLevelPosters.Count + "   <color=#005DFF>" + levelData.allLevelPosters[i].imageUrl;

            //point to the correct base path...(in case its just the file name and nothing else...)
            if(zipJsonPath != "")
            {
                //ALSO MAKRE SURE ITS NOT A GUID (or this part will mess up poster references)
                System.Guid posterGUID;
                if(System.Guid.TryParse(levelData.allLevelPosters[i].imageUrl, out posterGUID) == false)
                {
                yield return newPoster.GetComponent<PosterMeshCreator>().LoadImage(System.IO.Path.GetDirectoryName(zipJsonPath) + "/" + levelData.allLevelPosters[i].imageUrl);
                }
                else
                {
                yield return newPoster.GetComponent<PosterMeshCreator>().LoadImage(levelData.allLevelPosters[i].imageUrl);


                }
                yield return newPoster.GetComponent<PosterFootstepSound>().LoadFootStepSound(System.IO.Path.GetDirectoryName(zipJsonPath) + "/" + levelData.allLevelPosters[i].footstepSoundPath);

          
            }
            else
            {
                yield return newPoster.GetComponent<PosterMeshCreator>().LoadImage(levelData.allLevelPosters[i].imageUrl);
                yield return newPoster.GetComponent<PosterFootstepSound>().LoadFootStepSound(levelData.allLevelPosters[i].footstepSoundPath);
            }
            



            //frames
            for(int i2 = 0; i2 < levelData.allLevelPosters[i].allPosterFrames.Count; i2++)// a for loop is required due to complexity of algorithm
            {
            GameObject newFrame = GameObject.Instantiate(framePrefab, Vector3.zero, Quaternion.identity, newPoster.transform);
            newFrame.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
            newFrame.transform.localRotation = Quaternion.Euler(0, 0, 0);


            newFrame.GetComponent<PosterMeshCreator_BorderFrame>().frame_width = levelData.allLevelPosters[i].allPosterFrames[i2].frameWidth;
            newFrame.GetComponent<PosterMeshCreator_BorderFrame>().frame_height = levelData.allLevelPosters[i].allPosterFrames[i2].frameHeight;
            newFrame.GetComponent<PosterMeshCreator_BorderFrame>().heightDepth = levelData.allLevelPosters[i].allPosterFrames[i2].frameDepth;
            newFrame.GetComponent<PosterMeshCreator_BorderFrame>().rimColors[0] = levelData.allLevelPosters[i].allPosterFrames[i2].frameOuterColor;
            newFrame.GetComponent<PosterMeshCreator_BorderFrame>().rimColors[1] = levelData.allLevelPosters[i].allPosterFrames[i2].frameInnerColor;
            newFrame.GetComponent<PosterMeshCreator_BorderFrame>().rimLuminance[0] = levelData.allLevelPosters[i].allPosterFrames[i2].frameOuterLuminance;
            newFrame.GetComponent<PosterMeshCreator_BorderFrame>().rimLuminance[1] = levelData.allLevelPosters[i].allPosterFrames[i2].frameInnerLuminance;

            newPoster.GetComponent<PosterFrameList>().posterFrameList.Add(newFrame);
            }

            newPoster.GetComponent<PosterFrameList>().AssignWhichFrameIsLast();
            newPoster.GetComponent<PosterFrameList>().AssignWhichMeshesFramesDrawAround();


  

            //view data
            newPoster.GetComponent<PosterViewSettings>().scrollingMode = levelData.allLevelPosters[i].scrollingMode;
            newPoster.GetComponent<PosterViewSettings>().alignmentMode = levelData.allLevelPosters[i].alignmentMode;
            newPoster.GetComponent<PosterViewSettings>().zoomForcedOffset = levelData.allLevelPosters[i].zoomForcedOffset;
            newPoster.GetComponent<PosterViewSettings>().rotationEffectAmount = levelData.allLevelPosters[i].rotationEffectAmount;
            newPoster.GetComponent<PosterViewSettings>().canZoom = levelData.allLevelPosters[i].canZoom;
            newPoster.GetComponent<PosterViewSettings>().extraBorder = levelData.allLevelPosters[i].extraBorder;
            newPoster.GetComponent<PosterViewSettings>().inverseLook = levelData.allLevelPosters[i].inverseLook;


            //color key data
            newPoster.GetComponent<PosterColorKeySettings>().colorKey = levelData.allLevelPosters[i].colorKey;
            newPoster.GetComponent<PosterColorKeySettings>().threshold = levelData.allLevelPosters[i].colorKey_threshold;
            newPoster.GetComponent<PosterColorKeySettings>().transparencyThreshold = levelData.allLevelPosters[i].transparencyThreshold;
            newPoster.GetComponent<PosterColorKeySettings>().spillCorrection = levelData.allLevelPosters[i].spillCorrection;
            newPoster.GetComponent<PosterColorKeySettings>().ApplyColorKeySettingsToShader();

            //xray settings
            newPoster.GetComponent<PosterXraySettings>().xray_Size = levelData.allLevelPosters[i].xray_Size;
            newPoster.GetComponent<PosterXraySettings>().xray_Transparency = levelData.allLevelPosters[i].xray_Transparency;
            newPoster.GetComponent<PosterXraySettings>().xray_Softness = levelData.allLevelPosters[i].xray_Softness;
            newPoster.GetComponent<PosterXraySettings>().UpdateShaderWithXraySettings();

            newPoster.GetComponent<IsScannable>().isScannable = levelData.allLevelPosters[i].isScannable;


            newPoster.SetActive(levelData.allLevelPosters[i].isActive);

            newPoster.GetComponent<PosterMeshCreator>().ChangeShaderOfPoster();

            if(loadType == 1)
            {
                newPoster.GetComponent<Collider>().enabled = false; //turn off collision so player does not get crushed on start
                newPoster.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }

            allLoadedPosters.Add(newPoster);
            recentlyLoadedGameObjectList.Add(newPoster);
            listOfLoadedObjectsInThisCoroutine.Add(newPoster);
        }






        Debug.Log("Loading Level Blocks...");
        loadingText.text = "Loading: Level Blocks";
        foreach(BlockData block in levelData.allLevelBlocks)
        {
            bool matFound_y = false;
            bool matFound_yneg = false;
            bool matFound_x = false;
            bool matFound_xneg = false;
            bool matFound_z = false;
            bool matFound_zneg = false;

            GameObject newBlock = GameObject.Instantiate(blockPrefab, block.blockPos, block.blockRotation);

            newBlock.GetComponent<IsScannable>().isScannable = block.isScannable;
            newBlock.GetComponent<Note>().note = block.note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(block.id, out guid))
            {
                newBlock.name = block.id;
                newBlock.transform.GetComponent<GeneralObjectInfo>().id = block.id;
            }
            else
            {
                newBlock.name = System.Guid.NewGuid().ToString();
            }
            
                    if(block.isTrigger)
                    {
                        newBlock.transform.GetComponent<MeshCollider>().convex = true;
                        newBlock.transform.GetComponent<MeshCollider>().isTrigger = true;                    }
                    else
                    {
                        newBlock.transform.GetComponent<MeshCollider>().convex = false;
                        newBlock.transform.GetComponent<MeshCollider>().isTrigger = false;
                    }

            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().originalPivot = block.originalPivot;

            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(0,block.materialColor_y);
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(1,block.materialColor_x);
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(2,block.materialColor_yneg);
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(3,block.materialColor_xneg);
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(4,block.materialColor_zneg);
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(5,block.materialColor_z);
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_y = block.materialName_y;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_yneg = block.materialName_yneg;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_x = block.materialName_x;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_xneg = block.materialName_xneg;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_z = block.materialName_z;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_zneg = block.materialName_zneg;


            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_X_Z.corner_Pos = block.corner_Y_X_Z.corner_Pos;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_X_Zneg.corner_Pos = block.corner_Y_X_Zneg.corner_Pos;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_Xneg_Z.corner_Pos = block.corner_Y_Xneg_Z.corner_Pos;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_Xneg_Zneg.corner_Pos = block.corner_Y_Xneg_Zneg.corner_Pos;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_X_Z.corner_Pos = block.corner_Yneg_X_Z.corner_Pos;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_X_Zneg.corner_Pos = block.corner_Yneg_X_Zneg.corner_Pos;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_Xneg_Z.corner_Pos = block.corner_Yneg_Xneg_Z.corner_Pos;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_Xneg_Zneg.corner_Pos = block.corner_Yneg_Xneg_Zneg.corner_Pos;




            //LEGEACY: only do this for older maps or else on newer maps collision for additively loaded maps will not work.... terrible... just terrible *WTF CODE*
            if(levelData.gameVersion == default(string))
            {
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().UpdateCorners();
            }


            //  LEGACY SUPPORT: convert block scale -> new corner positions
            //if there is not vertex data saved... asume the map is really old...  (oldest version which used block scaling)
            if(block.corner_Y_X_Z.corner_Pos == default(Vector3)  && levelData.gameVersion == default(string))
            {
                //use the old scale... by using the custom function made for "scaling"
                newBlock.GetComponent<BlockFaceTextureUVProperties>().ScaleBlock_Add(block.blockScale - (new Vector3(1f,1f,1f)));
        
                //add the old map world center offset too... (that other function for gameobjects does NOT change verticies...)
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_X_Z.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_X_Zneg.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_Xneg_Z.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_Xneg_Zneg.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_X_Z.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_X_Zneg.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_Xneg_Z.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_Xneg_Zneg.corner_Pos += new Vector3(-23f, -0.5f, 28f);

                //add tiny offset to align exactly
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_X_Z.corner_Pos += new Vector3(0.5f, 0.5f, 0.5f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_X_Zneg.corner_Pos += new Vector3(0.5f, 0.5f, 0.5f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_Xneg_Z.corner_Pos += new Vector3(0.5f, 0.5f, 0.5f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_Xneg_Zneg.corner_Pos += new Vector3(0.5f, 0.5f, 0.5f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_X_Z.corner_Pos += new Vector3(0.5f, 0.5f, 0.5f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_X_Zneg.corner_Pos += new Vector3(0.5f, 0.5f, 0.5f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_Xneg_Z.corner_Pos += new Vector3(0.5f, 0.5f, 0.5f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_Xneg_Zneg.corner_Pos += new Vector3(0.5f, 0.5f, 0.5f);

        
                //add this offset... it simulates the old block structure
                newBlock.transform.position += new Vector3(0.5f, 0.5f, 0.5f);
                newBlock.transform.GetComponent<GeneralObjectInfo>().position += new Vector3(0.5f, 0.5f, 0.5f);
                newBlock.transform.GetComponent<GeneralObjectInfo>().position_original += new Vector3(0.5f, 0.5f, 0.5f);
             }
             //IF THE MATERIAL NAME IS NOT A GUID... then assume it was when block scaling wasnt needed anymore... (slightly newer version)
             else if(System.Guid.TryParse(newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_z, out var result) == false  && levelData.gameVersion == default(string))
             {
                //add the old map world center offset too... (that other function for gameobjects does NOT change verticies...)
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_X_Z.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_X_Zneg.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_Xneg_Z.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_Xneg_Zneg.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_X_Z.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_X_Zneg.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_Xneg_Z.corner_Pos += new Vector3(-23f, -0.5f, 28f);
                newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_Xneg_Zneg.corner_Pos += new Vector3(-23f, -0.5f, 28f);
 
             }



            foreach(GameObject poster in allLoadedPosters)
            {
                if(poster.name  == block.materialName_y)
                {
                    newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(0, poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial);
                    matFound_y = true;
                }
                if(poster.name == block.materialName_yneg)
                {
                    newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(2, poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial);
                    matFound_yneg = true;
                }
                if(poster.name == block.materialName_x)
                {
                    newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(1, poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial);
                    matFound_x = true;
                }
                if(poster.name == block.materialName_xneg)
                {
                    newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(3, poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial);
                    matFound_xneg = true;
                }
                if(poster.name == block.materialName_z)
                {
                    newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(5, poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial);
                    matFound_z = true;
                }
                if(poster.name == block.materialName_zneg)
                {
                    newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(4, poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial);
                    matFound_zneg = true;
                }
            }




            //this part is important for legacy purposes... (7/8/2024).
            //what this does it check if a face contains a path that used to be part of the now legacy custom materials. If you dont do this... then matFound will equal "false" 
            for(int i = 0; i < levelData.allLevelMaterials.Count; i++)
            {
                if(levelData.allLevelMaterials[i].materialPath == block.materialName_y)
                {
                    matFound_y = true;
                }
                if(levelData.allLevelMaterials[i].materialPath == block.materialName_yneg)
                {
                    matFound_yneg = true;
                }
                if(levelData.allLevelMaterials[i].materialPath == block.materialName_x)
                {
                    matFound_x = true;
                }
                if(levelData.allLevelMaterials[i].materialPath == block.materialName_xneg)
                {
                    matFound_xneg = true;
                }
                if(levelData.allLevelMaterials[i].materialPath == block.materialName_z)
                {
                    matFound_z = true;
                }
                if(levelData.allLevelMaterials[i].materialPath == block.materialName_zneg)
                {
                    matFound_zneg = true;
                }
            }



            //if no custome material was found, excecute the older code

                    //IF material resource load is null asign a default, ELSE assign the the loaded mat

                    //if material is null, asign a "default" material
                    
                if(matFound_y == false)
                {  
                    if(Resources.Load("Materials/" + block.materialName_y, typeof(Material)) as Material == null)
                    {
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(0, defaultMat[0]);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_y = defaultMat[0].name;
                    }
                    else
                    {
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(0, Resources.Load("Materials/" + block.materialName_y, typeof(Material)) as Material );
                    }
                }


                if(matFound_yneg == false)
                {  
                    if(Resources.Load("Materials/" + block.materialName_yneg, typeof(Material)) as Material == null)
                    {
                        Debug.Log("TRUE!");
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(2, defaultMat[2]);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_yneg = defaultMat[2].name;
                    }
                    else
                    {
                            Debug.Log("FALSE!");
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(2, Resources.Load("Materials/" + block.materialName_yneg, typeof(Material)) as Material );
                    }
                }


                if(matFound_x == false)
                {  
                    if(Resources.Load("Materials/" + block.materialName_x, typeof(Material)) as Material == null)
                    {
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(1, defaultMat[1]);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_x = defaultMat[1].name;
                    }
                    else
                    {
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(1, Resources.Load("Materials/" + block.materialName_x, typeof(Material)) as Material );
                    }
                }

                if(matFound_xneg == false)
                {  
                    if(Resources.Load("Materials/" + block.materialName_xneg, typeof(Material)) as Material == null)
                    {
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(3, defaultMat[3]);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_xneg = defaultMat[3].name;
                    }
                    else
                    {
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(3, Resources.Load("Materials/" + block.materialName_xneg, typeof(Material)) as Material );
                    }
                }

                if(matFound_z == false)
                {  
                    if(Resources.Load("Materials/" + block.materialName_z, typeof(Material)) as Material == null)
                    {
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(5, defaultMat[5]);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_z = defaultMat[5].name;
                    }
                    else
                    {
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(5, Resources.Load("Materials/" + block.materialName_z, typeof(Material)) as Material );
                    }
                }


                if(matFound_zneg == false)
                {  
                    if(Resources.Load("Materials/" + block.materialName_zneg, typeof(Material)) as Material == null)
                    {
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(4, defaultMat[4]);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_zneg = defaultMat[4].name;
                    }
                    else
                    {
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(4, Resources.Load("Materials/" + block.materialName_zneg, typeof(Material)) as Material );
                    }
                }
        

            //LEGACY!!! USED TO PREVENT OLDER MAPS FROM BREAKING
            if(!string.IsNullOrWhiteSpace(block.materialName))
            {
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(0, Resources.Load("Materials/" + block.materialName, typeof(Material)) as Material);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(1, Resources.Load("Materials/" + block.materialName, typeof(Material)) as Material);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(2, Resources.Load("Materials/" + block.materialName, typeof(Material)) as Material);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(3, Resources.Load("Materials/" + block.materialName, typeof(Material)) as Material);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(4, Resources.Load("Materials/" + block.materialName, typeof(Material)) as Material);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().ChangeFaceMat(5, Resources.Load("Materials/" + block.materialName, typeof(Material)) as Material);
            }

            //LEGACY!!! USED TO PREVENT OLDER MAPS FROM BREAKING
            if(block.materialColor.a != 0f)
            {
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(0,block.materialColor);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(1,block.materialColor);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(2,block.materialColor);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(3,block.materialColor);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(4,block.materialColor);
                        newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().SetMaterialColorBasedOnIndex(5,block.materialColor);
            }



            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().UVScale = block.UVScale;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().UVOffSet = block.UVOffset;
            newBlock.transform.GetComponent<BlockFaceTextureUVProperties>().UVRotation = block.UVRotation;


            //assign to appropriate layer or else other logic wont detect
            if(block.isTrigger)
            {
            newBlock.layer = 17;
            }

            newBlock.SetActive(block.isActive);




            //general object info
            newBlock.transform.GetComponent<GeneralObjectInfo>().isActive = block.isActive;
            newBlock.transform.GetComponent<GeneralObjectInfo>().isActive_original = block.isActive;
            newBlock.transform.GetComponent<GeneralObjectInfo>().position = block.blockPos;
            newBlock.transform.GetComponent<GeneralObjectInfo>().position_original = block.blockPos;
            newBlock.transform.GetComponent<GeneralObjectInfo>().rotation = block.blockRotation;
            newBlock.transform.GetComponent<GeneralObjectInfo>().rotation_original = block.blockRotation;

            newBlock.transform.GetComponent<GeneralObjectInfo>().idOfParent = block.idOfParent;
            newBlock.transform.GetComponent<GeneralObjectInfo>().childIndex = block.childIndex;
            newBlock.transform.GetComponent<GeneralObjectInfo>().children = block.children;

            newBlock.transform.GetComponent<GeneralObjectInfo>().ignorePlayer = block.ignorePlayer;
            newBlock.transform.GetComponent<GeneralObjectInfo>().ignorePlayerClick = block.ignorePlayerClick;
            newBlock.transform.GetComponent<GeneralObjectInfo>().isTrigger = block.isTrigger;
            newBlock.transform.GetComponent<GeneralObjectInfo>().UpdateGeneralObjectLayerProperties();


            newBlock.transform.GetComponent<EventHolderList>().events = CreateEventListDeepClone(block.events);


            if(loadType == 1)
            {
                newBlock.GetComponent<Collider>().enabled = false; //turn off collision so player does not get crushed on start
                newBlock.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }
            allLoadedBlocks.Add(newBlock);
            recentlyLoadedGameObjectList.Add(newBlock);
            listOfLoadedObjectsInThisCoroutine.Add(newBlock);
        }





        //LEGACY: this is what converts the old custom material mechanic into posters. (prevents old maps from before the feature was added from breaking)
        //Make sure to load this after blocks in case of legacy materails (to fix the paths of block faces)
        
        DateTime dateNewPosterMaterialMechanicWasAdded = new DateTime(2024, 7, 8);

        DateTime parsedDate;
    
        // Try to parse using system's default date settings
        bool isValidDate = DateTime.TryParse(levelData.gameVersion, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);

        if(string.IsNullOrEmpty(levelData.gameVersion)    ||      (isValidDate && (parsedDate < dateNewPosterMaterialMechanicWasAdded))      )
        {
            //create a dictionary to hold the corelated posters and old materials (for use in updating block paths)
            //first parameter = old path(materialPath), second parameter = newpath(posterID)
            Dictionary<string, string> pathDictionary = new Dictionary<string, string>();

            for(int i = 0; i < levelData.allLevelMaterials.Count; i++)
            {
                loadingText.text = "Legacy materials found... converting materials into posters: " + (i+1) + " of " + levelData.allLevelMaterials.Count;
                yield return ApplyLegacyCustomMaterialMapAdjustments_CreatePosterFromMaterialData(levelData.allLevelMaterials[i], pathDictionary);
            }

            yield return ApplyLegacyCustomMaterialMapAdjustments_UpdateBlockPaths(pathDictionary);
        }




      Debug.Log("Loading Level Doors...");

     //load doors
        foreach(DoorData door in levelData.allLevelDoors)
        {
            GameObject newDoor = GameObject.Instantiate(doorPrefab, door.doorPos, door.doorRot);
            newDoor.GetComponent<Note>().note = door.note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(door.id, out guid))
            {
                newDoor.name = door.id;
                newDoor.transform.GetComponent<GeneralObjectInfo>().id = door.id;
            }
            else
            {
                newDoor.name = System.Guid.NewGuid().ToString();
            }
           
            //general object info
            newDoor.transform.GetComponent<GeneralObjectInfo>().isActive = door.isActive;
            newDoor.transform.GetComponent<GeneralObjectInfo>().isActive_original = door.isActive;
            newDoor.transform.GetComponent<GeneralObjectInfo>().position = door.doorPos;
            newDoor.transform.GetComponent<GeneralObjectInfo>().position_original = door.doorPos;
            newDoor.transform.GetComponent<GeneralObjectInfo>().rotation = door.doorRot;
            newDoor.transform.GetComponent<GeneralObjectInfo>().rotation_original = door.doorRot;
            newDoor.transform.GetComponent<GeneralObjectInfo>().idOfParent = door.idOfParent;
            newDoor.transform.GetComponent<GeneralObjectInfo>().childIndex = door.childIndex;
            newDoor.transform.GetComponent<GeneralObjectInfo>().children = door.children;


            //  LEGACY SUPPORT: prevents a problem where doors are not rotated properly.
            if(levelData.gameVersion == default(string))
            {
                //BUT ONLY IF ITS FROM A CERTAIN DATE! (use materialname to detect this...)
                if(levelData.allLevelBlocks.Count > 0)
                {
                if(string.IsNullOrEmpty(levelData.allLevelBlocks[0].materialName))
                    {
                      newDoor.transform.GetComponent<GeneralObjectInfo>().rotation_original *= Quaternion.AngleAxis(90f, Vector3.up);
                    }
                }
            }


            newDoor.GetComponent<DoorInfo>().pathFileToLoad = door.doorUrl;
            newDoor.tag = "Item";
            newDoor.SetActive(door.isActive);

            if(loadType == 1)
            {
                newDoor.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }


            recentlyLoadedGameObjectList.Add(newDoor);
            listOfLoadedObjectsInThisCoroutine.Add(newDoor);

        }

     //load dialogue
        foreach(DialogueData dialogue in levelData.allLevelDialogue)
        {
            GameObject newDialogue = GameObject.Instantiate(dialoguePrefab, dialogue.position, dialogue.rotation);
            newDialogue.GetComponent<Note>().note = dialogue.note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(dialogue.id, out guid))
            {
                newDialogue.name = dialogue.id;
                newDialogue.transform.GetComponent<GeneralObjectInfo>().id = dialogue.id;
            }
            else
            {
                newDialogue.name = System.Guid.NewGuid().ToString();
            }
           
            //general object info
            newDialogue.transform.GetComponent<GeneralObjectInfo>().position = dialogue.position;
            newDialogue.transform.GetComponent<GeneralObjectInfo>().position_original = dialogue.position;
            newDialogue.transform.GetComponent<GeneralObjectInfo>().rotation = dialogue.rotation;
            newDialogue.transform.GetComponent<GeneralObjectInfo>().rotation_original = dialogue.rotation;
            newDialogue.transform.GetComponent<GeneralObjectInfo>().idOfParent = dialogue.idOfParent;
            newDialogue.transform.GetComponent<GeneralObjectInfo>().childIndex = dialogue.childIndex;
            newDialogue.transform.GetComponent<GeneralObjectInfo>().children = dialogue.children;



            newDialogue.GetComponent<DialogueContentObject>().dialogue = dialogue.dialogue;
            newDialogue.GetComponent<DialogueContentObject>().voicePath = dialogue.voicePath;
            newDialogue.GetComponent<DialogueContentObject>().pitch = dialogue.pitch;
            newDialogue.GetComponent<DialogueContentObject>().clearPreviousDialogue = dialogue.clearPreviousDialogue;
            newDialogue.GetComponent<EventHolderList>().events = CreateEventListDeepClone(dialogue.events);

            newDialogue.tag = "Item";

            if(zipJsonPath != "")
            {
            yield return newDialogue.GetComponent<DialogueContentObject>().LoadAudio(System.IO.Path.GetDirectoryName(zipJsonPath) + "/" + dialogue.voicePath);
            }
            else
            {
            yield return newDialogue.GetComponent<DialogueContentObject>().LoadAudio(dialogue.voicePath);
            }
            

           if(loadType == 1)
            {
                newDialogue.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }

            recentlyLoadedGameObjectList.Add(newDialogue);
            listOfLoadedObjectsInThisCoroutine.Add(newDialogue);
        }

     //load audio sources
        foreach(AudioSourceData audioSource in levelData.allLevelAudioSources)
        {
            GameObject newAudioSource = GameObject.Instantiate(audioSourcePrefab, audioSource.position, audioSource.rotation);
            newAudioSource.GetComponent<Note>().note = audioSource.note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(audioSource.id, out guid))
            {
                newAudioSource.name = audioSource.id;
                newAudioSource.transform.GetComponent<GeneralObjectInfo>().id = audioSource.id;
            }
            else
            {
                newAudioSource.name = System.Guid.NewGuid().ToString();
            }
           
            //general object info
            newAudioSource.transform.GetComponent<GeneralObjectInfo>().position = audioSource.position;
            newAudioSource.transform.GetComponent<GeneralObjectInfo>().position_original = audioSource.position;
            newAudioSource.transform.GetComponent<GeneralObjectInfo>().rotation = audioSource.rotation;
            newAudioSource.transform.GetComponent<GeneralObjectInfo>().rotation_original = audioSource.rotation;
            newAudioSource.transform.GetComponent<GeneralObjectInfo>().idOfParent = audioSource.idOfParent;
            newAudioSource.transform.GetComponent<GeneralObjectInfo>().childIndex = audioSource.childIndex;
            newAudioSource.transform.GetComponent<GeneralObjectInfo>().children = audioSource.children;




            newAudioSource.GetComponent<AudioSourceObject>().audioPath = audioSource.audioPath;
            newAudioSource.GetComponent<AudioSourceObject>().pitch = audioSource.pitch;
            newAudioSource.GetComponent<AudioSourceObject>().spatialBlend = audioSource.spatialBlend;
            newAudioSource.GetComponent<AudioSourceObject>().minDistance = audioSource.minDistance;
            newAudioSource.GetComponent<AudioSourceObject>().maxDistance = audioSource.maxDistance;
            newAudioSource.GetComponent<AudioSourceObject>().repeat = audioSource.repeat;
            newAudioSource.GetComponent<AudioSourceObject>().SetValuesOnAudioSource();



            newAudioSource.GetComponent<EventHolderList>().events = CreateEventListDeepClone(audioSource.events);

            newAudioSource.tag = "Item";


            if(zipJsonPath != "")
            {
            yield return newAudioSource.GetComponent<AudioSourceObject>().LoadAudio(System.IO.Path.GetDirectoryName(zipJsonPath) + "/" + audioSource.audioPath);
            }
            else
            {
            yield return newAudioSource.GetComponent<AudioSourceObject>().LoadAudio(audioSource.audioPath);
            }



            if(loadType == 1)
            {
                newAudioSource.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }


            recentlyLoadedGameObjectList.Add(newAudioSource);
            listOfLoadedObjectsInThisCoroutine.Add(newAudioSource);
        }



     //load counters
        foreach(CounterData counter in levelData.allLevelCounters)
        {
            GameObject newCounter = CreateCounterEntity(counter, false);

            if(loadType == 1)
            {
                newCounter.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }
            recentlyLoadedGameObjectList.Add(newCounter);
            listOfLoadedObjectsInThisCoroutine.Add(newCounter);
        }

     //load Strings
        foreach(StringData stringEnt in levelData.allLevelStrings)
        {
            GameObject newString = CreateStringEntity(stringEnt, false);

            if(loadType == 1)
            {
                newString.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }
            recentlyLoadedGameObjectList.Add(newString);
            listOfLoadedObjectsInThisCoroutine.Add(newString);
        }

    //load dates
        foreach(DateData date in levelData.allLevelDates)
        {
            GameObject newDate = CreateDateEntity(date, false);

            if(loadType == 1)
            {
            recentlyLoadedGameObjectList.Add(newDate);
            listOfLoadedObjectsInThisCoroutine.Add(newDate);
            }
        }

     //load states
        foreach(StateData state in levelData.allLevelStates)
        {
            GameObject newState = CreateStateEntity(state);

            if(loadType == 1)
            {
                newState.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }
            recentlyLoadedGameObjectList.Add(newState);
            listOfLoadedObjectsInThisCoroutine.Add(newState);
        }


       //load level paths
       for(int i = 0; i < levelData.allLevelPaths.Count; i++)
        {
            GameObject newPath = GameObject.Instantiate(pathPrefab, levelData.allLevelPaths[i].position, levelData.allLevelPaths[i].rotation);
            newPath.GetComponent<Note>().note = levelData.allLevelPaths[i].note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(levelData.allLevelPaths[i].id, out guid))
            {
                newPath.name = levelData.allLevelPaths[i].id;
                newPath.transform.GetComponent<GeneralObjectInfo>().id = levelData.allLevelPaths[i].id;
            }
            else
            {
                newPath.name = System.Guid.NewGuid().ToString();
            }
      
            //general object info
            newPath.transform.GetComponent<GeneralObjectInfo>().position = levelData.allLevelPaths[i].position;
            newPath.transform.GetComponent<GeneralObjectInfo>().position_original = levelData.allLevelPaths[i].position;
            newPath.transform.GetComponent<GeneralObjectInfo>().rotation = levelData.allLevelPaths[i].rotation;
            newPath.transform.GetComponent<GeneralObjectInfo>().rotation_original = levelData.allLevelPaths[i].rotation;
            newPath.transform.GetComponent<GeneralObjectInfo>().children = levelData.allLevelPaths[i].children;
            newPath.transform.GetComponent<GeneralObjectInfo>().idOfParent = levelData.allLevelPaths[i].idOfParent;
            newPath.transform.GetComponent<GeneralObjectInfo>().childIndex = levelData.allLevelPaths[i].childIndex;



            //path parameters
            newPath.GetComponent<PathNode>().idOfObjectToMove = levelData.allLevelPaths[i].idOfObjectToMove;
            newPath.GetComponent<PathNode>().moveToPath = levelData.allLevelPaths[i].moveToPath;
            newPath.GetComponent<PathNode>().time = levelData.allLevelPaths[i].time;
            newPath.GetComponent<PathNode>().loopType = levelData.allLevelPaths[i].loopType;
            newPath.GetComponent<PathNode>().closeLoop = levelData.allLevelPaths[i].closeLoop;
            newPath.GetComponent<PathNode>().pathType = levelData.allLevelPaths[i].pathType;
            newPath.GetComponent<PathNode>().wayPointRotation = levelData.allLevelPaths[i].wayPointRotation;
            newPath.GetComponent<PathNode>().easeType = levelData.allLevelPaths[i].easeType;
            newPath.GetComponent<PathNode>().wayPointRotation = levelData.allLevelPaths[i].wayPointRotation;


            newPath.GetComponent<EventHolderList>().events = CreateEventListDeepClone(levelData.allLevelPaths[i].events);

            newPath.GetComponent<PathNode>().CreatePath();


            if(loadType == 1)
            {
                newPath.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }
            recentlyLoadedGameObjectList.Add(newPath);
            listOfLoadedObjectsInThisCoroutine.Add(newPath);

        }

       //load level child waypoints
       for(int i = 0; i < levelData.allLevelChildPaths.Count; i++)
        {
            GameObject newPathChild = GameObject.Instantiate(pathChildPrefab, Vector3.zero, Quaternion.identity);

            //Check if the id is a guid value (randomly generated format), if it is not, generate one
            System.Guid pathChildGuid;
            if(System.Guid.TryParse(levelData.allLevelChildPaths[i].id, out pathChildGuid))
            {
                newPathChild.name = levelData.allLevelChildPaths[i].id;
                newPathChild.transform.GetComponent<GeneralObjectInfo>().id = levelData.allLevelChildPaths[i].id;
            }
            else
            {
                newPathChild.name = System.Guid.NewGuid().ToString();
            }

            newPathChild.transform.GetComponent<GeneralObjectInfo>().idOfParent = levelData.allLevelChildPaths[i].idOfParent;
            newPathChild.transform.GetComponent<GeneralObjectInfo>().childIndex = levelData.allLevelChildPaths[i].childIndex;
            newPathChild.transform.GetComponent<GeneralObjectInfo>().UpdateParent();

            newPathChild.transform.localPosition = levelData.allLevelChildPaths[i].position;
            newPathChild.transform.localRotation = levelData.allLevelChildPaths[i].rotation;

            //general object info
            newPathChild.transform.GetComponent<GeneralObjectInfo>().position_original = levelData.allLevelChildPaths[i].position;
            newPathChild.transform.GetComponent<GeneralObjectInfo>().rotation_original = levelData.allLevelChildPaths[i].rotation;

            if(loadType == 1)
            {
                newPathChild.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }
            recentlyLoadedGameObjectList.Add(newPathChild);
            listOfLoadedObjectsInThisCoroutine.Add(newPathChild);

        }





         //load player movers 
        foreach(PlayerMoverData playerMover in levelData.allPlayerMovers)
        {
            GameObject newPlayerMover = GameObject.Instantiate(playerMoverPrefab, playerMover.position, playerMover.rotation);
            newPlayerMover.GetComponent<Note>().note = playerMover.note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(playerMover.id, out guid))
            {
                newPlayerMover.name = playerMover.id;
                newPlayerMover.transform.GetComponent<GeneralObjectInfo>().id = playerMover.id;
            }
            else
            {
                newPlayerMover.name = System.Guid.NewGuid().ToString();
            }

            //general object info
            newPlayerMover.transform.GetComponent<GeneralObjectInfo>().position = playerMover.position;
            newPlayerMover.transform.GetComponent<GeneralObjectInfo>().position_original = playerMover.position;
            newPlayerMover.transform.GetComponent<GeneralObjectInfo>().rotation = playerMover.rotation;
            newPlayerMover.transform.GetComponent<GeneralObjectInfo>().rotation_original = playerMover.rotation;
            newPlayerMover.transform.GetComponent<GeneralObjectInfo>().idOfParent = playerMover.idOfParent;
            newPlayerMover.transform.GetComponent<GeneralObjectInfo>().childIndex = playerMover.childIndex;
            newPlayerMover.transform.GetComponent<GeneralObjectInfo>().children = playerMover.children;


            newPlayerMover.GetComponent<EventHolderList>().events = CreateEventListDeepClone(playerMover.events);

            newPlayerMover.tag = "Item";


            if(loadType == 1)
            {
                newPlayerMover.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }

            recentlyLoadedGameObjectList.Add(newPlayerMover);
            listOfLoadedObjectsInThisCoroutine.Add(newPlayerMover);

        }



         //load global entity pointers 
        foreach(GlobalEntityPointerData globalEntityPointer in levelData.allGlobalEntityPointers)
        {
            GameObject newGlobalEntityPointer = GameObject.Instantiate(globalEntityPointerPrefab, globalEntityPointer.position, globalEntityPointer.rotation);
            newGlobalEntityPointer.GetComponent<Note>().note = globalEntityPointer.note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(globalEntityPointer.id, out guid))
            {
                newGlobalEntityPointer.name = globalEntityPointer.id;
                newGlobalEntityPointer.transform.GetComponent<GeneralObjectInfo>().id = globalEntityPointer.id;
            }
            else
            {
                newGlobalEntityPointer.name = System.Guid.NewGuid().ToString();
            }

            //general object info
            newGlobalEntityPointer.transform.GetComponent<GeneralObjectInfo>().position = globalEntityPointer.position;
            newGlobalEntityPointer.transform.GetComponent<GeneralObjectInfo>().position_original = globalEntityPointer.position;
            newGlobalEntityPointer.transform.GetComponent<GeneralObjectInfo>().rotation = globalEntityPointer.rotation;
            newGlobalEntityPointer.transform.GetComponent<GeneralObjectInfo>().rotation_original = globalEntityPointer.rotation;
            newGlobalEntityPointer.transform.GetComponent<GeneralObjectInfo>().idOfParent = globalEntityPointer.idOfParent;
            newGlobalEntityPointer.transform.GetComponent<GeneralObjectInfo>().childIndex = globalEntityPointer.childIndex;
            newGlobalEntityPointer.transform.GetComponent<GeneralObjectInfo>().children = globalEntityPointer.children;


            newGlobalEntityPointer.transform.GetComponent<GlobalParameterPointerEntity>().idOfGlobalEntityToPointTo = globalEntityPointer.idOfGlobalEntityToPointTo;
            newGlobalEntityPointer.transform.GetComponent<GlobalParameterPointerEntity>().AssignImageOfEntityToPointTo();
           
            newGlobalEntityPointer.GetComponent<EventHolderList>().events = CreateEventListDeepClone(globalEntityPointer.events);

            newGlobalEntityPointer.tag = "Item";


            if(loadType == 1)
            {
                newGlobalEntityPointer.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }

            recentlyLoadedGameObjectList.Add(newGlobalEntityPointer);
            listOfLoadedObjectsInThisCoroutine.Add(newGlobalEntityPointer);

        }


         //load lights 
        foreach(LightData light in levelData.allLevelLights)
        {
            GameObject newLight = GameObject.Instantiate(lightPrefab, light.position, light.rotation);
            newLight.GetComponent<Note>().note = light.note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(light.id, out guid))
            {
                newLight.name = light.id;
                newLight.transform.GetComponent<GeneralObjectInfo>().id = light.id;
            }
            else
            {
                newLight.name = System.Guid.NewGuid().ToString();
            }

            //general object info
            newLight.transform.GetComponent<GeneralObjectInfo>().position = light.position;
            newLight.transform.GetComponent<GeneralObjectInfo>().position_original = light.position;
            newLight.transform.GetComponent<GeneralObjectInfo>().rotation = light.rotation;
            newLight.transform.GetComponent<GeneralObjectInfo>().rotation_original = light.rotation;
            newLight.transform.GetComponent<GeneralObjectInfo>().idOfParent = light.idOfParent;
            newLight.transform.GetComponent<GeneralObjectInfo>().childIndex = light.childIndex;
            newLight.transform.GetComponent<GeneralObjectInfo>().children = light.children;
            newLight.transform.GetComponent<GeneralObjectInfo>().isActive = light.isActive;
            newLight.transform.GetComponent<GeneralObjectInfo>().isActive_original = light.isActive;


            newLight.transform.GetComponent<LightEntity>().lightType = light.lightType;
            newLight.transform.GetComponent<LightEntity>().range = light.range;
            newLight.transform.GetComponent<LightEntity>().strength = light.strength;
            newLight.transform.GetComponent<LightEntity>().color = light.color;
            newLight.transform.GetComponent<LightEntity>().spotAngle = light.spotAngle;
            newLight.transform.GetComponent<LightEntity>().shadowType = light.shadowType;
            newLight.transform.GetComponent<LightEntity>().UpdateParameters();

            newLight.GetComponent<EventHolderList>().events = CreateEventListDeepClone(light.events);

            newLight.tag = "Item";


            if(loadType == 1)
            {
                newLight.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }

            recentlyLoadedGameObjectList.Add(newLight);
            listOfLoadedObjectsInThisCoroutine.Add(newLight);

        }




         //load prefabs 
        foreach(PrefabData prefab in levelData.allLevelPrefabs)
        {
            GameObject newPrefab = GameObject.Instantiate(prefabPrefab, prefab.position, prefab.rotation);
            newPrefab.GetComponent<Note>().note = prefab.note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(prefab.id, out guid))
            {
                newPrefab.name = prefab.id;
                newPrefab.transform.GetComponent<GeneralObjectInfo>().id = prefab.id;
            }
            else
            {
                newPrefab.name = System.Guid.NewGuid().ToString();
            }

            //general object info
            newPrefab.transform.GetComponent<GeneralObjectInfo>().position = prefab.position;
            newPrefab.transform.GetComponent<GeneralObjectInfo>().position_original = prefab.position;
            newPrefab.transform.GetComponent<GeneralObjectInfo>().rotation = prefab.rotation;
            newPrefab.transform.GetComponent<GeneralObjectInfo>().rotation_original = prefab.rotation;
            newPrefab.transform.GetComponent<GeneralObjectInfo>().idOfParent = prefab.idOfParent;
            newPrefab.transform.GetComponent<GeneralObjectInfo>().childIndex = prefab.childIndex;
            newPrefab.transform.GetComponent<GeneralObjectInfo>().children = prefab.children;
            newPrefab.transform.GetComponent<GeneralObjectInfo>().isActive = prefab.isActive;
            newPrefab.transform.GetComponent<GeneralObjectInfo>().isActive_original = prefab.isActive;


            newPrefab.transform.GetComponent<PrefabEntity>().mapToLoad = prefab.mapToLoad;
            newPrefab.transform.GetComponent<PrefabEntity>().loadOnStart = prefab.loadOnStart;

            newPrefab.GetComponent<EventHolderList>().events = CreateEventListDeepClone(prefab.events);

            newPrefab.tag = "Item";


            if(loadType == 1)
            {
                newPrefab.GetComponent<GeneralObjectInfo>().wasLoadedAdditive = true;
            }

            recentlyLoadedGameObjectList.Add(newPrefab);
            listOfLoadedObjectsInThisCoroutine.Add(newPrefab);

        }







        //global entity events

        if(loadType == 1)
        {
                foreach(Event e in levelData.globalEvents)
                {
                    additiveGlobalEvents.Add(e);
                }
        }
        else if(loadType == 0)
        {  
            GlobalEventEntityComponent.Instance.GetComponent<EventHolderList>().events = new List<Event>(levelData.globalEvents);
        }



        if(loadType == 1)
        {
            Debug.Log("WASSS, ADDITIVE");
            yield return SetHierchies();
            yield return PostLoadData(levelData, 1);


            yield return PostLoad_SetPositioningRelativeToPrefab(listOfLoadedObjectsInThisCoroutine, objToParentTo);


            //play the global events for the additive loaded map
            if(PlayerMovementTypeKeySwitcher.Instance.isInFlyMode == false)
            {
                foreach(Event e in levelData.globalEvents)
                {
                    Debug.Log("EVENT:" + e);
                    EventActionManager.Instance.TryPlayEvent_Single(e);
                }
            }
            //OnLoad event for prefab that was passed
            if(objToParentTo)
            {
                EventActionManager.Instance.TryPlayEvent(objToParentTo.gameObject, "OnPrefabLoad");
            }


            
        }





        totalAmountOfCoroutinesBeingUsedForLoading--;


    }
    

    public GameObject CreateStateEntity(StateData state)
    {
            GameObject newState = GameObject.Instantiate(statePrefab, state.position, state.rotation);
            newState.GetComponent<Note>().note = state.note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(state.id, out guid))
            {
                newState.name = state.id;
                newState.transform.GetComponent<GeneralObjectInfo>().id = state.id;
            }
            else
            {
                newState.name = System.Guid.NewGuid().ToString();
            }

            //general object info
            newState.transform.GetComponent<GeneralObjectInfo>().position = state.position;
            newState.transform.GetComponent<GeneralObjectInfo>().position_original = state.position;
            newState.transform.GetComponent<GeneralObjectInfo>().rotation = state.rotation;
            newState.transform.GetComponent<GeneralObjectInfo>().rotation_original = state.rotation;
            newState.transform.GetComponent<GeneralObjectInfo>().idOfParent = state.idOfParent;
            newState.transform.GetComponent<GeneralObjectInfo>().childIndex = state.childIndex;
            newState.transform.GetComponent<GeneralObjectInfo>().children = state.children;


            newState.GetComponent<State>().states = state.states;
            newState.GetComponent<State>().timeUntilChoiceBoxCloses = state.timeUntilChoiceBoxCloses;

            newState.GetComponent<EventHolderList>().events = CreateEventListDeepClone(state.events);

            newState.tag = "Item";

            return newState;
    }
    

                                                        //the bool is for making sure global persistent entities get the "current_value" set to "current_value" instead of being reset 
    public GameObject CreateCounterEntity(CounterData counter, bool isGlobalEntity)
    {
            GameObject newCounter = GameObject.Instantiate(counterPrefab, counter.position, counter.rotation);
            newCounter.GetComponent<Note>().note = counter.note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(counter.id, out guid))
            {
                newCounter.name = counter.id;
                newCounter.transform.GetComponent<GeneralObjectInfo>().id = counter.id;
            }
            else
            {
                newCounter.name = System.Guid.NewGuid().ToString();
            }

            //general object info
            newCounter.transform.GetComponent<GeneralObjectInfo>().position = counter.position;
            newCounter.transform.GetComponent<GeneralObjectInfo>().position_original = counter.position;
            newCounter.transform.GetComponent<GeneralObjectInfo>().rotation = counter.rotation;
            newCounter.transform.GetComponent<GeneralObjectInfo>().rotation_original = counter.rotation;
            newCounter.transform.GetComponent<GeneralObjectInfo>().idOfParent = counter.idOfParent;
            newCounter.transform.GetComponent<GeneralObjectInfo>().childIndex = counter.childIndex;
            newCounter.transform.GetComponent<GeneralObjectInfo>().children = counter.children;



            newCounter.GetComponent<Counter>().currentCount_default = counter.currentCount_default;

            if(isGlobalEntity)
            {
                newCounter.GetComponent<Counter>().currentCount = counter.currentCount;
            }
            else
            {
                newCounter.GetComponent<Counter>().currentCount = counter.currentCount_default;
            }

            newCounter.GetComponent<EventHolderList>().events = CreateEventListDeepClone(counter.events);

            newCounter.GetComponent<WidgetInfo>().info = newCounter.GetComponent<Counter>().currentCount.ToString();

            newCounter.tag = "Item";

            return newCounter;
    }


                                                        //the bool is for making sure global persistent entities get the "current_value" set to "current_value" instead of being reset 
    public GameObject CreateStringEntity(StringData stringEnt, bool isGlobalEntity)
    {
            GameObject newString = GameObject.Instantiate(stringPrefab, stringEnt.position, stringEnt.rotation);
            newString.GetComponent<Note>().note = stringEnt.note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(stringEnt.id, out guid))
            {
                newString.name = stringEnt.id;
                newString.transform.GetComponent<GeneralObjectInfo>().id = stringEnt.id;
            }
            else
            {
                newString.name = System.Guid.NewGuid().ToString();
            }

            //general object info
            newString.transform.GetComponent<GeneralObjectInfo>().position = stringEnt.position;
            newString.transform.GetComponent<GeneralObjectInfo>().position_original = stringEnt.position;
            newString.transform.GetComponent<GeneralObjectInfo>().rotation = stringEnt.rotation;
            newString.transform.GetComponent<GeneralObjectInfo>().rotation_original = stringEnt.rotation;
            newString.transform.GetComponent<GeneralObjectInfo>().idOfParent = stringEnt.idOfParent;
            newString.transform.GetComponent<GeneralObjectInfo>().childIndex = stringEnt.childIndex;
            newString.transform.GetComponent<GeneralObjectInfo>().children = stringEnt.children;



            newString.GetComponent<StringEntity>().currentString_default = stringEnt.currentString_default;

            if(isGlobalEntity)
            {
                newString.GetComponent<StringEntity>().currentString = stringEnt.currentString;
            }
            else
            {
                newString.GetComponent<StringEntity>().currentString = stringEnt.currentString_default;
            }

            newString.GetComponent<EventHolderList>().events = CreateEventListDeepClone(stringEnt.events);

            newString.GetComponent<WidgetInfo>().info = newString.GetComponent<StringEntity>().currentString;

            newString.tag = "Item";

            return newString;
    }



                                                        //the bool is for making sure global persistent entities get the "current_value" set to "current_value" instead of being reset 
    public GameObject CreateDateEntity(DateData date, bool isGlobalEntity)
    {
            GameObject newDate = GameObject.Instantiate(DatePrefab, date.position, date.rotation);
            newDate.GetComponent<Note>().note = date.note;

            //Check if the id is a guid value (randomly generated format), if it is not, generate one for the block
            System.Guid guid;
            if(System.Guid.TryParse(date.id, out guid))
            {
                newDate.name = date.id;
                newDate.transform.GetComponent<GeneralObjectInfo>().id = date.id;
            }
            else
            {
                newDate.name = System.Guid.NewGuid().ToString();
            }

            //general object info
            newDate.transform.GetComponent<GeneralObjectInfo>().position = date.position;
            newDate.transform.GetComponent<GeneralObjectInfo>().position_original = date.position;
            newDate.transform.GetComponent<GeneralObjectInfo>().rotation = date.rotation;
            newDate.transform.GetComponent<GeneralObjectInfo>().rotation_original = date.rotation;
            newDate.transform.GetComponent<GeneralObjectInfo>().idOfParent = date.idOfParent;
            newDate.transform.GetComponent<GeneralObjectInfo>().childIndex = date.childIndex;
            newDate.transform.GetComponent<GeneralObjectInfo>().children = date.children;

            DateTime parsedValue;
            bool successfulParse = DateTime.TryParse(date.date, out parsedValue);
            
            if(successfulParse)
            {
                if(isGlobalEntity)
                {
                    newDate.GetComponent<Date>().date = parsedValue;
                }
                else
                {
                    newDate.GetComponent<Date>().date = default(DateTime);
                }
            }
            else
            {
                    newDate.GetComponent<Date>().date = default(DateTime);
            }




            newDate.GetComponent<EventHolderList>().events = CreateEventListDeepClone(date.events);

            newDate.GetComponent<WidgetInfo>().info = newDate.GetComponent<Date>().date.ToString();

            newDate.tag = "Item";

            return newDate;
    }



    public IEnumerator SetHierchies(GameObject objToParentTo = null, bool positionOnOrigin = false)
    {
            //position the objects to an origin(if any)
        foreach(GameObject obj in recentlyLoadedGameObjectList)
        {
            if(objToParentTo)
            { 
                obj.transform.SetParent(objToParentTo.transform);

                if(obj.GetComponent<GeneralObjectInfo>())
                {

                    if(positionOnOrigin)
                    {
                        obj.transform.localPosition = obj.GetComponent<GeneralObjectInfo>().position_original;
                        obj.transform.localRotation = obj.GetComponent<GeneralObjectInfo>().rotation_original;
                    }
                    else
                    {
                        obj.transform.position = obj.GetComponent<GeneralObjectInfo>().position_original;
                        obj.transform.localRotation = obj.GetComponent<GeneralObjectInfo>().rotation_original;
                    }
                }

                //add to door's loaded entity list
                objToParentTo.GetComponent<Door>().loadedEntitiesList.Add(obj);
            }

        }


        //loop through all objects in scene to set correct hierchies
        foreach(GeneralObjectInfo obj in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
        {
            if(obj != null)
            {
                    obj.SetParentAccordingToParentID();
                    obj.UpdateChildrenObjects();
                    obj.UpdateParentObject();
            }
        }



        if(objToParentTo)
        {
            //parent to the door as the last step or else the parenting to-the-door wont work
            foreach(GameObject obj in objToParentTo.GetComponent<Door>().loadedEntitiesList)
            {
                obj.transform.SetParent(objToParentTo.transform);
            }
        }




        yield return null;
    }


    IEnumerator PostLoadData(SaveAndLoadLevel.LevelData levelData, int loadType = 0)
    {
        //FOR LEGACY MAPS
        //Add an event to all posters that either plays or views poster
        if(levelData.gameVersion == default(string))
        {
            //spawn directional light entity (global light paramaters)
            GameObject newLight = GameObject.Instantiate(lightPrefab, Vector3.zero, Quaternion.Euler(new Vector3(50f, 56.428f, 2.23f)));

            newLight.GetComponent<Note>().note = "Directional light automatically added to imitate old map";
            newLight.name = System.Guid.NewGuid().ToString();

            newLight.transform.GetComponent<GeneralObjectInfo>().rotation = newLight.transform.rotation;
            newLight.transform.GetComponent<GeneralObjectInfo>().rotation_original = newLight.transform.rotation;

            newLight.transform.GetComponent<LightEntity>().lightType = "directional";
            newLight.transform.GetComponent<LightEntity>().strength = levelData.lightIntensity - 1; //remove one to simulate old maps
            newLight.transform.GetComponent<LightEntity>().color = levelData.lightColor;
            newLight.transform.GetComponent<LightEntity>().shadowType = "hard";
            newLight.transform.GetComponent<LightEntity>().UpdateParameters();
            newLight.tag = "Item";

            recentlyLoadedGameObjectList.Add(newLight);





            //set the ambience color (global shadow parameters)
            Shader.SetGlobalFloat("_HighIntensity", levelData.lightIntensity);
            Shader.SetGlobalColor("_HighColor", levelData.lightColor);
            Shader.SetGlobalFloat("_LowIntensity", levelData.shadowIntensity);
            Shader.SetGlobalColor("_LowColor", levelData.shadowColor);


            //Add an on click play video to all posters?
            foreach(GameObject poster in allLoadedPosters)
            {
                if(poster != null)
                {
                    if(poster.GetComponent<PosterMeshCreator>().isVideo)
                    {
                    SaveAndLoadLevel.Event e = new SaveAndLoadLevel.Event();
                    e.onAction = "OnClick";
                    e.doAction = "PlayPosterVideo_EnableControlls";
                    e.id = poster.name;
                    poster.GetComponent<EventHolderList>().events.Add(e);
                    }
                }
            }
        }










        //loop through all objects in scene to set correct hierchies
        foreach(GeneralObjectInfo obj in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
        {
            if(obj != null)
            {
                    obj.SetParentAccordingToParentID();
                    obj.UpdateChildrenObjects();
                    obj.UpdateParentObject();
            }
        }



        //loop through all objects in scene to set correct child indexes
        foreach(GeneralObjectInfo obj in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
        {
            if(obj != null)
            {
                obj.SetChildIndex();
            }
        }

        
        //loop through all objects in scene to set correct isActive
        foreach(GeneralObjectInfo obj in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
        {
            if(obj != null)
            {
                obj.ResetVisibility();
            }
        }



        //if it is an old map... then load with an offset + a room
        if(levelData.gameVersion == default(string))
        {
            //special ambient settings just to imitate the old style
            RenderSettings.ambientLight = levelData.shadowColor * levelData.shadowIntensity;
            GlobalMapSettingsManager.Instance.lightingIntensity_shadow = Mathf.Log(levelData.shadowIntensity, 2);
            GlobalMapSettingsManager.Instance.lightingColor_shadow = levelData.shadowColor;
            Shader.SetGlobalColor("_AmbientLightColor", RenderSettings.ambientLight);
            Shader.SetGlobalFloat("_AmbientLightIntensity", GlobalMapSettingsManager.Instance.lightingIntensity_shadow);

            yield return Apply2022LegacyMapAdjustments();
        }



        //Update "original position"s 
        foreach(GeneralObjectInfo obj in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
        {
            if(obj != null)
            {
                    obj.ResetPosition();
            }
        }


        //store the base materials for all objects
        foreach(GeneralObjectInfo obj in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
        {
            if(obj != null)
            {
                    obj.UpdateBaseMaterialList();
            }

            foreach (Transform child in obj.transform)
            {
                if(child.GetComponent<GeneralObjectInfo>())
                child.GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();
            }

        }


        //after all posters have been loaded, update the stencil layer data
        PosterDepthLayerStencilRefManager.Instance.AssignCorrectStencilRefsToAllPostersInScene();
    
        //loop through all posters and their depth layers to assign the appropriate poster references based on poster id
        foreach(GameObject poster in allLoadedPosters)
        {
            if(poster != null)
            {
                foreach(GameObject depthLayer in poster.GetComponent<PosterDepthLayerList>().posterDepthLayerList)// a for loop is required due to complexity of algorithm
                {
                    yield return depthLayer.GetComponent<Poster_DepthStencilFrame>().SetMaterialFromReferencedPosterID();
                    depthLayer.GetComponent<Poster_DepthStencilFrame>().UpdateGeneralValues();
                }
            }
        }





        //loop through all posters and set the correct shared material based on id
        foreach(GameObject poster in allLoadedPosters)
        {
            if(poster != null)
            {
                poster.GetComponent<PosterMeshCreator>().AssignSharedMaterialBasedOnGUID();
            }
        }
        //loop through all posters and set the correct shared material based on id
        foreach(GameObject block in allLoadedBlocks)
        {
            if(block != null)
            {
                block.GetComponent<BlockFaceTextureUVProperties>().AssignSharedMaterialBasedOnGUID();
            }
        }





        //VERY IMPORTANT! (fixed on 6/26/2024). Make sure these block functions get called AFTER the hierchies and resting positions are set... or else there will be vertex problems!
        foreach(GameObject block in allLoadedBlocks)
        {
            if(block != null)
            {
            block.transform.GetComponent<BlockFaceTextureUVProperties>().UpdateCorners();
            block.GetComponent<BlockFaceTextureUVProperties>().SetPivotToBlockCenter();
            block.transform.GetComponent<BlockFaceTextureUVProperties>().UpdateBlockUV();
            }
        }
        //call this right after or else the vertices/positions will get messed up on reset!
        foreach(GameObject block in allLoadedBlocks)
        {
            if(block != null)
            {
                block.transform.GetComponent<GeneralObjectInfo>().UpdatePosition();
            }
        }



        //load music
        loadingText.text = "Loading Music: " + levelData.musicTrackPath;

        yield return musicHandler.LoadMusic(levelData.musicTrackPath);


        //load skybox media
        loadingText.text = "Loading Skybox: " + levelData.globalSkyboxMediaPath;
        
        yield return globalSkyboxMedia.LoadImage(levelData.globalSkyboxMediaPath);

        //skybox properties
        globalSkyboxMedia.skyboxTextureScaleX = levelData.globalSkyboxMedia_UVScaleX;
        globalSkyboxMedia.skyboxTextureScaleY = levelData.globalSkyboxMedia_UVScaleY;
        globalSkyboxMedia.skyboxTextureOffsetX = levelData.globalSkyboxMedia_UVOffsetX;
        globalSkyboxMedia.skyboxTextureOffsetY = levelData.globalSkyboxMedia_UVOffsetY;
        globalSkyboxMedia.scrollSpeedX = levelData.globalSkyboxMedia_UVOffsetScrollX;
        globalSkyboxMedia.scrollSpeedY = levelData.globalSkyboxMedia_UVOffsetScrollY;
        
    
        if(!RenderSettings.skybox)
        RenderSettings.skybox = GlobalMapSettingsManager.Instance.linearColorskybox_Mat;



    GlobalParameterManager.Instance.LoadGlobalEntities();


    if(EditModeStaticParameter.isInEditMode == false)
    {
        toolTipsCanvas.SetActive(false);
    }


    Debug.Log("Finished Building Level...");
    loadingScreenCanvas.SetActive(false);

    GlobalUtilityFunctions.DuplicateGUIDChecker_Fixer();
    
    //do a check here or else there will be problems with switching movement type on level start
    if(loadType == 0)
    {
        EnablePlayer();
    }

    //level start event (ONLY WHEN NOT IN EDIT MODE)
    if(EditModeStaticParameter.isInEditMode == false && ResetLevelParametersManager.Instance.levelRestartEventPopped == false)
    {
     }


    yield return new WaitForSeconds(0.1f); //bootleg fix to make sure player views a poster before they are able to see the level(OnLevelStart -> ViewPoster)

    blackFadeImage.DOColor(Color.clear, 0.55f);



    musicHandler.PlayMusic();

    isLevelLoaded = true;
    }




    public void EnablePlayer()
    {
        //start player in fly mode when in edit mode
        if(EditModeStaticParameter.isInEditMode)
        {
            PlayerMovementTypeKeySwitcher.Instance.EnableFlyMode_Raw();
            PlayerMovementTypeKeySwitcher.Instance.noClipIcon_Canvas.GetComponent<Canvas>().enabled = true;
        }
        else
        {
            PlayerMovementTypeKeySwitcher.Instance.EnablePlayerController_Raw();
        }

        playerObject.SetActive(true);

        playerObject.GetComponentInChildren<SimpleSmoothMouseLook>().enabled = true;
        playerObject.GetComponentInChildren<PlayerObjectInteractionStateMachine>().enabled = true;

        
        if(EscapeToggleToolBar.toolBarisOpened)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }



    }

    IEnumerator GetLevelURLData(string dataPathURL) 
    {

        if(ConfigMenuUIEvents.Instance.allowURLmedia == true)
        {

            UnityWebRequest www = UnityWebRequest.Get(dataPathURL);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
            UINotificationHandler.Instance.SpawnNotification("<color=red>LvlLd Error! " + www.error, UINotificationHandler.NotificationStateType.error);
            }
            else {
                // Show results as text
                Debug.Log(www.downloadHandler.text);
    
                // Or retrieve results as binary data
                byte[] results = www.downloadHandler.data;

            string json = www.downloadHandler.text;
            fileLevelData = JsonUtility.FromJson<SaveAndLoadLevel.LevelData>(json);
            yield return LoadLevel();
            }
        }
        else
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>Url Media disabled!");
        }
    }




    public int GetAllObjectCount(SaveAndLoadLevel.LevelData levelData)
    {
        int total = 0;

        total += levelData.allLevelBlocks.Count;
        total += levelData.allLevelPosters.Count;
        total += levelData.allLevelDoors.Count;
        total += levelData.allLevelAudioSources.Count;
        total += levelData.allLevelCounters.Count;
        total += levelData.allLevelDialogue.Count;
        total += levelData.allLevelPaths.Count;
        total += levelData.allLevelStates.Count;

        return total;        
    }




    public IEnumerator LoadMapAdditively(string jsonPath, GameObject doorToAddTo = null)
    {  
        Debug.Log(" LoadMapAdditively called ");
        LevelData additiveData;
        
            try
            {
                if(!GlobalUtilityFunctions.IsPathSafe(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp))
                {
                    throw new UnauthorizedAccessException($"Invalid path: {GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp}");
                }

                string json = File.ReadAllText(jsonPath);
                Debug.Log(json);
                additiveData = JsonUtility.FromJson<LevelData>(json);
            }
            catch(Exception ex)
            {
                UINotificationHandler.Instance.SpawnNotification("<color=red>AddMp: " + ex.Message);
                yield break; // Exit the coroutine early if an exception occurs
            }

        //note... there is no indicator here that says it was loaded "additvely"...

        if(additiveData != null)
        {
            yield return LoadEntities(additiveData);
        }

        if(doorToAddTo != null)
        {
                //add the newly loaded objects to the door
                //SUPER IMPORTANT NOTE... because "recentlyLoadedGameObjectList" is a global variable in this class. This functionality will break if multiple maps are loaded at the same time!!!
                //either... 1. prevent the user from loading multiple maps at once. 2. make the LoadEntities function its own class. (look at that online example about coroutines returning values)
                foreach(GameObject obj in recentlyLoadedGameObjectList)
                {
                    obj.transform.SetParent(doorToAddTo.transform);
                }
        }
    }





    IEnumerator PostLoad_SetPositioningRelativeToPrefab(List<GameObject> listOfLoadedObjectsInThisCoroutine, Transform objToParentTo)
    {
        //only apply the parent to the prefab entity if the parent ID is false...
        foreach(GameObject obj in listOfLoadedObjectsInThisCoroutine)
        {
            //only set the parent to the prefab entity if the object has nothing to parent to...
            if(obj.GetComponent<GeneralObjectInfo>().parentObject == null)
            {
                if(objToParentTo != null)
                {
                    obj.GetComponent<GeneralObjectInfo>().idOfParent = objToParentTo.gameObject.name;
                    obj.transform.SetParent(objToParentTo);
                    obj.GetComponent<GeneralObjectInfo>().ResetPosition();
                }
            }

            //turn on the collision back on
            obj.GetComponent<Collider>().enabled = true;

            //update parent or else the object wont have its "parent object" set
            obj.GetComponent<GeneralObjectInfo>().UpdateParent();

        }

        yield return null;

    }









    IEnumerator  Apply2022LegacyMapAdjustments()
    {
        foreach(GeneralObjectInfo obj in GlobalUtilityFunctions.GetAllGeneralObjectInfoClassesInMap())
        {
            if(obj != null)
            {
                obj.position_original += new Vector3(-23f, -0.5f, 28f);
            }
        }
        



        //load-in fake legacy room
        yield return LoadMapAdditively(Application.dataPath + "/StreamingAssets/CampaignLevelMedia/2022_LEGACY_template.json");
    }




    //"custom materials" were removed in favor or posters as materials...
    //What this does: step 1. create a poster for each legacy custom material found. 
    IEnumerator  ApplyLegacyCustomMaterialMapAdjustments_CreatePosterFromMaterialData(SaveAndLoadLevel.MaterialData legacyCustomMat, Dictionary<string, string> pathDictionary)
    {
            //spawn directional light entity (global light paramaters)
            GameObject poster = GameObject.Instantiate(posterPrefab, Vector3.zero, Quaternion.identity);

            poster.GetComponent<Note>().note = "Poster Added because map contained legacy custom materials";
            poster.name = System.Guid.NewGuid().ToString();

            poster.transform.GetComponent<GeneralObjectInfo>().rotation = poster.transform.rotation;
            poster.transform.GetComponent<GeneralObjectInfo>().rotation_original = poster.transform.rotation;
            poster.transform.GetComponent<GeneralObjectInfo>().UpdateID();



            yield return poster.GetComponent<PosterMeshCreator>().LoadImage(legacyCustomMat.materialPath);
            yield return poster.GetComponent<PosterFootstepSound>().LoadFootStepSound(legacyCustomMat.materialPath);


            //because the older version had this...
            if(poster.GetComponent<PosterMeshCreator>().texture_original)
            {
                Debug.Log("HAD TEXT _OG");
            if(poster.GetComponent<PosterMeshCreator>().height <= 64 && poster.GetComponent<PosterMeshCreator>().width <= 64)
            {
                poster.GetComponent<PosterMeshCreator>().textureFiltering = false;
                poster.GetComponent<PosterMeshCreator>().UpdateTextureFiltering();
            }
            }

            
            if(legacyCustomMat.shaderName == "Standard")
            {
                legacyCustomMat.shaderName = "StandardCelShaded";
            }

            poster.GetComponent<PosterMeshCreator>().shaderName = legacyCustomMat.shaderName;
            poster.GetComponent<PosterMeshCreator>().ChangeShaderOfPoster();




            allLoadedPosters.Add(poster);

            if(!pathDictionary.ContainsKey(legacyCustomMat.materialPath))
            {
                pathDictionary.Add(legacyCustomMat.materialPath, poster.name);
            }
    }

    //step 2. assign the id of that poster to the corelated block that holds a material path
    IEnumerator ApplyLegacyCustomMaterialMapAdjustments_UpdateBlockPaths(Dictionary<string, string> pathDictionary)
    {
        foreach(GameObject block in allLoadedBlocks)
        {
                if(block != null)
                {
                if(pathDictionary.ContainsKey(block.GetComponent<BlockFaceTextureUVProperties>().materialName_y))
                {
                    block.GetComponent<BlockFaceTextureUVProperties>().materialName_y = pathDictionary[block.GetComponent<BlockFaceTextureUVProperties>().materialName_y];
                }
                if(pathDictionary.ContainsKey(block.GetComponent<BlockFaceTextureUVProperties>().materialName_yneg))
                {
                    block.GetComponent<BlockFaceTextureUVProperties>().materialName_yneg = pathDictionary[block.GetComponent<BlockFaceTextureUVProperties>().materialName_yneg];
                }
                if(pathDictionary.ContainsKey(block.GetComponent<BlockFaceTextureUVProperties>().materialName_x))
                {
                    block.GetComponent<BlockFaceTextureUVProperties>().materialName_x = pathDictionary[block.GetComponent<BlockFaceTextureUVProperties>().materialName_x];
                }
                if(pathDictionary.ContainsKey(block.GetComponent<BlockFaceTextureUVProperties>().materialName_xneg))
                {
                    block.GetComponent<BlockFaceTextureUVProperties>().materialName_xneg = pathDictionary[block.GetComponent<BlockFaceTextureUVProperties>().materialName_xneg];
                }
                if(pathDictionary.ContainsKey(block.GetComponent<BlockFaceTextureUVProperties>().materialName_z))
                {
                    block.GetComponent<BlockFaceTextureUVProperties>().materialName_z = pathDictionary[block.GetComponent<BlockFaceTextureUVProperties>().materialName_z];
                }
                if(pathDictionary.ContainsKey(block.GetComponent<BlockFaceTextureUVProperties>().materialName_zneg))
                {
                    block.GetComponent<BlockFaceTextureUVProperties>().materialName_zneg = pathDictionary[block.GetComponent<BlockFaceTextureUVProperties>().materialName_zneg];
                }

                block.GetComponent<BlockFaceTextureUVProperties>().AssignSharedMaterialBasedOnGUID();
            }
        }


        yield return new WaitForSeconds(0);
    }





    public LevelData GetLevelDataFromMapPath(string path)
    {
        LevelData data;

        string json = File.ReadAllText(path);
        data = JsonUtility.FromJson<LevelData>(json);
        return data;
    }



}
