using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

[RequireComponent(typeof(Button))]
public class LevelGlobalMedia_FilePicker : MonoBehaviour, IPointerDownHandler {

public LevelGlobalMediaManager levelGlobalMedia_Manager;

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

        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", extensions, false);
        if (paths.Length > 0) {
            
            levelGlobalMedia_Manager.globalMediaUrl_InputField.text = new System.Uri(paths[0]).LocalPath;
            levelGlobalMedia_Manager.OnUrlFieldFinishEditing();
        }
    }
#endif


}