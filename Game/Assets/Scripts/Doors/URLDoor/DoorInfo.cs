using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInfo : MonoBehaviour
{
    public string pathFileToLoad;
    
    public GameObject imageBackground;

    public GameObject stencilGroup;
    public GameObject backgroundGroup;


    public void DisableBackground()
    {
        stencilGroup.SetActive(true);
        backgroundGroup.SetActive(false);
    }

    public void EnableBackground()
    {
        stencilGroup.SetActive(false);
        backgroundGroup.SetActive(true);
    }

}
