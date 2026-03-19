using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEventEntityComponent : MonoBehaviour
{
    private static GlobalEventEntityComponent _instance;
    public static GlobalEventEntityComponent Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public void PopGlobalEvent(string doParameter)
    {
        //check global entity

        foreach(SaveAndLoadLevel.Event e in GlobalEventEntityComponent.Instance.GetComponent<EventHolderList>().events)
        {
            if(e.onAction == "OnGlobalEvent" && e.onParamater == doParameter)
            {
                EventActionManager.Instance.TryPlayEvent_Single(e);
            }
        }
    }
}
