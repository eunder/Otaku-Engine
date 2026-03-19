using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnPlayerEnterEvent : MonoBehaviour
{

    public UnityEvent myEvent;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            myEvent.Invoke();
            GetComponent<BoxCollider>().enabled = false;
        }
    }

}
