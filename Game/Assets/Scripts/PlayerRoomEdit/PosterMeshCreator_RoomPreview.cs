using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;
using DG.Tweening;

public class PosterMeshCreator_RoomPreview : MonoBehaviour   //FOR MAKING SURE THE ROOM PREVIEW POSTERS DONT GET SAVED (posters get fetched by component name)
{

    public float width = 1;
    public float height = 1;
    public Material imageMat;
    public Material videoMat;
    public Material videoStaticMat;

    public Vector3[] vertices;

    public string urlFilePath;
    public Texture2D imageToTestFrom;
    public Texture2D imageTemplatePreset;
    List<GameObject> frameObjects = new List<GameObject>();
    Mesh mesh;
    void Start()
    {
        int tempSeed = (int)System.DateTime.Now.Ticks;
        UnityEngine.Random.InitState(tempSeed);
        
         
        image = new Vector2(512,512);
        if(!mesh)
        {
        BuildPoster();
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

        return filesFound[UnityEngine.Random.Range(0, fileCount)];

    }

    public IEnumerator LoadImage(string filePath)
    {
        urlFilePath = filePath;

        if(urlFilePath.StartsWith("SA:"))
        {
            urlFilePath = Application.dataPath + "/StreamingAssets/" + urlFilePath.Substring(3);
        }

        if(urlFilePath.StartsWith("http") && (urlFilePath.Contains(".jpg") || urlFilePath.Contains(".jpeg") || urlFilePath.Contains(".png") || urlFilePath.Contains(".PNG")))
        {
            AssignImageMaterial();
            yield return LoadImageURL();
        }
        else if(urlFilePath.Contains(".gif"))
        {
          AssignImageMaterial();
          urlIsValid = true;
          if(GetComponent<PosterGifPlayer>() == null)
          {
          gameObject.AddComponent<PosterGifPlayer>();
          }
          
          GetComponent<PosterGifPlayer>().posterMaterial = meshRenderer.material;
          yield return StartCoroutine(GetComponent<PosterGifPlayer>().SetGifFromUrlCoroutine(urlFilePath));

        }
        else if(urlFilePath.Contains(".mp4") || urlFilePath.Contains(".webm"))
        {
            StartCoroutine(AssignVideoTemplate());
        }
        else
        {
         // filePath.Replace("\\","/");

          if(File.Exists(urlFilePath))
          {
            AssignImageMaterial();

            yield return LoadImageLocal(false);
          }
          else if(Directory.Exists(urlFilePath))
          {
            AssignImageMaterial();
            yield return LoadImageLocal(true);
          }
          else
          {
              
            StartCoroutine(AssignEmptyTemplate());
            Debug.Log("ERROR: File or directory does not exist!");
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("Poster: File or directory does not exist!");

          }

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


        foreach(GameObject frame in GetComponent<PosterFrameList>().posterFrameList)// makes sure the frames get built, AFTER the poster is created, THIS IS A QUICK AND DIRTY SOLUTION, IN REALITY. THIS WHOLE FUNCTION SHOULD BE A COROUTINE!!!
        {
            frame.GetComponent<PosterMeshCreator_BorderFrame>().BuildFrame();
        }


        meshFilter.sharedMesh.RecalculateBounds();
        meshFilter.sharedMesh.RecalculateNormals();

     }
    public float scaleOffset = 1.0f;


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
        GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=red>ERROR:" + www.error);

    }
    else
    {
        urlIsValid = true;
        isVideo = false;

        if(GameObject.Find("ItemEditStateMachine").GetComponent<ItemEditStateMachine>().currentObjectEditing != null)
        {
        GameObject.Find("ItemEditStateMachine").GetComponent<ItemEditStateMachine>().successfullImageUploadEventHandler.SuccessfulLocalImageUploadSoundEvent();
        }
        
        imageToTestFrom = ((DownloadHandlerTexture)www.downloadHandler).texture;
        imageToTestFrom.wrapMode = TextureWrapMode.Repeat;

        width = imageToTestFrom.width;
        height = imageToTestFrom.height;
        image = new Vector2(width,height);

        GetComponent<PosterFrameList>().AssignWhichFrameIsLast();
        GetComponent<PosterFrameList>().AssignWhichMeshesFramesDrawAround();

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


    public void TrailerDownload(string path) //fake effect
    {
        GetComponent<Collider>().enabled = false;
        //transform.DOMove(GameObject.Find("HarddriveScanner").transform.position, 0.8f);
        GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=green>File Succesfully downloaded!");
        transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.3f).OnComplete(() => transform.DOScale(new Vector3(0f, 0f, 0f), 0.2f).OnComplete(() => StartCoroutine(PosterTwinkle())));
    }


    //download loaded image...
    public void DownloadImage(string path)
    {
                if(urlFilePath.StartsWith("http"))
        {
        StartCoroutine(DownloadTheImage(path));
        }
        else
        {
        Debug.Log("Image is a local file!");
        }
    }
    IEnumerator DownloadTheImage(string path)
    {
        string filepathName = urlFilePath; // so that the function underneath dosnt mess up the base filepath.

        // Validate and sanitize path
        if (string.IsNullOrWhiteSpace(path) || path.Contains("..") || Path.IsPathRooted(path))
        {
            throw new UnauthorizedAccessException("Invalid path.");
        }



        if(filepathName.Contains(".jpg")) //this should be done with fileName instead! but theres a problem!
        {
            int index = filepathName.LastIndexOf(".jpg");
            filepathName = filepathName.Substring (0, index + 4); //4 characters in '.jpg'    LATER use a better function to check
        }
        if(filepathName.Contains(".gif")) 
        {
            int index = filepathName.LastIndexOf(".gif");
            filepathName = filepathName.Substring (0, index + 4); 
        }

        string fileName = filepathName.Split('/')[filepathName.Split('/').Length - 1];
        
        fileName = fileName.Replace("=jpg", ".jpg");
        fileName = fileName.Replace("=png", ".png");
        fileName = fileName.Replace("=gif", ".gif");
        fileName = fileName.Replace("?", "");
        fileName = fileName.Replace("=", "");

        fileName = fileName.ToLower();



        if(!Directory.Exists(path))
        {    
            Directory.CreateDirectory(path);
        }


        if(urlFilePath.StartsWith("http") && (urlFilePath.Contains(".jpg") || urlFilePath.Contains(".jpeg") || urlFilePath.Contains(".png") || urlFilePath.Contains(".PNG")))
        {
            File.WriteAllBytes(path + fileName, ((DownloadHandlerTexture)www.downloadHandler).data);
        }
        else if(urlFilePath.Contains(".gif"))
        {
            File.WriteAllBytes(path + fileName, GetComponent<PosterGifPlayer>().gifbytes);
        }

        
        if(!File.Exists(path + fileName))
        {
            Debug.Log("Image not in dir yet... waiting");
        }

        yield return null;
        
        Debug.Log("image saved: " + path + fileName);
        
        GetComponent<Collider>().enabled = false;
        //transform.DOMove(GameObject.Find("HarddriveScanner").transform.position, 0.8f);
        GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=green>File Succesfully downloaded!");
        transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.3f).OnComplete(() => transform.DOScale(new Vector3(0f, 0f, 0f), 0.2f).OnComplete(() => StartCoroutine(PosterTwinkle())));
    }



    UnityWebRequest videoDownloadwww;

        //download loaded video...
    public void DownloadVideo(string path)
    {
                if(urlFilePath.StartsWith("http"))
        {
        StartCoroutine(DownloadTheVideo(path));
        }
        else
        {
        Debug.Log("Image is a local file!");
        }
    }

    IEnumerator DownloadTheVideo(string path)
    {
        GetComponent<Collider>().enabled = false; //to prevent from scanning again while downloading video


        videoDownloadwww = new UnityWebRequest(urlFilePath);



        // Validate and sanitize path
        if (string.IsNullOrWhiteSpace(path) || path.Contains("..") || Path.IsPathRooted(path))
        {
            throw new UnauthorizedAccessException("Invalid path.");
        }



        string filepathName = urlFilePath; // so that the function underneath dosnt mess up the base filepath.

        if(filepathName.Contains(".webm")) //this should be done with fileName instead! but theres a problem!
        {
            int index = filepathName.LastIndexOf(".webm");
            filepathName = filepathName.Substring (0, index + 5); //4 characters in '.jpg'    LATER use a better function to check
        }
        if(filepathName.Contains(".mp4")) 
        {
            int index = filepathName.LastIndexOf(".mp4");
            filepathName = filepathName.Substring (0, index + 4); 
        }

        string fileName = filepathName.Split('/')[filepathName.Split('/').Length - 1];
        
        fileName = fileName.Replace("=webm", ".webm");
        fileName = fileName.Replace("=mp4", ".mp4");
        fileName = fileName.Replace("?", "");
        fileName = fileName.Replace("=", "");

        fileName = fileName.ToLower();



        if(!Directory.Exists(path))
        {    
            Directory.CreateDirectory(path);
        }


        videoDownloadwww.downloadHandler = new DownloadHandlerFile(path + fileName);
        yield return videoDownloadwww.SendWebRequest();
        if (videoDownloadwww.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(videoDownloadwww.error);
        }
        else
        {
            Debug.Log("File successfully downloaded and saved to " + path + fileName);
            GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=green>File Succesfully downloaded!");
            transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.3f).OnComplete(() => transform.DOScale(new Vector3(0f, 0f, 0f), 0.2f).OnComplete(() => StartCoroutine(PosterTwinkle())));

        }
        
    }


    IEnumerator PosterTwinkle()
    {

        
        GameObject particleSystemTwinkle = GameObject.Find("PARTICLE_POSTERTWINKLE");

        particleSystemTwinkle.transform.position = transform.position; //particle happens at the location where poster shrunk to
        particleSystemTwinkle.GetComponent<ParticleSystem>().Play();
        particleSystemTwinkle.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
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
 public IEnumerator LoadImageLocal(bool loadFromFolderDir) //FIX!!! ADD A TIMER IF THE IMAGE TAKES TOO LONG, FAIL...
 {
     yield return new WaitForSeconds(0.1f); //problem if you dont inclide this... (due to setting image variable dimensions on Start())

     byte[] imgByte;
     if(loadFromFolderDir)
     {
                    imgByte = File.ReadAllBytes(FetchRandomImageFromFilePath());
     }
     else
     {
                    imgByte = File.ReadAllBytes(urlFilePath);
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

        if(GameObject.Find("ItemEditStateMachine").GetComponent<ItemEditStateMachine>().currentObjectEditing != null)
        {
        GameObject.Find("ItemEditStateMachine").GetComponent<ItemEditStateMachine>().successfullImageUploadEventHandler.SuccessfulLocalImageUploadSoundEvent();
        }

        width = imageToTestFrom.width;
        height = imageToTestFrom.height;
        image = new Vector2(width,height);

        GetComponent<PosterFrameList>().AssignWhichFrameIsLast();
        GetComponent<PosterFrameList>().AssignWhichMeshesFramesDrawAround();

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

      //  meshRenderer.sharedMaterial = videoStaticMat;

        yield return new WaitForSeconds(0.1f); //problem if you dont inclide this... (due to setting image variable dimensions on Start())

        //do...
        width = box.x;
        height = box.y;
        image = new Vector2(width, height);
        
        BuildPoster();


        yield return new WaitForSeconds(0.1f); //problem if you dont inclide this... (due to setting image variable dimensions on Start())

    //problem with this, it seems the poster frame does not update when using a coroutine...
        foreach(GameObject frame in GetComponent<PosterFrameList>().posterFrameList)// makes sure the frames get built, AFTER the poster is created, THIS IS A QUICK AND DIRTY SOLUTION, IN REALITY. THIS WHOLE FUNCTION SHOULD BE A COROUTINE!!!
        {
            frame.GetComponent<PosterMeshCreator_BorderFrame>().UpdateFrame();
        }
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
        meshRenderer.sharedMaterial = roomPreviewMat;

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


 }
