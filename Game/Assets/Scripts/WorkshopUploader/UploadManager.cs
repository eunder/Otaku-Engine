using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.IO;
using System;
using System.Threading;
using System.Linq;

public class UploadManager : MonoBehaviour
{

    public TMP_InputField mapName_InputField;
    public TMP_InputField mapDesc_InputField;
    public string imageThunbmailPath = "";
    public Image imageThumbnail;
    public RawImage imageThumbnailRaw;

    public TMP_InputField mapPath_InputField;

    public GameObject upload_Window;
    public GameObject uploadConfirm_Window;
    public GameObject updateConfirm_Window;
    public GameObject successfulUploadNotification_Window;

    public TextMeshProUGUI uploadResult_Text;
    public TMP_InputField uploadConfirm_InputField;
    public TMP_InputField updateConfirm_InputField;

    public ulong itemId = 0; //for updating

    public TMP_Dropdown ageRating_DropDown;

    public Button upload_Button;
    public Button update_Button;

    public TextMeshProUGUI changeLog_Text;
    public TMP_InputField changeLog_InputField;
    public TextMeshProUGUI itemId_Text; 

    public GameObject copyingMapFiles_Canvas;
    public TextMeshProUGUI copyingMapFiles_CurrentFile_Text;

    public long maxUploadSize = 1073741824; // bytes 

    public void RefreshUploadWindow_Upload() //resets the stats when pressed to upload a map
    {
        //reset stats
        mapName_InputField.text = "";
        mapDesc_InputField.text = "";
        imageThunbmailPath = "";
        imageThumbnail.sprite = null;
        uploadResult_Text.text = "";
        mapPath_InputField.text = "";
        ageRating_DropDown.value = 1;
        itemId = 0;
        //show correct components(for uploading)
        upload_Button.gameObject.SetActive(true);
        update_Button.gameObject.SetActive(false);

        changeLog_Text.gameObject.SetActive(false);
        changeLog_InputField.gameObject.SetActive(false);
        itemId_Text.gameObject.SetActive(false);


    }




    public void AttemptUpload()
    {

        if(uploadConfirm_InputField.text.ToUpper().Equals("I AGREE"))
        {
            uploadConfirm_InputField.text = "";

            if(CheckIfTotalUploadSizeIsInvalid())
            {
                Debug.Log("Total Upload Size Too big! Must be 1gb or under");
                return;
            }

            UploadConfirm();
        } 
        else
        {
            uploadConfirm_InputField.text = "";
            Debug.Log("Incorect Confirmation!");
        }
    }

    public void AttemptUpdate()
    {

        if(updateConfirm_InputField.text.ToUpper().Equals("I AGREE"))
        {
            updateConfirm_InputField.text = "";
           
            if(CheckIfTotalUploadSizeIsInvalid())
            {
                GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>Total Upload Size Too big! Must be under 1gb");
                return;
            }

            UploadConfirm();
        } 
        else
        {
            updateConfirm_InputField.text = "";
            Debug.Log("Incorect Confirmation!");
        }
    }


/*
    private CancellationTokenSource _cancellationTokenSource;

    // This method will be triggered by the 'Cancel' button click to cancel the operation
    private void CancelButton_Click(object sender, EventArgs e)
    {
        // Cancel the operation
        _cancellationTokenSource?.Cancel();
    }
*/


    bool CheckIfTotalUploadSizeIsInvalid()
    {
           long totalBytes = 0;

            foreach(string s in zipFilesToUpload)
            {
                if(File.Exists(s))
                {
                    totalBytes += new FileInfo(s).Length;
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



    public List<string> zipFilesToUpload = new List<string>();



    public void UploadConfirm()
    {



        if(zipFilesToUpload.Count <= 0)
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>No map ZIP files found!");
            return;
        }

        if(itemId != 0)
        {
        UpdateLevelToSteamWorkshopAsync(mapName_InputField.text, mapDesc_InputField.text, zipFilesToUpload ,imageThunbmailPath, uploadResult_Text, upload_Window, updateConfirm_Window, successfulUploadNotification_Window, itemId, changeLog_InputField.text, ageRating_DropDown, copyingMapFiles_Canvas, copyingMapFiles_CurrentFile_Text);
        }
        else
        {
        UploadLevelToSteamWorkshopAsync(mapName_InputField.text, mapDesc_InputField.text, zipFilesToUpload ,imageThunbmailPath, uploadResult_Text, upload_Window, uploadConfirm_Window, successfulUploadNotification_Window, ageRating_DropDown, copyingMapFiles_Canvas, copyingMapFiles_CurrentFile_Text);
        }
    }

    public void PressedButtonToTakeScreenshot()
    {
        PlayerObjectInteractionStateMachine playerStateMachine = GameObject.FindObjectOfType<PlayerObjectInteractionStateMachine>();
        playerStateMachine.enabled = true;
        playerStateMachine.transform.gameObject.GetComponent<SimpleSmoothMouseLook>().wheelPickerIsTurnedOn = false;

        playerStateMachine.currentState = playerStateMachine.PlayerObjectInteractionStateWorkshopPicture;
    }


    // NOTE: It seems File.Copy does not copy duplicates
    // NOTE:  await Task.Delay is used here to give time for the menu UI elements to update and show up

    //WHAT THIS DOES: copies the appropriate zip files to the steam workshop_upload folder
    public static async Task PrepareLevelForUpload(TextMeshProUGUI uploadresult, List<string> zipPathsToUpload, GameObject copyingFileCanvas, TextMeshProUGUI copyingFileAdress)
    {
            //PREPARE
            //---------------------
            copyingFileCanvas.SetActive(true);
            await Task.Delay(100);
            //Delete the folder (make space for new content)
            if(System.IO.Directory.Exists(Application.dataPath + "/StreamingAssets/workshop_upload"))
            System.IO.Directory.Delete(Application.dataPath + "/StreamingAssets/workshop_upload", true); 

            //create workshop folder directory if it dosnt exist
            System.IO.Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/workshop_upload"); 


            //FILE PROCESS
            //---------------------






/*
            int totalFiles = zipPathsToUpload.Count;
            int processedFiles = 0;

            // Create a progress object to update the progress bar on the UI thread
            var progress = new Progress<int>(percent =>
            {
                progressBar.Value = percent;
            });
*/

            //asynchornous task lsit
            List<Task> copyTasks = new List<Task>();



            string zipDirectory = Path.GetDirectoryName(zipPathsToUpload.First());
            string parentDirectory = Directory.GetParent(zipDirectory).FullName;

            string projectFile = Path.Combine(parentDirectory, "Project.json");



    if(File.Exists(projectFile))
    {
            //create a "maps" folder inside the workshop folder directory
            System.IO.Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/workshop_upload/maps"); 



            //COPY THE PROJECT.JSON FILE
            if(File.Exists(projectFile))
            {
                    // Add the copy task to the list
                    copyTasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            if(GlobalUtilityFunctions.IsPathSafe(projectFile))
                            {
                                File.Copy(projectFile, Application.dataPath + "/StreamingAssets/workshop_upload/Project.json", true);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }));
            } 
            else
            {
            Debug.Log("Project File not found");
            }

            //COPY THE SAVEDATA_BASE.JSON FILE
            string saveDataBaseFile = Path.Combine(parentDirectory, "SaveData_Base.json");
            if(File.Exists(saveDataBaseFile))
            {
                    // Add the copy task to the list
                    copyTasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            if(GlobalUtilityFunctions.IsPathSafe(saveDataBaseFile))
                            {
                                File.Copy(saveDataBaseFile, Application.dataPath + "/StreamingAssets/workshop_upload/SaveData_Base.json", true);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }));
            } 
            else
            {
            Debug.Log("Project File not found");
            }
    }






            // COPY ZIP FILES TO WORKSHOP FOLDER (inside of the maps directory)
            foreach (var zipFile in zipPathsToUpload)
            {
             //    cancellationToken.ThrowIfCancellationRequested();

                if (File.Exists(zipFile))
                {
                    string fileName = Path.GetFileName(zipFile);
                    string destinationPath;


                    if(File.Exists(projectFile))
                    {
                       destinationPath = Application.dataPath + "/StreamingAssets/workshop_upload/maps/" + fileName;
                    }
                    else
                    {
                       destinationPath = Application.dataPath + "/StreamingAssets/workshop_upload/" + fileName;
                    }


                    // Add the copy task to the list
                    copyTasks.Add(Task.Run(() =>
                    {
                        try
                        {
                        //   await Task.Delay(500); // Simulate some delay for copying DEBUGGING
                       //     processedFiles++;
                     //       int progressPercent = (int)((processedFiles / (float)totalFiles) * 100);
                     //       ((IProgress<int>)progress).Report(progressPercent);
                         

                            if(GlobalUtilityFunctions.IsPathSafe(zipFile))
                            {
                                // Copy the file asynchronously
                                File.Copy(zipFile, destinationPath, true);
                                Console.WriteLine($"Successfully copied {fileName} to {destinationPath}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to copy {zipFile}: {ex.Message}");
                        }
                    }));
                }
                else
                {
                    Debug.Log("File not found: + " + zipFile);
                }
            }

            // Wait for all copy tasks to complete
            await Task.WhenAll(copyTasks);










            //ON FINISHED
            //-------------------
            copyingFileCanvas.SetActive(false);
            
   
   

    }



    private static async Task UploadLevelToSteamWorkshopAsync(string mapName, string mapDesc, List<string> zipPathsToUpload, string imagePath, TextMeshProUGUI uploadresult, GameObject UploadWindow, GameObject UploadCofirmWindow, GameObject SuccessfulUploadNotificationWindow, TMP_Dropdown agerating_dropdown, GameObject copyingFileCanvas, TextMeshProUGUI copyingFileAdress)
    {
        await PrepareLevelForUpload(uploadresult, zipPathsToUpload, copyingFileCanvas, copyingFileAdress);

      
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
        uploadresult.text = "<color=red> FAILED! Make sure you are using a valid file type and file size for the thumbnail!";
        GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>Upload Fail");

        }
        else
        {
            GameObject.Find("AchievementManager").GetComponent<AchievementManager>().UnlockAchivement("ACH_UPLOADTOWORKSHOP");
           // SuccessfulUploadNotificationWindow.SetActive(true);
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=green> Congratulations! <color=white>Map is now on the workshop!", UINotificationHandler.NotificationStateType.uploadedmap);

            UploadWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
            UploadCofirmWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
      
        }

        
        if(result.NeedsWorkshopAgreement)
        {
         UINotificationHandler.Instance.SpawnNotification("<color=red>Workshop Agreement required!");
        }





    }


    private static async Task UpdateLevelToSteamWorkshopAsync(string mapName, string mapDesc, List<string> zipPathsToUpload, string imagePath, TextMeshProUGUI uploadresult, GameObject UploadWindow, GameObject UploadCofirmWindow, GameObject SuccessfulUploadNotificationWindow, ulong itemId, string changeLog, TMP_Dropdown agerating_dropdown, GameObject copyingFileCanvas, TextMeshProUGUI copyingFileAdress)
    {
        await PrepareLevelForUpload(uploadresult, zipPathsToUpload, copyingFileCanvas, copyingFileAdress);



        uploadresult.text = "";
        //read from current file and write to workshop folder

        //folder with content (check if it exists, if not, create it)
        //copy the contents of the current map onto there(or save it there)
        
        //Application.dataPath + "/StreamingAssets/workshop_upload_do_not_touch    (using the straight level file path works instead)

        var result = await new Steamworks.Ugc.Editor(itemId)
                            .WithChangeLog(changeLog)
                            .WithTitle(mapName)
                            .WithDescription(mapDesc)
                            .WithContent(Application.dataPath + "/StreamingAssets/workshop_upload")
                            .WithPreviewFile(imagePath)
                            .WithPublicVisibility()
                            .WithTag(agerating_dropdown.options[agerating_dropdown.value].text)
                            .SubmitAsync(new ProgressClass());


        Debug.Log("Successfuly Updated" + result.Success + "Need");

        if(result.Success == false)
        {
        Debug.Log("FAIL");
            UploadCofirmWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
        uploadresult.text = "<color=red> FAILED! Make sure you are using a valid file type and file size for the thumbnail!";
        GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>Update Fail");

        }
        else
        {
           // SuccessfulUploadNotificationWindow.SetActive(true);
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=green> Congratulations! <color=white>Map is now updated!", UINotificationHandler.NotificationStateType.uploadedmap);

            UploadWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
            UploadCofirmWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
        }

        
        if(result.NeedsWorkshopAgreement)
        {
         UINotificationHandler.Instance.SpawnNotification("<color=red>Workshop Agreement required!");
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


    public void UpdateFields(Steamworks.Ugc.Item itemInfo, bool previewImageAvailable) //for updating, this is called after picking the block map to update
    {
        itemId = itemInfo.Id;
        itemId_Text.text = "Workshop ID: " + itemId.ToString();

        mapPath_InputField.text = "";

        mapName_InputField.text = itemInfo.Title;
        mapDesc_InputField.text = itemInfo.Description;


        //tag management
        var tags = itemInfo.Tags;
    //Debug.Log("TAG LENGTH" + tags.Length);
    if(tags.Length > 0)
    {
    //Debug.Log("TAG[0]" + tags[0]);
        if(tags[0].Equals("safe"))
        {
            ageRating_DropDown.value = 0;
        }
        if(tags[0].Equals("questionable"))
        {
            ageRating_DropDown.value = 1;
        }
        if(tags[0].Equals("mature"))
        {
            ageRating_DropDown.value = 2;
        }
    }
        //preview image
        if(previewImageAvailable)
        {
            imageThunbmailPath = Application.dataPath + "/StreamingAssets/" + "update_picture.png";
            LoadImage();
        }
        else
        {
            imageThunbmailPath = "";
            imageThumbnail.sprite = null;
        }

        //show correct components(for updating)
        upload_Button.gameObject.SetActive(false);
        update_Button.gameObject.SetActive(true);
        changeLog_Text.gameObject.SetActive(true);
        changeLog_InputField.gameObject.SetActive(true);
        itemId_Text.gameObject.SetActive(true);
        
    }

}
