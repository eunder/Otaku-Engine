using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

public class DoorGenerateDisplayInfo : MonoBehaviour
{
    public TextMeshProUGUI tmpMapName;
    public TextMeshProUGUI tmpMapInfo;

    public Renderer thisRenderer;

    public Texture2D emptyPath_tex;
    public Texture2D localPath_tex;
    public Texture2D onlinePath_tex;
    public Texture2D workshopPath_tex;


    SaveAndLoadLevel.LevelData allLoadedObjects;


    public void LoadRoomPreview(string fileToPreview)
    {
            tmpMapName = GameObject.Find("CANVAS_ROOMINFO").transform.GetChild(1).transform.GetComponent<TextMeshProUGUI>();
            tmpMapInfo = GameObject.Find("CANVAS_ROOMINFO").transform.GetChild(2).transform.GetComponent<TextMeshProUGUI>();

        if(fileToPreview.StartsWith("SA:"))
        {
            fileToPreview = Application.dataPath + "/StreamingAssets/" + fileToPreview.Substring(3);
        }

        if(fileToPreview.StartsWith("http") && fileToPreview.EndsWith(".json"))
        {
            tmpMapName.text = "<color=#00FFD0>" + fileToPreview;

            thisRenderer.material.SetTexture("_MainTex", onlinePath_tex);
            //StartCoroutine(GetLevelURLData(fileToPreview));
        }
        else if(fileToPreview.EndsWith(".json")) //local path
        {
            tmpMapName.text = "<color=#FFE300>" + fileToPreview;
            thisRenderer.material.SetTexture("_MainTex", localPath_tex);
        }
        else if(string.IsNullOrEmpty(fileToPreview))
        {
            tmpMapName.text = "EMPTY";
            thisRenderer.material.SetTexture("_MainTex", emptyPath_tex);
        }
        else if(GlobalUtilityFunctions.IsDigitsOnly(fileToPreview)) // workshop path (ID number)
        {
            tmpMapName.text = "<color=blue>" + fileToPreview;
            thisRenderer.material.SetTexture("_MainTex", workshopPath_tex);

        }
        else
        {
            tmpMapName.text = "EMPTY";
            thisRenderer.material.SetTexture("_MainTex", emptyPath_tex);

        }


    }


    void Start() //without this, the door info wont load on scene start...
    {
      if(GetComponent<DoorInfo>())
      LoadRoomPreview(GetComponent<DoorInfo>().pathFileToLoad); 
    }





}
