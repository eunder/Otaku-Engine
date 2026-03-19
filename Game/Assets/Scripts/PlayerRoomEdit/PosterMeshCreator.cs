using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;
using DG.Tweening;

public class PosterMeshCreator : MonoBehaviour
{

    public float width = 1;
    public float height = 1;

   // public Material currentMaterial;


    public Material imageMat;

    public Texture2D videoTemplateTexture;

    private MaterialPropertyBlock propBlock;


    public Vector3[] vertices;


    public string urlFilePath;
    public string urlFilePath_eventModified; //mostly used to check the type of media of the swapped media

    public bool isGUIDreference = true; //used by other classes to quickly check if this poster is a source material or not

    public string urlVideoPathRandom; //prevents errors when a video is picked from a folder randomly, pass this to the video play function
    public Texture2D texture_original;
    public Texture2D imageTemplatePreset;
    List<GameObject> frameObjects = new List<GameObject>();
    Mesh mesh;

    public GameObject currentDepthLayerAssignedTo;

    public delegate void PosterEventHandler();
    public event PosterEventHandler OnSuccesfulMediaChange;

    public void TriggerOnSuccesfulMediaChangeEvent() //mostly used for the media swapping event
    {
        OnSuccesfulMediaChange?.Invoke();
    }


    private void CheckIfPropertyBlocksExist()
    {
        if(propBlock == null)
        {
            propBlock = new MaterialPropertyBlock();
        }

    }


    public Color _color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public void SetMaterialProperties()
    {
        SetMaterialProperties(_color);
    }


    public void SetMaterialProperties(Color color)
    {
        CheckIfPropertyBlocksExist();

        _color = color;

        meshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color",color);
        meshRenderer.SetPropertyBlock(propBlock);
    }

    public void NullfiySetMaterialProperties()
    {
        meshRenderer.GetPropertyBlock(propBlock);
        meshRenderer.SetPropertyBlock(null);
    }



    void Start()
    {
        int tempSeed = (int)System.DateTime.Now.Ticks;
        UnityEngine.Random.InitState(tempSeed);

        CheckIfPropertyBlocksExist();


        if(!mesh)
        {
        BuildPoster();
        }
    }

/*
    public void AssignNewMatInstance()
    {
        //create a new instance of imageMat on poster spawn!
        imageMat = new Material(imageMat);
        imageMat.name = GetComponent<GeneralObjectInfo>().id;
    }
*/

    void OnDestroy()
	{
        if(isGUIDreference == false)
        {
            		Destroy(meshRenderer.sharedMaterial);
        }

        if(SaveAndLoadLevel.Instance.allLoadedPosters.Contains(gameObject))
        SaveAndLoadLevel.Instance.allLoadedPosters.Remove(gameObject);
	}


    public string FetchRandomImageFromFilePath()
    {

    List<string> filesFound = new List<string>();  // list of files found in a folder(that also meet the file type criteria)

    var filters = new string[] { "jpg", "jpeg", "png", "gif", "mp4", "webm"};
        foreach (var filter in filters)
        {
        filesFound.AddRange(Directory.GetFiles(urlFilePath, string.Format("*.{0}", filter)));
        }
    
        int fileCount = filesFound.Count;

        return filesFound[UnityEngine.Random.Range(0, fileCount)];

    }



    //this usually runs at the end of the level load steps
    public void AssignSharedMaterialBasedOnGUID()
    {
        foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
        {
            if(poster.name  == urlFilePath)
            {
                meshRenderer.sharedMaterial = poster.GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial;

                SetMaterialProperties();
                GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();
            }
        }
    }







    public IEnumerator LoadImage(string filePath)
    {


        //for making sure on empty template posters... that the width and height are set based on the saved box dimmensions
        width = box.x;
        height = box.y;
        image = new Vector2(width, height);


        if (string.IsNullOrWhiteSpace(filePath) || filePath.Contains(".."))
        {
                    yield break;
        }


        //do this so that if the poster is in a view mode... the shared materials dont get messed up (due to calling "ResetMaterials" to exit view mode)
        GetComponent<GeneralObjectInfo>().ResetMaterials();


        System.Guid guid;


        //only load media if the poster has no depth layers AND if it is not a GUID(refernce)
        if(GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count <= 0 && System.Guid.TryParse(filePath, out guid) == false)
        {
        isGUIDreference = false;

        urlFilePath = filePath;
        urlFilePath = GlobalUtilityFunctions.UrlChecker_PictureFormat(urlFilePath);

    

        //makes sure to stop playing the video if the media was changed on it
        gameObject.GetComponent<VLC_MediaPlayer>().StopVideoPlayerIfPosterWhereVideoIsPlayingWasChanged(this);


        //messy solution... is modifies "filePath". down below there is a check for "filePath" rather than "urlFilePath"
        if(urlFilePath.StartsWith("SA:"))
        {
            filePath = Application.dataPath + "/StreamingAssets/" + urlFilePath.Substring(3);
        }


        if(GlobalUtilityFunctions.IsURL(urlFilePath) && (urlFilePath.Contains(".jpg") || urlFilePath.Contains(".jpeg") || urlFilePath.Contains(".png") || urlFilePath.Contains(".PNG")))
        {
            //remove gif player if there is one
            if(GetComponent<PosterGifPlayer>())
            {
                Destroy(GetComponent<PosterGifPlayer>());
            }

            if(ConfigMenuUIEvents.Instance.allowURLmedia)
            {
                AssignImageMaterial();
                yield return LoadImageURL();
            }
            else
            {
                UINotificationHandler.Instance.SpawnNotification("<color=red>Url Media disabled!");
            }


        }
        else if(urlFilePath.Contains(".gif"))
        {
            //make sure to not load urls if they are blocked
            if(GlobalUtilityFunctions.IsURL(urlFilePath) && ConfigMenuUIEvents.Instance.allowURLmedia == false)
            {

            }
            else
            {
                AssignImageMaterial();
                urlIsValid = true;
                if(GetComponent<PosterGifPlayer>() == null)
                {
                gameObject.AddComponent<PosterGifPlayer>();
                }
                
                //check if the media is in the same folder (for workshop media)
                if(File.Exists(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath ))
                {
                    yield return StartCoroutine(GetComponent<PosterGifPlayer>().SetGifFromUrlCoroutine(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath));
                }
                else
                {
                    yield return StartCoroutine(GetComponent<PosterGifPlayer>().SetGifFromUrlCoroutine(urlFilePath));
                }
            }

        }
        else if(urlFilePath.Contains(".mp4") || urlFilePath.Contains(".webm") || urlFilePath.Contains(".mkv"))
        {
            //remove gif player if there is one
            if(GetComponent<PosterGifPlayer>())
            {
                Destroy(GetComponent<PosterGifPlayer>());
            }

            urlVideoPathRandom = urlFilePath;
            StartCoroutine(AssignVideoTemplate());
        }
        else
        {
            //remove gif player if there is one
            if(GetComponent<PosterGifPlayer>())
            {
                Destroy(GetComponent<PosterGifPlayer>());
            }

         // filePath.Replace("\\","/");
            Debug.Log(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp)  + "/" + urlFilePath);
          if(File.Exists(filePath))
          {
            AssignImageMaterial();
            yield return LoadImageLocal(urlFilePath);
          }
          //check if the media is in the same folder (for workshop media)
          else if(File.Exists(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath ))
          {
            AssignImageMaterial();
            yield return LoadImageLocal(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath);
          }
          else if(Directory.Exists(urlFilePath)) // WHAT A MESS CLEAN THIS UP LATER
          {
            /*      SCRAPPED RANDOM FILE FROM FOLDER PATH
            AssignImageMaterial();

            //if its a directory string... then fetch a random string path
            string randomFileFromFolder = FetchRandomImageFromFilePath();
            Debug.Log("RandomFileFromDoler:" + randomFileFromFolder);
                                //if the string path contains .gif... use the load gif functions.. else just load local image
                                if(randomFileFromFolder.Contains(".gif"))
                                {
                                    AssignImageMaterial();
                                    urlIsValid = true;
                                    if(GetComponent<PosterGifPlayer>() == null)
                                    {
                                    gameObject.AddComponent<PosterGifPlayer>();
                                    }
                                    yield return StartCoroutine(GetComponent<PosterGifPlayer>().SetGifFromUrlCoroutine(randomFileFromFolder));
                                }
                                else if(randomFileFromFolder.Contains(".mp4") || randomFileFromFolder.Contains(".webm"))
                                {
                                    urlVideoPathRandom = randomFileFromFolder;
                                    StartCoroutine(AssignVideoTemplate());
                                }
                                else
                                {
                                      yield return LoadImageLocal(randomFileFromFolder);
                                }
*/
          }
          else
          {

                    /*
                        //to prevent players from getting confused on map start up(they think the map is broke)
                        if( !string.IsNullOrWhiteSpace(urlFilePath) && !urlFilePath.Equals("Incorrect File Type"))
                        {
                           GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("Poster: File or directory does not exist!");
                        }
                    */

                        StartCoroutine(AssignEmptyTemplate());

          }

        }


                                    }
                                    else    //asign the stencil write material if there are no depth layers
                                    {
                                                isGUIDreference = true;

                                                //make sure to have this or else the poster wont automatically adjust
                                                width = box.x;
                                                height = box.y;
                                                image = new Vector2(width, height);

                                        BuildPoster();
                                    }
    }



    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    MeshCollider meshCollider;
    
    public Vector2 box = new Vector2(500,500);

     public Vector2 image = new Vector2(512,512); //this is what sizes the image
     Vector2 resizedImage;


    public float distanceFromWall = 0f;
    public float distanceFromWall_base = 0.04f; //small for posters(0.002), a litte bigger for frames(0.04)
    public bool urlIsValid = false; //for making sure the poster is resized when editing box

    public bool isVideo = false;
    public bool canVideoSeek = true;
    public void BuildPoster()
    {

        if(box.x <= 0 || box.y <= 0)  //prevents there from being AABB errors
        {
            box = new Vector2(0.5f,0.5f);
        }

        if(!mesh)
        {
        mesh = new Mesh {
			name = "Procedural Mesh"
		};
        }

        if(!GetComponent<MeshRenderer>())
        {
         meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        else
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }

        

        if(meshRenderer.material == null)
        {
            meshRenderer.material = new Material(imageMat);
            meshRenderer.sharedMaterial.name = gameObject.name;
        }
        else
        {
            meshRenderer.sharedMaterial.name = gameObject.name;
        }

        ChangeShaderOfPoster();


        if(texture_original != null)
        {
          //asssign image to appropriate Object
        meshRenderer.sharedMaterial.mainTexture = texture_original;
        OnSuccesfulMediaChange?.Invoke();

        }
        if(!GetComponent<MeshFilter>())
        {
         meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        else
        {
         meshFilter = gameObject.GetComponent<MeshFilter>();
        }

        //calculating aspect ratio
        float dbl = image.x / image.y;
        if((box.y * dbl) <= box.x )
        {
            resizedImage = new Vector2((box.y * dbl), box.y);
        }
        else
        {
            resizedImage = new Vector2(box.x, box.x / dbl);
        }

        // THE EXTRA FOUR(4) THINGS USED(verticy, nomal, and uv) IS TO GIVE THE MESH SOME DEPTH FOR THE MESH CONVEX COLLIDER

        vertices = new Vector3[8]
        {                               //y axis controlls how far from the ground the image is!!!    // 0.4f from the ground
            new Vector3(resizedImage.x , distanceFromWall, resizedImage.y ),
            new Vector3(resizedImage.x , distanceFromWall, -resizedImage.y ),
            new Vector3(-resizedImage.x , distanceFromWall, -resizedImage.y ),
            new Vector3(-resizedImage.x , distanceFromWall, resizedImage.y ),

            new Vector3(resizedImage.x  , 0.0f, resizedImage.y ),
            new Vector3(resizedImage.x, 0.0f, -resizedImage.y ),
            new Vector3(-resizedImage.x , 0.0f, -resizedImage.y ),
            new Vector3(-resizedImage.x , 0.0f, resizedImage.y ),
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // upper left triangle
            0, 1, 2,
            // lower right triangle
            2, 3, 0
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[8]
        {
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[8]
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };
        mesh.uv = uv;

        meshFilter.mesh = mesh;

       //create and add mesh collider
       if(!GetComponent<MeshCollider>())
       {
       meshCollider = gameObject.AddComponent<MeshCollider>();
       }
       else
       {
       meshCollider = gameObject.GetComponent<MeshCollider>();
       }
       meshCollider.sharedMesh = meshFilter.mesh;
       meshCollider.convex = true;


        foreach(GameObject frame in GetComponent<PosterFrameList>().posterFrameList)// makes sure the frames get built, AFTER the poster is created, THIS IS A QUICK AND DIRTY SOLUTION, IN REALITY. THIS WHOLE FUNCTION SHOULD BE A COROUTINE!!!
        {
            frame.GetComponent<PosterMeshCreator_BorderFrame>().BuildFrame();
        }


        meshFilter.sharedMesh.RecalculateBounds();
        meshFilter.sharedMesh.RecalculateNormals();




      //  currentMaterial = meshRenderer.sharedMaterial;

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided; 

        SetMaterialProperties();
        GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();

        UpdateTextureFiltering();
     }



    //used to keep a reference of the gifs used for media switching, this is useful so the poster media trully resets on "resetMedia" event
    public List<GameObject> gifPostersUsedForMediaSwitching = new List<GameObject>();



    //mostly used for events
    public void ResetMedia()
    {
    if(GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count <= 0)
    {
            //reset media
            if(GetComponent<PosterGifPlayer>())
            {
                GetComponent<PosterGifPlayer>().posterMaterial = GetComponent<PosterGifPlayer>().posterMaterial_original;
            }
            else
            {
                 meshRenderer.material.SetTexture("_MainTex", texture_original); //for some reason if you use "sharedMaterial"... it wont work...
            }
    }
    else
    {
        BuildPoster();
        ChangeShaderOfPoster(shaderName);
    }

        //reset the base material of the referenced poster with gifs back... or else the media wont be trully reset for the poster
        foreach(GameObject obj in gifPostersUsedForMediaSwitching)
        {
            obj.GetComponent<PosterGifPlayer>().posterMaterial = obj.GetComponent<PosterGifPlayer>().posterMaterial_original;
        }

        if(GetComponent<PosterGifPlayer>())
        {
            GetComponent<PosterGifPlayer>().enabled = true;
        }

        gifPostersUsedForMediaSwitching.Clear();

    }



    public void RebuildMeshCollider()
    {
        if(meshCollider)
        {
        Destroy(meshCollider.sharedMesh);
        meshCollider.sharedMesh = meshFilter.mesh;
        meshCollider.convex = true;
        }
    }


     void Update() //OPTIMIZE THISS!!!
     {

        //not optimized!
        meshFilter.sharedMesh.RecalculateNormals();

        //calculating aspect ratio
        float dbl = image.x / image.y;

        if((box.y * dbl) <= box.x )
        {
            resizedImage = new Vector2((box.y * dbl), box.y);
        }
        else
        {
            resizedImage = new Vector2(box.x, box.x / dbl);
        }

        if(mesh)
        {
        vertices = new Vector3[8]
        {                               //y axis controlls how far from the ground the image is!!!    // 0.4f from the ground
            new Vector3(resizedImage.x , distanceFromWall, resizedImage.y ),
            new Vector3(resizedImage.x , distanceFromWall, -resizedImage.y ),
            new Vector3(-resizedImage.x , distanceFromWall, -resizedImage.y ),
            new Vector3(-resizedImage.x , distanceFromWall, resizedImage.y ),

            new Vector3(resizedImage.x  , 0.0f, resizedImage.y ),
            new Vector3(resizedImage.x, 0.0f, -resizedImage.y ),
            new Vector3(-resizedImage.x , 0.0f, -resizedImage.y ),
            new Vector3(-resizedImage.x , 0.0f, resizedImage.y ),
        };
        mesh.vertices = vertices;
        if(meshFilter)
        {
        meshFilter.mesh = mesh;
        }
        meshFilter.sharedMesh.RecalculateBounds();
        }
     }
        void OnDrawGizmosSelected() //DEBUGGING VERTICES ORDER
    {
   
    /*
              //draw main frame quad
         int i = 0;
                 foreach (Vector3 vertex in vertices) {
        Vector3 vertexWorldPos = gameObject.transform.TransformPoint(vertex);

        Handles.Label(vertexWorldPos, i.ToString());
        i++;
          }  
        
    */
    }

    UnityWebRequest www;

    //http image loading coroutines
 public IEnumerator LoadImageURL()
 {
    www = UnityWebRequestTexture.GetTexture(urlFilePath);
    //www.SetRequestHeader("Range", "bytes=0-31457"); ONLY WORKS WITH JPG IMAGES FOR SOME REASON

    StartCoroutine(WatForResponse(www));
    yield return www.SendWebRequest();


    //on complete
    if(www.result != UnityWebRequest.Result.Success)
    {
        StartCoroutine(AssignEmptyTemplate());
        Debug.Log(www.error);
        GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>Poster ERROR:" + www.error);

    }
    else
    {
        urlIsValid = true;
        isVideo = false;

        if(GameObject.Find("ItemEditStateMachine").GetComponent<ItemEditStateMachine>().currentObjectEditing != null)
        {
        GameObject.Find("ItemEditStateMachine").GetComponent<ItemEditStateMachine>().successfullImageUploadEventHandler.SuccessfulLocalImageUploadSoundEvent();
        }
        
        imgByte = ((DownloadHandlerTexture)www.downloadHandler).data;
        texture_original = ((DownloadHandlerTexture)www.downloadHandler).texture;
        texture_original.wrapMode = TextureWrapMode.Repeat;


        width = texture_original.width;
        height = texture_original.height;
        image = new Vector2(width,height);
    
        GetComponent<PosterFrameList>().AssignWhichFrameIsLast();
        GetComponent<PosterFrameList>().AssignWhichMeshesFramesDrawAround();

        OnSuccesfulMediaChange?.Invoke();

        SetMaterialProperties();
        GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();

        BuildPoster();
    }
 }

    IEnumerator WatForResponse(UnityWebRequest request)
    {
           while (!request.isDone)
           {
              //  Debug.Log("Loading image... " + request.downloadProgress);
                yield return null;
           }
    }


    //download loaded image...
    public void DownloadImage(string dir)
    {
       //         if(urlFilePath.StartsWith("http") || urlFilePath.StartsWith("SA:"))
       // {
        StartCoroutine(DownloadTheImage(dir));
       // }
       // else
       // {
       // Debug.Log("Image is a local file!");
       // }
    }
       IEnumerator DownloadTheImage(string dir)
    {


        Debug.Log("Attempeting to download image. Path: " + dir + ImageName(true));
        if(!Directory.Exists(dir))
        {    
            Directory.CreateDirectory(dir);
        }

        try{
        string safeDirectory = Path.Combine(Application.persistentDataPath, "SavedMedia");
        safeDirectory = safeDirectory.Replace("\\", "/");
        string fullPath = Path.GetFullPath(Path.Combine(dir, ImageName(true)));
        fullPath = fullPath.Replace("\\", "/");

        // Validate and sanitize path
        if (string.IsNullOrWhiteSpace(fullPath) || fullPath.Contains(".."))
        {
            throw new UnauthorizedAccessException("Invalid path.");
        }



        //check base file
        string baseFilePath = urlFilePath;
        //check if the media is in the same folder
        if(File.Exists(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath ))
        {
            baseFilePath = System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath;
        }
        //check if video is inside the current unzipped temp dir
        if(File.Exists(ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory + "/" + urlFilePath))
        {
        baseFilePath = ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory + "/" + urlFilePath;
        }

        if(!GlobalUtilityFunctions.IsSafeToReadBytes(baseFilePath))
        {
            throw new UnauthorizedAccessException("Not safe");
            yield break;
        }
    


        Debug.Log("fullPath: " + fullPath );
        Debug.Log("safeDirectory: " + safeDirectory );
        // Check if the resolved path is still within the safe directory
        if (!fullPath.StartsWith(safeDirectory))
        {
            throw new UnauthorizedAccessException("Path traversal attempt detected.");
        }




        if(urlFilePath.Contains(".jpg") || urlFilePath.Contains(".jpeg") || urlFilePath.Contains(".png") || urlFilePath.Contains(".PNG"))
        {
            File.WriteAllBytes(dir + ImageName(true), imgByte);
        }
        else if(urlFilePath.Contains(".gif"))
        {
            File.WriteAllBytes(dir + ImageName(true), GetComponent<PosterGifPlayer>().gifbytes);
        }

        
        if(!File.Exists(dir + ImageName(true)))
        {
            Debug.Log("Image not in dir yet... waiting");
        }

        Debug.Log("image saved: " + dir + ImageName(true));
        
        GetComponent<Collider>().enabled = false;
        //transform.DOMove(GameObject.Find("HarddriveScanner").transform.position, 0.8f);
        GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=green>File Succesfully downloaded!");
        transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.3f).OnComplete(() => transform.DOScale(new Vector3(0f, 0f, 0f), 0.2f).OnComplete(() => StartCoroutine(PosterTwinkle())));

        }
        catch (Exception ex)
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>Poster Error: " + ex.Message);
        }

        yield return null;
        
    }



    UnityWebRequest videoDownloadwww;

        //download loaded video...
    public void DownloadVideo(string path)
    {
       //         if(urlFilePath.StartsWith("http") || urlFilePath.StartsWith("SA:"))
       // {
        StartCoroutine(DownloadTheVideo(path));
       // }
       // else
       // {
       // Debug.Log("Image is a local file!");
       // }
    }

    IEnumerator DownloadTheVideo(string path)
    {
        GetComponent<Collider>().enabled = false; //to prevent from scanning again while downloading video


        try{
        string safeDirectory = Path.Combine(Application.persistentDataPath, "SavedMedia");
        safeDirectory = safeDirectory.Replace("\\", "/");
        string fullPath = Path.GetFullPath(Path.Combine(path, ImageName(true)));
        fullPath = fullPath.Replace("\\", "/");

        // Validate and sanitize path
        if (string.IsNullOrWhiteSpace(fullPath) || fullPath.Contains(".."))
        {
            throw new UnauthorizedAccessException("Invalid path.");
        }

        //check base file
        string baseFilePath = urlFilePath;
        //check if the media is in the same folder
        if(File.Exists(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath ))
        {
            baseFilePath = System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath;
        }
        //check if video is inside the current unzipped temp dir
        if(File.Exists(ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory + "/" + urlFilePath))
        {
        baseFilePath = ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory + "/" + urlFilePath;
        }
        
        if(!GlobalUtilityFunctions.IsSafeToReadBytes(baseFilePath))
        {
            throw new UnauthorizedAccessException("Not safe");
            yield break;
        }
    






        Debug.Log("fullPath: " + fullPath );
        Debug.Log("safeDirectory: " + safeDirectory );
        // Check if the resolved path is still within the safe directory
        if (!fullPath.StartsWith(safeDirectory))
        {
            throw new UnauthorizedAccessException("Path traversal attempt detected.");
        }




        if(urlFilePath.StartsWith("SA:"))
        {
        videoDownloadwww = new UnityWebRequest(Application.dataPath + "/StreamingAssets/" + urlFilePath.Substring(3));
        }
        else
        {
        videoDownloadwww = new UnityWebRequest(urlFilePath);
        }


        if(!Directory.Exists(path))
        {    
            Directory.CreateDirectory(path);
        }

        }
        catch (Exception ex)
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>Poster Error: " + ex.Message);
        }



        videoDownloadwww.downloadHandler = new DownloadHandlerFile(path + ImageName(true));
        yield return videoDownloadwww.SendWebRequest();
        if (videoDownloadwww.result != UnityWebRequest.Result.Success)
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>Poster Error: " + videoDownloadwww.error);
        }
        else
        {
            //makes sure to stop playing the video if the video was saved via scanner gun
            gameObject.GetComponent<VLC_MediaPlayer>().StopVideoPlayerIfPosterWhereVideoIsPlayingWasChanged(this);

            Debug.Log("File successfully downloaded and saved to " + path + ImageName(true));
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=green>File Succesfully downloaded!");
            transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.3f).OnComplete(() => transform.DOScale(new Vector3(0f, 0f, 0f), 0.2f).OnComplete(() => StartCoroutine(PosterTwinkle())));

        }
        
    }


    IEnumerator PosterTwinkle()
    {

        //events that happen on scan
        EventActionManager.Instance.TryPlayEvent(gameObject, "OnScanned");


        //effects
        GameObject particleSystemTwinkle = GameObject.Find("PARTICLE_POSTERTWINKLE");

        particleSystemTwinkle.transform.position = transform.position; //particle happens at the location where poster shrunk to
        particleSystemTwinkle.GetComponent<ParticleSystem>().Play();
        particleSystemTwinkle.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.1f);
        
        gameObject.SetActive(false);
        ResetLevelParametersManager.Instance.AddEntityChangeToList(gameObject, "Poster - Deactivated After being scanned");
    }


    public string ImageName(bool includeExtension = false)
    {
                string filepathName = urlFilePath;


            if(includeExtension == false)
            {
                if(filepathName.Contains(".jpg"))
                {
                    int index = filepathName.LastIndexOf(".jpg");
                    filepathName = filepathName.Substring (0, index); 
                }
                if(filepathName.Contains(".png")) 
                {
                    int index = filepathName.LastIndexOf(".png");
                    filepathName = filepathName.Substring (0, index); 
                }
                if(filepathName.Contains(".gif")) 
                {
                    int index = filepathName.LastIndexOf(".gif");
                    filepathName = filepathName.Substring (0, index); 
                }

                if(filepathName.Contains(".webm")) 
                {
                    int index = filepathName.LastIndexOf(".webm");
                    filepathName = filepathName.Substring (0, index); 
                }
                if(filepathName.Contains(".wav")) 
                {
                    int index = filepathName.LastIndexOf(".wav");
                    filepathName = filepathName.Substring (0, index); 
                }
                if(filepathName.Contains(".mp4")) 
                {
                    int index = filepathName.LastIndexOf(".mp4");
                    filepathName = filepathName.Substring (0, index); 
                }
            }

                string fileName = urlFilePath;

                if(filepathName.Contains("/"))
                {
                fileName = filepathName.Split('/')[filepathName.Split('/').Length - 1]; //for web files
                }
                if(filepathName.Contains("\\"))
                {
                fileName = filepathName.Split('\\')[filepathName.Split('\\').Length - 1]; // for local files
                }
                

                fileName = fileName.Replace("=jpg", ".jpg");
                fileName = fileName.Replace("=png", ".png");
                fileName = fileName.Replace("=webm", ".webm");
                fileName = fileName.Replace("=wave", ".wav");
                fileName = fileName.Replace("=mp4", ".mp4");
                fileName = fileName.Replace("?", "");
                fileName = fileName.Replace("=", "");

                fileName = fileName.ToLower();

                Debug.Log("Filename:" + fileName);
                return fileName;
    }

    public string GetFileType()
    {
                string filepathName = urlFilePath;

                string fileName = filepathName.Split('/')[filepathName.Split('/').Length - 1]; //for web files
                fileName = filepathName.Split('\\')[filepathName.Split('\\').Length - 1]; // for local files

                fileName = fileName.Replace("=jpg", ".jpg");
                fileName = fileName.Replace("=png", ".png");
                fileName = fileName.Replace("?", "");
                fileName = fileName.Replace("=", "");

                if(fileName.Contains(".jpg") || fileName.Contains(".JPG"))
                {
                    return "jpg";
                }
                else if(fileName.Contains(".png") || fileName.Contains(".PNG"))
                {
                    return "png";
                }
                else if(fileName.Contains(".mp4") || fileName.Contains(".MP4"))
                {
                    return "mp4";
                }
                else if(fileName.Contains(".webm") || fileName.Contains(".WEBM"))
                {
                    return "webm";
                }
                else if(fileName.Contains(".gif") || fileName.Contains(".gif"))
                {
                    return "gif";
                }
                else
                {
                    return "";
                }

    }

    public string GetImageDomain()
    {
        if (System.Uri.TryCreate(urlFilePath, System.UriKind.Absolute, out System.Uri uri))
        {
            return uri.Host;
        }
        else
        {
            // Handle invalid URI format
            return urlFilePath;
        }
    }


    public bool isLocalFile()
    {
        bool isLocal = true;

        if(urlFilePath.StartsWith("http") || urlFilePath.Contains("SA:"))
        {
            isLocal = false;
        }

        return isLocal;
    }

    public byte[] imgByte;

    //http image loading coroutines
 public IEnumerator LoadImageLocal(string path) //FIX!!! ADD A TIMER IF THE IMAGE TAKES TOO LONG, FAIL...
 {
     yield return new WaitForSeconds(0.1f); //problem if you dont inclide this... (due to setting image variable dimensions on Start())


    if(path.StartsWith("SA:"))
    {
        path = Application.dataPath + "/StreamingAssets/" + path.Substring(3);
    }

    if(!GlobalUtilityFunctions.IsSafeToReadBytes(path))
    {
        yield break;
    }
    

    imgByte = File.ReadAllBytes(path);
     
    if(!GlobalUtilityFunctions.IsFileDataSafe(imgByte))
    {
        UINotificationHandler.Instance.SpawnNotification("<color=red>Poster ERROR: " + path);
        yield break;
    }



    texture_original = new Texture2D(2, 2, TextureFormat.ARGB32, false);
    texture_original.LoadImage(imgByte);
    texture_original.wrapMode = TextureWrapMode.Repeat;


    //on complete
        urlIsValid = true;
        isVideo = false;

        if(GameObject.Find("ItemEditStateMachine").GetComponent<ItemEditStateMachine>().currentObjectEditing != null)
        {
        GameObject.Find("ItemEditStateMachine").GetComponent<ItemEditStateMachine>().successfullImageUploadEventHandler.SuccessfulLocalImageUploadSoundEvent();
        }

        width = texture_original.width;
        height = texture_original.height;
        image = new Vector2(width,height);

        GetComponent<PosterFrameList>().AssignWhichFrameIsLast();
        GetComponent<PosterFrameList>().AssignWhichMeshesFramesDrawAround();

        OnSuccesfulMediaChange?.Invoke();

        SetMaterialProperties();
        GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();

        BuildPoster();
    }



    public IEnumerator AssignEmptyTemplate()
    {
        urlIsValid = false;
        isVideo = false;
        yield return new WaitForSeconds(0.1f); //problem if you dont inclide this... (due to setting image variable dimensions on Start())

        //do...
        width = box.x;
        height = box.y;
        image = new Vector2(width, height);


                if(!GetComponent<MeshRenderer>())
        {
         meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        else
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }
        ChangeShaderOfPoster();
        meshRenderer.sharedMaterial.mainTexture = imageTemplatePreset;

        OnSuccesfulMediaChange?.Invoke();

        SetMaterialProperties();
        GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();

        BuildPoster();

        yield return new WaitForSeconds(0.1f); //problem if you dont inclide this... (due to setting image variable dimensions on Start())

    //problem with this, it seems the poster frame does not update when using a coroutine...
        foreach(GameObject frame in GetComponent<PosterFrameList>().posterFrameList)// makes sure the frames get built, AFTER the poster is created, THIS IS A QUICK AND DIRTY SOLUTION, IN REALITY. THIS WHOLE FUNCTION SHOULD BE A COROUTINE!!!
        {
            frame.GetComponent<PosterMeshCreator_BorderFrame>().UpdateFrame();
        }
    }



    public IEnumerator AssignVideoTemplate()
    {
        urlIsValid = false;
        isVideo = true;


        if(!GetComponent<MeshRenderer>())
        {
         meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        else
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }


        yield return new WaitForSeconds(0.1f); //problem if you dont inclide this... (due to setting image variable dimensions on Start())

        //do...
        width = box.x;
        height = box.y;
        image = new Vector2(width, height);

        OnSuccesfulMediaChange?.Invoke();

        SetMaterialProperties();
        GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();

        BuildPoster();
        ChangeShaderOfPoster();

        yield return new WaitForSeconds(0.1f); //problem if you dont inclide this... (due to setting image variable dimensions on Start())

    //problem with this, it seems the poster frame does not update when using a coroutine...
        foreach(GameObject frame in GetComponent<PosterFrameList>().posterFrameList)// makes sure the frames get built, AFTER the poster is created, THIS IS A QUICK AND DIRTY SOLUTION, IN REALITY. THIS WHOLE FUNCTION SHOULD BE A COROUTINE!!!
        {
            frame.GetComponent<PosterMeshCreator_BorderFrame>().UpdateFrame();
        }
    }

    //basically, makes sure there is a shared material assigned
    public void AssignImageMaterial()
    {
           if(!GetComponent<MeshRenderer>())
        {
         meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        else
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }

        if(meshRenderer.sharedMaterial == null)
        {
            meshRenderer.material = new Material(imageMat);
            meshRenderer.sharedMaterial.name = gameObject.name;
        }
        else
        {
            meshRenderer.sharedMaterial.name = gameObject.name;
        }


        ChangeShaderOfPoster();
    }

    public void AssignVideoMaterial(RenderTexture renderTex)
    {
        ChangeShaderOfPoster(shaderName);
        meshRenderer.sharedMaterial.SetTexture("_MainTex",renderTex );

        if(!GetComponent<MeshRenderer>())
        {
         meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        else
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }


        //makes sure the color keys get applied(because it changes from television static material to poster material)
        GetComponent<PosterColorKeySettings>().ApplyColorKeySettingsToShader();


        OnSuccesfulMediaChange?.Invoke();

        SetMaterialProperties();
        GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();

    }

    public Material roomPreviewMat;

    public void LoadRoomPreviewInfo()
    {
        //all of this makes sure the poster has a mesh and renderer or else there will be errror!
        
          if(!GetComponent<MeshRenderer>())
        {
         meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        else
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }


        if(!GetComponent<MeshFilter>())
        {
         meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        else
        {
         meshFilter = gameObject.GetComponent<MeshFilter>();
        }

                if(!mesh)
        {
        mesh = new Mesh {
			name = "Procedural Mesh"
		};
        }

                vertices = new Vector3[8]
        {                               //y axis controlls how far from the ground the image is!!!    // 0.4f from the ground
            new Vector3(resizedImage.x , distanceFromWall, resizedImage.y ),
            new Vector3(resizedImage.x , distanceFromWall, -resizedImage.y ),
            new Vector3(-resizedImage.x , distanceFromWall, -resizedImage.y ),
            new Vector3(-resizedImage.x , distanceFromWall, resizedImage.y ),

            new Vector3(resizedImage.x  , 0.0f, resizedImage.y ),
            new Vector3(resizedImage.x, 0.0f, -resizedImage.y ),
            new Vector3(-resizedImage.x , 0.0f, -resizedImage.y ),
            new Vector3(-resizedImage.x , 0.0f, resizedImage.y ),
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // upper left triangle
            0, 1, 2,
            // lower right triangle
            2, 3, 0
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[8]
        {
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[8]
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };
        mesh.uv = uv;

        meshFilter.mesh = mesh;

    }
    
    public void TriggerVideoPauseEvent()
    {
        OnSuccesfulMediaChange?.Invoke();
    }



    public string shaderName;

    public void ChangeShaderOfPoster()
    {
        ChangeShaderOfPoster(shaderName);
    }

    public void ChangeShaderOfPoster(string name)
    {
        //assign the depth stencil write material if there are ANY depth layers
        if(GetComponent<PosterDepthLayerList>().posterDepthLayerList.Count >= 1)
        {
            meshRenderer.sharedMaterial.shader = Shader.Find("PosterStencil/write");
            PosterDepthLayerStencilRefManager.Instance.AssignCorrectStencilRefsToAllPostersInScene();
            return;
        }
            shaderName = name;



        //create material based on name
        if(string.IsNullOrWhiteSpace(shaderName))
        {
            shaderName = "Shader Graphs/UnlitTransparentDither";
        }


        if(meshRenderer.sharedMaterial != null)
        {
            //prevents errors if shader name is not found
            if(Shader.Find(shaderName) != null)
            {
                    meshRenderer.sharedMaterial.shader = Shader.Find(name);
                shaderName = name;
            }
            else
            {
                meshRenderer.sharedMaterial.shader = Shader.Find("Shader Graphs/UnlitTransparentDither");
                shaderName = "Shader Graphs/UnlitTransparentDither";
            }
        }


        //apply a template video playing image to let players know it is a video
        if(isVideo)
        meshRenderer.sharedMaterial.SetTexture("_MainTex",videoTemplateTexture);



        SetMaterialProperties();
        GetComponent<GeneralObjectInfo>().UpdateBaseMaterialList();
    }


    public bool textureFiltering = true;


    public void UpdateTextureFiltering()
    {
        //images
        if(texture_original != null)
        {
            if(!textureFiltering)
            {
                texture_original.filterMode = FilterMode.Point;
            }
            else
            {
                texture_original.filterMode = FilterMode.Bilinear;
            }
        }

        //gifs
        if(GetComponent<PosterGifPlayer>())
        {
            if(GetComponent<PosterGifPlayer>().m_gifTextureList != null)
            {
                foreach(UniGif.GifTexture t in GetComponent<PosterGifPlayer>().m_gifTextureList)
                {
                    if(!textureFiltering)
                    {
                        t.m_texture2d.filterMode = FilterMode.Point;
                    }
                    else
                    {
                        t.m_texture2d.filterMode = FilterMode.Bilinear;
                    }
                }
            }
        }

        //videos
        if(GetComponent<VLC_MediaPlayer>().texture != null)
        {
            if(!textureFiltering)
            {
                GetComponent<VLC_MediaPlayer>().texture.filterMode = FilterMode.Point;
            }
            else
            {
                GetComponent<VLC_MediaPlayer>().texture.filterMode = FilterMode.Bilinear;
            }
        }
    }




    public bool CheckIfPosterMaterialHasXrayProperties()
    {
        if(meshRenderer.sharedMaterial.GetFloat("_size") > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

 }
