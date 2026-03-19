using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadMapAdditive : MonoBehaviour
{

    private static LoadMapAdditive _instance;
    public static LoadMapAdditive Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public SaveAndLoadLevel.LevelData fileLevelData;

    public string pathdebug; 
      public  IEnumerator LoadMap_Additive(string path, Transform objToParentTo = null)
    {
        pathdebug = path;
        fileLevelData = new SaveAndLoadLevel.LevelData();

        string json = File.ReadAllText(path);
        fileLevelData = JsonUtility.FromJson<SaveAndLoadLevel.LevelData>(json);


        Debug.Log("Loading path ADDITIVE: " + path);

        yield return SaveAndLoadLevel.Instance.LoadEntities(fileLevelData, 1, "", objToParentTo);




        if(EditModeStaticParameter.isInEditMode)
        {
           UINotificationHandler.Instance.SpawnNotification("Note: any game objects loaded additevly will not be saved on the current map.");
        }
    }

}
