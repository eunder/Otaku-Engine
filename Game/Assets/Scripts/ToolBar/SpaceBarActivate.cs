using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBarActivate : MonoBehaviour //used for find functions to be able to find the componenets of deactivated objects...
{
public GameObject toolbar;

        SimpleSmoothMouseLook mouseLooker;
    void Start()
    {
                mouseLooker = GameObject.Find("MainPlayerCam").GetComponent<SimpleSmoothMouseLook>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mouseLooker != null)
        {
            if(Input.GetKeyDown(KeyCode.C))
            {
               Cursor.lockState = CursorLockMode.None;
               mouseLooker.wheelPickerIsTurnedOn = true;
               toolbar.SetActive(true);
            
            }

           if(Input.GetKeyUp(KeyCode.C))
            {
            Cursor.lockState = CursorLockMode.Locked;
            mouseLooker.wheelPickerIsTurnedOn = false;
            toolbar.SetActive(false);
            }
        }
    }
}
