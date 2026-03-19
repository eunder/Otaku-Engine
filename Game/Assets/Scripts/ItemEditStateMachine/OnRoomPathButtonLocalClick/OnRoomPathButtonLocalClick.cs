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
public class OnRoomPathButtonLocalClick : MonoBehaviour, IPointerDownHandler {

//public ItemEditStateMachine itemEditorStateMachine;
public ItemEditStateMachine itemEditorStateMachine;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", "json", false);
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

            string pathToOpen;


        //if player is in a project... then open the project map dir. if not... then open the persistant data path
        if(ProjectManager.Instance != null && ProjectManager.Instance.IsCurrentMapPartOfAProject())
        {
            pathToOpen = ProjectManager.currentOpenedProjectPath + "/maps/";
        }
        else
        {
            pathToOpen = Application.persistentDataPath;
        }


            var extensions = new [] {
                new ExtensionFilter("Map Files", "json", "zip", "otaku" ),
            };

        var paths = StandaloneFileBrowser.OpenFilePanel("Title", pathToOpen, extensions, false);
        if (paths.Length > 0) {

            if(ProjectManager.Instance != null && ProjectManager.Instance.IsCurrentMapPartOfAProject())
            {
                
                       itemEditorStateMachine.currentObjectEditing.GetComponent<DoorInfo>().pathFileToLoad = Path.GetFileName(new System.Uri(paths[0]).LocalPath);
                       itemEditorStateMachine.currentObjectEditing.GetComponent<DoorGenerateDisplayInfo>().LoadRoomPreview(Path.GetFileName(new System.Uri(paths[0]).LocalPath));

            }
            else
            {
                       itemEditorStateMachine.currentObjectEditing.GetComponent<DoorInfo>().pathFileToLoad = new System.Uri(paths[0]).LocalPath;
                       itemEditorStateMachine.currentObjectEditing.GetComponent<DoorGenerateDisplayInfo>().LoadRoomPreview(new System.Uri(paths[0]).LocalPath);
            }

        }
    }
#endif

}