using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventChangeSkyBox : MonoBehaviour
{

    public void ChangeSkyBox(Material newskyboxMat)
    {
        RenderSettings.skybox = newskyboxMat;
    }

}
