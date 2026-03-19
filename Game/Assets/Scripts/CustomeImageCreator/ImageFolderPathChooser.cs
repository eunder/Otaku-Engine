using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

[RequireComponent(typeof(Button))]
public class ImageFolderPathChooser : MonoBehaviour, IPointerDownHandler {

//public ItemEditStateMachine itemEditorStateMachine;
public ItemEditStateMachine itemEditorStateMachine;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", "png, .jpg", false);
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
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
            };

        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", Application.dataPath, true);
        if (paths.Length > 0) {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).LocalPath));
        }
    }
#endif

    private IEnumerator OutputRoutine(string url) {
        var loader = new WWW(url);
        yield return loader;
       
       Debug.Log(url);

       StartCoroutine(itemEditorStateMachine.currentObjectEditing.GetComponent<PosterMeshCreator>().LoadImage(url));
    }
}