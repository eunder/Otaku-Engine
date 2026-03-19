using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    private static ProjectManager _instance;
    public static ProjectManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

    }

    
    [System.Serializable]
    public class ProjectData
    {
    public string name;
    public string version;
    public string author;
    public string startMap = "InitialMap";
    }


    //make sure these are static... so that they keep their value... this class gets re-started because its part of "MainMenu" scene

    public static string currentOpenedProjectPath;
    public static ProjectData projectData;

    //creates a new project in the "MyProjects" folder located in the persistant data path.
    public void CreateNewProject(string nameOfProject)
    {
        string projectFolderPath = Application.persistentDataPath + "/MyProjects/" + nameOfProject;

        //check if project already exists
        if(System.IO.Directory.Exists(projectFolderPath))
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>That name already exists!");
            return;
        }

        //create the new project folder
        System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/MyProjects/" + nameOfProject);


        //create project.json
        projectData = new ProjectData();
        projectData.name = nameOfProject;
        
        string filePath = projectFolderPath + "/Project.json";
        string jsonContent = JsonUtility.ToJson(projectData, true);
        File.WriteAllText(filePath, jsonContent);


        //create the "Maps" folder
        System.IO.Directory.CreateDirectory(projectFolderPath + "/maps");

        //copy the NewMap.json file to the newly created "maps" folder
        File.Copy(Application.dataPath + "/StreamingAssets/CampaignLevelMedia/NewMap.json", projectFolderPath + "/maps/InitialMap.json");



        UINotificationHandler.Instance.SpawnNotification("New Project Created!");

        EditModeStaticParameter.isInEditMode = true;

        //open project
        OpenProject(filePath);
    }



    //opens the project, (usually the set startMap)
    public void OpenProject(string projectFilePath)
    {  
        //check to see if there is a project.json
        if(!File.Exists(projectFilePath))
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red> no project.json found inside of folder");
            return;
        }

        currentOpenedProjectPath = Path.GetDirectoryName(projectFilePath);
        Debug.Log("currentOpenedProjectPath: " + currentOpenedProjectPath);

        string json = File.ReadAllText(currentOpenedProjectPath + "/" + "Project.json");
        projectData = JsonUtility.FromJson<ProjectData>(json);


        GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = currentOpenedProjectPath + "/maps/" + projectData.startMap; 

        //if the project has no maps... load an empty map...
        if(File.Exists(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) == false)
        {
        GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = Application.dataPath + "/StreamingAssets/CampaignLevelMedia/NewMap.json";
        }

        GlobalUtilityFunctions.OpenMap(GlobalUtilityFunctions.InsertVariableValuesInText(projectData.startMap), false);


    }
    
    public TMP_InputField projectData_name_InputField;
    public TMP_InputField projectData_version_InputField;
    public TMP_InputField projectData_author_InputField;
    public TMP_InputField projectData_startMap_InputField;





    //usually after UI input field has finished being edited
    public void SaveProjectData()
    {
        if(Directory.Exists(currentOpenedProjectPath))
        {
        projectData.name = projectData_name_InputField.text;
        projectData.version = projectData_version_InputField.text;
        projectData.author = projectData_author_InputField.text;
        projectData.startMap = projectData_startMap_InputField.text;

        //write to file...
        string json = JsonUtility.ToJson(projectData, true);
        File.WriteAllText(currentOpenedProjectPath + "/" + "Project.json", json);
        }
    }



    //returns true if the map's parent directory has a "Project.Json"

    string parentDir;
    public bool IsCurrentMapPartOfAProject()
    {
            //if there is a source zip file being worked on... use the path of the source zip file and NOT the "LevelFileToPathToLoadOnStartUp" because it would be in the windows temp folder 
            
            if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp != null)
            {
            if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("TEMPORARY_EXTRACTED_ZIP_"))
            {
                string mapDir = Path.GetDirectoryName(ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith);
                parentDir = Path.GetDirectoryName(mapDir);
            }
            else
            {
                string mapDir = Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp);
                parentDir = Path.GetDirectoryName(mapDir);
            }
            }
            else
            {
                parentDir = null;
                return false;
            }

            //if the file does not exist... exit the function
            if(File.Exists(parentDir + "/Project.json"))
            {
               return true;
            }
            else
            {
                parentDir = null;
                return false; 
            }

    }
    public GameObject GlobalMapSettingsCanvas;

    void Start()
    {
        //the thing that actually sets the "currentOpenedProjectPath"... for both, edit and play mode... without this the game wouldnt know "currentOpenedProjectPath"
        if( IsCurrentMapPartOfAProject())
        {
            //this area is here to make sure if the player opens-up a project from within /maps... the project stuff gets loaded and set!
            currentOpenedProjectPath = parentDir;
            Debug.Log("currentOpenedProjectPath: " + currentOpenedProjectPath);
            string json = File.ReadAllText(currentOpenedProjectPath + "/" + "Project.json");
            projectData = JsonUtility.FromJson<ProjectData>(json);
        }

        //only open global settings for project in edit mode AND IsCurrentMapPartOfAProject()
          if(EditModeStaticParameter.isInEditMode && IsCurrentMapPartOfAProject())
          {
            GlobalMapSettingsCanvas.SetActive(true);

            //needs to be set on start because this class gets restarted as part of "MainMenu" scene
            projectData_name_InputField.text = projectData.name;
            projectData_version_InputField.text = projectData.version;
            projectData_author_InputField.text = projectData.author;
            projectData_startMap_InputField.text = projectData.startMap;
          }
          else
          {
            GlobalMapSettingsCanvas.SetActive(false);
          }
    }


    //DETECTING ALL PROJECT MAPS

    //returns the names of all the valid elements in the "maps" folder
    public List<string> ReturnProjectMapList()
    {
        List<string> projectMaps = new List<string>();

        string[] filesInMapsFolder = Directory.GetFiles(currentOpenedProjectPath + "/maps/");
        //string[] directoriesInMapsFolder = Directory.GetDirectories(directoryPath);

        foreach (string file in filesInMapsFolder)
        {
            //get the file name of the file path
            string fileName = Path.GetFileName(file);

            //only add .otaku files
            if(file.EndsWith(".otaku") || file.EndsWith(".zip")) // || file.EndsWith(".json")
            {
                projectMaps.Add(fileName);
            }
        }

        return projectMaps;
    }
 







}
