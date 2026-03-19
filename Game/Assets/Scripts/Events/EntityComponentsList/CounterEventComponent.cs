using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterEventComponent : MonoBehaviour
{
    public void Increment()
    {
        GetComponent<Counter>().currentCount += 1;

        GetComponent<WidgetInfo>().info = GetComponent<Counter>().currentCount.ToString();
        GlobalParameterManager.Instance.SaveGlobalEntities();
    }



    public void Add(int count)
    {
        GetComponent<Counter>().currentCount += count;

        GetComponent<WidgetInfo>().info = GetComponent<Counter>().currentCount.ToString();
        GlobalParameterManager.Instance.SaveGlobalEntities();
    }


    public void Substract(int count )
    {
        GetComponent<Counter>().currentCount -= count;

        GetComponent<WidgetInfo>().info = GetComponent<Counter>().currentCount.ToString();
        GlobalParameterManager.Instance.SaveGlobalEntities();
    }

    public void SetCount(int count)
    {
        GetComponent<Counter>().currentCount = count;

        GetComponent<WidgetInfo>().info = GetComponent<Counter>().currentCount.ToString();
        GlobalParameterManager.Instance.SaveGlobalEntities();
    }

    public void SetCount_RandomRange(int minRange, int maxRange)
    {
        GetComponent<Counter>().currentCount = Random.Range(minRange, maxRange + 1);
        
        GetComponent<WidgetInfo>().info = GetComponent<Counter>().currentCount.ToString();
        GlobalParameterManager.Instance.SaveGlobalEntities();
    }




    //HOW THESE ON EVENTS WORK: goes through each event in the obj passed-in list
    //note.. the "GlobalUtilityFunctions.InsertVariableValuesInText" is used to convert variables inside <> into values

    public void CheckAndTriggerLogic(GameObject fromObj)
    {
        OnCounterIsGreaterThanOrEqualTo(fromObj);
        OnCounterIsGreaterThanOrEqualTo_False(fromObj);
        OnCounterIsLessThanOrEqualTo(fromObj);
        OnCounterIsLessThanOrEqualTo_False(fromObj);
        OnCounterEquals(fromObj);
        OnCounterEquals_False(fromObj);
    }



    public void OnCounterIsGreaterThan(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnCounterIsGreaterThan")
            {
                if(int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) < GetComponent<Counter>().currentCount)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }
    public void OnCounterIsGreaterThanOrEqualTo(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnCounterIsGreaterThanOrEqualTo")
            {
                if(int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) <= GetComponent<Counter>().currentCount)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }

    public void OnCounterIsGreaterThanOrEqualTo_False(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnCounterIsGreaterThanOrEqualTo_False")
            {
                if((int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) <= GetComponent<Counter>().currentCount) == false)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }


    public void OnCounterIsLessThan(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnCounterIsLessThan")
            {
                if(int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) > GetComponent<Counter>().currentCount)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }

    public void OnCounterIsLessThanOrEqualTo(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnCounterIsLessThanOrEqualTo")
            {
                if(int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) >= GetComponent<Counter>().currentCount)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }
    
    public void OnCounterIsLessThanOrEqualTo_False(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnCounterIsLessThanOrEqualTo_False")
            {
                if((int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) >= GetComponent<Counter>().currentCount) == false)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }


    public void OnCounterEquals(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnCounterEquals")
            {
                if(int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) == GetComponent<Counter>().currentCount)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }

    public void OnCounterEquals_False(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnCounterEquals_False")
            {
                if((int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) == GetComponent<Counter>().currentCount) == false)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }


    public void ResetToDefault()
    {
        GetComponent<Counter>().currentCount = GetComponent<Counter>().currentCount_default;
    }


}
