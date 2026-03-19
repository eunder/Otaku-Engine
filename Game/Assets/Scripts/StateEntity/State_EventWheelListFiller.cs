using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_EventWheelListFiller : MonoBehaviour
{
    //creates, sets the values, and adds the elements to the wheel picker list(after these are executed. the wheel picker gets refreshed in another script)

    public GameObject wheelPickerEventButtonStateOn_Prefab;
    public GameObject wheelPickerEventButtonStateDo_Prefab;


    public void CreateAndFillWheelEventListWithStateNames_On()
    {

        foreach(string state in GetComponent<State>().states)
        {
            GameObject wheelObjectState = Instantiate(wheelPickerEventButtonStateOn_Prefab, Vector3.zero, Quaternion.identity);
            wheelObjectState.GetComponent<ToolTipTextHolder>().ObjectName = "OnDialogueChoice - " + state;
            wheelObjectState.GetComponent<WheelPickerReturnEventNameOnClick>().eventParameter = state;
            wheelObjectState.transform.SetParent(WheelPickerHandler.Instance.Objectlist.transform);
      
            WheelPickerHandler.Instance.AddToWheelPickerList(wheelObjectState);

        }
    }


 /*
    public void CreateAndFillWheelEventListWithStateNames_Do()
    {

       
        foreach(string state in GetComponent<State>().states)
        {
            GameObject wheelObjectState = Instantiate(wheelPickerEventButtonStateDo_Prefab, Vector3.zero, Quaternion.identity);
            wheelObjectState.GetComponent<ToolTipTextHolder>().ObjectName = "Set State - " + state;
            wheelObjectState.GetComponent<WheelPickerReturnEventNameOnClick>().eventParameter = state;
            wheelObjectState.transform.SetParent(WheelPickerHandler.Instance.Objectlist.transform);

            WheelPickerHandler.Instance.AddToWheelPickerList(wheelObjectState);
        }

           
    }

     */
}
