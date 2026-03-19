using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDebugMenu : MonoBehaviour
{
    public GameObject debugPanel;
    bool toggle = false;
    // Update is called once per frame
    void Update()
    {
          if (Input.GetKeyDown("`")) {
    if(toggle == true)
    {
        debugPanel.SetActive(true);
        toggle = false;
    }
    else
    {
        debugPanel.SetActive(false);
        toggle = true;    
    }
  }
    }
}
