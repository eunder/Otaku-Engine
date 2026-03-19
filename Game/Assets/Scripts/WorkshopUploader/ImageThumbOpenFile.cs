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

[RequireComponent(typeof(Button))]
public class ImageThumbOpenFile : MonoBehaviour, IPointerDownHandler {

    public UploadManager uploadManager;
    public AIWife_MissionSubmitUploadManager AIWifeMission_uploadManager;

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

    private void OnClick() {
            var extensions = new [] {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg"),
            };

        var paths = StandaloneFileBrowser.OpenFilePanel("Choose a local thumbnail", "", extensions, false);
        if (paths.Length > 0) {

            if(uploadManager != null)
            {
            //send path name to uploadmanager
            uploadManager.imageThunbmailPath = paths[0];
            uploadManager.LoadImage();
            }

            if(AIWifeMission_uploadManager != null)
            {
            //send path name to mission uploadmanager
            AIWifeMission_uploadManager.imageThunbmailPath = paths[0];
            AIWifeMission_uploadManager.LoadImage();
            }
        }
    }
#endif

   
}