using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode_WaypointLineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public void UpdateLineRenderer()
    {
        //set positions array size
        int posCount = 1; //parent
        posCount += GetComponent<GeneralObjectInfo>().children.Count;
        lineRenderer.positionCount = posCount;

        
        //set positions
        int index = 0;
        lineRenderer.SetPosition(index, transform.position); //parent
        index++;

        foreach(GameObject child in GetComponent<GeneralObjectInfo>().childrenObjects)
        {
        if(child != null)
        lineRenderer.SetPosition(index,child.transform.position);
        index++;
        }

    }

    void LateUpdate()
    {
        if(ObjectVisibilityViewManager.Instance.currentViewMode != 0)
        UpdateLineRenderer();
    }

    public void RemoveLineRenderer()
    {
        lineRenderer.positionCount = 0;
    }

}
