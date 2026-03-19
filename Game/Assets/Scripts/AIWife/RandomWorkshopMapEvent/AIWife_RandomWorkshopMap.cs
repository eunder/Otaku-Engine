using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AIWife_RandomWorkshopMap : MonoBehaviour
{
 
    public DialogueBoxStateMachine dialogueBoxStateMachine;

    public DialogueContentObject mapFound_dialogueObject;

    public DialogueContentObject mapStats_dialogueObject;

    public DialogueContentObject mapNotFound_dialogueObject;


    //positioning the door (making it face the player)
    public Camera mainPlayerCam;
    public GameObject teleportDoor;

    //load visuals
    public GameObject loading_Particle;
    public AudioSource loading_AudioSource;

    bool searchForMatureMap = false;

    public void StartSearchForRandomWorkshopMap()
    {
        searchForMatureMap = false;
        DownloadAsy();
    }

        public void StartSearchForRandomWorkshopMap_MATURE()
    {
        searchForMatureMap = true;
        DownloadAsy();
    }

    public static event Action<float> DownloadProg;

     public async void DownloadAsy()
    {

            //query list
        Steamworks.Ugc.Item item = new Steamworks.Ugc.Item();
        Debug.Log("Item ID:" +  item.Id);

        var query = Steamworks.Ugc.Query.Items;
        query = query.SortByVoteScore();

        if(searchForMatureMap)
        {
            query = query.WithTag("Mature");
        }
        else
        {
            query = query.WithTag("Safe");
            query = query.WithTag("Questionable");
            query = query.MatchAnyTag();
        }


        var result = await query.GetPageAsync(1);
        Debug.Log( $"ResultCount: {result?.ResultCount}" );
        Debug.Log( $"TotalCount: {result?.TotalCount}" );

        if(result.HasValue)
        {
        foreach ( Steamworks.Ugc.Item entry in result.Value.Entries )
        {
            if(entry.IsSubscribed == false)
            {
                item = entry;
                break;
            }
        }
        }
        else
        {
            //Add dialogue, wierd... couldnt load anything... (back to menu)
            dialogueBoxStateMachine.AddDialogue(mapNotFound_dialogueObject);

        }
        Debug.Log("Item ID 2:" +  item.Id);

        if(item.Id != 0)
        {

            //start load visuals
            loading_Particle.SetActive(true);
            loading_AudioSource.Play();

           // var itemInfo = await Steamworks.Ugc.Item.GetAsync( Convert.ToUInt64(2948003980) );

            await item.Subscribe();
            WorkshopDownloadProgressBar.Instance.RefreshQueBar();
            await item.DownloadAsync(DownloadProg);


            //stop load visuals
            loading_Particle.SetActive(false);
            loading_AudioSource.Stop();



            //pick from list that comments abour the map stats

            //few comments
            //"check it out, you might find a hidden gem..."

            //10+ comments
            //"dosnt seem too dead"


            //this helps get the directory... for some reason the old Item var dosnt update by itself...
          //  var itemInfo = await Steamworks.Ugc.Item.GetAsync( GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().itemInfo.Id );

          //  GameObject.Find("DownloadingWorkshopItemScreen").transform.GetChild(0).gameObject.SetActive(false);


        
            try
            {
            DirectoryInfo dir = new DirectoryInfo(item.Directory);            
            FileInfo[] info = dir.GetFiles("*.*");
            foreach (FileInfo f in info) 
            {
                //Debug.Log(f.ToString());
                if(f.ToString().Contains(".json"))
                {
                        dialogueBoxStateMachine.AddDialogue(mapFound_dialogueObject);

                        //[unkown map] map has x comments and x subscribers..

                        mapStats_dialogueObject.dialogue = "This map has <color=red>" + item.NumComments.ToString() + " comments.<color=white> And <color=red>" + item.NumSubscriptions.ToString() + " subscribers...";
                        dialogueBoxStateMachine.AddDialogue(mapStats_dialogueObject);

                          //set the map url to load
                          GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = f.ToString();

                          //spawn and set the level blocks and posters

                          //position the door
                                  teleportDoor.transform.rotation = Quaternion.LookRotation(teleportDoor.transform.position - mainPlayerCam.transform.position);
                                  teleportDoor.transform.rotation = Quaternion.Euler(0f, teleportDoor.transform.rotation.eulerAngles.y, 0f);

                }
            }
            }
            catch
            {

               //Add dialogue, wierd... couldnt load anything... (back to menu)
                dialogueBoxStateMachine.AddDialogue(mapNotFound_dialogueObject);


                //Debug.Log("ERROR OCCURED!!!, No internet? file path null?");
                //GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>ERROR while trying to play map, does the file exist?");

            }
        }
        else //if item id = 0, then...
        {
             dialogueBoxStateMachine.AddDialogue(mapNotFound_dialogueObject);
        }
    }


    public AudioSource teleportOut_AudioSource;
    public RawImage blackFadeImage;

    public void ExitHubTroughTeleporterEvent()
    {
    teleportOut_AudioSource.Play();
    blackFadeImage.DOColor(Color.white, 1.35f).OnComplete(() => blackFadeImage.DOColor(Color.black, 0.45f).OnComplete(() => ChangeScenes()));
    }

    public void ChangeScenes()
    {
        SceneManager.LoadScene("PlayerRoom", LoadSceneMode.Single);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }


}
