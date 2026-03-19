using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{

    public Camera playerCam;
    
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - playerCam.transform.position);
     //   transform.rotation *= Quaternion.Euler(-90, 0, 0); 
    }
}

