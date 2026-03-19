using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{


    public List<GameObject> loadedEntitiesList = new List<GameObject>();


    public void LoadMap()
    {
        GlobalUtilityFunctions.OpenMap(GlobalUtilityFunctions.InsertVariableValuesInText(GetComponent<DoorInfo>().pathFileToLoad), false);
    }

/*
    public IEnumerator LoadContent_Additively()
    {
        yield return SaveAndLoadLevel.Instance.LoadEntities(SaveAndLoadLevel.Instance.GetLevelDataFromMapPath(GetComponent<DoorInfo>().pathFileToLoad));
        yield return SaveAndLoadLevel.Instance.SetHierchies(gameObject, false);
    }

    public IEnumerator LoadContent_Additively_DoorOrigin()
    {
        yield return SaveAndLoadLevel.Instance.LoadEntities(SaveAndLoadLevel.Instance.GetLevelDataFromMapPath(GetComponent<DoorInfo>().pathFileToLoad));
        yield return SaveAndLoadLevel.Instance.SetHierchies(gameObject, true);
    }
*/
}
