using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCornersLineRenderer : MonoBehaviour
{
    //draws a line between all selected corners of the block being edited

    public Transform playerCam;
    public LineRenderer lineRenderer;

    //for auto sizing
    float size;

    // Update is called once per frame
    void Update()
    {

        //draw line between all selected corners
        Vector3[] positions = new Vector3[GridBuilderStateMachine.Instance.selectedCorners.Count];
        for (int i = 0; i < GridBuilderStateMachine.Instance.selectedCorners.Count; i++)
        {
            positions[i] = GridBuilderStateMachine.Instance.selectedCorners[i].corner_Pos;
        }
        lineRenderer.SetPositions(positions);


        lineRenderer.positionCount = GridBuilderStateMachine.Instance.selectedCorners.Count;





    //auto adjust size depending on player camera distance
    Vector3 averagePositionOfSelectedCorners = GlobalUtilityFunctions.CalculateAverageVectorPositionFromListOfGameObjects(GridBuilderStateMachine.Instance.listOfSelectedCornerObjects);
    size = ((PlayerObjectInteractionStateMachine.Instance.playerCamera.transform.position - averagePositionOfSelectedCorners).magnitude) * 0.02f;
    lineRenderer.widthMultiplier = size;
    }
}
