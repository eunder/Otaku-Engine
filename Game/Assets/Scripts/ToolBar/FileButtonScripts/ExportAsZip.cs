using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SFB;
using System.IO;

public class ExportAsZip : MonoBehaviour
{


    public GameObject Window_ZipExportOptions;
    public TMP_InputField password_InputField;
    public TMP_InputField passwordConfirmation_InputField;



        public void OnExportAsZip() 
    {
           if(string.IsNullOrWhiteSpace(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) == false )
           {
            Window_ZipExportOptions.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();
           }
           else
           {
            UINotificationHandler.Instance.SpawnNotification("<color=red>Load a map first.", UINotificationHandler.NotificationStateType.error);
           }
    }

    public void OnPressConfirm()
    {
        if(password_InputField.text == passwordConfirmation_InputField.text)
        {
                    var levelFilePath = StandaloneFileBrowser.SaveFilePanel("Export as zip...", GlobalUtilityFunctions.OpenCertainPathBasedOnIfCurrentMapIsPartOfProject(), Path.GetFileNameWithoutExtension(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp), "zip");


                    //in case player hits "cancel" in file browser
                    if(!string.IsNullOrEmpty(levelFilePath))
                    {
                        ZipFileHandler_GlobalStaticInfo.currentPasswordWorkingWith = passwordConfirmation_InputField.text;
                        Window_ZipExportOptions.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
                        
                        ZipFileHandler_GlobalStaticInfo.pathToSaveZipAs = levelFilePath;
                        ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith = levelFilePath;
                        StartCoroutine(MapPackerHandler.Instance.PackAllMediaInMapIntoFolder());
                    }


        }
        else
        {
                    UINotificationHandler.Instance.SpawnNotification("<color=red>Password Fields Do Not Match!", UINotificationHandler.NotificationStateType.error);
        }

    }

}
