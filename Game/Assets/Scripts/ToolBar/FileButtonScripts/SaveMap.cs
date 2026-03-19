using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class SaveMap : MonoBehaviour {
  
    private static SaveMap _instance;
    public static SaveMap Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    

    // Listen OnClick event in standlone builds



   public SaveAndLoadLevel.LevelData levelDataToSave;

    public void OnSave() {
    
        //used only to prevent the button events from registering on play mode
        if(EditModeStaticParameter.isInEditMode == false)
        return;

            //is zip
            if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("TEMPORARY_"))
            {
                    ZipFileHandler_GlobalStaticInfo.pathToSaveZipAs = ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith;
                    StartCoroutine(MapPackerHandler.Instance.PackAllMediaInMapIntoFolder());
                    return;
            }

                if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp != null)
                {
            if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("StreamingAssets") || GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("steamapps") )      //makes sure not to overrwrite the temple maps
            {
                if(!Directory.Exists(Application.persistentDataPath + "/MyMaps/"))
                {    
                    Directory.CreateDirectory(Application.persistentDataPath + "/MyMaps/");
                }
                    var levelFilePath = StandaloneFileBrowser.SaveFilePanel("Save Map as... (You can save maps in any folder)", GlobalUtilityFunctions.OpenCertainPathBasedOnIfCurrentMapIsPartOfProject(), Path.GetFileNameWithoutExtension(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp), "json");
                    if(!string.IsNullOrEmpty(levelFilePath))
                    {
                        dataSave(levelFilePath);
                    }
            }
            else if(File.Exists(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp))
            {
              dataSave(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp);
            } 
            else
            {
                if(!Directory.Exists(Application.persistentDataPath + "/MyMaps/"))
                {    
                    Directory.CreateDirectory(Application.persistentDataPath + "/MyMaps/");
                }
                var levelFilePath = StandaloneFileBrowser.SaveFilePanel("Save Map as... (You can save maps in any folder)", GlobalUtilityFunctions.OpenCertainPathBasedOnIfCurrentMapIsPartOfProject(), Path.GetFileNameWithoutExtension(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp), "json");
                if(!string.IsNullOrEmpty(levelFilePath))
                {
                    dataSave(levelFilePath);
                }
            }
             }
    }


    public void OnSaveAs()
    {
        //used only to prevent the button events from registering on play mode
        if(EditModeStaticParameter.isInEditMode == false)
        return;

        //used only to prevent the button events from registering on play mode
        if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("TEMPORARY_"))
        return;


                if(!Directory.Exists(Application.persistentDataPath + "/MyMaps/"))
                {    
                    Directory.CreateDirectory(Application.persistentDataPath + "/MyMaps/");
                }




        var levelFilePath = StandaloneFileBrowser.SaveFilePanel("Save Map as...", GlobalUtilityFunctions.OpenCertainPathBasedOnIfCurrentMapIsPartOfProject(), Path.GetFileNameWithoutExtension(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp), "json");
              
        if(!string.IsNullOrEmpty(levelFilePath))
        {
            dataSave(levelFilePath);
        }
    }


public void dataSave(string path, bool convertMediaPathToJustFileName = false, bool autoBackup = false)
{
    //  ResetLevelParametersManager.Instance.ResetAllLevelObjectsThatHaveChanged();



      levelDataToSave = new SaveAndLoadLevel.LevelData();

      SetGlobalData(levelDataToSave);
      SetEntityData(levelDataToSave);

      
        //gather global entity data
        levelDataToSave.globalEvents = GlobalEventEntityComponent.Instance.GetComponent<EventHolderList>().events;


    if(convertMediaPathToJustFileName)
    {
        ConvertAllMediaPathsToJustFileName(levelDataToSave);
    }




    //write to file...
    try
    {
        if(!GlobalUtilityFunctions.IsPathSafe(path))
        {
            throw new UnauthorizedAccessException($"Invalid path: {path}");
        }

        string json = JsonUtility.ToJson(levelDataToSave, true);
        File.WriteAllText(path, json);

            

            // Debug.Log("ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith  " + ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith);
            // Debug.Log("GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp  " + GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp);
            // Debug.Log("ZipFileHandler_GlobalStaticInfo.pathToSaveZipAs  " + ZipFileHandler_GlobalStaticInfo.pathToSaveZipAs);
            // Debug.Log("ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory  " + ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory);

            //dont set it as the main file being worked on if it is an autobackup save
            if(autoBackup == false)
            {
                    GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = path;
            }


                //if it is a zip file save...
                if(convertMediaPathToJustFileName)
                {
                //        UINotificationHandler.Instance.SpawnNotification("map saved with shortened paths", UINotificationHandler.NotificationStateType.ping);

                }
                else
                {
                    //dont show notification if it is an autobackup save
                    if(autoBackup == false)
                    {
                        UINotificationHandler.Instance.SpawnNotification("<color=green>map saved!", UINotificationHandler.NotificationStateType.saved);
                    }
                }

    }
    catch (Exception ex)
    {
        UINotificationHandler.Instance.SpawnNotification("<color=red>" + ex.Message);
    }
}




public void SetGlobalData(SaveAndLoadLevel.LevelData levelData)
{
      levelData.gameVersion = Application.version;


      //gather global map data
      levelData.shadowIntensity = GlobalMapSettingsManager.Instance.lightingIntensity_shadow; 
      levelData.shadowColor = GlobalMapSettingsManager.Instance.lightingColor_shadow;
      levelData.lightIntensity = Shader.GetGlobalFloat("_HighIntensity");
      levelData.lightColor = Shader.GetGlobalColor("_HighColor");

            


      levelData.skyboxTopColor = GlobalMapSettingsManager.Instance.skybox_Color_Top_original;
      levelData.skyboxMidColor = GlobalMapSettingsManager.Instance.skybox_Color_Middle_original;
      levelData.skyboxBotColor = GlobalMapSettingsManager.Instance.skybox_Color_Bottom_original;
      levelData.skyBoxExp =      GlobalMapSettingsManager.Instance.skybox_Color_Exp_original;


      levelData.musicTrackPath = GameObject.Find("MusicHandler").GetComponent<MusicHandler>().musicTrackPath;
    
      levelData.globalSkyboxMediaPath = GameObject.Find("LevelGlobalMediaManager").GetComponent<LevelGlobalMediaManager>().urlFilePath;
      levelData.globalSkyboxMedia_UVScaleX = GameObject.Find("LevelGlobalMediaManager").GetComponent<LevelGlobalMediaManager>().skyboxTextureScaleX;
      levelData.globalSkyboxMedia_UVScaleY = GameObject.Find("LevelGlobalMediaManager").GetComponent<LevelGlobalMediaManager>().skyboxTextureScaleY;
      levelData.globalSkyboxMedia_UVOffsetX = GameObject.Find("LevelGlobalMediaManager").GetComponent<LevelGlobalMediaManager>().skyboxTextureOffsetX;
      levelData.globalSkyboxMedia_UVOffsetY = GameObject.Find("LevelGlobalMediaManager").GetComponent<LevelGlobalMediaManager>().skyboxTextureOffsetY;
      levelData.globalSkyboxMedia_UVOffsetScrollX = GameObject.Find("LevelGlobalMediaManager").GetComponent<LevelGlobalMediaManager>().scrollSpeedX;
      levelData.globalSkyboxMedia_UVOffsetScrollY = GameObject.Find("LevelGlobalMediaManager").GetComponent<LevelGlobalMediaManager>().scrollSpeedY;

      levelData.fog_isOn = GlobalMapSettingsManager.Instance.fog_isOn;
      levelData.fog_Color = GlobalMapSettingsManager.Instance.fog_Color;
      levelData.fog_Start = GlobalMapSettingsManager.Instance.fog_Start;
      levelData.fog_End = GlobalMapSettingsManager.Instance.fog_End;


    levelData.bloom_isOn = PostProcessingManager.Instance.bloom_isOn;
    levelData.bloom_intensity = PostProcessingManager.Instance.bloom_intensity; 
    levelData.bloom_threshold = PostProcessingManager.Instance.bloom_threshold;
    levelData.bloom_color = PostProcessingManager.Instance.bloom_color;
    levelData.bloom_softKnee = PostProcessingManager.Instance.bloom_softKnee;
    levelData.bloom_diffusion = PostProcessingManager.Instance.bloom_diffusion;

    levelData.dof_isOn = PostProcessingManager.Instance.dof_isOn;
    levelData.dof_maxFocusDistance = PostProcessingManager.Instance.dof_maxFocusDistance;

    levelData.ao_isOn = PostProcessingManager.Instance.ao_isOn;




      levelData.vertexSnapping = GlobalMapSettingsManager.Instance.vertexSnapping;
}

public void SetEntityData(SaveAndLoadLevel.LevelData levelData)
{
      //gather block data
        Block[] allCurrentBlocksInScene = FindObjectsOfType(typeof(Block), true) as Block[];
        for(int i = 0; i < allCurrentBlocksInScene.Length; i++)
        {
            AddObjectToList(levelData, allCurrentBlocksInScene[i].gameObject);
        }


        //gather poster data
        PosterMeshCreator[] allCurrentPostersInScene = FindObjectsOfType(typeof(PosterMeshCreator), true) as PosterMeshCreator[];
        for(int i = 0; i < allCurrentPostersInScene.Length; i++)
        {
            AddObjectToList(levelData, allCurrentPostersInScene[i].gameObject);
        }

        //gather door data
        DoorInfo[] allCurrentDoorsInScene = FindObjectsOfType(typeof(DoorInfo), true) as DoorInfo[];
        for(int i = 0; i < allCurrentDoorsInScene.Length; i++)
        {
            AddObjectToList(levelData, allCurrentDoorsInScene[i].gameObject);
        }

        //gather dialogue data
        DialogueContentObject[] allCurrentDialogueInScene = FindObjectsOfType(typeof(DialogueContentObject), true) as DialogueContentObject[];
        for(int i = 0; i < allCurrentDialogueInScene.Length; i++)
        {
            AddObjectToList(levelData, allCurrentDialogueInScene[i].gameObject);
        }

        //gather audioSource data
        AudioSourceObject[] allCurrentAudioSourcesInScene = FindObjectsOfType(typeof(AudioSourceObject), true) as AudioSourceObject[];
        for(int i = 0; i < allCurrentAudioSourcesInScene.Length; i++)
        {
            AddObjectToList(levelData, allCurrentAudioSourcesInScene[i].gameObject);
        }


        //gather counter data
        Counter[] allCurrentCountersInScene = FindObjectsOfType(typeof(Counter), true) as Counter[];
        for(int i = 0; i < allCurrentCountersInScene.Length; i++)
        {
            //SHITTY TEMPORARY FIX REMOVE LATER. Makes sure the game does not save global entities
            if(GlobalParameterManager.Instance.allLoadedGlobalEntities.Contains(allCurrentCountersInScene[i].gameObject) == false)
            {
                AddObjectToList(levelData, allCurrentCountersInScene[i].gameObject);
            }
        }

        //gather string data
        StringEntity[] allCurrentStringsInScene = FindObjectsOfType(typeof(StringEntity), true) as StringEntity[];
        for(int i = 0; i < allCurrentStringsInScene.Length; i++)
        {
            //SHITTY TEMPORARY FIX REMOVE LATER. Makes sure the game does not save global entities
            if(GlobalParameterManager.Instance.allLoadedGlobalEntities.Contains(allCurrentStringsInScene[i].gameObject) == false)
            {
                AddObjectToList(levelData, allCurrentStringsInScene[i].gameObject);
            }
        }

        //gather counter data
        Date[] allCurrentDatesInScene = FindObjectsOfType(typeof(Date), true) as Date[];
        for(int i = 0; i < allCurrentDatesInScene.Length; i++)
        {
            //SHITTY TEMPORARY FIX REMOVE LATER. Makes sure the game does not save global entities
            if(GlobalParameterManager.Instance.allLoadedGlobalEntities.Contains(allCurrentDatesInScene[i].gameObject) == false)
            {
                AddObjectToList(levelData, allCurrentDatesInScene[i].gameObject);
            }
        }

        //gather state data
        State[] allCurrentStatesInScene = FindObjectsOfType(typeof(State), true) as State[];
        for(int i = 0; i < allCurrentStatesInScene.Length; i++)
        {
            //SHITTY TEMPORARY FIX REMOVE LATER. Makes sure the game does not save global entities
            if(GlobalParameterManager.Instance.allLoadedGlobalEntities.Contains(allCurrentStatesInScene[i].gameObject) == false)
            {
                AddObjectToList(levelData, allCurrentStatesInScene[i].gameObject);
            }
        }


        //gather path data
        PathNode[] allCurrentPathsInScene = FindObjectsOfType(typeof(PathNode), true) as PathNode[];
        for(int i = 0; i < allCurrentPathsInScene.Length; i++)
        {
            AddObjectToList(levelData, allCurrentPathsInScene[i].gameObject);
        }

        //gather path data
        PathNode_Child[] allCurrentChildPathsInScene = FindObjectsOfType(typeof(PathNode_Child), true) as PathNode_Child[];
        for(int i = 0; i < allCurrentChildPathsInScene.Length; i++)
        {
            AddObjectToList(levelData, allCurrentChildPathsInScene[i].gameObject);
        }

        //gather playerMover data
        PlayerMoverEventComponent[] allCurrentPlayerMoversInScene = FindObjectsOfType(typeof(PlayerMoverEventComponent), true) as PlayerMoverEventComponent[];
        for(int i = 0; i < allCurrentPlayerMoversInScene.Length; i++)
        {
            AddObjectToList(levelData, allCurrentPlayerMoversInScene[i].gameObject);
        }

        //gather globalEntityPointer data
        GlobalParameterPointerEntity[] allCurrentGlobalEntityPointersInScene = FindObjectsOfType(typeof(GlobalParameterPointerEntity), true) as GlobalParameterPointerEntity[];
        for(int i = 0; i < allCurrentGlobalEntityPointersInScene.Length; i++)
        {
            AddObjectToList(levelData, allCurrentGlobalEntityPointersInScene[i].gameObject);
        }

        //gather globalEntityPointer data
        LightEntity[] allCurrentLightEntitiesInScene = FindObjectsOfType(typeof(LightEntity), true) as LightEntity[];
        for(int i = 0; i < allCurrentLightEntitiesInScene.Length; i++)
        {
            AddObjectToList(levelData, allCurrentLightEntitiesInScene[i].gameObject);
        }

        //gather prefab data
        PrefabEntity[] allCurrentPrefabEntitiesInScene = FindObjectsOfType(typeof(PrefabEntity), true) as PrefabEntity[];
        for(int i = 0; i < allCurrentPrefabEntitiesInScene.Length; i++)
        {
            AddObjectToList(levelData, allCurrentPrefabEntitiesInScene[i].gameObject);
        }

}

    public void AddObjectToList(SaveAndLoadLevel.LevelData levelData, GameObject obj)
    {

        if(obj == null)
        {
            return;
        }

        if(obj)
        {

        }
        else
        {
            return;
        }

        if(obj.GetComponent<GeneralObjectInfo>() == null)
        {
            return;
        }

        //dont add object to list if it was added additively (is not on the allLoadedGameObjects list)
        if(obj.GetComponent<GeneralObjectInfo>().wasLoadedAdditive)
        {
            return;
        }





/*
        if(obj.GetComponent<MaterialMedia>())
        {
            SaveAndLoadLevel.MaterialData ttt = new SaveAndLoadLevel.MaterialData();
            ttt.materialPath = obj.GetComponent<MaterialMedia>().urlFilePath;
            ttt.shaderName  = obj.GetComponent<MaterialMedia>().shaderName;
            ttt.scrollSpeed = obj.GetComponent<MaterialMedia>().scrollSpeed;
            ttt.textureFiltering = obj.GetComponent<MaterialMedia>().textureFiltering;
            ttt.footstepSoundPath = obj.GetComponent<MaterialMedia>().footstepSoundPath;
            levelData.allLevelMaterials.Add(ttt);

            return;
        }
*/
        if(obj.GetComponent<Block>())
        {


            SaveAndLoadLevel.BlockData ttt = new SaveAndLoadLevel.BlockData();

            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.isActive = obj.GetComponent<GeneralObjectInfo>().isActive_original;

            ttt.isTrigger = obj.GetComponentInChildren<Collider>().isTrigger;
            ttt.isScannable = obj.GetComponent<IsScannable>().isScannable;


            ttt.blockPos = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.blockRotation = obj.GetComponent<GeneralObjectInfo>().rotation_original;

            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.GetComponent<GeneralObjectInfo>().childIndex;
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!
           
            ttt.childIndex = obj.transform.GetSiblingIndex();

            ttt.ignorePlayer = obj.GetComponent<GeneralObjectInfo>().ignorePlayer;
            ttt.ignorePlayerClick = obj.GetComponent<GeneralObjectInfo>().ignorePlayerClick;


            ttt.originalPivot = obj.transform.GetComponent<BlockFaceTextureUVProperties>().originalPivot;

            ttt.materialColor_y = obj.transform.GetComponent<BlockFaceTextureUVProperties>().materialColor_y;
            ttt.materialColor_yneg = obj.transform.GetComponent<BlockFaceTextureUVProperties>().materialColor_yneg;
            ttt.materialColor_x = obj.transform.GetComponent<BlockFaceTextureUVProperties>().materialColor_x;
            ttt.materialColor_xneg = obj.transform.GetComponent<BlockFaceTextureUVProperties>().materialColor_xneg;
            ttt.materialColor_z = obj.transform.GetComponent<BlockFaceTextureUVProperties>().materialColor_z;
            ttt.materialColor_zneg = obj.transform.GetComponent<BlockFaceTextureUVProperties>().materialColor_zneg;

            ttt.materialName_y = obj.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_y;
            ttt.materialName_yneg = obj.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_yneg;
            ttt.materialName_x = obj.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_x;
            ttt.materialName_xneg = obj.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_xneg;
            ttt.materialName_z = obj.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_z;
            ttt.materialName_zneg = obj.transform.GetComponent<BlockFaceTextureUVProperties>().materialName_zneg;
            

            //top
            ttt.corner_Y_X_Z = obj.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_X_Z;
            ttt.corner_Y_X_Zneg = obj.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_X_Zneg;
            ttt.corner_Y_Xneg_Z = obj.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_Xneg_Z;
            ttt.corner_Y_Xneg_Zneg = obj.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Y_Xneg_Zneg;

            //bottom
            ttt.corner_Yneg_X_Z = obj.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_X_Z;
            ttt.corner_Yneg_X_Zneg = obj.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_X_Zneg;
            ttt.corner_Yneg_Xneg_Z = obj.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_Xneg_Z;
            ttt.corner_Yneg_Xneg_Zneg = obj.transform.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_Xneg_Zneg;


            ttt.UVScale = obj.transform.GetComponent<BlockFaceTextureUVProperties>().UVScale;
            ttt.UVOffset= obj.transform.GetComponent<BlockFaceTextureUVProperties>().UVOffSet;
            ttt.UVRotation = obj.transform.GetComponent<BlockFaceTextureUVProperties>().UVRotation;
        
            ttt.events = obj.transform.GetComponent<EventHolderList>().events;
        
            levelData.allLevelBlocks.Add(ttt); 

            return;
        }


        if(obj.GetComponent<PosterMeshCreator>())
        {

            //important... if billboard is being used... update the position.
            //This is only used due to the way blocks need to be loaded-in... the other entities work nicely, but blocks dont (when parented to a billboard)
            if(obj.GetComponent<PosterBillboard>().useCharacterBillboard)
            {   
                obj.GetComponent<GeneralObjectInfo>().UpdatePosition();
            }


            SaveAndLoadLevel.PosterData ttt = new SaveAndLoadLevel.PosterData();
            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.GetComponent<GeneralObjectInfo>().childIndex;
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!
            ttt.isActive = obj.transform.GetComponent<GeneralObjectInfo>().isActive_original;
            ttt.imageUrl = obj.GetComponent<PosterMeshCreator>().urlFilePath;
            ttt._color = obj.GetComponent<PosterMeshCreator>()._color;
            ttt.areaWidth = obj.GetComponent<PosterMeshCreator>().box.x;
            ttt.areaHeight = obj.GetComponent<PosterMeshCreator>().box.y;
            ttt.posterPos = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.posterRot = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;
            ttt.isBillboard = obj.GetComponent<PosterBillboard>().useBillboard;
            ttt.isCharacterBillboard = obj.GetComponent<PosterBillboard>().useCharacterBillboard;
            ttt.shaderName = obj.GetComponent<PosterMeshCreator>().shaderName;
            ttt.textureFiltering = obj.GetComponent<PosterMeshCreator>().textureFiltering;
            ttt.scrollSpeed = obj.GetComponent<PosterTextureScroll>().scrollSpeed;
            ttt.footstepSoundPath = obj.GetComponent<PosterFootstepSound>().footstepSoundPath;
            ttt.events = obj.GetComponent<EventHolderList>().events;


            //gather frame data for this certain poster(ttt)
            for(int i2 = 0; i2 < obj.GetComponent<PosterFrameList>().posterFrameList.Count; i2++) // a for loop is required due to complexity of algorithm
            {
            SaveAndLoadLevel.FrameData frameDat = new SaveAndLoadLevel.FrameData();
            frameDat.frameWidth = obj.GetComponent<PosterFrameList>().posterFrameList[i2].GetComponent<PosterMeshCreator_BorderFrame>().frame_width;
            frameDat.frameHeight = obj.GetComponent<PosterFrameList>().posterFrameList[i2].GetComponent<PosterMeshCreator_BorderFrame>().frame_height;
            frameDat.frameDepth = obj.GetComponent<PosterFrameList>().posterFrameList[i2].GetComponent<PosterMeshCreator_BorderFrame>().heightDepth;
        
            frameDat.frameOuterColor = obj.GetComponent<PosterFrameList>().posterFrameList[i2].GetComponent<PosterMeshCreator_BorderFrame>().rimColors[0];
            frameDat.frameInnerColor = obj.GetComponent<PosterFrameList>().posterFrameList[i2].GetComponent<PosterMeshCreator_BorderFrame>().rimColors[1];

            frameDat.frameOuterLuminance = obj.GetComponent<PosterFrameList>().posterFrameList[i2].GetComponent<PosterMeshCreator_BorderFrame>().rimLuminance[0];

            frameDat.frameInnerLuminance = obj.GetComponent<PosterFrameList>().posterFrameList[i2].GetComponent<PosterMeshCreator_BorderFrame>().rimLuminance[1];

            ttt.allPosterFrames.Add(frameDat);
            }

            //gather depth layer data for this certain poster(ttt)
            for(int i2 = 0; i2 < obj.GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count; i2++) // a for loop is required due to complexity of algorithm
            {
            SaveAndLoadLevel.DepthLayerData depthLayerDat = new SaveAndLoadLevel.DepthLayerData();
            depthLayerDat.IdOfPosterToReference = obj.GetComponent<PosterDepthLayerList>().posterDepthLayerList[i2].GetComponent<Poster_DepthStencilFrame>().IdOfPosterToReference;
            depthLayerDat.depth = obj.GetComponent<PosterDepthLayerList>().posterDepthLayerList[i2].GetComponent<Poster_DepthStencilFrame>().depth;
            depthLayerDat.size = obj.GetComponent<PosterDepthLayerList>().posterDepthLayerList[i2].GetComponent<Poster_DepthStencilFrame>().size;
            depthLayerDat.shapeKey_CurveX = obj.GetComponent<PosterDepthLayerList>().posterDepthLayerList[i2].GetComponent<Poster_DepthStencilFrame>().shapeKey_CurveX;
            depthLayerDat.shapeKey_CurveY = obj.GetComponent<PosterDepthLayerList>().posterDepthLayerList[i2].GetComponent<Poster_DepthStencilFrame>().shapeKey_CurveY;

            ttt.allPosterDepthLayers.Add(depthLayerDat);
            }


            //view settings
            ttt.scrollingMode = obj.GetComponent<PosterViewSettings>().scrollingMode;
            ttt.alignmentMode = obj.GetComponent<PosterViewSettings>().alignmentMode;
            ttt.zoomForcedOffset = obj.GetComponent<PosterViewSettings>().zoomForcedOffset;
            ttt.rotationEffectAmount = obj.GetComponent<PosterViewSettings>().rotationEffectAmount;
            ttt.canZoom = obj.GetComponent<PosterViewSettings>().canZoom;
            ttt.extraBorder = obj.GetComponent<PosterViewSettings>().extraBorder;
            ttt.inverseLook = obj.GetComponent<PosterViewSettings>().inverseLook;

            //color key settings
            ttt.colorKey = obj.GetComponent<PosterColorKeySettings>().colorKey;
            ttt.colorKey_threshold = obj.GetComponent<PosterColorKeySettings>().threshold;
            ttt.transparencyThreshold = obj.GetComponent<PosterColorKeySettings>().transparencyThreshold;
            ttt.spillCorrection = obj.GetComponent<PosterColorKeySettings>().spillCorrection;

            //xray settings
            ttt.xray_Size = obj.GetComponent<PosterXraySettings>().xray_Size;
            ttt.xray_Transparency = obj.GetComponent<PosterXraySettings>().xray_Transparency;
            ttt.xray_Softness = obj.GetComponent<PosterXraySettings>().xray_Softness;
            
            ttt.isScannable = obj.GetComponent<IsScannable>().isScannable;





            levelData.allLevelPosters.Add(ttt); 

            return;
        }

        if(obj.GetComponent<DoorInfo>())
        {
            SaveAndLoadLevel.DoorData ttt = new SaveAndLoadLevel.DoorData();
            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.GetComponent<GeneralObjectInfo>().childIndex;
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!
            ttt.isActive = obj.transform.GetComponent<GeneralObjectInfo>().isActive_original;
            ttt.doorPos = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.doorRot = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;
            ttt.doorUrl = obj.GetComponent<DoorInfo>().pathFileToLoad;
            levelData.allLevelDoors.Add(ttt); 

            return;
        }

        if(obj.GetComponent<DialogueContentObject>())
        {
            SaveAndLoadLevel.DialogueData ttt = new SaveAndLoadLevel.DialogueData();
            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.GetComponent<GeneralObjectInfo>().childIndex;
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!
            ttt.position = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.rotation = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;
            ttt.dialogue = obj.GetComponent<DialogueContentObject>().dialogue;
            ttt.voicePath = obj.GetComponent<DialogueContentObject>().voicePath;
            ttt.pitch = obj.GetComponent<DialogueContentObject>().pitch;
            ttt.clearPreviousDialogue = obj.GetComponent<DialogueContentObject>().clearPreviousDialogue;
            ttt.events = obj.GetComponent<EventHolderList>().events;

            levelData.allLevelDialogue.Add(ttt); 

            return;
        }

        if(obj.GetComponent<AudioSourceObject>())
        {
            SaveAndLoadLevel.AudioSourceData ttt = new SaveAndLoadLevel.AudioSourceData();
            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.GetComponent<GeneralObjectInfo>().childIndex;
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!
            ttt.position = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.rotation = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;
            ttt.audioPath = obj.GetComponent<AudioSourceObject>().audioPath;
            ttt.pitch = obj.GetComponent<AudioSourceObject>().pitch;
            ttt.spatialBlend = obj.GetComponent<AudioSourceObject>().spatialBlend;
            ttt.minDistance = obj.GetComponent<AudioSourceObject>().minDistance;
            ttt.maxDistance = obj.GetComponent<AudioSourceObject>().maxDistance;
            ttt.repeat = obj.GetComponent<AudioSourceObject>().repeat;
            ttt.events = obj.GetComponent<EventHolderList>().events;

            levelData.allLevelAudioSources.Add(ttt); 

            return;
        }

        if(obj.GetComponent<Counter>())
        {
            SaveAndLoadLevel.CounterData ttt = new SaveAndLoadLevel.CounterData();
            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.GetComponent<GeneralObjectInfo>().childIndex;
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!
            ttt.position = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.rotation = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;
            ttt.currentCount_default = obj.GetComponent<Counter>().currentCount_default;
            ttt.currentCount = obj.GetComponent<Counter>().currentCount;
            ttt.events = obj.GetComponent<EventHolderList>().events;

            levelData.allLevelCounters.Add(ttt); 

            return;
        }


        if(obj.GetComponent<StringEntity>())
        {
            SaveAndLoadLevel.StringData ttt = new SaveAndLoadLevel.StringData();
            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.GetComponent<GeneralObjectInfo>().childIndex;
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!
            ttt.position = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.rotation = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;
            ttt.currentString_default = obj.GetComponent<StringEntity>().currentString_default;
            ttt.currentString = obj.GetComponent<StringEntity>().currentString;
            ttt.events = obj.GetComponent<EventHolderList>().events;

            levelData.allLevelStrings.Add(ttt); 

            return;
        }
        

        if(obj.GetComponent<State>())
        {
            SaveAndLoadLevel.StateData ttt = new SaveAndLoadLevel.StateData();
            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.GetComponent<GeneralObjectInfo>().childIndex;
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!
            ttt.position = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.rotation = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;
            ttt.states = obj.GetComponent<State>().states;
            ttt.timeUntilChoiceBoxCloses = obj.GetComponent<State>().timeUntilChoiceBoxCloses;
            ttt.events = obj.GetComponent<EventHolderList>().events;

            levelData.allLevelStates.Add(ttt); 

            return;
        }

        if(obj.GetComponent<PathNode>())
        {
            SaveAndLoadLevel.PathData ttt = new SaveAndLoadLevel.PathData();
            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.transform.GetSiblingIndex();
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!

            
            ttt.position = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.rotation = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;



            ttt.idOfObjectToMove = obj.GetComponent<PathNode>().idOfObjectToMove;
            ttt.moveToPath = obj.GetComponent<PathNode>().moveToPath;
            ttt.time = obj.GetComponent<PathNode>().time;
            ttt.loopType = obj.GetComponent<PathNode>().loopType;
            ttt.closeLoop = obj.GetComponent<PathNode>().closeLoop;
            ttt.pathType = obj.GetComponent<PathNode>().pathType;
            ttt.wayPointRotation = obj.GetComponent<PathNode>().wayPointRotation;
            ttt.easeType = obj.GetComponent<PathNode>().easeType;


            ttt.events = obj.GetComponent<EventHolderList>().events;


            for(int i2 = 0; i2 < obj.GetComponent<GeneralObjectInfo>().childrenObjects.Count; i2++) 
            {
                if(i2 >= 0 && i2 < obj.transform.childCount)
                {
                    SaveAndLoadLevel.PathChildData pathChild = new SaveAndLoadLevel.PathChildData();
                    pathChild.id = obj.transform.GetChild(i2).name;
                    pathChild.note = obj.transform.GetChild(i2).GetComponent<Note>().note;
                    pathChild.position = obj.transform.GetChild(i2).GetComponent<GeneralObjectInfo>().position_original;
                    pathChild.rotation = obj.transform.GetChild(i2).GetComponent<GeneralObjectInfo>().rotation_original;

                    pathChild.idOfParent = obj.transform.GetChild(i2).GetComponent<GeneralObjectInfo>().idOfParent;
                    pathChild.childIndex = obj.transform.GetChild(i2).transform.GetSiblingIndex();

                }
            }

            levelData.allLevelPaths.Add(ttt); 
            return;
        }


        if(obj.GetComponent<PathNode_Child>())
        {
            SaveAndLoadLevel.PathChildData ttt = new SaveAndLoadLevel.PathChildData();
            ttt.id = obj.transform.name;
            ttt.note = obj.transform.GetComponent<Note>().note;
            ttt.position = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.rotation = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;
            ttt.idOfParent = obj.transform.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.transform.GetSiblingIndex();

            levelData.allLevelChildPaths.Add(ttt); 
        }




        if(obj.GetComponent<PlayerMoverEventComponent>())
        {
            SaveAndLoadLevel.PlayerMoverData ttt = new SaveAndLoadLevel.PlayerMoverData();
            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.GetComponent<GeneralObjectInfo>().childIndex;
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!
            ttt.position = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.rotation = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;
            ttt.events = obj.GetComponent<EventHolderList>().events;

            levelData.allPlayerMovers.Add(ttt); 

            return;
        }



        if(obj.GetComponent<GlobalParameterPointerEntity>())
        {
            SaveAndLoadLevel.GlobalEntityPointerData ttt = new SaveAndLoadLevel.GlobalEntityPointerData();
            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.GetComponent<GeneralObjectInfo>().childIndex;
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!
            ttt.position = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.rotation = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;
            ttt.events = obj.GetComponent<EventHolderList>().events;

            ttt.idOfGlobalEntityToPointTo = obj.GetComponent<GlobalParameterPointerEntity>().idOfGlobalEntityToPointTo;

            levelData.allGlobalEntityPointers.Add(ttt); 

            return;
        }


        if(obj.GetComponent<Date>())
        {
            SaveAndLoadLevel.DateData ttt = new SaveAndLoadLevel.DateData();
            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.GetComponent<GeneralObjectInfo>().childIndex;
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!
            ttt.position = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.rotation = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;
            
            ttt.date = obj.GetComponent<Date>().date.ToString();

            ttt.events = obj.GetComponent<EventHolderList>().events;

            levelData.allLevelDates.Add(ttt); 

            return;
        }


        if(obj.GetComponent<LightEntity>())
        {
            SaveAndLoadLevel.LightData ttt = new SaveAndLoadLevel.LightData();
            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.GetComponent<GeneralObjectInfo>().childIndex;
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!
            ttt.position = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.rotation = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;
            ttt.isActive = obj.transform.GetComponent<GeneralObjectInfo>().isActive_original;

            ttt.lightType = obj.GetComponent<LightEntity>().lightType;
            ttt.range = obj.GetComponent<LightEntity>().range;
            ttt.strength = obj.GetComponent<LightEntity>().strength;
            ttt.color = obj.GetComponent<LightEntity>().color;
            ttt.spotAngle = obj.GetComponent<LightEntity>().spotAngle;
            ttt.shadowType = obj.GetComponent<LightEntity>().shadowType;
            
            ttt.events = obj.GetComponent<EventHolderList>().events;

            levelData.allLevelLights.Add(ttt); 

            return;
        }


        if(obj.GetComponent<PrefabEntity>())
        {
            SaveAndLoadLevel.PrefabData ttt = new SaveAndLoadLevel.PrefabData();
            ttt.id = obj.name;
            ttt.note = obj.GetComponent<Note>().note;
            ttt.idOfParent = obj.GetComponent<GeneralObjectInfo>().idOfParent;
            ttt.childIndex = obj.GetComponent<GeneralObjectInfo>().childIndex;
            ttt.children = new List<string>(obj.GetComponent<GeneralObjectInfo>().children); //create new list or else there will be hard to debug problems due to references!
            ttt.position = obj.transform.GetComponent<GeneralObjectInfo>().position_original;
            ttt.rotation = obj.transform.GetComponent<GeneralObjectInfo>().rotation_original;
            ttt.isActive = obj.transform.GetComponent<GeneralObjectInfo>().isActive_original;

            ttt.mapToLoad = obj.GetComponent<PrefabEntity>().mapToLoad;
            ttt.loadOnStart = obj.GetComponent<PrefabEntity>().loadOnStart;
            
            ttt.events = obj.GetComponent<EventHolderList>().events;

            levelData.allLevelPrefabs.Add(ttt); 

            return;
        }


    }


    //for use in zip files...
    //also make sure to remove ALL absolute paths from the player's PC!!! 

    public void ConvertAllMediaPathsToJustFileName(SaveAndLoadLevel.LevelData levelData)
    {
        //global media paths
        levelData.musicTrackPath = GlobalUtilityFunctions.GetOnlyFileNameFromPath(levelData.musicTrackPath);
        levelData.globalSkyboxMediaPath = GlobalUtilityFunctions.GetOnlyFileNameFromPath(levelData.globalSkyboxMediaPath);


        //loop through all blocks and correct the paths
        foreach(SaveAndLoadLevel.BlockData block in levelData.allLevelBlocks)
        {
            block.materialName_y = GlobalUtilityFunctions.GetOnlyFileNameFromPath(block.materialName_y);
            block.materialName_yneg = GlobalUtilityFunctions.GetOnlyFileNameFromPath(block.materialName_yneg);
            block.materialName_x = GlobalUtilityFunctions.GetOnlyFileNameFromPath(block.materialName_x);
            block.materialName_xneg = GlobalUtilityFunctions.GetOnlyFileNameFromPath(block.materialName_xneg);
            block.materialName_z = GlobalUtilityFunctions.GetOnlyFileNameFromPath(block.materialName_z);
            block.materialName_zneg = GlobalUtilityFunctions.GetOnlyFileNameFromPath(block.materialName_zneg);
        }


/*
        //the usual entities with media...
        foreach(SaveAndLoadLevel.MaterialData mat in levelData.allLevelMaterials)
        {
            mat.materialPath = GlobalUtilityFunctions.GetOnlyFileNameFromPath(mat.materialPath);
            mat.footstepSoundPath = GlobalUtilityFunctions.GetOnlyFileNameFromPath(mat.footstepSoundPath);
        }
*/
        foreach (SaveAndLoadLevel.PosterData poster in levelData.allLevelPosters)
        {
            poster.imageUrl = GlobalUtilityFunctions.GetOnlyFileNameFromPath(poster.imageUrl);
            
            poster.footstepSoundPath = GlobalUtilityFunctions.GetOnlyFileNameFromPath(poster.footstepSoundPath);
        }

        foreach (SaveAndLoadLevel.DialogueData dialogue in levelData.allLevelDialogue)
        {
            dialogue.voicePath = GlobalUtilityFunctions.GetOnlyFileNameFromPath(dialogue.voicePath);
        }

        foreach (SaveAndLoadLevel.AudioSourceData audioSource in levelData.allLevelAudioSources)
        {
            audioSource.audioPath = GlobalUtilityFunctions.GetOnlyFileNameFromPath(audioSource.audioPath);
        }

    }




//BUTTON GUI HANDLING


    public Button saveButton;
    public Button saveAsButton;


void Update()
{
    //NOTE!!! these things are only resonsible for toggling the button coloring


    if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp == null)
    {
        saveButton.interactable = false;
        saveAsButton.interactable = false;
    }
    else
    {
        saveButton.interactable = true;
        saveAsButton.interactable = true;
    }


    if(EditModeStaticParameter.isInEditMode == false)
    {
        saveButton.interactable = false;
        saveAsButton.interactable = false;
    }

    //zip file opened, deactivate save as...
    if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp != null && GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("TEMPORARY_"))
    {
        saveAsButton.interactable = false;
    }
}

}