using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformArrow_GimbalGroup : MonoBehaviour
{

    public List<GameObject> arrowGroup = new List<GameObject>();

    public GameObject mainPlayerCam;

    float size;
    void Update()
    {
        //resizing the gimbal according to the camera
        foreach(GameObject obj in arrowGroup)
        {
            if(obj.GetComponent<TransformArrow_DragRotate>() && obj.GetComponent<TransformArrow_DragRotate>().beingHeld == true) // to prevent the rotation arrows from getting whack when dragging them
            {

            }
            else
            {
             size = ((mainPlayerCam.transform.position - obj.transform.position).magnitude) * 0.10f;
             obj.transform.localScale = new Vector3(size,size,size);
            }
        }

    }

    public void HideAllExceptThisArrow(GameObject arrowobj)
    {
        foreach(GameObject obj in arrowGroup)
        {
                obj.SetActive(false);
        }

            arrowobj.SetActive(true);

    }

    public void HideAllArrows()
    {
        foreach(GameObject obj in arrowGroup)
        {
                obj.SetActive(false);
        }
    }

    public void UnhideAllArrows()
    {
        foreach(GameObject obj in arrowGroup)
        {
                obj.SetActive(true);
        }
    }
}
