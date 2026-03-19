using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockEvents : MonoBehaviour
{

    public EventHolderList eventList;


    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && GetComponentInChildren<MeshCollider>().isTrigger)
        {
            Debug.Log("PLayer entered!");
            EventActionManager.Instance.TryPlayEvent(transform.gameObject, "OnPlayerEnter");
        }


        //loop through the events of this block...
        foreach(SaveAndLoadLevel.Event e in eventList.events)
        {
            //...and check if there is an OnObjectEnter AND that the collider id matches the onParameter ID
            if(e.onAction == "OnObjectEnter" && e.onParamater == other.gameObject.name)
            {
                EventActionManager.Instance.TryPlayEvent(transform.gameObject, "OnObjectEnter");
            }
        }

    }


    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player" && GetComponentInChildren<MeshCollider>().isTrigger)
        {
                        Debug.Log("PLayer ex!");

            EventActionManager.Instance.TryPlayEvent(transform.gameObject, "OnPlayerExit");
        }

        //loop through the events of this block...
        foreach(SaveAndLoadLevel.Event e in eventList.events)
        {
            //...and check if there is an OnObjectEnter AND that the collider id matches the onParameter ID
            if(e.onAction == "OnObjectExit" && e.onParamater == other.gameObject.name)
            {
                EventActionManager.Instance.TryPlayEvent(transform.gameObject, "OnObjectExit");
            }
        }


    }

    //First, there will be an attempt
    public void TryPlayEvent(string onAction)
    {
        foreach(SaveAndLoadLevel.Event e in GetComponent<EventHolderList>().events)
        {
            //if the onAction event is found, execute a coroutine called the same as onAction string
            if(onAction.Equals(e.onAction))
            {
                StartCoroutine(e.onAction, e);
            }
        }
    }

    
    IEnumerator OnClick(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(e.delay);

        //Execute the coroutine called the same as the stored doAction string
        StartCoroutine(e.doAction, e);
    }


    IEnumerator Hide(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);


        //Find object
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if(go.name == e.id)
            {
                go.gameObject.SetActive(false);
                break;
            }
        }
    }


    IEnumerator UnHide(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        var d = FindObjectsOfType(typeof(BlockFaceTextureUVProperties), true);

        //Find object
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if(go.name == e.id)
            {
                go.gameObject.SetActive(true);
                break;
            }
        }
    }
}
