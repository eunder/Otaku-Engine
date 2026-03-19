using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class PosterPlacementEdit : MonoBehaviour
{

    Vector3 currentNormal; //if this changes while editing, put out a warning that the player is not on the same wall
    public LayerMask collidableLayers_layerMask;

    Vector3 startPoint;
    Vector3 endPoint;

    public Texture2D imageToTestFrom;
    public Vector2 imageDimesions;
    public float aspectQuotient = 0.0f;   //height / width = this number

    public Transform testQuadPos;
    RaycastHit hitInfo;

    public string filePath;

    public GameObject previewGameObject;
    public Material previewMaterial;
    float rotationOffset = 0f;
    void Update()
    {

          if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hitInfo, Mathf.Infinity, collidableLayers_layerMask))
         {  
             currentNormal = hitInfo.normal;
             startPoint = hitInfo.point;


         }
      


           if(Input.GetMouseButtonDown(0))
        {
             testQuadPos.position = previewGameObject.transform.position;
             testQuadPos.rotation = previewGameObject.transform.rotation;

            Destroy(previewGameObject);

        }



                if(Input.GetMouseButtonDown(1))
        {
          //upload image from filePath
          byte[] imgByte = File.ReadAllBytes(filePath);
          imageToTestFrom.LoadImage(imgByte);

          //asssign image to appropriate Object
          testQuadPos.GetComponent<Renderer>().material.mainTexture = imageToTestFrom;

            //change scale depending on image dimensions(making sure to keep the aspect ratio)

            imageDimesions = new Vector2(imageToTestFrom.width, imageToTestFrom.height);
            aspectQuotient = imageDimesions.y / imageDimesions.x;

            testQuadPos.localScale = new Vector3(imageDimesions.x * 0.001f,imageDimesions.y * 0.001f,0.01f);

          //create preview image and assign preview material & texture       
          previewGameObject = Instantiate(testQuadPos.gameObject);
          previewGameObject.GetComponent<Renderer>().material = previewMaterial;
          previewGameObject.GetComponent<Renderer>().material.mainTexture = imageToTestFrom;


        }


        //rotate preview object
          if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
        {
rotationOffset+= 15.0f ;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
        {
rotationOffset -= 15.0f;        }




          //object preview positioning
          if(previewGameObject)
          {
          previewGameObject.transform.position = hitInfo.point;
          previewGameObject.transform.rotation = Quaternion.LookRotation(hitInfo.normal) * Quaternion.AngleAxis(rotationOffset, Vector3.forward);
          }
    }


  public void CreateQuadFromImageResolution()
  {
  }

}
