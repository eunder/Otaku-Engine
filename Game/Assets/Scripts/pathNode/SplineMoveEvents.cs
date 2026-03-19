using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using DG.Tweening;

public class SplineMoveEvents : MonoBehaviour
{

    public void OnPathReachEnd()
    {       //make sure to send the pathNode gameobject, NOT the gameobject moving
        EventActionManager.Instance.TryPlayEvent(GetComponent<splineMove>().pathContainer.gameObject, "OnPathReachEnd");
  

        //remove rigidbodywhen path has stopped
        if(GetComponent<GeneralObjectInfo>())
        {
            if(GetComponent<GeneralObjectInfo>().entityName == "Block")
            {
                if(gameObject.GetComponent<Rigidbody>())
                Destroy(gameObject.GetComponent<Rigidbody>());
            }
        }
    }

    public void OnPathReachStart()
    {       //make sure to send the pathNode gameobject, NOT the gameobject moving
        EventActionManager.Instance.TryPlayEvent(GetComponent<splineMove>().pathContainer.gameObject, "OnPathReachStart");

        //if you dont add a rigidbody to the block... then moving platforms wont work
        if(GetComponent<GeneralObjectInfo>())
        {
            if(GetComponent<GeneralObjectInfo>().entityName == "Block")
            {
                gameObject.AddComponent<Rigidbody>();
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    public void OnPathPassWaypoint()
    {       //make sure to send the pathNode gameobject, NOT the gameobject moving
        EventActionManager.Instance.TryPlayEvent(GetComponent<splineMove>().pathContainer.gameObject, "OnPathPassWaypoint");
    }

    

     //note... "UpdatePathProperties()" in PathNode is similar, make sure to update that when this is updated



    public bool reverse = false;

    //wiring tool events
    public IEnumerator PlayOnPath(string pathID)
    {
        yield return new WaitForSeconds(0);
        if(GetComponent<PathEmulatorList>())
        {
            foreach(GameObject obj in GetComponent<PathEmulatorList>().pathEmulators)
            {
                
            }
        }
        PathEmulatorManager.Instance.CreatePathEmulatorObject(gameObject,GameObject.Find(pathID), false, false, reverse);
    }

    public IEnumerator PlayOnPath_Reverse(string pathID)
    {
        yield return new WaitForSeconds(0);
        if(GetComponent<PathEmulatorList>())
        {
            foreach(GameObject obj in GetComponent<PathEmulatorList>().pathEmulators)
            {
                
            }
        }
        PathEmulatorManager.Instance.CreatePathEmulatorObject(gameObject,GameObject.Find(pathID), false, false, reverse);
    }

    public IEnumerator PlayOnPath_Additive(string pathID)
    {
        yield return new WaitForSeconds(0);


        PathEmulatorManager.Instance.CreatePathEmulatorObject(gameObject,GameObject.Find(pathID), true, false, reverse);

    }
    public IEnumerator PlayOnPath_Additive_Reverse(string pathID)
    {
        yield return new WaitForSeconds(0);


        PathEmulatorManager.Instance.CreatePathEmulatorObject(gameObject,GameObject.Find(pathID), true, false, reverse);

    }


    public IEnumerator PlayOnPath_Additive_RotationOnly(string pathID)
    {
        yield return new WaitForSeconds(0);


        PathEmulatorManager.Instance.CreatePathEmulatorObject(gameObject,GameObject.Find(pathID), true, true, false);

    }

    
    public IEnumerator PlayOnPath_Additive_RotationOnly_Reverse(string pathID)
    {
        yield return new WaitForSeconds(0);


        PathEmulatorManager.Instance.CreatePathEmulatorObject(gameObject,GameObject.Find(pathID), true, true, true);

    }

    





}
