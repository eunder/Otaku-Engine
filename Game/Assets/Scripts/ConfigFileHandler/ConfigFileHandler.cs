using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class ConfigFileHandler : MonoBehaviour
{
    private static ConfigFileHandler _instance;
    public static ConfigFileHandler Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }



    public static float toolHoldAndTapAmount = 0.25f; //for the mechanic that tools use to tap/hold for options






    public ConfigData configData;

    [System.Serializable]
    public class ConfigData
    {
        public WorkshopTags Tags;
        public Game game;
    }

    [System.Serializable]
    public class Game
    {
    public string initialProject = "OtakuEngine";
    public string missionPrompt = "";
    public int missionsCompleted = 0;
    }


    [System.Serializable]
    public class WorkshopTags
    {
        //global map settings
        public bool safe = true;
        public bool questionable = true;
        public bool mature = false;
    }

    void Start()
    {
        StartCoroutine(InitializeGame());
    }

    IEnumerator InitializeGame()
    {
        //load the toolbar into the initial blank scene
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);

        // create file if it dosnt exit
        if(!File.Exists(Application.persistentDataPath + "/config.json"))
        {
            string json = JsonUtility.ToJson(configData, true);
            File.WriteAllText(Application.persistentDataPath + "/config.json", json);
        }
        else //load file if it exists
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/config.json");
            configData = JsonUtility.FromJson<ConfigData>(json);
        }

        yield return new WaitForSeconds(0.25f); //delay to make sure the classes AND the file load in time... not a very safe method but it works for now




        //load first map/project

        if(configData.game.initialProject == "OtakuEngine")
        {
            GlobalUtilityFunctions.OpenMap(Application.dataPath + "/StreamingAssets/Projects/OtakuEngine/Project.json", false);
        }
        else
        {
            GlobalUtilityFunctions.OpenMap(configData.game.initialProject, false);
        }


    }




    public void SetCurrentMapOrProjectAsDefaultOnStartup()
    {
        if(!string.IsNullOrEmpty(ProjectManager.currentOpenedProjectPath))
        {
            if(GlobalUtilityFunctions.IsPathSafe(ProjectManager.currentOpenedProjectPath + "/Project.json"))
            {
                UINotificationHandler.Instance.SpawnNotification("Project set as default!");
                configData.game.initialProject = ProjectManager.currentOpenedProjectPath + "/Project.json";
                SaveConfigFile();
                return;
            }
        }
        else if(!string.IsNullOrEmpty(ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith))
        {
            if(GlobalUtilityFunctions.IsPathSafe(ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith))
            {
                UINotificationHandler.Instance.SpawnNotification("Map set as default!");
                configData.game.initialProject = ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith;
                SaveConfigFile();
                return;
            }
        }
        else if(!string.IsNullOrEmpty(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp))
        {
            if(GlobalUtilityFunctions.IsPathSafe(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp))
            {
                UINotificationHandler.Instance.SpawnNotification("Map set as default!");
                configData.game.initialProject = GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp;
                SaveConfigFile();
                return;
            }
        }
        else
        {
            UINotificationHandler.Instance.SpawnNotification("no map to set as default!");
            return;
        }
    }

    public void SaveConfigFile()
    {
            string json = JsonUtility.ToJson(configData, true);
            File.WriteAllText(Application.persistentDataPath + "/config.json", json);
    }
}
