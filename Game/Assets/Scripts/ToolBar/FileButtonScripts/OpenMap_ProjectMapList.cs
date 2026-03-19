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

public class OpenMap_ProjectMapList : MonoBehaviour {

    private static OpenMap_ProjectMapList _instance;
    public static OpenMap_ProjectMapList Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    
    public bool isEditMode = true;
    public TMP_InputField inputFieldToFill;

    public void SelectLocalMap() {
            var extensions = new [] {
                new ExtensionFilter("Map Files", "json", "zip", "otaku"),
            };

        var paths = StandaloneFileBrowser.OpenFilePanel("Select a local map. (make sure its part of the project)", ProjectManager.currentOpenedProjectPath + "/maps",extensions, false);
        if (paths.Length > 0) 
        {            
            inputFieldToFill.text = Path.GetFileName(paths[0]);
            ProjectManager.Instance.SaveProjectData();
        }

    }


   
}