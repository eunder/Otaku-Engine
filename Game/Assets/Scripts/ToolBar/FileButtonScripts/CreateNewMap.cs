using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class CreateNewMap : MonoBehaviour
{

    public void OnClick()
    {
            //loads the map
            EditModeStaticParameter.isInEditMode = true;
            GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = Application.dataPath + "/StreamingAssets/CampaignLevelMedia/NewMap.json";
            SceneManager.LoadScene("PlayerRoom", LoadSceneMode.Single);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);

    }

}
