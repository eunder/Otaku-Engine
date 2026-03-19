using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AssignImageToPoster : MonoBehaviour
{

    public void AssignImageFromFilePath(string filePath)
    {
          filePath.Replace("\\","/");
          byte[] imgByte = File.ReadAllBytes(filePath);
        Texture2D imageToTestFrom = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        imageToTestFrom.LoadImage(imgByte);

          //asssign image to appropriate Object
          GetComponent<Renderer>().material.mainTexture = imageToTestFrom;

            //change scale depending on image dimensions(making sure to keep the aspect ratio)
            Vector2 imageDimesions;
            imageDimesions = new Vector2(imageToTestFrom.width, imageToTestFrom.height);

            gameObject.transform.localScale = new Vector3(imageDimesions.x * 0.001f,1.0f,imageDimesions.y * 0.001f);
    }
 
}
