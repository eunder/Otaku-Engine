using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Web;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Networking;
using SWS;
using DG.Tweening;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Globalization;

public class GlobalUtilityFunctions : MonoBehaviour
{

 public static bool IsDigitsOnly(string str)
    {
        foreach (char c in str)
        {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }



  public static string GetPathLastName(string filepathName, bool removeExtension = true)
    {
                string fileName = filepathName;

                if(filepathName.Contains("/"))
                {
                fileName = filepathName.Split('/')[filepathName.Split('/').Length - 1]; //for web files
                }
                if(filepathName.Contains("\\"))
                {
                fileName = filepathName.Split('\\')[filepathName.Split('\\').Length - 1]; // for local files
                }
  

                fileName = fileName.ToLower();

                fileName = HttpUtility.UrlDecode(fileName);

                //removes everything after the first '.' char
                if(removeExtension)
                {
                    int index = fileName.IndexOf(".");
                    if (index >= 0)
                    {
                        fileName = fileName.Substring(0, index); // or index + 1 to keep slash
                    }
                }

                return fileName;
    }

    public static bool IsCurrentMapForeign() // is current map a template, workshop, or online map...
    {
        bool isMapForeign = true;

        if(!string.IsNullOrEmpty(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp))
        {
            if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("steamapps") )
            {
                isMapForeign = true;
            }
            else if(File.Exists(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp))
            {
                isMapForeign = false;
            }
            else
            {
                isMapForeign = true;
            }
        }

        return isMapForeign;
    }


    public static Vector3 CalculateAverageVectorPositionFromListOfGameObjects(List<GameObject> list)
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;

        foreach(GameObject obj in list)
        {
            x += obj.transform.transform.position.x;
            y += obj.transform.transform.position.y;
            z += obj.transform.transform.position.z;
        }
        
        return new Vector3(x / list.Count, y / list.Count, z / list.Count);
    }

    public static Vector3 CalculateAverageVectorPositionFromListOfVector3(List<Vector3> list)
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;

        foreach(Vector3 vec3 in list)
        {
            
            x += vec3.x;
            y += vec3.y;
            z += vec3.z;

        }
        
        return new Vector3(x / list.Count, y / list.Count, z / list.Count);
    }


    public static string UrlChecker_AudioFormat(string url)
    {
        if(!GlobalUtilityFunctions.IsPathSafe(url))
        {
            return "Incorrect Path type"; 
        }

        string originalPath = url;
        string path = url;

        int firstExtensionIndex = 0;

        if(path.IndexOf(".mp3") != -1)
        {
            firstExtensionIndex = path.IndexOf(".mp3");
            path = path.Substring(0, firstExtensionIndex + 4);
        }
        if(path.IndexOf(".wav") != -1)
        {
            firstExtensionIndex = path.IndexOf(".wav");
            path = path.Substring(0, firstExtensionIndex + 4);
        }
        if(path.IndexOf(".ogg") != -1)
        {
            firstExtensionIndex = path.IndexOf(".ogg");
            path = path.Substring(0, firstExtensionIndex + 4);
        }

        if(path.EndsWith(".mp3") || path.EndsWith(".wav") || path.EndsWith(".ogg") 
        || path.Contains("..") || path.StartsWith("/") || path.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
                    return path;
        }
        else
        {
                    return "Incorrect File Type";
        }
    }





    public static string UrlChecker_PictureFormat(string url )
    {
        if(!GlobalUtilityFunctions.IsPathSafe(url))
        {
            return "Incorrect Path type"; 
        }

        string originalPath = url;
        string path = url;

        int firstExtensionIndex = 0;


        if(path.IndexOf(".png") != -1)
        {
            firstExtensionIndex = path.IndexOf(".png");
            path = path.Substring(0, firstExtensionIndex + 4);
        }
        if(path.IndexOf(".jpg") != -1)
        {
            firstExtensionIndex = path.IndexOf(".jpg");
            path = path.Substring(0, firstExtensionIndex + 4);
        }
        if(path.IndexOf(".mp4") != -1)
        {
            firstExtensionIndex = path.IndexOf(".mp4");
            path = path.Substring(0, firstExtensionIndex + 4);
        }
        if(path.IndexOf(".webm") != -1)
        {
            firstExtensionIndex = path.IndexOf(".webm");
            path = path.Substring(0, firstExtensionIndex + 5);
        }
        if(path.IndexOf(".gif") != -1)
        {
            firstExtensionIndex = path.IndexOf(".gif");
            path = path.Substring(0, firstExtensionIndex + 4);
        }

        if(path.IndexOf(".mkv") != -1)
        {
            firstExtensionIndex = path.IndexOf(".mkv");
            path = path.Substring(0, firstExtensionIndex + 4);
        }

        if(path.EndsWith(".jpg") || path.EndsWith(".jpeg") || path.EndsWith(".png") || path.EndsWith(".PNG") || path.EndsWith(".mp4") || path.EndsWith(".webm") || path.EndsWith(".mkv") || path.EndsWith(".gif")
         || path.Contains("..") || path.StartsWith("/") || path.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
                    return path;
        }
        else
        {
                    return "Incorrect File Type";
        }
    }

    public static string UrlChecker_Map(string url)
    {
        if(!GlobalUtilityFunctions.IsPathSafe(url))
        {
            return "Incorrect Path type"; 
        }

        string path = url;


        if(path.EndsWith(".json") || path.EndsWith(".zip") || path.EndsWith(".otaku")
        || path.Contains("..") || path.StartsWith("/") || path.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
                    return path;
        }
        else
        {
                    return "Incorrect File Type";
        }
    }

/*
    //used to make the game check the unzipped or current map opened dir. (for paths that use just a file name)
    public static string AttemptToGetPathFromPotentialFileName(string path)
    {
                if(File.Exists(path))
                {
                    return path;
                }
                //check if media is inside the current unzipped temp dir
                else if(File.Exists(ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory + "/" + path))
                {
                    return ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory + "/" + path;
                }
                //check if its in the same folder
                else if(File.Exists(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + path))
                {
                    return System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + path;
                }
                else
                {
                    return path;
                }
    }


*/



    public static int SetInputFieldValueFromString(TMP_Dropdown dropdown, string s)
    {
        return  dropdown.options.FindIndex(option => option.text == s);
    }

    public static Vector3 StringToVector3(string s)
    {
        // Remove parentheses and split the string by commas
        s = s.Replace("(", "").Replace(")", "");
        string[] components = s.Split(',');

        // Parse the components and create a new Vector3
        float x = float.Parse(components[0]);
        float y = float.Parse(components[1]);
        float z = float.Parse(components[2]);

        return new Vector3(z, y, x); //NOTE!!! the z and x are switched due to the orientation of the base game world
     }


    public static string CheckIfMapFileExistsInCurrentOpenedProject(string path)
    {
        if(File.Exists(ProjectManager.currentOpenedProjectPath + "/maps/" + path))
        {
            return  ProjectManager.currentOpenedProjectPath + "/maps/" + path;
        }

        //also check to see if a zip or json exists... (in case player only supplied the map name and not the file extension)
        else if(File.Exists(ProjectManager.currentOpenedProjectPath + "/maps/" + path + ".zip"))
        {
            return  ProjectManager.currentOpenedProjectPath + "/maps/" + path + ".zip";
        }
        else if(File.Exists(ProjectManager.currentOpenedProjectPath + "/maps/" + path + ".json"))
        {
            return  ProjectManager.currentOpenedProjectPath + "/maps/" + path + ".json";
        }
        else
        {
            return path;
        }
    }

    public static bool MapFileExistsInCurrentOpenedProject(string path)
    {
        if(File.Exists(ProjectManager.currentOpenedProjectPath + "/maps/" + path))
        {
            return true;
        }

        //also check to see if a zip or json exists... (in case player only supplied the map name and not the file extension)
        else if(File.Exists(ProjectManager.currentOpenedProjectPath + "/maps/" + path + ".zip"))
        {
            return true;
        }
        else if(File.Exists(ProjectManager.currentOpenedProjectPath + "/maps/" + path + ".json"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public static string OpenCertainPathBasedOnIfCurrentMapIsPartOfProject()
    {
        string pathToOpen;
        //if player is in a project... 
        if(ProjectManager.Instance != null && ProjectManager.Instance.IsCurrentMapPartOfAProject())
        {
            pathToOpen = ProjectManager.currentOpenedProjectPath + "/maps/";
        }
        else
        {
            pathToOpen = Application.persistentDataPath + "/MyMaps/";
        }

        return pathToOpen;
    }


    //checks to see which type of adress/path the map media is... then runs the appropriate class to handle the certain type
    public static void OpenMap(string path, bool wasOpenedWithMenu)
    {   
            //check this first... before the path string gets modified by any other function...
            if(!MapFileExistsInCurrentOpenedProject(path))
            {
                ProjectManager.currentOpenedProjectPath = "";
            }    


            path = CheckIfMapFileExistsInCurrentOpenedProject(path);

            Debug.Log("Opening map... path: " + path);

             if(path.EndsWith("Project.json"))
            {
                ProjectManager.Instance.OpenProject(path);
                return;
            }




            if(path.EndsWith(".zip") || path.EndsWith(".otaku"))
            {
                path = UrlChecker_Map(path);
                //zip file manager script
                ZipFileHandler.Instance.AttemptToOpenZipFile(path, "", false, wasOpenedWithMenu);
            }
            else if(path.EndsWith(".json"))
            {
                ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith = "";

                path = UrlChecker_Map(path);
                GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = path;
                SceneManager.LoadScene("PlayerRoom", LoadSceneMode.Single);
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
            }

    }

    public static bool IsJsonFileSafe(string json)
    {
        
        //check for very large JSON payloads
        if (json.Length > 25 * 1024 * 1024) // Max 25MB payload
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>json is too large! Max: 25mb");
            return false;
        }


         try
        {
            JsonUtility.FromJson<SaveAndLoadLevel.LevelData>(json); // Try parsing the JSON into a generic object
        }
        catch (ArgumentException) // If parsing fails, it's invalid
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>json failed to parse");
            return false;
        }


        return true;
    }


    public static bool IsPathSafe(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }


        string fullPath = Path.GetFullPath(path);

        if (fullPath.Contains(".."))
        {
            return false;
        }

        // Optional: Check if the path starts with "/" or "\\" (platform-dependent)
        if(path.StartsWith("/") || path.StartsWith("\\"))
        {
            return false;
        }



        if (string.IsNullOrEmpty(fullPath))
        {
            return false;
        }

        if (fullPath.Length > 260)
        { 
            return false; // Adjust for platform-specific max lengths
        }


        return true;
    }






    public static string GetOnlyFileNameFromPath(string mediaPath)
    {
        if(string.IsNullOrEmpty(mediaPath))
        {
            return "";
        }

        if(!IsPathSafe(mediaPath))
        {
            return "Unsafe Path";
        }

        if(Uri.IsWellFormedUriString(mediaPath, UriKind.Absolute))
        {
            Uri uri = new Uri(mediaPath);
            return uri.Segments[uri.Segments.Length - 1];
        }
        else
        {
             return Path.GetFileName(mediaPath);
        }
    }


    public static GeneralObjectInfo[] GetAllGeneralObjectInfoClassesInMap()
    {
            GeneralObjectInfo[] allCurrentEntitiesInScene = FindObjectsOfType(typeof(GeneralObjectInfo), true) as GeneralObjectInfo[];
            return allCurrentEntitiesInScene;
    }



    public static List<GameObject> GetAllMediaGameObjectsInCurrentMap()
    {
        List<GameObject> mediaObjects = new List<GameObject>();


        // Add global media paths
        mediaObjects.Add(LevelGlobalMediaManager.Instance.gameObject);
        mediaObjects.Add(MusicHandler.Instance.gameObject);



        // Add poster paths
        foreach (PosterMeshCreator poster in FindObjectsOfType(typeof(PosterMeshCreator), true) as PosterMeshCreator[])
        {
            if(poster.GetComponent<GeneralObjectInfo>().wasLoadedAdditive == false)
            {
                mediaObjects.Add(poster.gameObject);
            }
        }

        // Add dialogue paths
        foreach (DialogueContentObject dialogue in FindObjectsOfType(typeof(DialogueContentObject), true) as DialogueContentObject[])
        {
            if(dialogue.GetComponent<GeneralObjectInfo>().wasLoadedAdditive == false)
            {
            mediaObjects.Add(dialogue.gameObject);
            }
        }

        // Add audio source paths
        foreach (AudioSourceObject audioSource in FindObjectsOfType(typeof(AudioSourceObject), true) as AudioSourceObject[])
        {
            if(audioSource.GetComponent<GeneralObjectInfo>().wasLoadedAdditive == false)
            {
                mediaObjects.Add(audioSource.gameObject);
            }
        }

        return mediaObjects;
    }


    public static List<string> GetMediaPathFileNameFromGameObject(GameObject obj)
    {
        List<string> fileNames = new List<string>();

        if(obj.GetComponent<LevelGlobalMediaManager>())
        {
         fileNames.Add(GetOnlyFileNameFromPath(obj.GetComponent<LevelGlobalMediaManager>().urlFilePath));
        }

        if(obj.GetComponent<MusicHandler>())
        {
         fileNames.Add(GetOnlyFileNameFromPath(obj.GetComponent<MusicHandler>().musicTrackPath));
        }

        if(obj.GetComponent<PosterMeshCreator>())
        {
          fileNames.Add(GetOnlyFileNameFromPath(obj.GetComponent<PosterMeshCreator>().urlFilePath));
          fileNames.Add(GetOnlyFileNameFromPath(obj.GetComponent<PosterFootstepSound>().footstepSoundPath));
        }
        if(obj.GetComponent<DialogueContentObject>())
        {
          fileNames.Add(GetOnlyFileNameFromPath(obj.GetComponent<DialogueContentObject>().voicePath));
        }
        if(obj.GetComponent<AudioSourceObject>())
        {
          fileNames.Add(GetOnlyFileNameFromPath(obj.GetComponent<AudioSourceObject>().audioPath));
        }

        return fileNames;
    }



    //used instead of "Path.Copy", to handle exceptions
    public static void CopyFile(string from, string to, bool overwrite = true)
    {
        if(!IsPathSafe(to))
        {
            UINotificationHandler.Instance.SpawnNotification("Attempted to write to an unsafe path: " + to);
            return;
        }

        try
        {
            File.Copy(from, to, overwrite);
        }
        catch (Exception ex)
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>Error Copying File: " + ex.Message);
        }
        
    }



    public static void AssignSplineMovePropertiesFromPathParameters(splineMove splineMoveComponent, PathNode pathEntity)
    {
        //VERY ANNOYINGLY COMPLICATED. assigns the properties by getting the pathContainer class, and then finally getting the PathNode from that pathContrainer reference

        splineMoveComponent.moveToPath =  pathEntity.moveToPath;
        splineMoveComponent.speed = pathEntity.time;
        splineMoveComponent.loopType = (SWS.splineMove.LoopType)System.Enum.Parse( typeof(SWS.splineMove.LoopType), pathEntity.loopType);
        splineMoveComponent.closeLoop = pathEntity.closeLoop;


        splineMoveComponent.pathType = (PathType)System.Enum.Parse( typeof(PathType), pathEntity.pathType);
        splineMoveComponent.waypointRotation = (pathEntity.wayPointRotation) ?  SWS.splineMove.RotationType.all : SWS.splineMove.RotationType.none;


        splineMoveComponent.easeType = (Ease)System.Enum.Parse( typeof(Ease), pathEntity.easeType);
    }



    public static void UpdateChildrenObjectsOfAllObjectsInMap()
    {
        GeneralObjectInfo[] allObjectsInMap = FindObjectsOfType(typeof(GeneralObjectInfo), true) as GeneralObjectInfo[];
        for(int i = 0; i < allObjectsInMap.Length; i++)
        {
            allObjectsInMap[i].UpdateChildrenObjects();
        }
    }

    public static void UpdatePositionsOfAllObjectsInMap()
    {
        GeneralObjectInfo[] allObjectsInMap = FindObjectsOfType(typeof(GeneralObjectInfo), true) as GeneralObjectInfo[];
        for(int i = 0; i < allObjectsInMap.Length; i++)
        {
            allObjectsInMap[i].UpdatePosition();
        }
    }


    public static void MoveStringListElementUp(List<string> list, int index)
    {
        if (index <= 0 || index >= list.Count)
        {
            // Index out of range or already at the top
            return;
        }

        // Swap elements
        string temp = list[index];
        list[index] = list[index - 1];
        list[index - 1] = temp;
    }

    public static void MoveStringListElementDown(List<string> list, int index)
        {
            if (index < 0 || index >= list.Count - 1)
            {
                // Index out of range or already at the bottom
                return;
            }

            // Swap elements
            string temp = list[index];
            list[index] = list[index + 1];
            list[index + 1] = temp;
        }


    //Used for dialogue variable feature
    public static string InsertVariableValuesInText(string text)
    {
        string modifiedString = text;


        //grab anything inside a <>
        string pattern = "<(.*?)>";
        Regex regex = new Regex(pattern);
        MatchCollection matches = regex.Matches(text);

        foreach (Match match in matches)
        {
            // Get the captured value without the "<>" brackets
            string capturedValue = match.Groups[1].Value;

            //if you found a variable that matches the thing inside <>... replace the text with the variable content... <+capturedValue+> -> [variable]
            foreach(GameObject obj in GlobalParameterManager.Instance.ReturnObjectListOfGlobalEntitites())
            {
                if(obj.GetComponent<Note>().note == capturedValue)
                {
                    modifiedString = modifiedString.Replace("<"+capturedValue+">", GlobalParameterManager.Instance.ReturnCertainValueBasedOnVariableType(obj));
                }
            }

            //if you found a variable that matches the thing inside <>... replace the text with the variable content... <+capturedValue+> -> [variable]
           foreach(GameObject obj in GlobalParameterManager.Instance.ReturnObjectListOfLocalVariables())
            {
                if(obj.GetComponent<Note>().note == capturedValue)
                {
                    modifiedString = modifiedString.Replace("<"+capturedValue+">", GlobalParameterManager.Instance.ReturnCertainValueBasedOnVariableType(obj));
                }
            }
        }   

        return modifiedString;
    }


    public static bool IsURL(string path)
    {
        if(path.StartsWith("https://"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public static bool CheckIfPlayerIsEditingInputField()
    {
        TMP_InputField[] allCurrentInputFieldsInScene = FindObjectsOfType<TMP_InputField>();
        for(int i = 0; i < allCurrentInputFieldsInScene.Length; i++)
        {
            if(allCurrentInputFieldsInScene[i].isFocused)
            {
                return true;
            }
        }
        return false;

    }


    //USED TO EVALUATE THE ONCLICK EVENT: 
    public static void EvaluateClickRange(float hitDistance, GameObject objClickedOn)
    {
        foreach(SaveAndLoadLevel.Event ev in objClickedOn.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnClick")
            {
                                //IF THE OnParameter is zero or blank... assume its infinite range
                                if(ev.onParamater == "0" || string.IsNullOrEmpty(ev.onParamater))
                                {
                                    EventActionManager.Instance.TryPlayEvent(objClickedOn, "OnClick");
                                    return;
                                }


                                //RANGE MECHANIC

                                // Split the input string by comma
                                string[] numbersAsStrings = ev.onParamater.Split(',');

                                if(numbersAsStrings.Length == 2) //make sure there are only 2 parsed values
                                {

                                    float parsedMinRange;
                                    float parsedMaxRange;
                                    bool successfulParse1 = float.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(numbersAsStrings[0]), NumberStyles.Any, CultureInfo.InvariantCulture, out parsedMinRange);
                                    bool successfulParse2 = float.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(numbersAsStrings[1]), NumberStyles.Any, CultureInfo.InvariantCulture, out parsedMaxRange);

                                    //if either value fails to parse... do not execute the action
                                    if(successfulParse1 == false || successfulParse2 == false)
                                    {
                                        UINotificationHandler.Instance.SpawnNotification("<color=red>Error: Could not parse value.");
                                    }
                                    else
                                    {

                                        if(hitDistance >= parsedMinRange && hitDistance <= parsedMaxRange)
                                        {
                                            EventActionManager.Instance.TryPlayEvent_Single(ev);
                                        }
                                    }
                                }
                                else
                                {
                                    UINotificationHandler.Instance.SpawnNotification("<color=red>Error: Range failed.");
                                }
            }
        }



        
    }

    //USED TO EVALUATE THE ONCLICK EVENT: 
    public static bool IsWithinClickRange(float hitDistance, GameObject objClickedOn)
    {
        foreach(SaveAndLoadLevel.Event ev in objClickedOn.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnClick")
            {
                bool canContinueLoop = true;

                //check if event already happened (if "happenOnce")
                if(ev.hasTriggered == true && ev.happenOnce == true)
                {
                    canContinueLoop = false;
                }

                if(canContinueLoop)
                {
                                //IF THE OnParameter is zero or blank... assume its infinite range
                                if(ev.onParamater == "0" || string.IsNullOrEmpty(ev.onParamater))
                                {
                                    return true;
                                }


                                //RANGE MECHANIC

                                // Split the input string by comma
                                string[] numbersAsStrings = ev.onParamater.Split(',');

                                if(numbersAsStrings.Length == 2) //make sure there are only 2 parsed values
                                {

                                    float parsedMinRange;
                                    float parsedMaxRange;
                                    bool successfulParse1 = float.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(numbersAsStrings[0]), NumberStyles.Any, CultureInfo.InvariantCulture, out parsedMinRange);
                                    bool successfulParse2 = float.TryParse(GlobalUtilityFunctions.InsertVariableValuesInText(numbersAsStrings[1]), NumberStyles.Any, CultureInfo.InvariantCulture, out parsedMaxRange);

                                    //if either value fails to parse... do not execute the action
                                    if(successfulParse1 == false || successfulParse2 == false)
                                    {
                                        UINotificationHandler.Instance.SpawnNotification("<color=red>Error: Could not parse value.");
                                    }
                                    else
                                    {

                                        if(hitDistance >= parsedMinRange && hitDistance <= parsedMaxRange)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else
                                {
                                    return false;
                                    UINotificationHandler.Instance.SpawnNotification("<color=red>Error: Range failed.");
                                }
                }

                
            }
        }

        return false;
        
    }





    public static int GetTotalNumberOfFacesUsingPosterMaterial(string materialID)
    {
          int count = 0;

        //block faces
        foreach(GameObject block in SaveAndLoadLevel.Instance.allLoadedBlocks)
        {
            if(block.GetComponent<BlockFaceTextureUVProperties>().materialName_y == materialID)
            {
                count++;
            }
            if(block.GetComponent<BlockFaceTextureUVProperties>().materialName_yneg == materialID)
            {
                count++;
            }
            if(block.GetComponent<BlockFaceTextureUVProperties>().materialName_x == materialID)
            {
                count++;
            }
            if(block.GetComponent<BlockFaceTextureUVProperties>().materialName_xneg == materialID)
            {
                count++;
            }
            if(block.GetComponent<BlockFaceTextureUVProperties>().materialName_z == materialID)
            {
                count++;
            }
            if(block.GetComponent<BlockFaceTextureUVProperties>().materialName_zneg == materialID)
            {
                count++;
            }
        }


        //poster faces
        foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
        {
            if(poster.GetComponent<PosterMeshCreator>().urlFilePath == materialID)
            {
                count++;
            }
        }


        return count;
    }

    public static void DuplicateGUIDChecker_Fixer()
    {
        if(EditModeStaticParameter.isInEditMode)
        {        

            HashSet<string> objectNames = new HashSet<string>();

            foreach (GameObject obj in SaveAndLoadLevel.Instance.allLoadedGameObjects)
            {
                if (obj != null)
                {
                    string objectName = obj.GetComponent<GeneralObjectInfo>().id;

                    if (objectNames.Contains(objectName))
                    {
                    UINotificationHandler.Instance.SpawnNotification("<color=red> <size=65%> Fixed: duplicate object ID's found… certain object relationships will be broke: </color>" + objectName);
                    
                    obj.name = System.Guid.NewGuid().ToString();
                    obj.GetComponent<GeneralObjectInfo>().UpdateID();
                    }
                    else
                    {
                        objectNames.Add(objectName);
                    }
                }
            }

        }
    }


    public static bool IsMediaFile(string path)
    {
        string fullPath = Path.GetFullPath(path);

        if(
        fullPath.EndsWith(".png") ||
        fullPath.EndsWith(".PNG") || 
        fullPath.EndsWith(".jpg") ||
        fullPath.EndsWith(".gif") ||
        fullPath.EndsWith(".webm") || 
        fullPath.EndsWith(".mp4") ||
        fullPath.EndsWith(".mkv") ||
        fullPath.EndsWith(".ogg") || 
        fullPath.EndsWith(".wav") ||
        fullPath.EndsWith(".mp3")
        )
        {
            return true;
        }
        else
        {
            return false;
        }
    }







    

    //FILE SAFETY MECHANIC
    //----------------------
    public bool IsFileDataValid (byte[] fileData)
    {





        return true;
    }


    //used BEFORE checking if file is safe... prevent reading bytes from a file thats too large, etc.
    public static bool IsSafeToReadBytes(string filePath)
    {
        if(!IsPathSafe(filePath))
        {
            return false;
        }

        if(IsFileTooLarge(filePath))
        {
            return false;
        }





        return true;
    }


    public static bool IsFileDataSafe(byte[] fileData)
    {
        
        if(!HasValidMagicNumbers(fileData))
        {
            return false;
        }

        return true;
    }



    const long MaxFileSize = 1L * 1024 * 1024 * 1024; // 1GB in bytes
    public static bool IsFileTooLarge(string filePath)
    {
        try
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > MaxFileSize)
            {
                UINotificationHandler.Instance.SpawnNotification("<color=red>file too large (max: 1gb): " + Path.GetFileName(filePath));
                return true; 
            }

            return false;
        }
        catch (Exception ex)
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>FChk: " + ex.Message);
            return false;
        }

    }


      private static readonly List<byte[]> MagicNumbersWhitelist = new List<byte[]>
    {
        new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },    // PNG file (image format)
        new byte[] { 0xFF, 0xD8, 0xFF },    // JPG file (image format)
        new byte[] { 0x47, 0x49, 0x46, 0x38 },    // GIF file (image format)
        new byte[] { 0x1A, 0x45, 0xDF, 0xA3 },   // WebM file (video format)

        //mp4
        new byte[] { 0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70 },  // 24 bytes: ftyp
        new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70 },  // 32 bytes: ftyp
        new byte[] { 0x00, 0x00, 0x00, 0x1C, 0x66, 0x74, 0x79, 0x70 },  // 28 bytes: ftyp
        new byte[] { 0x00, 0x00, 0x00, 0x22, 0x66, 0x74, 0x79, 0x70 },  // 34 bytes: ftyp

        new byte[] { 0x1A, 0x45, 0xDF, 0xA3 },   // MKV file (video format)
        new byte[] { 0x4F, 0x67, 0x67, 0x53 }, // OggS
        new byte[] { 0x49, 0x44, 0x33 }, // ID3 header (for MP3)
        new byte[] { 0xFF, 0xFB}, // alt mp3 magic number...
        new byte[] { 0xFF, 0xF3}, // alt mp3 magic number...
        new byte[] { 0xFF, 0xF2}, // alt mp3 magic number...
        new byte[] { 0x57, 0x41, 0x56, 0x45} // WAV file     
     };

    private static readonly List<byte[]> MagicNumbersBlacklist = new List<byte[]>
    {
        new byte[] { 0x4D, 0x5A }, // Magic number for EXE files (MZ header)
        new byte[] { 0x2E, 0x63, 0x73, 0x76 }, // .JS files (magic number for JavaScript files)
        new byte[] { 0x2E, 0x56, 0x42, 0x53 }, // .VBS files (magic number for Visual Basic Script files)
        new byte[] { 0x43, 0x44, 0x30, 0x30 }, // ISO 9660 standard (disk image)
        new byte[] { 0x50, 0x4B, 0x03, 0x04 }, // ZIP (Common ZIP header)
        new byte[] { 0x52, 0x61, 0x72, 0x21 }, // RAR (Common RAR header)
        new byte[] { 0x75, 0x73, 0x74, 0x61 }, // TAR (Common TAR header)
        new byte[] { 0x25, 0x50, 0x44, 0x46 }, // PDF files (Common PDF header)
        new byte[] { 0x50, 0x4B, 0x03, 0x04 }, // .docx, .xlsx, .pptx (Zip-compressed formats)
        new byte[] { 0x50, 0x4B, 0x03, 0x04 }, // APK files (Zip-compressed format)
        new byte[] { 0x7F, 0x45, 0x4C, 0x46 }, // ELF (Executable header in Linux)
    };

    public static bool HasValidMagicNumbers(byte[] fileData)
    {
        // Check first 512 bytes (or any appropriate number of bytes)
        int lengthToCheck = Math.Min(512, fileData.Length);

        //first. check if the file contains any magic numbers for black listed file types (exit early if it does) return false;
        foreach (var magicNumber in MagicNumbersBlacklist)
        {
            int matchIndex = fileData.AsSpan(0, magicNumber.Length + 1).IndexOf(magicNumber);  //returns -1 if no match found
            if(matchIndex >= 0)
            {
                string magicNumberHex = BitConverter.ToString(magicNumber).Replace("-", " ");
                UINotificationHandler.Instance.SpawnNotification("<color=red>File type not allowed: " + magicNumberHex);
                return false;
            }
        }

        //second. check if the file contains allowed magic number
        foreach (var magicNumber in MagicNumbersWhitelist)
        {
            int matchIndex = fileData.AsSpan(0, magicNumber.Length + 1).IndexOf(magicNumber); //returns -1 if no match found
            if(matchIndex >= 0)
            {
                return true;
            }
        }

        //special check for wav files... because according to wikipedia.. the magic number can be: 52 49 46 46 ?? ?? ?? ?? 57 41 56 45   <--(last four numbers being the important ones!)
        if(fileData[8] == 0x57 && fileData[9] == 0x41 && fileData[10] == 0x56 && fileData[11] == 0x45)
        {
            return true;
        }

        char firstChar = (char)fileData[0];
        //special check for json files... because json files do not have a magic number
        if(firstChar == '{')
        {
            return true;
        }





        UINotificationHandler.Instance.SpawnNotification("<color=red>Supported file type not found!");
        return false;
    }


}
