using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class WorkShopMapInfo_ToolbarWindow : MonoBehaviour
{
    public GameObject workshopMapInfoWindow;

    public TextMeshProUGUI mapName_Text;
    public Image imageThumbnail;

    public Button voteUp_Button;
    public Button voteDown_Button;
    public TextMeshProUGUI mapCommentCount_Text;
    public TextMeshProUGUI mapDateCreated_Text;
    public TextMeshProUGUI mapCreatedBy_Text;

    public Button favorite_Button;

    // Start is called before the first frame update
    void Start()
    {
        if(!string.IsNullOrEmpty(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp))
        {
        if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Contains("Steam/steamapps/workshop/content"))
        {
            workshopMapInfoWindow.SetActive(true);
            tester3();
        }
        else
        {
            workshopMapInfoWindow.SetActive(false);
        }
        }
    

    }

        public async void tester3()
        {
            await GetMapInfo();
        }



        public async Task GetMapInfo()
    {
        string itemId = GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Split('\\')[GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Split('\\').Length - 2]; //grab folder name(should be item id)

	    var itemInfo = await Steamworks.Ugc.Item.GetAsync( ulong.Parse(itemId) );
        Debug.Log(itemInfo?.PreviewImageUrl);

        if(itemInfo.HasValue)
        {
        mapName_Text.text = itemInfo?.Title;
        mapCommentCount_Text.text = itemInfo?.NumComments.ToString();
        mapDateCreated_Text.text = itemInfo?.Created.ToShortDateString();
        mapCreatedBy_Text.text = itemInfo?.Owner.Name;
        StartCoroutine(LoadImageURL(itemInfo?.PreviewImageUrl));
        }

       // var userVote = itemInfo?.GetUserVote();

        //current user vote

       // if(userVote?.VotedDown == false)
       // {

       // } //CANT GET IT TO WORK, NO EXAMPLES FOUND ONLINE
    }

    public async void UpVoteMap()
    {
        string itemId = GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Split('\\')[GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Split('\\').Length - 2]; //grab folder name(should be item id)
	    var itemInfo = await Steamworks.Ugc.Item.GetAsync( ulong.Parse(itemId) );

        var result = await itemInfo?.Vote(true);

        if(result.HasValue)
        {
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=green> Map Voted Up", UINotificationHandler.NotificationStateType.ping);

            voteUp_Button.interactable = false;
            voteDown_Button.interactable = true;
        }
    }

    public async void DownVoteMap()
    {
        string itemId = GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Split('\\')[GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Split('\\').Length - 2]; //grab folder name(should be item id)
	    var itemInfo = await Steamworks.Ugc.Item.GetAsync( ulong.Parse(itemId) );

        var result = await itemInfo?.Vote(false);

        if(result.HasValue)
        {
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red> Map Voted Down", UINotificationHandler.NotificationStateType.ping);

            voteDown_Button.interactable = false;
            voteUp_Button.interactable = true;
        }
    }

    public async void OpenOveryWorkShopItem()
    {
        string itemId = GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Split('\\')[GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Split('\\').Length - 2]; //grab folder name(should be item id)
	    var itemInfo = await Steamworks.Ugc.Item.GetAsync( ulong.Parse(itemId) );

        Steamworks.SteamFriends.OpenWebOverlay(itemInfo?.Url);
    }

    public async void Favorite()
    {
        string itemId = GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Split('\\')[GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Split('\\').Length - 2]; //grab folder name(should be item id)
	    var itemInfo = await Steamworks.Ugc.Item.GetAsync( ulong.Parse(itemId) );

        var result = await itemInfo?.AddFavorite();

        if(result)
        {
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=green> Added to favorites", UINotificationHandler.NotificationStateType.ping);
            favorite_Button.interactable = false;
        }
    }


//THUMBNAIL LOADER
    UnityWebRequest www;
    Texture2D imageToTestFrom;

public IEnumerator LoadImageURL(string preURL)
 {
    www = UnityWebRequestTexture.GetTexture(preURL);

    StartCoroutine(WatForResponse(www));
    yield return www.SendWebRequest();


    //on complete
    if(www.result != UnityWebRequest.Result.Success)
    {
        Debug.Log(www.error);
    }
    else
    {
        imageToTestFrom = ((DownloadHandlerTexture)www.downloadHandler).texture;
        Sprite mySprite = Sprite.Create(imageToTestFrom, new Rect(0.0f, 0.0f, imageToTestFrom.width, imageToTestFrom.height), new Vector2(0.5f, 0.5f), 100.0f);
        imageThumbnail.sprite = mySprite;
        imageThumbnail.preserveAspect = true;
        yield return null;

    }
 }

  IEnumerator WatForResponse(UnityWebRequest request)
    {
           while (!request.isDone)
           {
                yield return null;
           }
    }

}
