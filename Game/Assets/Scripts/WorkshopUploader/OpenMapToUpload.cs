using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using System.Linq;

[RequireComponent(typeof(Button))]
public class OpenMapToUpload : MonoBehaviour, IPointerDownHandler {

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", ".json", false);
    }

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(OutputRoutine(url));
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public UploadManager uploadManager;


    public GameObject projectInfoUploadWarningWindow;
    public TextMeshProUGUI projectInfoUploadWarningText;

    private void OnClick() {

            var extensions = new [] {
                new ExtensionFilter("Map Or Project FIles", "zip", "json"),
            };
        

        var paths = StandaloneFileBrowser.OpenFilePanel("Pick a map to upload", Application.persistentDataPath, extensions, false);
      

        if (paths.Length > 0) {

            //update path adress input field
            uploadManager.mapPath_InputField.text = paths[0];

            //if it is a project.json... then proceed to open a notification window that shows the player what will be uploaded 
            if(paths[0].EndsWith("Project.json"))
            {
                string[] zipFiles;

                //check if it is a valid project class      TODO!!! 

                //find all zips in the project's maps folder...

                string projectDirectory = Path.GetDirectoryName(paths[0]);
                string mapsFolderPath = Path.Combine(projectDirectory, "maps");



                if (Directory.Exists(mapsFolderPath))
                {
                    zipFiles = Directory.GetFiles(mapsFolderPath, "*.zip");
                    

                    if(zipFiles.Length <= 0 || zipFiles == null)
                    {
                        UINotificationHandler.Instance.SpawnNotification("<color=red>No map ZIP files found!");
                        return;
                    }

                    //open warning window show what will be uploaded
                    projectInfoUploadWarningWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();

                    //update the warning window text
                    projectInfoUploadWarningText.text = "<br><color=red> Warning Read Carefully!</color> The following files will be uploaded: <color=#47FFDA> <br> <br> * Project.json <br> * SaveData_Base.json <br><br> "
                    + "<color=white>ZipFiles: <br> ----------------- </color> <br>";

                    foreach(string s in zipFiles)
                    {
                        projectInfoUploadWarningText.text += "* " + Path.GetFileName(s) + "<br>";
                    }
                    
                    //transfer this list of zips to uploadmanager
                    uploadManager.zipFilesToUpload.Clear();
                    uploadManager.zipFilesToUpload = zipFiles.ToList();
                }
            }
            else if (paths[0].EndsWith(".zip"))
            {
                uploadManager.zipFilesToUpload.Clear();
                uploadManager.zipFilesToUpload.Add(paths[0]);
            }
            else
            {
                UINotificationHandler.Instance.SpawnNotification("<color=red>No map ZIP files found!");
                uploadManager.zipFilesToUpload.Clear();
            }


        }
    }
#endif

   
}