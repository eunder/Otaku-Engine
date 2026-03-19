using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class WorkshopQuertOntoBlock_PressedToPlay : MonoBehaviour
{


    public async void PressedToPlay()
    {
    
            if(GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.IsDownloading)
            {
                GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("File Is downloading!");
            }

            if(GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.IsDownloadPending)
            {
                GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("File Is on download que!");
            }


            //PART THAT OPEN THE APPROPRIATE FILE TO PLAY
            if(GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.IsDownloading == false && GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.IsDownloadPending == false && GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.IsSubscribed == true)
            {
                Debug.Log(GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.Directory);
                DirectoryInfo dir = new DirectoryInfo(GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.Directory);            

                FileInfo[] zipFiles = dir.GetFiles("*.zip*");
                FileInfo[] jsonFiles = dir.GetFiles("*.json*");



                if(File.Exists(dir + "Project.json"))
                {
                    EditModeStaticParameter.isInEditMode = false;
                    GlobalUtilityFunctions.OpenMap(dir + "Project.json", true);
                }
                else if(zipFiles.Length > 0)
                {
                    EditModeStaticParameter.isInEditMode = false;
                    GlobalUtilityFunctions.OpenMap(zipFiles[0].ToString(), true);
                }
                else if(jsonFiles.Length > 0)
                {
                    EditModeStaticParameter.isInEditMode = false;
                    GlobalUtilityFunctions.OpenMap(jsonFiles[0].ToString(), true);
                }


            }
            else if(GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.IsSubscribed == false)
            {
            await GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.Subscribe(); // the await method makes sure not to call the next line until this is finished
                  GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.DownloadAsync(OnPlayerHurt);

            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("Subscribed to " + GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.Title);


            WorkshopDownloadProgressBar.Instance.RefreshQueBar();
            }

            GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().UpdateStatus_Icons();

    }
        
    public static event Action<float> OnPlayerHurt;

    public async void DownloadAsy()
    {
            Debug.Log("Is installed:" + GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.IsInstalled );

            WorkshopQuertOntoBlock_PressedToPlay.OnPlayerHurt = delegate (float fl)
            {
                Debug.Log("Download prog: " + fl);
            };


            GameObject.Find("DownloadingWorkshopItemScreen").transform.GetChild(0).gameObject.SetActive(true);

            await GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.Subscribe();
            //warning... DOES NOT SUBSCRIBE, ONLY DOWNLOAD
            await GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.DownloadAsync(OnPlayerHurt);

            Debug.Log("Is installed:" + GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.IsInstalled );

            //this helps get the directory... for some reason the old Item var dosnt update by itself...
            var itemInfo = await Steamworks.Ugc.Item.GetAsync( GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.Id );

            GameObject.Find("DownloadingWorkshopItemScreen").transform.GetChild(0).gameObject.SetActive(false);


            try
            {
            DirectoryInfo dir = new DirectoryInfo(itemInfo?.Directory);            
            FileInfo[] info = dir.GetFiles("*.*");
            foreach (FileInfo f in info) 
            {
                //Debug.Log(f.ToString());
                if(f.ToString().Contains(".json"))
                {
                          GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = f.ToString();
                }
            }
            SceneManager.LoadScene("PlayerRoom", LoadSceneMode.Single);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
            }
            catch
            {
                Debug.Log("ERROR OCCURED!!!, No internet? file path null?");
                GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>ERROR while trying to play map, does the file exist?");

            }
    }


}
