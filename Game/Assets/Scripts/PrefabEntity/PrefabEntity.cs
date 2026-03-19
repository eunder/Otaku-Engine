using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabEntity : MonoBehaviour
{

    public bool loadOnStart;
    public string mapToLoad;





    public void LoadPrefab()
    {
        string path = GlobalUtilityFunctions.CheckIfMapFileExistsInCurrentOpenedProject(mapToLoad);

        if(path.EndsWith(".zip") || path.EndsWith(".otaku"))
        {
            //zip file manager script
            ZipFileHandler.Instance.AttemptToOpenZipFile(path, "", true, false, gameObject.transform);
        }
        else if(path.EndsWith(".json"))
        {
            StartCoroutine(LoadMapAdditive.Instance.LoadMap_Additive(GlobalUtilityFunctions.InsertVariableValuesInText(path), gameObject.transform));
        }
    }



    public void ClearPrefab()
    {
        BlockBaseCubePositionFinder_Singleton.Instance.RemoveFromCube();

        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

}
