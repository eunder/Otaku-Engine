using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class AutoBackupManager : MonoBehaviour
{
    private static AutoBackupManager _instance;
    public static AutoBackupManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public int interval = 2;


    void Start()
    {
        interval = PlayerPrefs.GetInt("AutoBackupInterval", 2);

        StartCoroutine(CallFunctionRepeatedly());
    }


    private IEnumerator CallFunctionRepeatedly()
    {
        while (true)
        {
            interval = Mathf.Clamp(interval, 1, 60);

            yield return new WaitForSeconds(interval * 60); // Wait for the specified interval

            //only save map backup on edit mode...
            if(EditModeStaticParameter.isInEditMode)
            {
                SaveBackUpOfCurrentOpenedMapJson();
            }

            //runs on play mode AND in edit mode...
            //make sure a project is opened
            if(ProjectManager.Instance.IsCurrentMapPartOfAProject())
            {
                SaveBackUpOfCurrentProjectOpenedSaveData();
            }
        }
    }

    private void SaveBackUpOfCurrentOpenedMapJson()
        {     
            if(!Directory.Exists(Application.persistentDataPath + "/AutoBackup/"))
            {    
                Directory.CreateDirectory(Application.persistentDataPath + "/AutoBackup/");
            }

            if(!GlobalUtilityFunctions.IsPathSafe(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp))
            {
                UINotificationHandler.Instance.SpawnNotification("<color=red>Could not back-up unsafe map path");
                return;
            }

            string fileName = Path.GetFileNameWithoutExtension(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp);
            DateTime currentTime = DateTime.Now;
            string finalFileName = fileName + "_" +  currentTime.ToString("yyyy-MM-dd_HH-mm-ss") + ".json";

            SaveMap.Instance.dataSave(Application.persistentDataPath + "/AutoBackup/" + finalFileName, false, true);
    }



    private void SaveBackUpOfCurrentProjectOpenedSaveData()
    {
            if(!Directory.Exists(Application.persistentDataPath + "/AutoBackup_SaveData/"))
            {    
                Directory.CreateDirectory(Application.persistentDataPath + "/AutoBackup_SaveData/");
            }

            //FINISH! asdas dasd 

            string projectName = ProjectManager.projectData.name;
            DateTime currentTime = DateTime.Now;
            string finalFileName = projectName + "_SaveData_" +  currentTime.ToString("yyyy-MM-dd_HH-mm-ss") + ".json";


        File.Copy(ProjectManager.currentOpenedProjectPath + "/SaveData.json", Application.persistentDataPath + "/AutoBackup_SaveData/" + finalFileName);

    }


}
