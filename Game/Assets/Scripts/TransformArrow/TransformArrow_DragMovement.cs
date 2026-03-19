using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformArrow_DragMovement : MonoBehaviour
{
    //BIG NOTE:
    // THE GAME TAKES IN THE PLANETHATFACES PLAYER'S Z POSITIONS AND OFFSETS!!! DUE TO THE ANNOYING FORUMLA USED IN TRANSFORMARROW_PLANEFACEPLAYER_ONEAXISROTATION

    //ANOTHER NOTE: For some reason the start off set + current relative offset calulations need to be done this way or else it will be flipped and/or have start pos problems
    // -(PosrelativeToCenterPlane.z + startPosRelativeOffset.z)
    
    //without the -() at the beginning, the entire thing will be inverted
    //without the (.z + .x),

    public bool moveEntireParentBundle = false;
    public bool beingHeld = false;
    public bool beingHeldEvent = false;

    public GameObject planeThatFacesPlayer; //second phase plane

    Plane m_Plane;
    public Camera mainCam;
    public GameObject pointOnPlane; //ticker
    public Vector3 PosrelativeToCenterPlane;
    public GridSizeManager gridManager;

    public GridBuilderStateMachine gridBuilderStateMacine;

    public Vector3 StartPos;

    public Vector3 startPosRelativeOffset;
    public bool offsetStart = false;
    // Update is called once per frame
    void Update()
    {
            

         if(beingHeld)
         {

            if(beingHeldEvent == false)
            {
                //hide other arrows when this one being held
                GetComponentInParent<TransformArrow_GimbalGroup>().HideAllExceptThisArrow(gameObject);
                StartPos = transform.position;
                beingHeldEvent = true;

                //positioning the plane correctly...

                //parent it to get it positioned correctly
                planeThatFacesPlayer.transform.parent = gameObject.transform;
                planeThatFacesPlayer.transform.localRotation = Quaternion.Euler(0,0,90f);
                planeThatFacesPlayer.transform.localPosition = new Vector3(0f,0f,0f);

                //unparent it so that it dosnt follow
                planeThatFacesPlayer.transform.parent = null;
                planeThatFacesPlayer.transform.localScale = new Vector3(1.0f,1.0f,1.0f);

                //assign arrow to the script that faces player
                planeThatFacesPlayer.GetComponent<TransformArrow_PlaneFacePlayer_OneAxisRotation>().transformUp = transform;
                planeThatFacesPlayer.GetComponent<TransformArrow_PlaneFacePlayer_OneAxisRotation>().UpdateOrientation();

            }


            //plane that faces player...
            m_Plane = new Plane(planeThatFacesPlayer.transform.up, planeThatFacesPlayer.transform.position);

             //Create a ray from the Mouse click position
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            //Initialise the enter variable
            float enter = 0.0f;

            if (m_Plane.Raycast(ray, out enter))
            {
                //Get the point that is clicked
                Vector3 hitPoint = ray.GetPoint(enter);
                pointOnPlane.transform.position = hitPoint;

                //hitpoint local position of planeThatFacesPlayer
               PosrelativeToCenterPlane = planeThatFacesPlayer.transform.InverseTransformPoint(hitPoint);
               PosrelativeToCenterPlane = new Vector3(Mathf.Round(PosrelativeToCenterPlane.x * gridManager.gridSize)/gridManager.gridSize, Mathf.Round(PosrelativeToCenterPlane.y * gridManager.gridSize)/gridManager.gridSize, Mathf.Round(PosrelativeToCenterPlane.z*gridManager.gridSize)/gridManager.gridSize);

                if(offsetStart == false)
                {
              startPosRelativeOffset = planeThatFacesPlayer.transform.InverseTransformPoint(hitPoint);
               startPosRelativeOffset =   new Vector3(Mathf.Round(PosrelativeToCenterPlane.x * gridManager.gridSize)/gridManager.gridSize, Mathf.Round(PosrelativeToCenterPlane.y * gridManager.gridSize)/gridManager.gridSize, Mathf.Round(PosrelativeToCenterPlane.z*gridManager.gridSize)/gridManager.gridSize);

                    offsetStart = true;
                }


            //Actual  adjusting
            if(moveEntireParentBundle) //if this bool is set, then move the parent instead of this object
            {
                //  transform.up.x * (posrelative.x - startposrel.x), etc...
            transform.parent.position = StartPos  + (new Vector3(transform.up.x * -(PosrelativeToCenterPlane.z - startPosRelativeOffset.z), transform.up.y * -(PosrelativeToCenterPlane.z - startPosRelativeOffset.z), transform.up.z * -(PosrelativeToCenterPlane.z - startPosRelativeOffset.z)));
            }
            else
            {
                //offset not applied yet!
            transform.position = StartPos + (new Vector3(transform.up.x * PosrelativeToCenterPlane.x, transform.up.y * PosrelativeToCenterPlane.x, transform.up.z * PosrelativeToCenterPlane.x) );
            }
             }




        //grid size control
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && gridBuilderStateMacine.gridIncrementIndex < gridManager.gridIncrements.Length - 1) // forward
        {
            gridBuilderStateMacine.gridIncrementIndex++;
            gridManager.gridSize = gridManager.gridIncrements[gridBuilderStateMacine.gridIncrementIndex];
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && gridBuilderStateMacine.gridIncrementIndex > 0) // backwards
        {
            gridBuilderStateMacine.gridIncrementIndex--;
            gridManager.gridSize = gridManager.gridIncrements[gridBuilderStateMacine.gridIncrementIndex];
        } 

         }


    }

}
