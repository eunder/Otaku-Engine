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
using System.Linq;

public class GlobalParameterManager : MonoBehaviour
{  

    private static GlobalParameterManager _instance;
    public static GlobalParameterManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public SaveAndLoadLevel.LevelData savedEntities;

    public List<GameObject> allLoadedGlobalEntities = new List<GameObject>();

    //USED FOR GLOBAL ENTITIES (there is no similar function for local entities)
    public void LoadGlobalEntities()
    {

            //clear list
            foreach(GameObject obj in allLoadedGlobalEntities)
            {
                Destroy(obj);
            }
            allLoadedGlobalEntities.Clear();

        if(ProjectManager.Instance.IsCurrentMapPartOfAProject())
        {
            //read file
            //current level location -> up a directory -> check if there is a project.json
            string parentDir;

            //if there is a source zip file being worked on... use the path of the source zip file and NOT the "LevelFileToPathToLoadOnStartUp" because it would be in the windows temp folder 
            if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("TEMPORARY_EXTRACTED_ZIP_"))
            {
                parentDir = Path.GetDirectoryName(ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith);
            }
            else
            {
                parentDir = Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp);
            }


            //if the file does not exist... then load the "SaveData_Base.json" entities instead...
            if(!File.Exists(parentDir + "/../" + "SaveData.json"))
            {
                if(!File.Exists(parentDir + "/../" + "SaveData_Base.json"))
                {
                    UINotificationHandler.Instance.SpawnNotification("<color=red>No SaveData_Base.json found!");
                    return;
                }
                string json = File.ReadAllText(parentDir + "/../" + "SaveData_Base.json");
                savedEntities = JsonUtility.FromJson<SaveAndLoadLevel.LevelData>(json);
            }
            else
            {
                 string json = File.ReadAllText(parentDir + "/../" + "SaveData.json");
                 savedEntities = JsonUtility.FromJson<SaveAndLoadLevel.LevelData>(json);
            }

            



            //spawn the saved persistant entities
            foreach(SaveAndLoadLevel.CounterData counter in savedEntities.allLevelCounters)
            {
                allLoadedGlobalEntities.Add(SaveAndLoadLevel.Instance.CreateCounterEntity(counter, true));
            }

            //spawn the saved persistant entities
            foreach(SaveAndLoadLevel.StringData stringEnt in savedEntities.allLevelStrings)
            {
                allLoadedGlobalEntities.Add(SaveAndLoadLevel.Instance.CreateStringEntity(stringEnt, true));
            }

            //spawn the saved persistant entities
            foreach(SaveAndLoadLevel.DateData date in savedEntities.allLevelDates)
            {
                allLoadedGlobalEntities.Add(SaveAndLoadLevel.Instance.CreateDateEntity(date, true));
            }



            //add the component GlobalParameter_ComponentIdentifier so that later you can make sure local variables dont add global variables to the list...
            foreach(GameObject obj in allLoadedGlobalEntities)
            {
                obj.AddComponent<GlobalParameter_ComponentIdentifier>();
            }

            currentSelectedObjID = null;


            PopulateGlobalAndLocalEntityList();
            
        }
    }


    //USED FOR GLOBAL ENTITIES (there is no similar function for local entities)
    public void SaveGlobalEntities()
    {
            savedEntities = new SaveAndLoadLevel.LevelData();
            //gather all the global entities and put them in the list

            foreach(GameObject obj in allLoadedGlobalEntities)
            {
                SaveMap.Instance.AddObjectToList(savedEntities, obj);
            }



            string parentDir;

            //if there is a source zip file being worked on... use the path of the source zip file and NOT the "LevelFileToPathToLoadOnStartUp" because it would be in the windows temp folder 
            if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("TEMPORARY_EXTRACTED_ZIP_"))
            {
                parentDir = Path.GetDirectoryName(ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith);
            }
            else
            {
                parentDir = Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp);
            }
            

            //write to file...
            string json = JsonUtility.ToJson(savedEntities, true);
            File.WriteAllText(parentDir + "/../" + "SaveData.json", json);


            UpdateSaveData_Base();
            PopulateGlobalAndLocalEntityList();
    }



    //updates the base save data... this simply write the current game's "allLoadedGlobalEntities" into the "SaveData_Base.json" file using the default parameters... 
    public void UpdateSaveData_Base()
    {
         SaveAndLoadLevel.LevelData baseEntities = new SaveAndLoadLevel.LevelData();
           
        //gather all the global entities and put them in the list
        foreach(GameObject obj in allLoadedGlobalEntities)
        {
            //note! make sure to make a deep copy of the list... do NOT assign by reference
            SaveMap.Instance.AddObjectToList(baseEntities, obj);
        }



        //use their default values... reset them
        foreach(SaveAndLoadLevel.CounterData counter in baseEntities.allLevelCounters)
        {
            counter.currentCount = counter.currentCount_default;
        }
        foreach(SaveAndLoadLevel.StringData stringEntity in baseEntities.allLevelStrings)
        {
            stringEntity.currentString = stringEntity.currentString_default;
        }
        foreach(SaveAndLoadLevel.DateData date in baseEntities.allLevelDates)
        {
            date.date = default(DateTime).ToString();
        }


        string parentDir;
        //if there is a source zip file being worked on... use the path of the source zip file and NOT the "LevelFileToPathToLoadOnStartUp" because it would be in the windows temp folder 
        if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("TEMPORARY_EXTRACTED_ZIP_"))
        {
            parentDir = Path.GetDirectoryName(ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith);
        }
        else
        {
            parentDir = Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp);
        }

        



        //write to file...
        string json = JsonUtility.ToJson(baseEntities, true);
        File.WriteAllText(parentDir + "/../" + "SaveData_Base.json", json);


    }




    // used for the pointer entity and global entitity inspector

    
    public List<GameObject> ReturnObjectListOfGlobalEntitites()
    {

        //organize by alphabetical order

        List<GameObject> sortedList = allLoadedGlobalEntities.OrderBy(go => go.GetComponent<Note>().note).ToList();

        return sortedList;
    }
    

    public List<GameObject> ReturnObjectListOfLocalVariables()
    {
        List<GameObject> allLocalVariables = new List<GameObject>();


        //gather date data
        Date[] allCurrentDatesInScene = FindObjectsOfType(typeof(Date), true) as Date[];
        for(int i = 0; i < allCurrentDatesInScene.Length; i++)
        {
            //Makes sure the game does not save global entities
            if(allCurrentDatesInScene[i].gameObject.GetComponent<GlobalParameter_ComponentIdentifier>() == false)
            {
                allLocalVariables.Add(allCurrentDatesInScene[i].gameObject);
            }
        }

        //gather counter data
        Counter[] allCurrentCountersInScene = FindObjectsOfType(typeof(Counter), true) as Counter[];
        for(int i = 0; i < allCurrentCountersInScene.Length; i++)
        {
            //Makes sure the game does not save global entities
            if(allCurrentCountersInScene[i].gameObject.GetComponent<GlobalParameter_ComponentIdentifier>() == false)
            {
                allLocalVariables.Add(allCurrentCountersInScene[i].gameObject);
            }
        }

        //gather string data
        StringEntity[] allCurrentStringsInScene = FindObjectsOfType(typeof(StringEntity), true) as StringEntity[];
        for(int i = 0; i < allCurrentStringsInScene.Length; i++)
        {
            //Makes sure the game does not save global entities
            if(allCurrentStringsInScene[i].gameObject.GetComponent<GlobalParameter_ComponentIdentifier>() == false)
            {
                allLocalVariables.Add(allCurrentStringsInScene[i].gameObject);
            }
        }


        //organize by alphabetical order
        List<GameObject> sortedList = allLocalVariables.OrderBy(go => go.GetComponent<Note>().note).ToList();

        return sortedList;
    }




    //FOR POPULATING THE MENU


    public GameObject currentlyHighlightedEntityUIObject;

    public GameObject variableEntityEditor_UIEntityHolderPrefab;


    public List<GameObject> globalEntityEditor_UIentityList = new List<GameObject>();
    public GameObject globalVariableEntityEditor_UIEntityHolderContainer;

    public List<GameObject> localEntityEditor_UIentityList = new List<GameObject>();
    public GameObject localVariableEntityEditor_UIEntityHolderContainer;




     public void PopulateGlobalAndLocalEntityList()
    {
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

        //global
        foreach(GameObject obj in ReturnObjectListOfGlobalEntitites())
        {
        GameObject newUiEntity = Instantiate(variableEntityEditor_UIEntityHolderPrefab);
        newUiEntity.transform.SetParent(globalVariableEntityEditor_UIEntityHolderContainer.transform, false);

        newUiEntity.GetComponent<GlobalParameterUIHolder>().idOfGlobalEntityToPointTo = obj.GetComponent<GeneralObjectInfo>().id;
        newUiEntity.GetComponent<GlobalParameterUIHolder>().gameObjectEntity = obj;
        newUiEntity.GetComponent<GlobalParameterUIHolder>().UpdateUIInfo();

        globalEntityEditor_UIentityList.Add(newUiEntity);
        }


        //local
        foreach(GameObject obj in ReturnObjectListOfLocalVariables())
        {
        GameObject newUiEntity = Instantiate(variableEntityEditor_UIEntityHolderPrefab);
        newUiEntity.transform.SetParent(localVariableEntityEditor_UIEntityHolderContainer.transform, false);

        newUiEntity.GetComponent<GlobalParameterUIHolder>().idOfGlobalEntityToPointTo = obj.GetComponent<GeneralObjectInfo>().id;
        newUiEntity.GetComponent<GlobalParameterUIHolder>().gameObjectEntity = obj;
        newUiEntity.GetComponent<GlobalParameterUIHolder>().UpdateUIInfo();

        localEntityEditor_UIentityList.Add(newUiEntity);
        }


        SelectCurrentlyHighlightedUIEntityEntryBasedOnNoteName();
        RefreshLocalAndGlobalVariableList();
        
        UpdatePointerEntityReferences();
    }
    
    
    //HIGH LIGHTING/SELECTING MECHANIC
    //selecting the currently highlighted object needs to be done this way because the inspector menu gets refreshed (the objects get deleted and created again, thus losing the reference)
    public string currentSelectedObjID;
    public void SelectCurrentlyHighlightedUIEntityEntryBasedOnNoteName()
    {
        if(string.IsNullOrEmpty(currentSelectedObjID) == false)
        {
            foreach(GameObject obj in globalEntityEditor_UIentityList)
            {
                if(obj.GetComponent<GlobalParameterUIHolder>().gameObjectEntity.name == currentSelectedObjID)
                {
                    currentlyHighlightedEntityUIObject = obj;
                    break;
                }
            }
            foreach(GameObject obj in localEntityEditor_UIentityList)
            {
                if(obj.GetComponent<GlobalParameterUIHolder>().gameObjectEntity.name == currentSelectedObjID)
                {
                    currentlyHighlightedEntityUIObject = obj;
                    break;
                }
            }
        }
    }


    public void RefreshLocalAndGlobalVariableList()
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

    //update the references of the "widgetinfo" class or else when the player opens and closes the toolbar... the references will be removed
    public void UpdatePointerEntityReferences()
    {
                GlobalParameterPointerEntity[] allCurrentPointersInScene = FindObjectsOfType(typeof(GlobalParameterPointerEntity), true) as GlobalParameterPointerEntity[];
        for(int i = 0; i < allCurrentPointersInScene.Length; i++)
        {
            allCurrentPointersInScene[i].AssignImageOfEntityToPointTo();
        }
    }



    // CREATE/DELETE LOCAL/GLOBAL ENTITY MECHANIC



    public GameObject DeleteEntityWindow_Global;
    public GameObject DeleteEntityWindow_Local;

    public void EraseSelectedEntity_Window_Global()
    {  
        //prevent player from using the global menu to delete a local variable
        if(currentlyHighlightedEntityUIObject.GetComponent<GlobalParameterUIHolder>().gameObjectEntity.GetComponent<GlobalParameter_ComponentIdentifier>())
        {
            DeleteEntityWindow_Global.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();
        }
    }

    public void EraseSelectedEntity_Window_Local()
    {
        //prevent the player from using the local menu to delete a global variable
        if(currentlyHighlightedEntityUIObject.GetComponent<GlobalParameterUIHolder>().gameObjectEntity.GetComponent<GlobalParameter_ComponentIdentifier>() == null)
        {
            DeleteEntityWindow_Local.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();
        }
    }






    public void EraseSelectedEntity_Global()
    {
        if(currentlyHighlightedEntityUIObject != null)
        {
        DeleteEntityWindow_Global.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();

        //remove from list, delete the entity, and delete the UI entity holder
        allLoadedGlobalEntities.Remove(currentlyHighlightedEntityUIObject.GetComponent<GlobalParameterUIHolder>().gameObjectEntity);
        Destroy(currentlyHighlightedEntityUIObject.GetComponent<GlobalParameterUIHolder>().gameObjectEntity);
        globalEntityEditor_UIentityList.Remove(currentlyHighlightedEntityUIObject);
        Destroy(currentlyHighlightedEntityUIObject);

        SaveGlobalEntities();
        }
    }

    public void EraseSelectedEntity_Local()
    {
        if(currentlyHighlightedEntityUIObject != null)
        {
        DeleteEntityWindow_Local.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();

        Destroy(currentlyHighlightedEntityUIObject.GetComponent<GlobalParameterUIHolder>().gameObjectEntity);
        localEntityEditor_UIentityList.Remove(currentlyHighlightedEntityUIObject);
        Destroy(currentlyHighlightedEntityUIObject);
        
        }
    }


    public GameObject entityAddingWindow_Global;
    public GameObject entityAddingWindow_Local;

    //open up the add entity window
    public void OpenEntityAddingWindow_Global()
    {
        entityAddingWindow_Global.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();
    }
    public void OpenEntityAddingWindow_Local()
    {
        entityAddingWindow_Local.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();
    }

    //create the entity
    public void CreateEntity_Global(GameObject prefab)
    {
        entityAddingWindow_Global.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();

        GameObject obj = Instantiate(prefab, new Vector3(0f,0f,0f), Quaternion.identity);
        obj.name = System.Guid.NewGuid().ToString();
        obj.layer = 21;  //add the newly loaded global entities to the "gizmo_hidden" layer to prevent the user from tempering with them in the level
        obj.GetComponent<GeneralObjectInfo>().UpdateID();
        obj.GetComponent<GeneralObjectInfo>().UpdateParent();
        obj.GetComponent<GeneralObjectInfo>().UpdateChildren();

        obj.AddComponent<GlobalParameter_ComponentIdentifier>(); //to prevent it from being put into local variables
        
        allLoadedGlobalEntities.Add(obj);
        SaveGlobalEntities();
        PopulateGlobalAndLocalEntityList();
    }

    public void CreateEntity_Local(GameObject prefab)
    {
        entityAddingWindow_Local.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();

        GameObject obj = Instantiate(prefab, new Vector3(0f,0f,0f), Quaternion.identity);
        obj.name = System.Guid.NewGuid().ToString();
        obj.layer = 21;  //add the newly loaded global entities to the "gizmo_hidden" layer to prevent the user from tempering with them in the level
        obj.GetComponent<GeneralObjectInfo>().UpdateID();
        obj.GetComponent<GeneralObjectInfo>().UpdateParent();
        obj.GetComponent<GeneralObjectInfo>().UpdateChildren();

        PopulateGlobalAndLocalEntityList();
    }



    //RESET LOCAL/GLOBAL VALUES MECHANIC


    public GameObject ResetEntitiesWindow_Global;
    public GameObject ResetEntitiesWindow_Local;

    public void ResetAllEntitiesBackToDefault_Window_Global()
    {
        ResetEntitiesWindow_Global.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();
    }

    public void ResetAllEntitiesBackToDefault_Window_Local()
    {
        ResetEntitiesWindow_Local.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();
    }

    public void ResetAllEntitiesBackToDefault_Global()
    {
        ResetEntitiesWindow_Global.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();

        foreach(GameObject obj in ReturnObjectListOfGlobalEntitites())
        {


            if(obj.GetComponent<Counter>())
            {
                obj.GetComponent<CounterEventComponent>().ResetToDefault();
            }
            if(obj.GetComponent<StringEntity>())
            {
                obj.GetComponent<StringEventComponent>().ResetToDefault();
            }
            if(obj.GetComponent<DateEventComponent>())
            {
                obj.GetComponent<DateEventComponent>().ResetToDefault();
            }
        }

    SaveGlobalEntities();
    LoadGlobalEntities();

    UINotificationHandler.Instance.SpawnNotification("All global values reset.");

    }
    
        public void ResetAllEntitiesBackToDefault_Local()
    {
        ResetEntitiesWindow_Local.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();

        foreach(GameObject obj in ReturnObjectListOfLocalVariables())
        {

            if(obj.GetComponent<Counter>())
            {
                obj.GetComponent<CounterEventComponent>().ResetToDefault();
            }
            if(obj.GetComponent<StringEntity>())
            {
                obj.GetComponent<StringEventComponent>().ResetToDefault();
            }
            if(obj.GetComponent<DateEventComponent>())
            {
                obj.GetComponent<DateEventComponent>().ResetToDefault();
            }
        }

     PopulateGlobalAndLocalEntityList();
    UINotificationHandler.Instance.SpawnNotification("All local values reset.");
    }





    //mostly used for the text/dialogue variable feature 
    public string ReturnCertainValueBasedOnVariableType(GameObject obj)
    {
        if(obj.GetComponent<Counter>())
        {
            return obj.GetComponent<Counter>().currentCount.ToString();
        }
        if(obj.GetComponent<StringEntity>())
        {
            return obj.GetComponent<StringEntity>().currentString.ToString();
        }
        if(obj.GetComponent<Date>())
        {
            return obj.GetComponent<Date>().date.ToString();
        }
        return "";
    }

}