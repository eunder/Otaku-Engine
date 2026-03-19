using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ImageLoaderFromFile : MonoBehaviour
{
    public List<Texture2D> customTextures = new List<Texture2D>();
    // Start is called before the first frame update
    void Start()
    {
        Invoke("LoadImages", 1.0f);  // HAVE A PARAMETER TO CHECK IF IT IS DONE INSTEAD OF TIME?
    }

    void LoadImages()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] info = dir.GetFiles("*.png*");
        foreach (FileInfo f in info) 
        {
            Texture2D tex = null;
            byte[] fileData;
            fileData = File.ReadAllBytes(Application.persistentDataPath +"/"+ f.Name);
            tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
            tex.LoadImage(fileData);


            string fileNamewithoutExtension = System.IO.Path.GetFileNameWithoutExtension(Application.persistentDataPath +"/"+ f.Name);

            for(int index = 0; index < GetComponent<HomeObjectList>().allItems.Length; index++)
            {
                           
            if(fileNamewithoutExtension == GetComponent<HomeObjectList>().allItems[index].transform.name)
            {
                if(GetComponent<HomeObjectList>().allItems[index].transform.GetComponentInChildren<SkinnedMeshRenderer>())
                {
                GetComponent<HomeObjectList>().allItems[index].transform.GetComponentInChildren<SkinnedMeshRenderer>().material.SetTexture("_MainTex", tex);
                GetComponent<HomeObjectList>().allItems[index].transform.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture.filterMode = FilterMode.Point;
                }
                if(GetComponent<HomeObjectList>().allItems[index].transform.GetComponentInChildren<MeshRenderer>())
                {
                GetComponent<HomeObjectList>().allItems[index].transform.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", tex);
                GetComponent<HomeObjectList>().allItems[index].transform.GetComponentInChildren<MeshRenderer>().material.mainTexture.filterMode = FilterMode.Point;
                }
                            tex.name = fileNamewithoutExtension;

            }
            }

            customTextures.Add(tex);
            Debug.Log(f.Name);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
