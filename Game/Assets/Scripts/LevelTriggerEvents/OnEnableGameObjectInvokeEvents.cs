using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnableGameObjectInvokeEvents : MonoBehaviour
{

    public UnityEvent myEvent;

    void OnEnable()
    {
        myEvent.Invoke();
    }

}
