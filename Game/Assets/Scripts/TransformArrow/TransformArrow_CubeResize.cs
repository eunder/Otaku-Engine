using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformArrow_CubeResize : MonoBehaviour
{

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

    Vector3 startLocalScaleOffset;
    Vector3 cubeStartPos;
    public bool offsetStart = false;


    public enum ArrowDirection{y_front, y_back, x_front, x_back, z_front, z_back};

    public ArrowDirection arrowDirection;

    float size;

    // Update is called once per frame
    void Update()
    {
            //dynamic scale according to camera
            size = ((mainCam.transform.position - transform.position).magnitude) * 0.10f;
            transform.localScale = new Vector3(size,size,size);


         if(beingHeld)
         {

            if(beingHeldEvent == false)
            {
                //hide other arrows when this one being held
                GetComponentInParent<TransformArrow_CubeResize_GimbalGroup>().HideAllExceptThisArrow(gameObject);
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
               startPosRelativeOffset = new Vector3(Mathf.Round(PosrelativeToCenterPlane.x * gridManager.gridSize)/gridManager.gridSize, Mathf.Round(PosrelativeToCenterPlane.y * gridManager.gridSize)/gridManager.gridSize, Mathf.Round(PosrelativeToCenterPlane.z*gridManager.gridSize)/gridManager.gridSize);

                startLocalScaleOffset = gridBuilderStateMacine.currentCubeToResize.transform.localScale;

                    cubeStartPos = gridBuilderStateMacine.currentCubeToResize.transform.position;

                    offsetStart = true;
                }


            //Actual  adjusting

            
            //  transform.up.x * (posrelative.z - startposrel.z), etc... FOR EACH AXIS
                        transform.position = StartPos + (new Vector3(transform.up.x * -(PosrelativeToCenterPlane.z - startPosRelativeOffset.z), transform.up.y * -(PosrelativeToCenterPlane.z - startPosRelativeOffset.z) , transform.up.z * -(PosrelativeToCenterPlane.z - startPosRelativeOffset.z)) );


    
    



            NegativeDimensionCubeRepositioner();
             }





  void NegativeDimensionCubeRepositioner()
    {
            //if the scaller is 0 or in the negative, set the minimum amount according to the current grid size (1/grid size)

                    //y axis
                if(gridBuilderStateMacine.currentCubeToResize.transform.localScale.y <= 0)
                {
                    gridBuilderStateMacine.currentCubeToResize.transform.localScale = new Vector3(gridBuilderStateMacine.currentCubeToResize.transform.localScale.x, 1 / gridBuilderStateMacine.gridManager.gridIncrements[gridBuilderStateMacine.gridIncrementIndex] , gridBuilderStateMacine.currentCubeToResize.transform.localScale.z);
               
                   if(arrowDirection == ArrowDirection.y_back)
                    {
                    gridBuilderStateMacine.currentCubeToResize.transform.position = cubeStartPos + -gridBuilderStateMacine.currentCubeToResize.transform.up * (-startLocalScaleOffset.y + 1 / gridBuilderStateMacine.gridManager.gridIncrements[gridBuilderStateMacine.gridIncrementIndex]);
                    }
                }
                    //z axis 
                    if(gridBuilderStateMacine.currentCubeToResize.transform.localScale.z <= 0)
                {
                    gridBuilderStateMacine.currentCubeToResize.transform.localScale = new Vector3(gridBuilderStateMacine.currentCubeToResize.transform.localScale.x, gridBuilderStateMacine.currentCubeToResize.transform.localScale.y, 1 / gridBuilderStateMacine.gridManager.gridIncrements[gridBuilderStateMacine.gridIncrementIndex]);
             
                    if(arrowDirection == ArrowDirection.z_back)
                    {
                    gridBuilderStateMacine.currentCubeToResize.transform.position = cubeStartPos + -gridBuilderStateMacine.currentCubeToResize.transform.forward * (-startLocalScaleOffset.z + 1 / gridBuilderStateMacine.gridManager.gridIncrements[gridBuilderStateMacine.gridIncrementIndex]);
                    }
                }
                    //x axis 
                    if(gridBuilderStateMacine.currentCubeToResize.transform.localScale.x <= 0)
                {
                    gridBuilderStateMacine.currentCubeToResize.transform.localScale = new Vector3(1 / gridBuilderStateMacine.gridManager.gridIncrements[gridBuilderStateMacine.gridIncrementIndex], gridBuilderStateMacine.currentCubeToResize.transform.localScale.y, gridBuilderStateMacine.currentCubeToResize.transform.localScale.z);
               
                    if(arrowDirection == ArrowDirection.x_back)
                    {
                    gridBuilderStateMacine.currentCubeToResize.transform.position = cubeStartPos + -gridBuilderStateMacine.currentCubeToResize.transform.right * (-startLocalScaleOffset.x + 1 / gridBuilderStateMacine.gridManager.gridIncrements[gridBuilderStateMacine.gridIncrementIndex]);
                    }
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
