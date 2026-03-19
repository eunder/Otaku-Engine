using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class LevelGlobalMediaManager : MonoBehaviour
{


    private static LevelGlobalMediaManager _instance;
    public static LevelGlobalMediaManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    public float width = 1;
    public float height = 1;
    public Material imageMat;
    public Material videoMat;
    public Material videoStaticMat;

    public Vector3[] vertices;

    public string urlFilePath;
    public string urlFilePath_eventModified; //mostly used to check the type of media of the swapped media

    public Texture2D imageToTestFrom;
    public Texture2D imageTemplatePreset;

    public TMP_InputField globalMediaUrl_InputField;

    public VLC_GlobalMediaPlayer globalMedia_VideoPlayer;

    public Material linearColorskybox_Mat;


    public float  skyboxTextureScaleX = 0f;
    public float  skyboxTextureScaleY = 0f;
    public float  skyboxTextureOffsetX = 0f;
    public float  skyboxTextureOffsetY = 0f;
    public float  scrollSpeedX = 0f;
    public float  scrollSpeedY = 0f;

    Mesh mesh;
    void Start()
    {
        int tempSeed = (int)System.DateTime.Now.Ticks;
        Random.InitState(tempSeed);
        
         
        image = new Vector2(512,512);
        if(!mesh)
        {
        BuildPoster();
        }
    }


    public void OnUrlFieldFinishEditing()
    {
                if(!urlFilePath.Equals(globalMediaUrl_InputField.text))
        {
            
        //stop skybox media video player
        globalMedia_VideoPlayer.Stop();

        //load image onto skybox
        StartCoroutine(LoadImage(globalMediaUrl_InputField.text));
        }
    }

    public string FetchRandomImageFromFilePath()
    {

    List<string> filesFound = new List<string>();  // list of files found in a folder(that also meet the file type criteria)

    var filters = new string[] { "jpg", "jpeg", "png"};
        foreach (var filter in filters)
        {
        filesFound.AddRange(Directory.GetFiles(urlFilePath, string.Format("*.{0}", filter)));
        }
    
        int fileCount = filesFound.Count;

        return filesFound[Random.Range(0, fileCount)];

    }



    public IEnumerator LoadImage(string filePath)
    {
        urlFilePath = filePath;

        urlFilePath = GlobalUtilityFunctions.UrlChecker_PictureFormat(urlFilePath);

        if(urlFilePath.StartsWith("SA:"))
        {
            urlFilePath = Application.dataPath + "/StreamingAssets/" + urlFilePath.Substring(3);
        }

        if(GlobalUtilityFunctions.IsURL(urlFilePath) && (urlFilePath.Contains(".jpg") || urlFilePath.Contains(".jpeg") || urlFilePath.Contains(".png") || urlFilePath.Contains(".PNG")))
        {
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
          AssignImageMaterial();
          urlIsValid = true;
          isVideo = false;
          if(GetComponent<PosterGifPlayer>() == null)
          {
          gameObject.AddComponent<PosterGifPlayer>();
          }
          
          GetComponent<PosterGifPlayer>().posterMaterial = meshRenderer.material;

          if(File.Exists(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath ))
          {
          yield return StartCoroutine(GetComponent<PosterGifPlayer>().SetGifFromUrlCoroutine(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath));
          }
          else
          {
          yield return StartCoroutine(GetComponent<PosterGifPlayer>().SetGifFromUrlCoroutine(urlFilePath));
          }


        }
        else if(urlFilePath.Contains(".mp4") || urlFilePath.Contains(".webm") || urlFilePath.Contains(".mkv"))
        {
            StartCoroutine(AssignVideoTemplate());
        }
        else
        {
         // filePath.Replace("\\","/");

          if(File.Exists(urlFilePath))
          {
            AssignImageMaterial();

            yield return LoadImageLocal(false, urlFilePath);
          }
          //check if the media is in the same folder (for workshop media)
          else if(File.Exists(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath ))
          {
            AssignImageMaterial();
            yield return LoadImageLocal(false, System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + urlFilePath);
          }
          else if(Directory.Exists(urlFilePath))
          {
            AssignImageMaterial();
            yield return LoadImageLocal(true, urlFilePath);
          }
          else
          {
            StartCoroutine(AssignEmptyTemplate());
          }

        }


        //for asign linear skybox when there is nothing to use
        if(string.IsNullOrEmpty(urlFilePath) || urlFilePath.Equals("Incorrect File Type"))
        {
            RenderSettings.skybox = linearColorskybox_Mat;
            currentMaterial = linearColorskybox_Mat;
        }
        else
        {
                RenderSettings.skybox = GetComponent<Renderer>().sharedMaterial;

                if(isVideo)
                {
                    globalMedia_VideoPlayer.PlayVideoURL(this);
                    RenderSettings.skybox = GetComponent<Renderer>().sharedMaterial;
                }
            currentMaterial = GetComponent<Renderer>().sharedMaterial;

        }
    }


    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    MeshCollider meshCollider;
    
    public Vector2 box = new Vector2(500,500);
     public Vector2 image;
     Vector2 resizedImage;


    public float distanceFromWall = 0f;
    public float distanceFromWall_base = 0.04f; //small for posters(0.002), a litte bigger for frames(0.04)
    public bool urlIsValid = false; //for making sure the poster is resized when editing box

    public bool isVideo = false;
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

        if(imageToTestFrom != null)
        {
          //asssign image to appropriate Object
        meshRenderer.material.mainTexture = imageToTestFrom;
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

        meshFilter.sharedMesh.RecalculateBounds();
        meshFilter.sharedMesh.RecalculateNormals();


 
     }
    public float scaleOffset = 1.0f;

    public Material currentMaterial;


    public void SwitchMedia(GameObject poster)
    {        
        AssignImageMaterial();

        meshRenderer.sharedMaterial = poster.GetComponent<PosterMeshCreator>().meshRenderer.material;   
        urlFilePath_eventModified = poster.GetComponent<PosterMeshCreator>().urlFilePath;
        RenderSettings.skybox = GetComponent<Renderer>().sharedMaterial;
        Debug.Log("switch skybox");
    }

    //mostly used for events
    public void ResetMedia()
    {
        //fix the offsets back 
        Material mat = RenderSettings.skybox;



        RenderSettings.skybox = currentMaterial;

        mat.mainTextureOffset = new Vector2(0,0);
        mat.mainTextureScale = new Vector2(1,1);


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

    public float xOffset = 0f;
    public float yOffset = 0f;


     void Update() //OPTIMIZE THISS!!!
     {

        if(RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_MainTex"))
        RenderSettings.skybox.mainTextureScale = new Vector2(skyboxTextureScaleX, skyboxTextureScaleY);


        if(Mathf.Abs(scrollSpeedX) > 0)
        {
        xOffset += (Time.deltaTime*scrollSpeedX)/5f;
        if(xOffset > 2f)
        {
            xOffset = 0f;
        }
        if(RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_MainTex"))
        RenderSettings.skybox.mainTextureOffset = new Vector2(xOffset, RenderSettings.skybox.mainTextureOffset.y);
        }
        else
        {
        if(RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_MainTex"))
        RenderSettings.skybox.mainTextureOffset = new Vector2(skyboxTextureOffsetX, RenderSettings.skybox.mainTextureOffset.y);
        }


        if(Mathf.Abs(scrollSpeedY) > 0)
        {
        yOffset += (Time.deltaTime*scrollSpeedY)/5f;
        if(yOffset > 2f)
        {
            yOffset = 0f;
        }
        if(RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_MainTex"))
        RenderSettings.skybox.mainTextureOffset = new Vector2(RenderSettings.skybox.mainTextureOffset.x,yOffset);
        }
        else
        {
        if(RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_MainTex"))
        RenderSettings.skybox.mainTextureOffset = new Vector2(RenderSettings.skybox.mainTextureOffset.x, skyboxTextureOffsetY);
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
        //calculating aspect ratio

        //distanceFromWall = distanceFromWall_base / scaleOffset;
        //gameObject.transform.localScale = new Vector3(scaleOffset, scaleOffset, scaleOffset);

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
        GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>ERROR:" + www.error);
    }
    else
    {
        urlIsValid = true;
        isVideo = false;

        imgByte = ((DownloadHandlerTexture)www.downloadHandler).data;
        imageToTestFrom = ((DownloadHandlerTexture)www.downloadHandler).texture;
        imageToTestFrom.wrapMode = TextureWrapMode.Repeat;

        width = imageToTestFrom.width;
        height = imageToTestFrom.height;
        image = new Vector2(width,height);

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

    public string ImageName()
    {
                string filepathName = urlFilePath;
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


                string fileName = "";

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
                fileName = fileName.Replace("?", "");
                fileName = fileName.Replace("=", "");

                fileName = fileName.ToLower();

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
         var uri = new System.Uri(urlFilePath);
         return uri.Host;
    }


    public bool isLocalFile()
    {
        bool isLocal = true;

        if(urlFilePath.StartsWith("http"))
        {
            isLocal = false;
        }

        return isLocal;
    }


    //http image loading coroutines
    public byte[] imgByte;
 public IEnumerator LoadImageLocal(bool loadFromFolderDir, string path) //FIX!!! ADD A TIMER IF THE IMAGE TAKES TOO LONG, FAIL...
 {
     yield return new WaitForSeconds(0.1f); //problem if you dont inclide this... (due to setting image variable dimensions on Start())

     if(loadFromFolderDir)
     {
                    imgByte = File.ReadAllBytes(FetchRandomImageFromFilePath());
     }
     else
     {
                    imgByte = File.ReadAllBytes(path);
     }
    imageToTestFrom = new Texture2D(2, 2, TextureFormat.ARGB32, false);
    imageToTestFrom.LoadImage(imgByte);
    imageToTestFrom.wrapMode = TextureWrapMode.Repeat;

    //Debug.Log("Texture2d.LoadImage()");
    while(imageToTestFrom.width <= 8 || imageToTestFrom.height <= 8) 
    {
        yield return null;
    }

    //on complete
        urlIsValid = true;
        isVideo = false;
        width = imageToTestFrom.width;
        height = imageToTestFrom.height;
        image = new Vector2(width,height);

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
        meshRenderer.sharedMaterial = imageMat;
        meshRenderer.material.mainTexture = imageTemplatePreset;
        BuildPoster();

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

    //    meshRenderer.sharedMaterial = videoStaticMat;

        yield return new WaitForSeconds(0.1f); //problem if you dont inclide this... (due to setting image variable dimensions on Start())

        //do...
        width = box.x;
        height = box.y;
        image = new Vector2(width, height);
        
        BuildPoster();


        yield return new WaitForSeconds(0.1f); //problem if you dont inclide this... (due to setting image variable dimensions on Start())

    }

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
        meshRenderer.sharedMaterial = imageMat;
    }

    public void AssignVideoMaterial(RenderTexture renderTex)
    {
        videoMat.mainTexture = renderTex;

        if(!GetComponent<MeshRenderer>())
        {
         meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        else
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }
        meshRenderer.sharedMaterial = videoMat;
        
        RenderSettings.skybox = GetComponent<Renderer>().sharedMaterial;
        currentMaterial = GetComponent<Renderer>().sharedMaterial;

    }


 }
