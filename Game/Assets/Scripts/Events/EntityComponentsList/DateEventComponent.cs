using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateEventComponent : MonoBehaviour
{

    public void DateTrigger(GameObject fromObj, SaveAndLoadLevel.Event e)
    {

        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            //if the onAction event is found, execute a coroutine called the same as onAction string
            if(ev.onParamater == gameObject.GetComponent<State>().currentState)
            {
                Debug.Log("State match triggered!");
                EventActionManager.Instance.TryPlayEvent_Single(ev);
            }
        }

    }

    //note.. the "GlobalUtilityFunctions.InsertVariableValuesInText" is used to convert variables inside <> into values

    public void OnMonth(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnMonth")
            {
                int currentMonth = GetComponent<Date>().date.Month;

                if(int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) == currentMonth)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }   
    }

    public void OnDate(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnDate")
            {
                int currentDate = GetComponent<Date>().date.Day;

                if(int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) == currentDate)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }  
    }


    public void OnHour(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnHour")
            {
                int currentHour = GetComponent<Date>().date.Hour;

                if(int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) == currentHour)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }  
    }
    public void OnMinute(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnMinute")
            {
                int currentMinute = GetComponent<Date>().date.Minute;

                if(int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) == currentMinute)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }  
    }






    //HOW THESE ON EVENTS WORK: goes through each event in the obj passed-in list

    public void CheckAndTriggerDateLogic(GameObject fromObj)
    {
        OnMinutesPassedSinceTime_IsGreaterThanOrEqualTo(fromObj);
        OnMinutesPassedSinceTime_IsGreaterThanOrEqualTo_False(fromObj);
        OnMinutesPassedSinceTime_IsLessThanOrEqualTo(fromObj);
        OnMinutesPassedSinceTime_IsLessThanOrEqualTo_False(fromObj);
        OnHoursPassedSinceTime_IsGreaterThanOrEqualTo(fromObj);
        OnHoursPassedSinceTime_IsGreaterThanOrEqualTo_False(fromObj);
        OnHoursPassedSinceTime_IsLessThanOrEqualTo(fromObj);
        OnHoursPassedSinceTime_IsLessThanOrEqualTo_False(fromObj);

        OnMonth(fromObj);
        OnDate(fromObj);
        OnHour(fromObj);
        OnMinute(fromObj);
    }





    public void OnMinutesPassedSinceTime_IsGreaterThanOrEqualTo(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnMinutesPassedSinceTime_IsGreaterThanOrEqualTo")
            {
                // Calculate the time difference in minutes
                TimeSpan timePassed = DateTime.Now - GetComponent<Date>().date;
                int minutesPassed = (int)timePassed.TotalMinutes;

                if(int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) <= minutesPassed)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }
    public void OnMinutesPassedSinceTime_IsGreaterThanOrEqualTo_False(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnMinutesPassedSinceTime_IsGreaterThanOrEqualTo_False")
            {
                // Calculate the time difference in minutes
                TimeSpan timePassed = DateTime.Now - GetComponent<Date>().date;
                int minutesPassed = (int)timePassed.TotalMinutes;

                if((int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) <= minutesPassed) == false)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }

    public void OnMinutesPassedSinceTime_IsLessThanOrEqualTo(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnMinutesPassedSinceTime_IsLessThanOrEqualTo")
            {
                // Calculate the time difference in minutes
                TimeSpan timePassed = DateTime.Now - GetComponent<Date>().date;
                int minutesPassed = (int)timePassed.TotalMinutes;

                if(int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) >= minutesPassed)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }
    public void OnMinutesPassedSinceTime_IsLessThanOrEqualTo_False(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnMinutesPassedSinceTime_IsLessThanOrEqualTo_False")
            {
                // Calculate the time difference in minutes
                TimeSpan timePassed = DateTime.Now - GetComponent<Date>().date;
                int minutesPassed = (int)timePassed.TotalMinutes;

                if((int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) >= minutesPassed) == false)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }

    public void OnHoursPassedSinceTime_IsGreaterThanOrEqualTo(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnHoursPassedSinceTime_IsGreaterThanOrEqualTo")
            {
                // Calculate the time difference in minutes
                TimeSpan timePassed = DateTime.Now - GetComponent<Date>().date;
                int wholeHours = (int)timePassed.TotalHours;

                if(int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) <= wholeHours)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }
    public void OnHoursPassedSinceTime_IsGreaterThanOrEqualTo_False(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnHoursPassedSinceTime_IsGreaterThanOrEqualTo_False")
            {
                // Calculate the time difference in minutes
                TimeSpan timePassed = DateTime.Now - GetComponent<Date>().date;
                int wholeHours = (int)timePassed.TotalHours;

                if((int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) <= wholeHours) == false)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }
    public void OnHoursPassedSinceTime_IsLessThanOrEqualTo(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnHoursPassedSinceTime_IsLessThanOrEqualTo")
            {
                // Calculate the time difference in minutes
                TimeSpan timePassed = DateTime.Now - GetComponent<Date>().date;
                int wholeHours = (int)timePassed.TotalHours;

                if(int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) >= wholeHours)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }
    public void OnHoursPassedSinceTime_IsLessThanOrEqualTo_False(GameObject fromObj)
    {
        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnHoursPassedSinceTime_IsLessThanOrEqualTo_False")
            {
                // Calculate the time difference in minutes
                TimeSpan timePassed = DateTime.Now - GetComponent<Date>().date;
                int wholeHours = (int)timePassed.TotalHours;

                if((int.Parse(GlobalUtilityFunctions.InsertVariableValuesInText(ev.onParamater)) >= wholeHours) == false)
                {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
                }
            }
        }
    }


    public void SetToCurrentDateAndTime()
    {
        GetComponent<Date>().date = DateTime.Now; 

        GetComponent<WidgetInfo>().info = GetComponent<Date>().date.ToString();
        GlobalParameterManager.Instance.SaveGlobalEntities();
    }

    public void ResetToDefault()
    {
        GetComponent<Date>().date = default(DateTime);

        GetComponent<WidgetInfo>().info = GetComponent<Date>().date.ToString();
    }

}
