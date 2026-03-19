using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelPickerManager : MonoBehaviour
{
    public string filePathToFetchLevelsFrom;
    public int currentLevelIndex = 0;
    public  int fileCount = 0;
    public List<string> mapFilesInFilePath = new List<string>();  // list of files found in a folder(that also meet the file type criteria)

    public List<GameObject> previewBlockList = new List<GameObject>();
    public void GetMapFilesAvailableFromFilePath()
    {
        var filters = new string[] { "json" };
        foreach (var filter in filters)
        {
        mapFilesInFilePath.AddRange(Directory.GetFiles(filePathToFetchLevelsFrom, string.Format("*.{0}", filter)));
        }
    
       fileCount = mapFilesInFilePath.Count;
    }

    public void NextAvailableLevel()
    {
        if(currentLevelIndex < mapFilesInFilePath.Count-1)
        {
        currentLevelIndex++;
        GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = mapFilesInFilePath[currentLevelIndex];
        LoadLevelPreview();
        }
    }
    public void PreviousAvailableLevel()
    {
        if(currentLevelIndex > 0)
        {
        currentLevelIndex--;
        GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = mapFilesInFilePath[currentLevelIndex];
        LoadLevelPreview();
        }
    }

  [Header("Camera Orbit Settings")]
          public Transform levelCenter;
         public float distance = 2.0f;
         public float xSpeed = 20.0f;
         public float ySpeed = 20.0f;
         public float yMinLimit = -90f;
         public float yMaxLimit = 90f;

         public float distanceMin = 10f;
         public float distanceMax = 10f;
         public float distnaceIncrement = 0.4f;
         public float smoothTime = 2f;
         float rotationYAxis = 0.0f;
         float rotationXAxis = 0.0f;
         float velocityX = 0.0f;
         float velocityY = 0.0f;
         public float mouseScroll = 1.0f;
         public Transform levelPreviewCamera;


    public Vector3 GetAverageVectorOfAllBlocks() //for "correct" camera positioning
    {
        
     float x = 0f;
     float y = 0f;
     float z = 0f;
     foreach (GameObject block in previewBlockList)
     {
         x += block.transform.position.x;
         y += block.transform.position.y;
         z += block.transform.position.z;
     }
     return new Vector3(x / previewBlockList.Count, y / previewBlockList.Count, z / previewBlockList.Count);
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene("PlayerRoom", LoadSceneMode.Single);
    }

    public GameObject blockPrefab;
    public SaveAndLoadLevel.LevelData loadedObjects;
    public Material blockPreviewMaterial;


    //map data
    public TextMeshProUGUI mapDataUGUI;
    private int numberOfBlocks = 0;
    private int numberOfVideos = 0;
    private int numberOfPictures = 0;

    public void LoadLevelPreview()
    {
        //clear list
        if(previewBlockList.Count >0)
        {
             foreach(GameObject block in previewBlockList)
     {
         Destroy(block);
     }
        previewBlockList.Clear(); //unoptimzed for garbage collection
        }


        //build object from json
        string json = File.ReadAllText(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp);
        loadedObjects = JsonUtility.FromJson<SaveAndLoadLevel.LevelData>(json);

        //load blocks
        foreach(SaveAndLoadLevel.BlockData block in loadedObjects.allLevelBlocks)
        {
            GameObject newBlock = GameObject.Instantiate(blockPrefab, block.blockPos, Quaternion.identity);
            newBlock.transform.GetComponent<Renderer>().material = blockPreviewMaterial;
            //assign to appropriate layer or else other logic wont detect
            newBlock.layer = 14;
            
            previewBlockList.Add(newBlock);
        }

        numberOfBlocks = loadedObjects.allLevelBlocks.Count;
        numberOfPictures = loadedObjects.allLevelPosters.Count;

        mapDataUGUI.text = "Map Info: \n" + "Block Count: " + numberOfBlocks + "\n" 
                                          + "Video Count: " + numberOfVideos + "\n"
                                          + "Image Count: " + numberOfPictures;


        //set camera target
       // levelPreviewCamera.transform.position = GetAverageVectorOfAllBlocks() + levelCenterCameraOffset;
        levelCenter.position = GetAverageVectorOfAllBlocks();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetMapFilesAvailableFromFilePath();

        //makes sure first loaded level is ready-up(without having to press buttons)
        GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = mapFilesInFilePath[currentLevelIndex];
        LoadLevelPreview();
    }

    // Update is called once per frame
    void Update()
    {
    //    levelPreviewCamera.transform.LookAt(levelCenter);
    //    levelPreviewCamera.transform.Translate(Vector3.right * Time.deltaTime);
    }



          void LateUpdate()
         {

            if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
            {
                distance -= distnaceIncrement;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
            {
                distance += distnaceIncrement;
            }

            if(distance > distanceMax)
            {
                distance = distanceMax;
            }
            if(distance < distanceMin)
            {
                distance = distanceMin;
            }



             if (levelCenter)
             {
                 if (Input.GetMouseButton(1))
                 {
                     velocityX += xSpeed * Input.GetAxis("Mouse X") * 0.02f;
                     velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
                 }
                 rotationYAxis += velocityX;
                 rotationXAxis -= velocityY;
                 rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
                 Quaternion fromRotation = Quaternion.Euler(levelCenter.rotation.eulerAngles.x, levelCenter.rotation.eulerAngles.y, 0);
                 Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
                 Quaternion rotation = toRotation;
     
                // distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
                 //RaycastHit hit;
                 //if (Physics.Linecast(target.position, transform.position, out hit))
                 //{
                //     distance -= hit.distance;
              //   }
                 Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                 Vector3 position = rotation * negDistance + levelCenter.position;
     
                 levelPreviewCamera.rotation = rotation;
                 levelPreviewCamera.position = position;
                 velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
                 velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
             }
         }
         public static float ClampAngle(float angle, float min, float max)
         {
             if (angle < -360F)
                 angle += 360F;
             if (angle > 360F)
                 angle -= 360F;
             return Mathf.Clamp(angle, min, max);
         }


}
