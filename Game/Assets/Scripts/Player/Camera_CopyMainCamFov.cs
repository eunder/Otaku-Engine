using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_CopyMainCamFov : MonoBehaviour
{
    public Camera mainCam;
    public Camera thisCam;
    void Update()
    {
        thisCam.fieldOfView = mainCam.fieldOfView;
    }
}
