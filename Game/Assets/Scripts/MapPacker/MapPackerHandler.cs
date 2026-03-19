using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Networking;



public class MapPackerHandler : MonoBehaviour
{
    private static MapPackerHandler _instance;
    public static MapPackerHandler Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

    

    }


    public GameObject progressWindow;

    public Slider progressBar;
    public Button cancelButton;


    public bool canceled;

    public void CancelFileOperation()
    {
        canceled = true;
    }

    string tempFolderName;

    //packs all the media being used in a map
    // for local files: they get copied over to the folder
    // for urls: the texture is saved onto the folder
    public IEnumerator PackAllMediaInMapIntoFolder()
    {
        
        progressWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();

 


        //create temporary folder
        string tempPath = System.IO.Path.GetTempPath();
        tempFolderName = tempPath + "TEMPORARY_MAPMEDIAPACKING";

       //delete the temporary folder if there is one     
        if(Directory.Exists(tempFolderName))
        { 
        Directory.Delete(tempFolderName, true);
        }

        System.IO.Directory.CreateDirectory(tempFolderName);


        //loop through all possible media:
        List<GameObject> mediaObjects = GlobalUtilityFunctions.GetAllMediaGameObjectsInCurrentMap();
        


        //DUPLICATE ERROR PREVENTION MECHANIC

        //The way it works...
        // 1. When there is a duplicate... save the ID AND the URL into a dictionary.
        // 2. Save map normaly
        // 3. Modify the saved json map: loop through each entity that can have duplicate urls... and check if the id's match for any id's in the dictionary mentioned above. If it does... then replace the url
        // 4. Finally, save the json with the new modifed level data!

        //store each filename string looped over to check for duplicates
        List<string> fileNames = new List<string>();
        foreach(GameObject obj in mediaObjects)
        {
            fileNames.AddRange(GlobalUtilityFunctions.GetMediaPathFileNameFromGameObject(obj));
        }



        Dictionary<string, string> ObjectsThatHaveHadTheirPathsChanged_Dic = new Dictionary<string, string>();

        //gets passed onto the zip packer to verfiy the file names of the zip.... it has to be done this way in case of duplicates and GUIDS...
        List<string> fileNamesFinal = new List<string>(fileNames);


        //grab the total count, everytime something is loaded. increment the index. (for loading bar reasons)
        currentEntityIndex = 0;

        totalEntitiesToLoopThrough = mediaObjects.Count;

        foreach (GameObject obj in mediaObjects)
        {
            Debug.Log(obj.name);
            yield return SaveMediaObject(obj, tempFolderName + "/", fileNames, ObjectsThatHaveHadTheirPathsChanged_Dic, fileNamesFinal);
            currentEntityIndex++;
            UpdateProgressBar();

            if (canceled)
            {
                progressWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
                canceled = false;
                yield break;
            }
        }



        if(!GlobalUtilityFunctions.IsPathSafe(Path.GetFileNameWithoutExtension(ZipFileHandler_GlobalStaticInfo.pathToSaveZipAs) + ".json"))
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>json file name was not safe!");
            yield break;
        }
        //add json file name to verification list
        fileNamesFinal.Add(Path.GetFileNameWithoutExtension(ZipFileHandler_GlobalStaticInfo.pathToSaveZipAs) + ".json");


        if(!GlobalUtilityFunctions.IsPathSafe(tempFolderName + "/" + Path.GetFileNameWithoutExtension(ZipFileHandler_GlobalStaticInfo.pathToSaveZipAs) + ".json"))
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>json file path was not safe!");
            yield break;
        }
        //save the json
        SaveMap.Instance.dataSave(tempFolderName + "/" + Path.GetFileNameWithoutExtension(ZipFileHandler_GlobalStaticInfo.pathToSaveZipAs) + ".json", true);



        //DUPLICATE ERROR PREVENTION MECHANIC
        //modify the json!!! this is done in case the level had duplicate urls...
        SaveAndLoadLevel.LevelData modifiedLevelData;
        string json = File.ReadAllText(tempFolderName + "/" + Path.GetFileNameWithoutExtension(ZipFileHandler_GlobalStaticInfo.pathToSaveZipAs) + ".json");
        modifiedLevelData = JsonUtility.FromJson<SaveAndLoadLevel.LevelData>(json);

        //loop through each thing that can have a duplicate url media path...
        for(int i = 0; i < modifiedLevelData.allLevelPosters.Count; i++)
        {
            if (ObjectsThatHaveHadTheirPathsChanged_Dic.TryGetValue(modifiedLevelData.allLevelPosters[i].id, out string newUrl))
            {
                modifiedLevelData.allLevelPosters[i].imageUrl = newUrl;
            }
        }
        for(int i = 0; i < modifiedLevelData.allLevelDialogue.Count; i++)
        {
            if (ObjectsThatHaveHadTheirPathsChanged_Dic.TryGetValue(modifiedLevelData.allLevelDialogue[i].id, out string newUrl))
            {
                modifiedLevelData.allLevelDialogue[i].voicePath = newUrl;
            }
        }
        for(int i = 0; i < modifiedLevelData.allLevelAudioSources.Count; i++)
        {
            if (ObjectsThatHaveHadTheirPathsChanged_Dic.TryGetValue(modifiedLevelData.allLevelAudioSources[i].id, out string newUrl))
            {
                modifiedLevelData.allLevelAudioSources[i].audioPath = newUrl;
            }
        }

        //save the MODIFIED level data        
        string modifiedJson = JsonUtility.ToJson(modifiedLevelData, true);
        File.WriteAllText(tempFolderName + "/" + Path.GetFileNameWithoutExtension(ZipFileHandler_GlobalStaticInfo.pathToSaveZipAs) + ".json", modifiedJson);




        //make sure the temp folder name directory is correct!
        if(!tempFolderName.EndsWith("TEMPORARY_MAPMEDIAPACKING"))
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>Error packing file: incorrect temp folder");
            yield break;
        }






        //pack the media onto a zip
        string[] filesToPack = Directory.GetFiles(tempFolderName);

        ZipFileHandler.Instance.PackZipWithProgressAsync(filesToPack, ZipFileHandler_GlobalStaticInfo.pathToSaveZipAs,ZipFileHandler_GlobalStaticInfo.currentPasswordWorkingWith, fileNamesFinal);



        progressWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
    }

    int totalEntitiesToLoopThrough;
    int currentEntityIndex;
    void UpdateProgressBar()
    {
                float progress = (float)(currentEntityIndex) / totalEntitiesToLoopThrough;
                progressBar.value = progress;
    }

    

    public IEnumerator SaveMediaObject(GameObject obj, string folderPath, List<string> fileNames, Dictionary<string, string> ObjectsThatHaveHadTheirPathsChanged_Dic, List<string> fileNamesFinal)
    {
        //THERE IS SOME BAD LOGIC HERE!!! (FIX LATER) THE ENTIRE FUNCTION WORKS WITH JUST ONE FILE NAME!!! 
        List<string> firstFileName = new List<string>();
        firstFileName = GlobalUtilityFunctions.GetMediaPathFileNameFromGameObject(obj);

        // SHITTY TEMPORARY FIX!!!: only allow these to detect "duplicates"... because of shitty architecture and planning... you have to leave out dialogue content objects
        if(obj.GetComponent<PosterMeshCreator>() || obj.GetComponent<AudioSourceObject>())
        {
            //DUPLICATE SAFETY MECHANIC: the game handles duplicates file names for you automatically
            int duplicateCount = 0;
            //check for duplicates... if there is... then check the count
            foreach(string fName in fileNames)
            {
                if(fName == firstFileName[0])
                {
                    duplicateCount++;
                }
            }

            //if there are duplicates (more than 1 object named the same)... then add a random GUID.
            if(duplicateCount >= 2)
            {   
                //to quickly make sure it isnt a GUID
                if(Path.HasExtension(firstFileName[0]))
                {
                string appenededID = System.Guid.NewGuid().ToString();
                //append number at the end of files with duplicates.... this prevents maps from breaking when the same filename is used...
                firstFileName[0] = Path.GetFileNameWithoutExtension(firstFileName[0]) + "_" + appenededID + Path.GetExtension(firstFileName[0]);
                fileNamesFinal.Add(firstFileName[0]);

                //Important! make sure to save a dictionary of stuff that had their url's changed... so that later a correct modified json file is saved onto the zip!!!
                // have a check for each thing...
                if(obj)
                {
                    if(obj.GetComponent<PosterMeshCreator>())
                    {
                        if (!ObjectsThatHaveHadTheirPathsChanged_Dic.ContainsKey(obj.GetComponent<GeneralObjectInfo>().id))
                        {
                            ObjectsThatHaveHadTheirPathsChanged_Dic.Add(obj.GetComponent<GeneralObjectInfo>().id, firstFileName[0]);
                        }
                    }
            //       if(obj.GetComponent<DialogueContentObject>()) // COMMENTED-OUT: SHITTY TEMP FIX!!!
            //       {
            //           ObjectsThatHaveHadTheirPathsChanged_Dic.Add(obj.GetComponent<GeneralObjectInfo>().id, obj.GetComponent<DialogueContentObject>().voicePath);
            //      }
                    if(obj.GetComponent<AudioSourceObject>())
                    {
                        if (!ObjectsThatHaveHadTheirPathsChanged_Dic.ContainsKey(obj.GetComponent<GeneralObjectInfo>().id))
                        {
                            ObjectsThatHaveHadTheirPathsChanged_Dic.Add(obj.GetComponent<GeneralObjectInfo>().id, firstFileName[0]);
                        }
                    }
                }
                }
            }
        }





        if(obj.GetComponent<LevelGlobalMediaManager>())
        {
            yield return            VisualMediaDownloaderHelper(
            obj.GetComponent<LevelGlobalMediaManager>().isVideo,
            obj.GetComponent<LevelGlobalMediaManager>().urlFilePath,
            obj.GetComponent<PosterGifPlayer>(),
            folderPath + firstFileName[0],
            obj.GetComponent<LevelGlobalMediaManager>().imgByte);
        
        }

        if(obj.GetComponent<MusicHandler>())
        {
            yield return SaveAudioClip(obj.GetComponent<MusicHandler>().musicTrackPath, folderPath+ firstFileName[0]);
        }

        if(obj.GetComponent<PosterMeshCreator>())
        {
            yield return            VisualMediaDownloaderHelper(
            obj.GetComponent<PosterMeshCreator>().isVideo,
            obj.GetComponent<PosterMeshCreator>().urlFilePath,
            obj.GetComponent<PosterGifPlayer>(),
            folderPath + firstFileName[0],
            obj.GetComponent<PosterMeshCreator>().imgByte);


            //pack footstep sound
             yield return SaveAudioClip(obj.GetComponent<PosterFootstepSound>().footstepSoundPath, folderPath+GlobalUtilityFunctions.GetOnlyFileNameFromPath(obj.GetComponent<PosterFootstepSound>().footstepSoundPath));
        }

        if(obj.GetComponent<DialogueContentObject>())
        {
            yield return SaveAudioClip(obj.GetComponent<DialogueContentObject>().voicePath, folderPath + firstFileName[0]);
        }

        if(obj.GetComponent<AudioSourceObject>())
        {
            yield return SaveAudioClip(obj.GetComponent<AudioSourceObject>().audioPath, folderPath + firstFileName[0]);
        }

    }


    IEnumerator VisualMediaDownloaderHelper(bool isVideo,string urlFilePath, PosterGifPlayer gifPlayer, string filePath, byte[] bytes = null)
    {



            if(string.IsNullOrEmpty(urlFilePath) || urlFilePath.Equals("Incorrect File Type"))
            {
                yield break;
            }



            if(isVideo) //the video packing process is a little more complicated because videos are streamed in rather than loaded in completely in maps.
            {
                //check if video is to a path localy
                if(File.Exists(urlFilePath))
                {
                    GlobalUtilityFunctions.CopyFile(urlFilePath, filePath, true);
                }
                //check if video is inside the current unzipped temp dir
                else if(File.Exists(ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory + "/" + urlFilePath))
                {
                    GlobalUtilityFunctions.CopyFile(ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory + "/" + urlFilePath, filePath, true);
                }
                //check if current dir is in workshop folder AND if media is in the current dir map opened dir
                else if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("steamapps\\workshop") && File.Exists(Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath))
                {
                    GlobalUtilityFunctions.CopyFile(Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath, filePath, true);
                }
                else  //if it cant be found anywhere on the computer, or in the current temporary unzipped dir... then expect it to be a url
                {
                    yield return SaveMediaFromURL(urlFilePath ,filePath);
                }
            }
            else
            {
                if(urlFilePath.Contains(".gif"))
                {
                yield return SaveGif(gifPlayer,filePath);
                }
                else
                {
                yield return SaveImage(bytes ,filePath);
                }
            }

    }


    //NOTE!!! THESE MEDIA COPYING/WRITING COROUTINES ARE NOT ASYNCHRONOUS! (implement later maybe)


    // Validate file paths to prevent directory traversal
    private bool IsPathSafe(string filePath)
    {
        string fullPath = Path.GetFullPath(filePath);
        string basePath = Path.GetFullPath(tempFolderName);

        if (fullPath.Contains("..") || fullPath.StartsWith("/") || fullPath.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            return true;   
        }

        return fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase);

    }



  public IEnumerator SaveImage(byte[] bytes, string filePath)
    {
          if (!IsPathSafe(filePath))
        {
            Debug.LogError("Attempted to write to an unsafe path: " + filePath);
            yield break;
        }

        if(bytes != null)
        {
            Debug.Log("SAVING IMAGE: " + filePath);

            if(bytes != null)
            File.WriteAllBytes(filePath, bytes); // Write the bytes to a file
        }

        yield return null;
    }


  public IEnumerator SaveGif(PosterGifPlayer gifPlayer, string filePath)
  {
          if (!IsPathSafe(filePath))
        {
            Debug.LogError("Attempted to write to an unsafe path: " + filePath);
            yield break;
        }

            Debug.Log("SAVING gif: " + filePath);

            if(gifPlayer.gifbytes != null)
            File.WriteAllBytes(filePath, gifPlayer.gifbytes);

            yield return null;
  }


    //cannot use byte data because of the way wav, mp3, ogg, etc. works...

  public IEnumerator SaveAudioClip(string urlFilePath, string filePath)
    {
        if (!IsPathSafe(filePath))
        {
            Debug.LogError("Attempted to write to an unsafe path: " + filePath);
            yield break;
        }

        Debug.Log("SAVING AUDIOCLIP: " + filePath);

                    //check if audio is to a path localy
                    if(File.Exists(urlFilePath))
                    {
                        GlobalUtilityFunctions.CopyFile(urlFilePath, filePath, true);
                    }
                    //check if audio is inside the current unzipped temp dir
                    else if(File.Exists(ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory + "/" + urlFilePath))
                    {
                        GlobalUtilityFunctions.CopyFile(ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory + "/" + urlFilePath, filePath, true);
                    }
                    //check if current dir is in workshop folder AND if media is in the current dir map opened dir
                    else if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("steamapps\\workshop") && File.Exists(Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath))
                    {
                        GlobalUtilityFunctions.CopyFile(Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath, filePath, true);
                    }
                    else  //if it cant be found anywhere on the computer, or in the current temporary unzipped dir... then expect it to be a url
                    {
                        yield return SaveMediaFromURL(urlFilePath ,filePath);
                    }

                    yield return null;

    }

    //also used for saved local and url videos
  public IEnumerator SaveMediaFromURL(string url, string filePath)
    {

        if(string.IsNullOrEmpty(url))
        {
            yield break;
        }
        


        if(url.StartsWith("http") && ConfigMenuUIEvents.Instance.allowURLmedia == false)
        {
            Debug.LogError("url media turned off");
            yield break;
        }


        /*
        if (!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            Debug.LogError("Insecure URL detected: " + url);
            yield break;
        }
*/
        if(!IsPathSafe(filePath))
        {
            Debug.LogError("Attempted to write to an unsafe path: " + filePath);
            yield break;
        }

        if(!string.IsNullOrEmpty(url))
        {
                Debug.Log("SAVING FROM URL: " + url);
                Debug.Log("SAVING FROM URL - TO: " + filePath);


                UnityWebRequest videoDownloadwww = new UnityWebRequest(url);

                videoDownloadwww.downloadHandler = new DownloadHandlerFile(filePath);
                yield return videoDownloadwww.SendWebRequest();

                if (videoDownloadwww.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Failed to download video: " + videoDownloadwww.error);
                    yield break; // Exit the coroutine
                }

        }
                yield return null;
    }



}


