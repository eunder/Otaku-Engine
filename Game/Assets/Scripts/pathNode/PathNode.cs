using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using DG.Tweening;

public class PathNode : MonoBehaviour
{

    public string idOfObjectToMove;
    public GameObject ObjectToMove;

    Vector3 ObjectStartPos;
    Quaternion ObjectStartRot;

    public bool moveToPath = false;

    public float time = 1.0f;
    public string loopType = "Linear";
    public bool closeLoop = false;
    public string pathType = "Linear";
    public bool wayPointRotation = false;

    public string easeType = "Linear";


    public List<Transform> waypoints = new List<Transform>();
 


    public void SetObjectToMoveAccordingToID()
    {   
        ObjectToMove = GameObject.Find(idOfObjectToMove);
    }


    //call this whenever the path entity is edited or moved

    //note... "UpdatePropertiesAccordingToAssignedPath()" in SplineMoveEvents is similar, make sure to update that when this is updated
    public void UpdatePathProperties()
    {
        SetObjectToMoveAccordingToID();

        if(ObjectToMove)
        {
        ObjectToMove.GetComponent<splineMove>().pathContainer = GetComponent<PathManager>();

        ObjectToMove.GetComponent<splineMove>().moveToPath = moveToPath;
        ObjectToMove.GetComponent<splineMove>().speed = time;
        ObjectToMove.GetComponent<splineMove>().loopType = (SWS.splineMove.LoopType)System.Enum.Parse( typeof(SWS.splineMove.LoopType), loopType);
        ObjectToMove.GetComponent<splineMove>().closeLoop = closeLoop;
        ObjectToMove.GetComponent<splineMove>().easeType = (Ease)System.Enum.Parse( typeof(Ease), easeType);
        ObjectToMove.GetComponent<splineMove>().pathType = (PathType)System.Enum.Parse( typeof(PathType), pathType);
        ObjectToMove.GetComponent<splineMove>().waypointRotation = (wayPointRotation) ?  SWS.splineMove.RotationType.all : SWS.splineMove.RotationType.none;
        }
    }


    public void PlayPath()
    {
        CreatePath();
        UpdatePathProperties();

        if(ObjectToMove)
        {
        ObjectToMove.GetComponent<splineMove>().Stop();
        ObjectToMove.GetComponent<splineMove>().StartMove();
        }
    }

    public void CreatePath()
    {
        waypoints.Clear();
        waypoints.Add(transform);

        foreach(string child in GetComponent<GeneralObjectInfo>().children)
        {
            if(GameObject.Find(child) != null)
            waypoints.Add(GameObject.Find(child).transform);
        }
        

        GetComponent<PathManager>().Create(waypoints.ToArray());
    }


    public void StopPath()
    {
        if(ObjectToMove)
        {
        ObjectToMove.GetComponent<GeneralObjectInfo>().ResetPosition();

        ObjectToMove.GetComponent<splineMove>().Stop();
        }
    }

    public void PausePath()
    {
               ObjectToMove.GetComponent<splineMove>().Pause(); 
    }
    public void ResumePath()
    {
               ObjectToMove.GetComponent<splineMove>().Resume(); 
    }

    public GameObject path_Waypoint_Prefab;
    public void AddWayPoint()
    {
        //create a waypoint at the position of the last waypoint in the waypoint list * foreward
       GameObject waypoint = Instantiate(path_Waypoint_Prefab, GetComponent<PathManager>().waypoints[GetComponent<PathManager>().GetWaypointCount() - 1].transform.position + GetComponent<PathManager>().waypoints[GetComponent<PathManager>().GetWaypointCount() - 1].transform.forward * 0.5f, Quaternion.identity);
        //set parent this object
       waypoint.transform.parent = gameObject.transform;
       waypoint.name = System.Guid.NewGuid().ToString();
       waypoint.GetComponent<GeneralObjectInfo>().UpdateID();
       waypoint.GetComponent<GeneralObjectInfo>().UpdateParent();


        //update
        GetComponent<PathManager>().Create_ParentIsFirstNode();
    }

    public void RemoveWayPoint(int index)
    {
       //destroy the object
       GameObject.Destroy(GetComponent<PathManager>().waypoints[index].gameObject);
        
        //update
        GetComponent<PathManager>().Create_ParentIsFirstNode();
    }


}
