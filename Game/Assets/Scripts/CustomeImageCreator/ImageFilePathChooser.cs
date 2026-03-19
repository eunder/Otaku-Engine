using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using System.IO;

[RequireComponent(typeof(Button))]
public class ImageFilePathChooser : MonoBehaviour, IPointerDownHandler {

//public ItemEditStateMachine itemEditorStateMachine;
public ItemEditStateMachine itemEditorStateMachine;

#if UNITY_WEBGL && !UNITY_EDITOR

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
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg", "webm", "mp4", "mkv", "gif"),
            };



        //DECIDE MOST INTUITIVE DIRECTORY TO OPEN BASED ON FILEPATH
        string fileDirToOpen;
        string filePath = ItemEditStateMachine.Instance.currentObjectEditing.GetComponent<PosterMeshCreator>().urlFilePath;
        if(GlobalUtilityFunctions.IsURL(filePath) || string.IsNullOrWhiteSpace(filePath))
        {
            fileDirToOpen = Application.dataPath + "/StreamingAssets/Textures";
        }
        else if(Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            fileDirToOpen = Path.GetDirectoryName(filePath);
        }
        else
        {
            fileDirToOpen = Application.dataPath + "/StreamingAssets/Textures";
        }

        var paths = StandaloneFileBrowser.OpenFilePanel("Title", fileDirToOpen, extensions, false);
        if (paths.Length > 0) {

            string path = new System.Uri(paths[0]).LocalPath;


            //if path is in streaming assests... saved it with "SA:" instead of the user's direct streaming assests dir
            if(path.Contains("StreamingAssets"))
            {
                string newPath;
                newPath = "SA:" + path.Substring(path.IndexOf("StreamingAssets") + "StreamingAssets".Length);
                path = newPath;
            }

            StartCoroutine(itemEditorStateMachine.currentObjectEditing.GetComponent<PosterMeshCreator>().LoadImage(path));
        }
    }
#endif


}