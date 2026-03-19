using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.SceneManagement;

public class OpenStreamingAssestsDirectory : MonoBehaviour
{

    public void openStreamingAssestsDirectory()
    {
        Cursor.lockState = CursorLockMode.None;
        var paths = StandaloneFileBrowser.OpenFilePanel("Pick a map. (DO NOT OVERWRITE)", Application.dataPath + "/StreamingAssets", "json", false);
        if (paths.Length > 0) {

            GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = paths[0];
            SceneManager.LoadScene("PlayerRoom", LoadSceneMode.Single);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }

    }

}
