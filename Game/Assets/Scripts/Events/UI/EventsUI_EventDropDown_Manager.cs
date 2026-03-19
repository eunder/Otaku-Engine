using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsUI_EventDropDown_Manager : MonoBehaviour
{
    private static EventsUI_EventDropDown_Manager _instance;
    public static EventsUI_EventDropDown_Manager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public Transform eventList_Container;
    public GameObject eventList_Prefab;

    List<GameObject> eventUIList = new List<GameObject>();

    public void FillEventList(EventHolderList list)
    {
        EraseList();


        //sort the list of events by delay
        list.events.Sort((x, y) => x.delay.CompareTo(y.delay));


        foreach(SaveAndLoadLevel.Event e in list.events)
        {
            GameObject eventWindowPrefab = Instantiate(eventList_Prefab);
            eventWindowPrefab.GetComponent<EventsUI_EventDropDown_Event>().UpdateEventDropDown(list,e);
            eventWindowPrefab.transform.SetParent(eventList_Container);
            eventUIList.Add(eventWindowPrefab);
        }
    }

    public void EraseList()
    {
        foreach(GameObject obj in eventUIList)
        {
            Destroy(obj);
        }
        eventUIList.Clear();
    }

    public GameObject close_Button;

    public void EnableListEditMode()
    {
                close_Button.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                SimpleSmoothMouseLook.Instance.enabled = false;
    }

    public void DisableListEditMode()
    {
                close_Button.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                SimpleSmoothMouseLook.Instance.enabled = true;
                PlayerObjectInteractionStateMachine.Instance.editModeOpen = false;
    }


}
