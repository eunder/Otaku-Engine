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
public class OpenMap : MonoBehaviour {

    public bool isEditMode = true;


    public void OnClick() {
        if(!Directory.Exists(Application.persistentDataPath + "/MyMaps/"))
        {    
            Directory.CreateDirectory(Application.persistentDataPath + "/MyMaps/");
        }

            var extensions = new [] {
                new ExtensionFilter("Map Files", "json", "zip", "otaku"),
            };


        string info;
        if(isEditMode)
        {
            info =  "Open map to Edit...  ||  Zip/json = Map    ||    project.json = Project ||";
        }
        else
        {
            info =  "Open map to Play...  ||  Zip/json = Map    ||    project.json = Project ||";
        }

        var paths = StandaloneFileBrowser.OpenFilePanel(info, Application.persistentDataPath,extensions, false);
        if (paths.Length > 0) 
        {

            EditModeStaticParameter.isInEditMode = isEditMode;
            GlobalUtilityFunctions.OpenMap(paths[0], true);

        }
    }


   
}