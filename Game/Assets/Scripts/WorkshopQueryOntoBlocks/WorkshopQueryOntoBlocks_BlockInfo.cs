using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class WorkshopQueryOntoBlocks_BlockInfo : MonoBehaviour
{

    public TextMeshProUGUI mapName_Text;
    public Image imageThumbnail;
    public string localDirectory;

    public GameObject Unsub_Button;
    public GameObject Downloading_Text;

    public Steamworks.Ugc.Item itemInfo;

    public void AssignBlockInfo(string mapName, Steamworks.Ugc.Item ItemInfo, string previewUrl = "", string dir = "")
    {
        mapName_Text.text = mapName;
        localDirectory = dir;
        itemInfo = ItemInfo;
        StartCoroutine(LoadImageURL(previewUrl));
    }


    public void Unsub_Button_Clicked()
    {
    Unsub();
    }
    public async void Unsub()
    {
        await itemInfo.Unsubscribe();
        UpdateStatus_Icons();
        GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>Unsubcribed to " + itemInfo.Title);
    }

public void UpdateStatus_Icons()
{
    UpdateStatus();
}

public async void UpdateStatus()
{
    await Task.Delay(100);

    if(itemInfo.IsDownloading)
    {
        Downloading_Text.SetActive(true);
    }

    if(itemInfo.IsDownloadPending)
    {
        Downloading_Text.SetActive(true);
    }

    if(itemInfo.IsDownloading == false && itemInfo.IsDownloadPending == false && itemInfo.IsSubscribed == true)
    {
        Unsub_Button.SetActive(true);
        Downloading_Text.SetActive(false);
    }
    else if(itemInfo.IsSubscribed == false)
    {
        Unsub_Button.SetActive(false);
        Downloading_Text.SetActive(false);
    }
}


//THUMBNAIL LOADER
    UnityWebRequest www;
    public Texture2D imageToTestFrom;

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
