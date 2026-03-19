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
public class PosterFootstepSound_FilePickerButton : MonoBehaviour, IPointerDownHandler {

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
                new ExtensionFilter("Image Files", "mp3", "wav", "ogg"),
            };

       
        //DECIDE MOST INTUITIVE DIRECTORY TO OPEN BASED ON FILEPATH
        string fileDirToOpen;
        string filePath = ItemEditStateMachine.Instance.currentObjectEditing.GetComponent<PosterFootstepSound>().footstepSoundPath;
        if(GlobalUtilityFunctions.IsURL(filePath) || string.IsNullOrWhiteSpace(filePath))
        {
            fileDirToOpen = Application.dataPath + "/StreamingAssets/SFX";
        }
        else if(Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            fileDirToOpen = Path.GetDirectoryName(filePath);
        }
        else
        {
            fileDirToOpen = Application.dataPath + "/StreamingAssets/SFX";
        }



        var paths = StandaloneFileBrowser.OpenFilePanel("Choose footstep sound...", fileDirToOpen, extensions, false);
        if (paths.Length > 0) {
            
            ItemEditStateMachine.Instance.posterFootstepPath_InputField.text = new System.Uri(paths[0]).LocalPath;
            ItemEditStateMachine.Instance.OnFootStepAudioInputFieldFinishedEditing(new System.Uri(paths[0]).LocalPath);      
        }
    }
#endif


}