using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddToolBarToSceneIfNotPresent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AddToolBarIfNotPresent());
    }

    IEnumerator AddToolBarIfNotPresent(float secondToWaitForCheck = 0.1f)
    {
        yield return new WaitForSeconds(secondToWaitForCheck);
        if(GameObject.Find("CANVAS_TOOLBAR") == null)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }
    }
}
