using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformArrow_DragRotate : MonoBehaviour
{

    public enum RotationAxis {Y, X, Z};

    public RotationAxis rotation;

    public bool beingHeld = false;
    public bool beingHeldEvent = false;

    public GameObject planeThatFacesPlayer; //second phase plane

    Plane m_Plane;
    public Camera mainCam;
    public GameObject pointOnPlane; //ticker
    public Vector3 PosrelativeToCenterPlane;
    public GridSizeManager gridManager;

    public GridBuilderStateMachine gridBuilderStateMacine;

    public Quaternion StartRot;
    public Vector3 StartPos;

    public Transform transformGimbal;

    public Vector3 startPosRelativeOffset;
    public bool offsetStart = false;

    public MeshFilter meshFilter;
    public Mesh startMesh;
    public Mesh arrowMesh; //for showing to the player more clearly where to drag the mouse(switching meshes)

    // Update is called once per frame
    void Update()
    {
            
         if(beingHeld)
         {

            if(beingHeldEvent == false)
            {
                //hide other arrows when this one being held
                GetComponentInParent<TransformArrow_GimbalGroup>().HideAllExceptThisArrow(gameObject);
                StartRot = transformGimbal.localRotation;
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


                transform.parent = null;
                SwitchMeshToDragArrow();
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

                    offsetStart = true;
                }

        
            //Actual  adjusting
            transform.position = StartPos  + (new Vector3(transform.up.x * -(PosrelativeToCenterPlane.z + startPosRelativeOffset.x), transform.up.y * -(PosrelativeToCenterPlane.z + startPosRelativeOffset.x), transform.up.z * -(PosrelativeToCenterPlane.z + startPosRelativeOffset.x)));

            //rotation adjustment
            float deltaRotation = -(PosrelativeToCenterPlane.z - startPosRelativeOffset.z) * 15f;
            
            if (rotation == RotationAxis.Y)
            {
                Quaternion yRotation = Quaternion.Euler(0, deltaRotation, 0);
                transformGimbal.localRotation = StartRot * yRotation;
            }
            else if (rotation == RotationAxis.X)
            {
                Quaternion xRotation = Quaternion.Euler(deltaRotation, 0, 0);
                transformGimbal.localRotation = StartRot * xRotation;
            }
            else if (rotation == RotationAxis.Z)
            {
                Quaternion zRotation = Quaternion.Euler(0, 0, deltaRotation);
                transformGimbal.localRotation = StartRot * zRotation;
            }
         
 //        transformGimbal.eulerAngles = new Vector3((Mathf.Round(transformGimbal.eulerAngles.x / 15.0f) * 15.0f), (Mathf.Round(transformGimbal.eulerAngles.y / 15.0f) * 15.0f), (Mathf.Round(transformGimbal.eulerAngles.z / 15.0f) * 15.0f));
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

    public void SwitchMeshToRotateArrow()
    {
        meshFilter.sharedMesh = startMesh;
    }
    public void SwitchMeshToDragArrow()
    {
        meshFilter.sharedMesh = arrowMesh;
    }




}
