using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeDependingOnCameraDistance : MonoBehaviour
{


    float size;
    public float scale = 0.10f;
    public Camera playerCam;

    // Update is called once per frame
    void Update()
    {
            //dynamic scale according to camera
            size = ((playerCam.transform.position - transform.position).magnitude) * scale;
            transform.localScale = new Vector3(size,size,size);
    }
}
