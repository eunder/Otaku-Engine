using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.IO;
using System;

public class AIWife_MissionSubmitUploadManager : MonoBehaviour
{

    public TMP_InputField mapName_InputField;
    public TMP_InputField mapDesc_InputField;
    public string imageThunbmailPath = "";
    public Image imageThumbnail;
    public RawImage imageThumbnailRaw;

    public TMP_InputField mapPath_InputField;

    public GameObject upload_Window;
    public GameObject uploadConfirm_Window;
    public GameObject successfulUploadNotification_Window;

    public TextMeshProUGUI uploadResult_Text;
    public TMP_InputField uploadConfirm_InputField;

    public string mapPathToUpload;

    public TMP_Dropdown ageRating_DropDown;

    public Button upload_Button;

    public GameObject copyingMapFiles_Canvas;
    public TextMeshProUGUI copyingMapFiles_CurrentFile_Text;

    public long maxUploadSize = 1073741824; // bytes 


    public void AttemptUpload()
    {

        if(uploadConfirm_InputField.text.ToUpper().Equals("I AGREE"))
        {
            uploadConfirm_InputField.text = "";

            if(CheckIfTotalUploadSizeIsInvalid())
            {
                GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>Total Upload Size Too big! Must be under 1gb");
                return;
            }

            UploadConfirm();        } 
        else
        {
            uploadConfirm_InputField.text = "";
            Debug.Log("Incorect Confirmation!");
        }
    }


         bool CheckIfTotalUploadSizeIsInvalid()
        {
           long totalBytes = 0;

            string json = File.ReadAllText(mapPathToUpload);
            SaveAndLoadLevel.LevelData allLoadedObjects = JsonUtility.FromJson<SaveAndLoadLevel.LevelData>(json);

            for(int i = 0; i < allLoadedObjects.allLevelMaterials.Count; i++)
            {
                if(File.Exists(allLoadedObjects.allLevelMaterials[i].materialPath))
                {
                    totalBytes += new FileInfo(allLoadedObjects.allLevelMaterials[i].materialPath).Length;
                }
            }
            
            for(int i = 0; i < allLoadedObjects.allLevelPosters.Count; i++)
            {
                if(File.Exists(allLoadedObjects.allLevelPosters[i].imageUrl))
                {
                    totalBytes += new FileInfo(allLoadedObjects.allLevelPosters[i].imageUrl).Length;
                }
            }   
            Debug.Log("totalBytes" + totalBytes);
            if(totalBytes >= maxUploadSize)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }


    public void UploadConfirm()
    {
        UploadLevelToSteamWorkshopAsync(mapName_InputField.text, mapDesc_InputField.text, mapPathToUpload ,imageThunbmailPath, uploadResult_Text, upload_Window, uploadConfirm_Window, successfulUploadNotification_Window, ageRating_DropDown, copyingMapFiles_Canvas, copyingMapFiles_CurrentFile_Text);
    }

    public void PressedButtonToTakeScreenshot()
    {
        PlayerObjectInteractionStateMachine playerStateMachine = GameObject.FindObjectOfType<PlayerObjectInteractionStateMachine>();
        playerStateMachine.enabled = true;
        playerStateMachine.transform.gameObject.GetComponent<SimpleSmoothMouseLook>().wheelPickerIsTurnedOn = false;

        playerStateMachine.currentState = playerStateMachine.PlayerObjectInteractionStateWorkshopPictureMission;
    }



    // NOTE: It seems File.Copy does not copy duplicates
    // NOTE:  await Task.Delay is used here to give time for the menu UI elements to update and show up
    public static async Task PrepareLevelForUpload(TextMeshProUGUI uploadresult, string mapPath, GameObject copyingFileCanvas, TextMeshProUGUI copyingFileAdress)
    {
    copyingFileCanvas.SetActive(true);

            await Task.Delay(100);

            //Delete the folder (make space for new content)
            if(System.IO.Directory.Exists(Application.dataPath + "/StreamingAssets/workshop_upload"))
            System.IO.Directory.Delete(Application.dataPath + "/StreamingAssets/workshop_upload", true); 


            //create workshop folder directory if it dosnt exist
            System.IO.Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/workshop_upload"); 

            
            //get map object data
            string json = File.ReadAllText(mapPath);
            SaveAndLoadLevel.LevelData allLoadedObjects = JsonUtility.FromJson<SaveAndLoadLevel.LevelData>(json);

            //copy global music file
            Debug.Log("Copying global media File");
            if(File.Exists(allLoadedObjects.musicTrackPath))
            {
                    //check for duplicates
                    if(!File.Exists(Application.dataPath + "/StreamingAssets/workshop_upload/" + Path.GetFileName(allLoadedObjects.musicTrackPath)))
                    {
                    copyingFileAdress.text = allLoadedObjects.musicTrackPath;
                    await Task.Delay(20);
                    File.Copy(allLoadedObjects.musicTrackPath, Application.dataPath + "/StreamingAssets/workshop_upload/" + Path.GetFileName(allLoadedObjects.musicTrackPath));
                    allLoadedObjects.musicTrackPath = Path.GetFileName(allLoadedObjects.musicTrackPath);
                    }
                    else
                    {
                    allLoadedObjects.musicTrackPath = Path.GetFileName(allLoadedObjects.musicTrackPath);
                    }
            }
            else if(!allLoadedObjects.musicTrackPath.StartsWith("https"))
            {
                allLoadedObjects.musicTrackPath = "";
            }

            Debug.Log("Copying global media File");
            //copy global media file
            if(File.Exists(allLoadedObjects.globalSkyboxMediaPath))
            {
                    //check for duplicates
                    if(!File.Exists(Application.dataPath + "/StreamingAssets/workshop_upload/" + Path.GetFileName(allLoadedObjects.globalSkyboxMediaPath)))
                    {
                    copyingFileAdress.text = allLoadedObjects.globalSkyboxMediaPath;
                    await Task.Delay(20);
                    File.Copy(allLoadedObjects.globalSkyboxMediaPath, Application.dataPath + "/StreamingAssets/workshop_upload/" + Path.GetFileName(allLoadedObjects.globalSkyboxMediaPath));
                    allLoadedObjects.globalSkyboxMediaPath = Path.GetFileName(allLoadedObjects.globalSkyboxMediaPath);
                    }
                    else
                    {
                    allLoadedObjects.globalSkyboxMediaPath = Path.GetFileName(allLoadedObjects.globalSkyboxMediaPath);
                    }
            }
            else if(!allLoadedObjects.globalSkyboxMediaPath.StartsWith("https"))
            {
                allLoadedObjects.globalSkyboxMediaPath = "";
            }


            Debug.Log("Copying monster media File");
            //copy global media file
            if(File.Exists(allLoadedObjects.AIMonsterMediaPath))
            {
                    //check for duplicates
                    if(!File.Exists(Application.dataPath + "/StreamingAssets/workshop_upload/" + Path.GetFileName(allLoadedObjects.AIMonsterMediaPath)))
                    {
                    copyingFileAdress.text = allLoadedObjects.AIMonsterMediaPath;
                    await Task.Delay(20);
                    File.Copy(allLoadedObjects.AIMonsterMediaPath, Application.dataPath + "/StreamingAssets/workshop_upload/" + Path.GetFileName(allLoadedObjects.AIMonsterMediaPath));
                    allLoadedObjects.AIMonsterMediaPath = Path.GetFileName(allLoadedObjects.AIMonsterMediaPath);
                    }
                    else
                    {
                    allLoadedObjects.AIMonsterMediaPath = Path.GetFileName(allLoadedObjects.AIMonsterMediaPath);
                    }
            }
            else if(!allLoadedObjects.AIMonsterMediaPath.StartsWith("https"))
            {
                allLoadedObjects.AIMonsterMediaPath = "";
            }




            Debug.Log("Copying custome materials media File");

            //copy the custom material files
            for(int i = 0; i < allLoadedObjects.allLevelMaterials.Count; i++)
            {
                if(File.Exists(allLoadedObjects.allLevelMaterials[i].materialPath))
                {
                    //check for duplicates
                    if(!File.Exists(Application.dataPath + "/StreamingAssets/workshop_upload/" + Path.GetFileName(allLoadedObjects.allLevelMaterials[i].materialPath)))
                    {
                    copyingFileAdress.text = allLoadedObjects.allLevelMaterials[i].materialPath;
                    await Task.Delay(20);
                    File.Copy(allLoadedObjects.allLevelMaterials[i].materialPath, Application.dataPath + "/StreamingAssets/workshop_upload/" + Path.GetFileName(allLoadedObjects.allLevelMaterials[i].materialPath));
                    allLoadedObjects.allLevelMaterials[i].materialPath = Path.GetFileName(allLoadedObjects.allLevelMaterials[i].materialPath);
                    }
                    else
                    {
                    allLoadedObjects.allLevelMaterials[i].materialPath = Path.GetFileName(allLoadedObjects.allLevelMaterials[i].materialPath);
                    }
                }
                else if(!allLoadedObjects.allLevelMaterials[i].materialPath.StartsWith("https"))
                {
                    allLoadedObjects.allLevelMaterials[i].materialPath = "";
                }
            }

            Debug.Log("Copying custome poster file");

            //copy the poster files   . This one is messy. because i have not implement the "poster media pool list"
            for(int i = 0; i < allLoadedObjects.allLevelPosters.Count; i++)
            {
                if(File.Exists(allLoadedObjects.allLevelPosters[i].imageUrl))
                {
                    //check for duplicates
                    if(!File.Exists(Application.dataPath + "/StreamingAssets/workshop_upload/" + Path.GetFileName(allLoadedObjects.allLevelPosters[i].imageUrl)))
                    {
                    copyingFileAdress.text = allLoadedObjects.allLevelPosters[i].imageUrl;
                    await Task.Delay(20);
                    File.Copy(allLoadedObjects.allLevelPosters[i].imageUrl, Application.dataPath + "/StreamingAssets/workshop_upload/" + Path.GetFileName(allLoadedObjects.allLevelPosters[i].imageUrl));
                    allLoadedObjects.allLevelPosters[i].imageUrl = Path.GetFileName(allLoadedObjects.allLevelPosters[i].imageUrl);
                    }

                    else if(new Uri(allLoadedObjects.allLevelPosters[i].imageUrl).IsFile) //without this, duplicate posters will show the full saved url path
                    {
                        allLoadedObjects.allLevelPosters[i].imageUrl = Path.GetFileName(allLoadedObjects.allLevelPosters[i].imageUrl);
                    }
                }
                else if(!allLoadedObjects.allLevelPosters[i].imageUrl.StartsWith("https"))
                {
                    allLoadedObjects.allLevelPosters[i].imageUrl = "";
                }
            }


            //clear any door url local directory information
            for(int i = 0; i < allLoadedObjects.allLevelDoors.Count; i++)
            {
                if(File.Exists(allLoadedObjects.allLevelDoors[i].doorUrl))
                {
                    allLoadedObjects.allLevelDoors[i].doorUrl = "";
                }
                else if(GlobalUtilityFunctions.IsDigitsOnly(allLoadedObjects.allLevelDoors[i].doorUrl)) //for doors with workshop
                {

                }
                else if(!allLoadedObjects.allLevelDoors[i].doorUrl.StartsWith("https"))
                {
                    allLoadedObjects.allLevelDoors[i].doorUrl = "";
                }
            }






        //change the block material url data (if it is a local file, then only have the end file path name)
        for(int i = 0; i < allLoadedObjects.allLevelBlocks.Count; i++)
        {
            if(File.Exists(allLoadedObjects.allLevelBlocks[i].materialName_y))
            {
                allLoadedObjects.allLevelBlocks[i].materialName_y = Path.GetFileName(allLoadedObjects.allLevelBlocks[i].materialName_y);
            }
            else if(!allLoadedObjects.allLevelBlocks[i].materialName_y.StartsWith("https"))
            {
                allLoadedObjects.allLevelBlocks[i].materialName_y = "";
            }

            if(File.Exists(allLoadedObjects.allLevelBlocks[i].materialName_yneg))
            {
                allLoadedObjects.allLevelBlocks[i].materialName_yneg = Path.GetFileName(allLoadedObjects.allLevelBlocks[i].materialName_yneg);
            }
            else if(!allLoadedObjects.allLevelBlocks[i].materialName_yneg.StartsWith("https"))
            {
                allLoadedObjects.allLevelBlocks[i].materialName_yneg = "";
            }

            if(File.Exists(allLoadedObjects.allLevelBlocks[i].materialName_x))
            {
                allLoadedObjects.allLevelBlocks[i].materialName_x = Path.GetFileName(allLoadedObjects.allLevelBlocks[i].materialName_x);
            }
            else if(!allLoadedObjects.allLevelBlocks[i].materialName_x.StartsWith("https"))
            {
                allLoadedObjects.allLevelBlocks[i].materialName_x = "";
            }

            if(File.Exists(allLoadedObjects.allLevelBlocks[i].materialName_xneg))
            {
                allLoadedObjects.allLevelBlocks[i].materialName_xneg = Path.GetFileName(allLoadedObjects.allLevelBlocks[i].materialName_xneg);
            }
            else if(!allLoadedObjects.allLevelBlocks[i].materialName_xneg.StartsWith("https"))
            {
                allLoadedObjects.allLevelBlocks[i].materialName_xneg = "";
            }


            if(File.Exists(allLoadedObjects.allLevelBlocks[i].materialName_z))
            {
                allLoadedObjects.allLevelBlocks[i].materialName_z = Path.GetFileName(allLoadedObjects.allLevelBlocks[i].materialName_z);
            }
            else if(!allLoadedObjects.allLevelBlocks[i].materialName_z.StartsWith("https"))
            {
                allLoadedObjects.allLevelBlocks[i].materialName_z = "";
            }

            if(File.Exists(allLoadedObjects.allLevelBlocks[i].materialName_zneg))
            {
                allLoadedObjects.allLevelBlocks[i].materialName_zneg = Path.GetFileName(allLoadedObjects.allLevelBlocks[i].materialName_zneg);
            }
            else if(!allLoadedObjects.allLevelBlocks[i].materialName_zneg.StartsWith("https"))
            {
                allLoadedObjects.allLevelBlocks[i].materialName_zneg = "";
            }
        }


            //save the map file with the modified url paths onto the workshop upload  folder
            string toJson = JsonUtility.ToJson(allLoadedObjects);
            File.WriteAllText(Application.dataPath + "/StreamingAssets/workshop_upload/" + Path.GetFileName(mapPath), toJson);


            copyingFileCanvas.SetActive(false);
            
            await Task.Yield();


    }


    private static async Task UploadLevelToSteamWorkshopAsync(string mapName, string mapDesc, string mapPath, string imagePath, TextMeshProUGUI uploadresult, GameObject UploadWindow, GameObject UploadCofirmWindow, GameObject SuccessfulUploadNotificationWindow, TMP_Dropdown agerating_dropdown, GameObject copyingFileCanvas, TextMeshProUGUI copyingFileAdress)
    {

        await PrepareLevelForUpload(uploadresult, mapPath, copyingFileCanvas, copyingFileAdress);

        uploadresult.text = "";
        //read from current file and write to workshop folder

        //folder with content (check if it exists, if not, create it)
        //copy the contents of the current map onto there(or save it there)
        
        //Application.dataPath + "/StreamingAssets/workshop_upload_do_not_touch    (using the straight level file path works instead)

        var result = await Steamworks.Ugc.Editor.NewCommunityFile
                            .WithTitle(mapName)
                            .WithDescription(mapDesc)
                            .WithContent(Application.dataPath + "/StreamingAssets/workshop_upload")
                            .WithPreviewFile(imagePath)
                            .WithPublicVisibility()
                            .WithTag(agerating_dropdown.options[agerating_dropdown.value].text)
                            .SubmitAsync(new ProgressClass());


        Debug.Log("Successfuly Uploaded" + result.Success + "Need");

        if(result.Success == false)
        {
        Debug.Log("FAIL");
        UploadCofirmWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
        uploadresult.text = "<color=red> FAILED! Make sure you are using a valid file type and that the total upload size(including the thumbnail) is not over 4mb!";
        GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>Upload Fail");


        }
        else
        {
            GameObject.Find("AchievementManager").GetComponent<AchievementManager>().UnlockAchivement("ACH_SHOWOFF");
           // SuccessfulUploadNotificationWindow.SetActive(true);
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=green> Congratulations! <color=white>Map is now on the workshop!", UINotificationHandler.NotificationStateType.uploadedmap);

            UploadWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
            UploadCofirmWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();

            //MISSION COMPLETE statements
            
            GameObject.Find("AIWifeLogic").GetComponent<AIWife_MissionEventManager>().MissionComplete();

            Steamworks.SteamUserStats.AddStat("missions_completed", 1);
            Steamworks.SteamUserStats.IndicateAchievementProgress("ACH_MISSIONS", Steamworks.SteamUserStats.GetStatInt("missions_completed"), 10);

        }

    }

        public void LoadImage()
        {
            StartCoroutine(LoadImageLocalAssignToPreviewImage());
        }



    Texture2D imageToTestFrom;
  //http image loading coroutines
 public IEnumerator LoadImageLocalAssignToPreviewImage()
    {
        byte[] imgByte;
        imgByte = File.ReadAllBytes(imageThunbmailPath);
        imageToTestFrom = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        imageToTestFrom.LoadImage(imgByte);
        imageToTestFrom.wrapMode = TextureWrapMode.Repeat;
      //  yield return new WaitForSeconds(1);

        //imageThumbnailRaw.texture = imageToTestFrom;
        Sprite mySprite = Sprite.Create(imageToTestFrom, new Rect(0.0f, 0.0f, imageToTestFrom.width, imageToTestFrom.height), new Vector2(0.5f, 0.5f), 100.0f);
      //  yield return new WaitForSeconds(1);

        imageThumbnail.sprite = mySprite;
        imageThumbnail.preserveAspect = true;
        yield return null;

    }


    //upload process events...
    public void PlayerCloseWindowDialogue()
    {
        GameObject.Find("AIWifeLogic").GetComponent<AIWife_MissionEventManager>().PlayerCloseWindowDialogue();
    }


}
