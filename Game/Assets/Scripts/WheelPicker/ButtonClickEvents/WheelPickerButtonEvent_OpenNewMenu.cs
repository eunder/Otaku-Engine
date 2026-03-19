using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPickerButtonEvent_OpenNewMenu : MonoBehaviour
{
    public GameObject menuObjectList;
 
    public void OpenNewList()
    {
        GameObject wheelPicker = GameObject.Find("CANVAS_WHEELPICKER");
        wheelPicker.GetComponent<WheelPickerHandler>().Objectlist = menuObjectList;
        wheelPicker.GetComponent<WheelPickerHandler>().BuildWheelPickerFromList();
    }

}
