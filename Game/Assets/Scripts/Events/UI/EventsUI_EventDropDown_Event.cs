using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EventsUI_EventDropDown_Event : MonoBehaviour
{    
    public TMP_Dropdown eventOn_DropDown;
    public TMP_InputField onEventParameter_Inputfield;
    public TMP_InputField doEventParameter_Inputfield;
    public TMP_Dropdown eventDo_DropDown;
    public TMP_InputField eventDelay_Inputfield;
    public Toggle eventTriggerOnce_Toggle;

     EventHolderList eventList;
    SaveAndLoadLevel.Event objEvent;


    //these are used because the state events wont show up, because they are not on the wheel list. the icons need to be added manually
    public Sprite OnStateTrigger_Icon;
   // public Sprite SetState_Icon;

    public void UpdateOnDropDown(EventHolderList list)
    {
        eventOn_DropDown.ClearOptions();

        PopulateDropDownFromGameObjectPrefab(eventOn_DropDown, list.wheelPickerEventList_On.transform);


        //used because the state events dont show up...
        foreach(SaveAndLoadLevel.Event ev in list.events)
        {
            if(ev.onAction == "OnDialogueChoice")
            {
                var option = new TMP_Dropdown.OptionData("OnDialogueChoice", OnStateTrigger_Icon);
                eventOn_DropDown.options.Add(option);
                break;
            }
            
        }

    }

    public void UpdateDoDropDown(EventHolderList list, SaveAndLoadLevel.Event e)
    {
        eventDo_DropDown.ClearOptions();

        //Find object
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if(go.name == e.id)
            {   
                //if it is a pointer
                if(go.GetComponent<GlobalParameterPointerEntity>() && go.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity())
                {
                    //when the object is found... populate the drop down like in the previous function
                    foreach(Transform child in go.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponentInChildren<EventHolderList>().wheelPickerEventList_Do.transform)
                    {                                                     // wheelpicker event name                                            wheelpicker event image
                        var option = new TMP_Dropdown.OptionData(child.GetComponent<WheelPickerReturnEventNameOnClick>().eventCoroutineName, child.GetComponent<Image>().sprite);
                        eventDo_DropDown.options.Add(option);
                    }
                }
                else
                {
                    if(go.GetComponentInChildren<EventHolderList>().wheelPickerEventList_Do)
                    {
                        PopulateDropDownFromGameObjectPrefab(eventDo_DropDown, go.GetComponentInChildren<EventHolderList>().wheelPickerEventList_Do.transform);
                    }
                }
                break;
            }
        }

        /*
        //used because the state events dont show up...
        foreach(SaveAndLoadLevel.Event ev in list.events)
        {
            if(ev.doAction == "SetState")
            {
                var option = new TMP_Dropdown.OptionData("SetState", SetState_Icon);
                eventDo_DropDown.options.Add(option);
                break;
            }
            
        }
*/
    }


    //loops through each objects in the "source" and checks to see if there are more expandable lists (like path event group) to populate the drop down with...
    void PopulateDropDownFromGameObjectPrefab(TMP_Dropdown dropdown, Transform source)
    {
        dropdown.ClearOptions();

        foreach(Transform child in source)
        {    
            //if there is an expandable list group... then loop through that one...                                
            if(child.GetComponent<WheelPickerButtonEvent_OpenNewMenu>())
            {
                foreach(Transform child2 in child.GetComponent<WheelPickerButtonEvent_OpenNewMenu>().menuObjectList.transform)
                { 
                                                                // wheelpicker event name                                            wheelpicker event image
                var option = new TMP_Dropdown.OptionData(child2.GetComponent<WheelPickerReturnEventNameOnClick>().eventCoroutineName, child2.GetComponent<Image>().sprite);
                dropdown.options.Add(option);
                }    
            }
            else
            {
                                                                // wheelpicker event name                                            wheelpicker event image
                var option = new TMP_Dropdown.OptionData(child.GetComponent<WheelPickerReturnEventNameOnClick>().eventCoroutineName, child.GetComponent<Image>().sprite);
                dropdown.options.Add(option);
            }
        }

    }







    public void UpdateEventDropDown(EventHolderList list,SaveAndLoadLevel.Event e)
    {
        eventList = list;
        objEvent = e;
        UpdateOnDropDown(list);

        onEventParameter_Inputfield.text = e.onParamater;


        UpdateDoDropDown(list, e);
        doEventParameter_Inputfield.text = e.doParameter;



        eventDelay_Inputfield.text = e.delay.ToString();

        eventTriggerOnce_Toggle.isOn = e.happenOnce;

        //set the dropdown values based on event strings
        eventOn_DropDown.value = eventOn_DropDown.options.FindIndex(option => option.text == e.onAction);
        eventDo_DropDown.value = eventDo_DropDown.options.FindIndex(option => option.text == e.doAction);
    }

    public void UpdateEventOn_OnDropDownValueChange()
    {
        if(eventOn_DropDown.options.Count >= 1) //to prevent out of range errors when certain events for an entity simply dont exist
        objEvent.onAction = eventOn_DropDown.options[eventOn_DropDown.value].text;
    }

    public void UpdateEventDo_OnDropDownValueChange()
    {
        if(eventDo_DropDown.options.Count >= 1) //to prevent out of range errors when certain events for an entity simply dont exist
        objEvent.doAction = eventDo_DropDown.options[eventDo_DropDown.value].text;
    }

    public void UpdateEventDelayOnInputFieldChange()
    {
        objEvent.delay = float.Parse(eventDelay_Inputfield.text);
    }


    public void UpdateEventhappenOnceOnToggle(bool isEnabled)
    {
        objEvent.happenOnce = isEnabled;
    }

    public void UpdateOnParameter(string parameter)
    {
        objEvent.onParamater = parameter;
    }

    public void UpdateDoParameter(string parameter)
    {
        objEvent.doParameter = parameter;
    }


    public GameObject onParameterInputField;
    public GameObject doParameterInputField;

    public void ShowOrHideParameterBoxDependingOnEventType()
    {
       

    }




    public void OnHoverRecolorLine()
    {
        if(eventList.events.IndexOf(objEvent) < eventList.events.Count)
        {
        //set color to red(used the index of the obj)
        eventList.lineRenderers[eventList.events.IndexOf(objEvent)].startColor = EventLineStylingManager.Instance.eventLineColor_Hover;
        eventList.lineRenderers[eventList.events.IndexOf(objEvent)].endColor = EventLineStylingManager.Instance.eventLineColor_Hover;

        eventList.lineRenderers[eventList.events.IndexOf(objEvent)].startWidth = EventLineStylingManager.Instance.eventLineWidth_Hover;
        eventList.lineRenderers[eventList.events.IndexOf(objEvent)].endWidth = EventLineStylingManager.Instance.eventLineWidth_Hover;
        }
    }

    public void OnExitHoverRecolorLine()
    {

        if(eventList.events.IndexOf(objEvent) < eventList.events.Count)
        {
        //set color to red(used the index of the obj)
        eventList.lineRenderers[eventList.events.IndexOf(objEvent)].startColor = EventLineStylingManager.Instance.eventLineColorStart_Highlight;
        eventList.lineRenderers[eventList.events.IndexOf(objEvent)].endColor = EventLineStylingManager.Instance.eventLineColorEnd_Highlight;

        eventList.lineRenderers[eventList.events.IndexOf(objEvent)].startWidth = EventLineStylingManager.Instance.eventLineWidthStart_Standard;
        eventList.lineRenderers[eventList.events.IndexOf(objEvent)].endWidth = EventLineStylingManager.Instance.eventLineWidthEnd_Standard;
        }
    }

    public void EraseEventDropDown()
    {
        if(eventList.events.IndexOf(objEvent) < eventList.lineRenderers.Count)
        {
        Destroy(eventList.lineRenderers[eventList.events.IndexOf(objEvent)].gameObject);
        eventList.lineRenderers.Remove(eventList.lineRenderers[eventList.events.IndexOf(objEvent)]);
        }

        eventList.events.Remove(objEvent);

        Destroy(gameObject);
    }

    
    public float closedHeight;
    public float expandedHeight;

    public bool isExpanded = false;

    public GameObject eventParameterParentHolder;
    //if the event has parameters... let players expand on click
    public void ExpandEventDropDown()
    {
        if(isExpanded == false)
        {
        isExpanded = true;
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, expandedHeight);
        eventParameterParentHolder.SetActive(true);
        }
        else
        {
        isExpanded = false;
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, closedHeight);
        eventParameterParentHolder.SetActive(false);
        }
    }





}
