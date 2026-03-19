using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnPlayerClick : MonoBehaviour
{
    public UnityEvent myEvent;

    public void onClickElementActivateEvent()
    {
        myEvent.Invoke();
    }
}
