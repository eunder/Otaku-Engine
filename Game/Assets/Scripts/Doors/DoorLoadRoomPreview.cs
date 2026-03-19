using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;

public class DoorLoadRoomPreview : MonoBehaviour
{

    SaveAndLoadLevel.LevelData allLoadedObjects;

    public Transform StartOrigin;

    public GameObject roomPreviewSkybox;

    public GameObject currentDoor;
    public string currentFilePath;

    public List<GameObject> blocks = new List<GameObject>();
    public List<GameObject> posters = new List<GameObject>();
    //public List<GameObject> doors = new List<GameObject>();

    public Animator doorInfoCanvas_Anim;
    public Animator doorWorldGroup_Anim;
    public AudioSource doorWorld_audioSource;
    public GameObject doorInfo_CANVAS;
    public TextMeshProUGUI tmpMapInfo;
    public TextMeshProUGUI tmpMapName;


    public void ClickedToLoadPreview(string urlFilePath, GameObject door)
    {
        doorInfo_CANVAS.transform.position = door.transform.Find("Canvas_Positioner").transform.position;
        doorInfo_CANVAS.transform.rotation = door.transform.Find("Canvas_Positioner").transform.rotation;

        if(urlFilePath.StartsWith("SA:"))
        {
            urlFilePath = Application.dataPath + "/StreamingAssets/" + urlFilePath.Substring(3);
        }

        //door.GetComponent<DoorGenerateDisplayInfo>().LoadRoomPreview(urlFilePath);

        if(currentDoor == null || currentFilePath == null)
        {
            currentDoor = door;
            currentFilePath = urlFilePath;
            currentDoor.GetComponent<DoorInfo>().DisableBackground();
            ClearList();
            StartCoroutine(LoadRoomPreview(urlFilePath, door));
        }

        if(currentDoor != door || currentFilePath != urlFilePath)
        {
            currentDoor.GetComponent<DoorInfo>().EnableBackground();
            currentDoor = door;
            currentFilePath = urlFilePath;
            currentDoor.GetComponent<DoorInfo>().DisableBackground();
            ClearList();
            StartCoroutine(LoadRoomPreview(urlFilePath, door));
        }

    }

    private IEnumerator LoadRoomPreview(string urlFilePath, GameObject door)
    {
        bool isWorkshopMap = false;

        workshopPreview_Image.gameObject.SetActive(false);


        //reset the origin so that the blocks get aligned correctly
        transform.position = StartOrigin.position;
        transform.rotation = StartOrigin.rotation;


        //download the file and get the json level data from the file

        if(urlFilePath.StartsWith("http") && urlFilePath.EndsWith(".json")) //url path
        {
            tmpMapName.text = "<color=#00FFD0>" + urlFilePath;

            yield return LoadURL(urlFilePath);
        }
        else if(urlFilePath.EndsWith(".json")) //local path
        {
            tmpMapName.text = "<color=#FFE300>" + urlFilePath;

            yield return LoadLocal(urlFilePath);
        }
        else if(string.IsNullOrEmpty(urlFilePath))
        {
            tmpMapName.text = "EMPTY";

            currentDoor.GetComponent<DoorInfo>().EnableBackground();
            yield break;
        }
        else if(GlobalUtilityFunctions.IsDigitsOnly(urlFilePath))// workshop path (ID number)
        {
            isWorkshopMap = true;
            ClearList();
            Task task = LoadWorkShop(urlFilePath);
            yield return new WaitUntil(() => task.IsCompleted);
        }



        //generate the level

        if(isWorkshopMap == false)
        {
        yield return LoadLevelData();
        }


        //position this gameobject on the door origin
        transform.position = door.transform.position;
        transform.eulerAngles = new Vector3(door.transform.eulerAngles.x, door.transform.eulerAngles.y + 90, door.transform.eulerAngles.z);

        //position the room preview skybox on door origin
        roomPreviewSkybox.transform.position = door.transform.position;


        PlayRoomLoadCanvasAnimation();


    }

    public void PlayRoomLoadCanvasAnimation()
    {
        doorInfoCanvas_Anim.Play("load_room", -1, 0f);
        doorWorldGroup_Anim.Play("worldPreview_generate1", -1, 0f);
        doorWorld_audioSource.Play();
    }

    int imageCount = 0;
    int gifCount = 0;
    int videoCount = 0;

    public void GetMediaCount()
    {
        imageCount = 0;
        gifCount = 0;
        videoCount = 0;

        foreach(SaveAndLoadLevel.PosterData poster in allLoadedObjects.allLevelPosters)
        {
            if(poster.imageUrl.Contains(".jpg") || poster.imageUrl.Contains(".jpeg") || poster.imageUrl.Contains(".png") || poster.imageUrl.Contains(".PNG"))
            {
                imageCount++;
            }
            if(poster.imageUrl.Contains(".gif") || poster.imageUrl.Contains(".GIF"))
            {
                gifCount++;
            }
            if(poster.imageUrl.Contains(".mp4") || poster.imageUrl.Contains(".webm"))
            {
                videoCount++;
            }
        }

        tmpMapInfo.text = "images: " + imageCount + 
                       "\n \n gifs: " + gifCount +
                       "\n \n  videos: " + videoCount;
    }


    IEnumerator LoadURL(string path)
    {
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>" + www.error, UINotificationHandler.NotificationStateType.error);
            currentDoor.GetComponent<DoorInfo>().EnableBackground();
            Debug.Log(www.error);
        }
        else 
        {
            try
            {
            string json = www.downloadHandler.text;
            allLoadedObjects = JsonUtility.FromJson<SaveAndLoadLevel.LevelData>(json);
            GetMediaCount();
            }
            catch
            {
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>Error getting online file!", UINotificationHandler.NotificationStateType.error);
            currentDoor.GetComponent<DoorInfo>().EnableBackground();
            yield break;
            }
        }
    }

    IEnumerator LoadLocal(string path)
    {
            try
            {
                string json = File.ReadAllText(path);
                allLoadedObjects = JsonUtility.FromJson<SaveAndLoadLevel.LevelData>(json);
                GetMediaCount();
            }
            catch
            {
             GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>Error getting local file!", UINotificationHandler.NotificationStateType.error);
            currentDoor.GetComponent<DoorInfo>().EnableBackground();
            yield break;
            }
    }




        public async Task LoadWorkShop(string path) //make it a task
    {
            var item = await Steamworks.SteamUGC.QueryFileAsync(Convert.ToUInt64(path));
            Debug.Log(item?.PreviewImageUrl);

            tmpMapName.text = item?.Title;
            tmpMapInfo.text = "<color=blue> Workshop Map";

            StartCoroutine(LoadImageURL(item?.PreviewImageUrl));
    }

        UnityWebRequest www;
        public Texture2D workshopPreview_Texture2D;
        public Image workshopPreview_Image;


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
            workshopPreview_Image.gameObject.SetActive(true);
            workshopPreview_Texture2D = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite mySprite = Sprite.Create(workshopPreview_Texture2D, new Rect(0.0f, 0.0f, workshopPreview_Texture2D.width, workshopPreview_Texture2D.height), new Vector2(0.5f, 0.5f), 100.0f);
            workshopPreview_Image.sprite = mySprite;
            workshopPreview_Image.preserveAspect = true;
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







    public GameObject blockPrefab;
    public GameObject posterPrefab;

    public Material overdrawBlockMaterial;

    IEnumerator LoadLevelData()
    {

        Debug.Log("Loading Level Blocks...");
        foreach(SaveAndLoadLevel.BlockData block in allLoadedObjects.allLevelBlocks)
        {
            GameObject newBlock = GameObject.Instantiate(blockPrefab, block.blockPos, Quaternion.identity);
            newBlock.transform.GetComponent<Renderer>().material = overdrawBlockMaterial;
            newBlock.transform.GetComponent<Renderer>().shadowCastingMode  = UnityEngine.Rendering.ShadowCastingMode.Off;
            //remove collider
            Destroy(newBlock.transform.GetComponent<MeshCollider>());

            newBlock.transform.parent = gameObject.transform;
            blocks.Add(newBlock);
        }

         //load posters
       for(int i = 0; i < allLoadedObjects.allLevelPosters.Count; i++)
        {
            GameObject newPoster = GameObject.Instantiate(posterPrefab, allLoadedObjects.allLevelPosters[i].posterPos, allLoadedObjects.allLevelPosters[i].posterRot);
            newPoster.GetComponent<PosterMeshCreator_RoomPreview>().box.x = allLoadedObjects.allLevelPosters[i].areaWidth;
            newPoster.GetComponent<PosterMeshCreator_RoomPreview>().box.y = allLoadedObjects.allLevelPosters[i].areaHeight;
            newPoster.GetComponent<PosterBillboard>().useBillboard = allLoadedObjects.allLevelPosters[i].isBillboard;
            newPoster.GetComponent<PosterMeshCreator_RoomPreview>().LoadRoomPreviewInfo();
            newPoster.GetComponent<Renderer>().shadowCastingMode  = UnityEngine.Rendering.ShadowCastingMode.Off;

            newPoster.transform.parent = gameObject.transform;
            posters.Add(newPoster);
        }

/*
     //load doors
        foreach(SaveAndLoadLevel.DoorData door in allLoadedObjects.allLevelDoors)
        {
            GameObject newDoor = GameObject.Instantiate(doorPrefab, door.position, door.rotation);
            newDoor.GetComponent<Renderer>().material = overdrawBlockMaterial;
            newDoor.transform.parent = gameObject.transform;
            doors.Add(newDoor);
        }
*/

        yield return null;

    }

    public void ClearList()
    {
        foreach(GameObject go in blocks)
        {
            Destroy(go);
        }
        blocks.Clear();

        foreach(GameObject go in posters)
        {
            Destroy(go);
        }
        posters.Clear();

/*
        foreach(GameObject go in doors)
        {
            Destroy(go);
        }
        doors.Clear();
*/
    }


}
