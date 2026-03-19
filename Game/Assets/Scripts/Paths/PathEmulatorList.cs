using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;

public class PathEmulatorList : MonoBehaviour
{

    //used to store the list of emulators currently affecting this gameobject
    public List<GameObject> pathEmulators = new List<GameObject>();
    
    //EVENTS
        //NOTE: these events loop through all of the path emulator list

    public IEnumerator StopPath() //cancels the paths
    {
        yield return new WaitForSeconds(0);

        foreach(GameObject obj in pathEmulators)
        {
            obj.GetComponent<splineMove>().Stop();
        }
    }

    public IEnumerator PausePath()
    {
        yield return new WaitForSeconds(0);

        foreach(GameObject obj in pathEmulators)
        {
            obj.GetComponent<splineMove>().Pause(); 
        }
    }

    public IEnumerator ResumePath()
    {
        yield return new WaitForSeconds(0);

        foreach(GameObject obj in pathEmulators)
        {
            obj.GetComponent<splineMove>().Resume();
        }
    }

    public IEnumerator ReverseDirection()
    {
        yield return new WaitForSeconds(0);

        foreach(GameObject obj in pathEmulators)
        {
            obj.GetComponent<splineMove>().Reverse();
        }
    }

}
