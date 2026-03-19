using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEventComponent : MonoBehaviour
{

    public IEnumerator SetState(SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);

        GetComponent<State>().currentState = e.doParameter;
        GlobalParameterManager.Instance.SaveGlobalEntities();
    }

    public IEnumerator StateTrigger(GameObject fromObj, SaveAndLoadLevel.Event e)
    {
        yield return new WaitForSeconds(0);



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

    public IEnumerator SetNextState()
    {
        yield return new WaitForSeconds(0);

        //get index of current state
        int indexOfCurrentElement = GetComponent<State>().states.IndexOf(GetComponent<State>().currentState);

        //set the next state as the current state via (index + 1)
        if(indexOfCurrentElement + 1 < GetComponent<State>().states.Count)
        {
        GetComponent<State>().currentState = GetComponent<State>().states[indexOfCurrentElement +1];
        }
        else //set back to beginning: state 0 
        {
        GetComponent<State>().currentState = GetComponent<State>().states[0];
        }

        GlobalParameterManager.Instance.SaveGlobalEntities();
    }


    public IEnumerator SetPreviousState()
    {
        yield return new WaitForSeconds(0);

        //get index of current state
        int indexOfCurrentElement = GetComponent<State>().states.IndexOf(GetComponent<State>().currentState);

        //set the previous state as the current state via (index - 1)
        if(indexOfCurrentElement - 1 > -1)
        {
        GetComponent<State>().currentState = GetComponent<State>().states[indexOfCurrentElement -1];
        }
        else //if there is no previous state, it will get set to the last state 
        {
        GetComponent<State>().currentState = GetComponent<State>().states[GetComponent<State>().states.Count - 1];
        }


        GlobalParameterManager.Instance.SaveGlobalEntities();
    }


    public IEnumerator SetRandomState()
    {
        yield return new WaitForSeconds(0);

        GetComponent<State>().currentState = GetComponent<State>().states[Random.Range(0, GetComponent<State>().states.Count)];
        GlobalParameterManager.Instance.SaveGlobalEntities();
    }
    

}
