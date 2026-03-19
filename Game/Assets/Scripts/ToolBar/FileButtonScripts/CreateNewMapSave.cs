using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class CreateNewMapSave : MonoBehaviour {

    public void OnClick() {
            if(!Directory.Exists(Application.persistentDataPath + "/MyMaps/"))
            {    
                Directory.CreateDirectory(Application.persistentDataPath + "/MyMaps/");
            }

        var path = StandaloneFileBrowser.SaveFilePanel("New Map", Application.persistentDataPath + "/MyMaps/", Path.GetFileNameWithoutExtension(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp), "json");
        if (!string.IsNullOrEmpty(path)) {

            SaveAndLoadLevel.LevelData ppp2 = new SaveAndLoadLevel.LevelData();
            string json = JsonUtility.ToJson(ppp2);
            File.WriteAllText(path, json);



            //load level after creating it...
            GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = path;
            SceneManager.LoadScene("PlayerRoom", LoadSceneMode.Single);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }
    }

}