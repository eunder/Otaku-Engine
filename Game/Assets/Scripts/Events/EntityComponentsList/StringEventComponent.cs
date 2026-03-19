using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class StringEventComponent : MonoBehaviour
{
    public void SetString(string text)
    {
        GetComponent<StringEntity>().currentString = text;

        GetComponent<WidgetInfo>().info = GetComponent<StringEntity>().currentString;
        GlobalParameterManager.Instance.SaveGlobalEntities();
    }




    //HOW THESE ON EVENTS WORK: goes through each event in the obj passed-in list
    //note.. the "GlobalUtilityFunctions.InsertVariableValuesInText" is used to convert variables inside <> into values

    public void CheckAndTriggerStringLogic(GameObject fromObj)
    {
        OnStringEquals(fromObj);
        OnStringEquals_False(fromObj);
        OnStringEquals_CaseSensitive(fromObj);
        OnStringEquals_CaseSensitive_False(fromObj);
        OnStringContains(fromObj);
        OnStringContains_False(fromObj);
        OnStringStartsWith(fromObj);
        OnStringStartsWith_False(fromObj);
        OnStringEndsWith(fromObj);
        OnStringEndsWith_False(fromObj);
    }



    public void OnStringEquals(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnStringEquals")
            {
                if(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater).Equals(GetComponent<StringEntity>().currentString, StringComparison.OrdinalIgnoreCase))
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }

    public void OnStringEquals_False(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnStringEquals_False")
            {
                if(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater).Equals(GetComponent<StringEntity>().currentString, StringComparison.OrdinalIgnoreCase) == false)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }

    public void OnStringEquals_CaseSensitive(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnStringEquals_CaseSensitive")
            {
                if(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater) == GetComponent<StringEntity>().currentString)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }
    public void OnStringEquals_CaseSensitive_False(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnStringEquals_CaseSensitive_False")
            {
                if((GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater) == GetComponent<StringEntity>().currentString) == false)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }
    
    public void OnStringContains(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnStringContains")
            {
                if(GetComponent<StringEntity>().currentString.Contains(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater), StringComparison.OrdinalIgnoreCase))
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }


    public void OnStringContains_False(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnStringContains_False")
            {
                if(GetComponent<StringEntity>().currentString.Contains(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater), StringComparison.OrdinalIgnoreCase) == false)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }

    public void OnStringEndsWith(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnStringEndsWith")
            {
                if(GetComponent<StringEntity>().currentString.EndsWith(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater), StringComparison.OrdinalIgnoreCase))
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }

    public void OnStringEndsWith_False(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnStringEndsWith_False")
            {
                if(GetComponent<StringEntity>().currentString.EndsWith(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater), StringComparison.OrdinalIgnoreCase) == false)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }



    public void OnStringStartsWith(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnStringStartsWith")
            {
                if(GetComponent<StringEntity>().currentString.StartsWith(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater), StringComparison.OrdinalIgnoreCase))
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }


    public void OnStringStartsWith_False(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnStringStartsWith_False")
            {
                if(GetComponent<StringEntity>().currentString.StartsWith(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater), StringComparison.OrdinalIgnoreCase) == false)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }



    public void ResetToDefault()
    {
        GetComponent<StringEntity>().currentString = GetComponent<StringEntity>().currentString_default;
    }


}
