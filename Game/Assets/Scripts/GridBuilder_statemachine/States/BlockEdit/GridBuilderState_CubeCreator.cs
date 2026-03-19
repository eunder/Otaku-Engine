using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.IO;

public class GridBuilderState_CubeCreator : IGridBuilderState 
{
    int indexState = 0;  
    //-1 = exit(mouse 2 on 0)
    // 0 = picking grid axis to start on
    // 1 = picking first spot to start square on the grid(phase 1)
    // 2 = picking second spot to start square on the grid (phase 1)
    // 3 = picking the "height" axis aka. the last remining axis to adjust(phase 2)
    // 4 = Finallizing the placement(buying)


    //NOTE!!! the preview block is SCALED... just like the old fashion way otaku engine used to do block "resizing". the preview block has a child at local pos (0.5, 0.5, 0.5)


    RaycastHit hit;
    Vector3 repositionedPreviewCubePosition;

    bool canProgress = false; //used to check if cube is a applicable size

    Vector3 rayHit;

    Vector3 startHitPointPos; //prevents problems on second phase when changing grid size (the preview block gets MOVED around)
    float blockVolume;

    public IGridBuilderState DoState(GridBuilderStateMachine gridBuilder)
    {
        if(gridBuilder.newStateOnceEvent == false)
        {
            OnEnter(gridBuilder);
            gridBuilder.newStateOnceEvent = true;
        }


        OnStay(gridBuilder);

            if(indexState == -1)
            {
                OnExit(gridBuilder);
                return gridBuilder.GridBuilderStateIdle;
            }
           
        return gridBuilder.GridBuilderStateCubeCreator;
    }

        int totalBlockInScene = 0;

    
        void OnEnter(GridBuilderStateMachine gridBuilder)
        {
            gridBuilder.toolTipText.text = "Left Mouse: <color=green>Create <color=white>| RIght Mouse: <color=red>Go Back <color=white>| Scroll Wheel: Change size | C: unlock mouse";

            gridBuilder.gridManager.gridSize = gridBuilder.gridManager.gridIncrements[gridBuilder.gridIncrementIndex];
            indexState = 0;

            gridBuilder.grid.gameObject.SetActive(true);
            gridBuilder.EditorCube.SetActive(true);

            gridBuilder.BluePrint_Model.SetActive(true);

        totalBlockInScene = GameObject.FindGameObjectsWithTag("LevelCustomBlock").Length;
        gridBuilder.bluePrintTotalBlocks_Text.text = "Total: \n" + totalBlockInScene;
        }   

        int NumberOfAxisThatEqualZero(GridBuilderStateMachine gridBuilder)
        {
            int numberOfAxisThatEqualZero = 0;

            if(gridBuilder.EditorCube.transform.localScale.x == 0)
            {
                numberOfAxisThatEqualZero++;
            }
            if(gridBuilder.EditorCube.transform.localScale.y == 0)
            {
                numberOfAxisThatEqualZero++;
            }
            if(gridBuilder.EditorCube.transform.localScale.z == 0)
            {
                numberOfAxisThatEqualZero++;
            }

            return numberOfAxisThatEqualZero;
        }

        void OnStay(GridBuilderStateMachine gridBuilder)
        {  
            Debug.Log("can progress " + canProgress);

    gridBuilder.bluePrintDimensions_Text.text = "<color=red>X: " + Mathf.Abs(gridBuilder.EditorCube.transform.localScale.x).ToString("F2") + "\n" +
                                                "<color=green>Y: " + Mathf.Abs(gridBuilder.EditorCube.transform.localScale.y).ToString("F2") + "\n" +
                                                "<color=yellow>Z: " + Mathf.Abs(gridBuilder.EditorCube.transform.localScale.z).ToString("F2") + "\n";

   if(Input.GetMouseButtonDown(0))
        {
                  if(canProgress == true)
                 {
            if(indexState == 0)
            {
            gridBuilder.gridBuilderAudioSource_Build.PlayOneShot(gridBuilder.gridBuilderPlacePoint1_AudioClip,1.0f);
            gridBuilder.BluePrintPencil_Model.transform.DORewind ();
            gridBuilder.BluePrintPencil_Model.transform.DOShakePosition(0.5f, new Vector3(0.1f,0,0.05f), 6, 70);

            indexState++;
            }
            else if(indexState == 1)
            {
            gridBuilder.gridBuilderAudioSource_Build.PlayOneShot(gridBuilder.gridBuilderPlacePoint2_AudioClip,1.0f);
             NegativeDimensionCubePreviewRepositioner(gridBuilder);
             repositionedPreviewCubePosition = gridBuilder.EditorCube.transform.position;
            gridBuilder.BluePrintPencil_Model.transform.DORewind ();
            gridBuilder.BluePrintPencil_Model.transform.DOShakePosition(0.5f, new Vector3(0.1f,0,0.05f), 6, 70);
             indexState++;
            }
            else if(indexState == 2)
            {
            gridBuilder.grid.gameObject.SetActive(false);
            gridBuilder.gridBuilderAudioSource_Build.PlayOneShot(gridBuilder.gridBuilderPlacePoint3_AudioClip,1.0f);
            NegativeDimensionCubePreviewRepositioner(gridBuilder);
            gridBuilder.BluePrintPencil_Model.transform.DORewind ();
            //gridBuilder.BluePrintPencil_Model.transform.DOShakePosition(0.5f, new Vector3(0.1f,0,0.05f), 6, 70);
            gridBuilder.BluePrintPencil_Model.transform.DOShakePosition(0.8f, new Vector3(0.1f,0,0.05f), 12, 70);

            indexState++;
            }
            else if(indexState == 3)
            {

            blockVolume = gridBuilder.EditorCube.transform.localScale.x * gridBuilder.EditorCube.transform.localScale.y * gridBuilder.EditorCube.transform.localScale.z;

            gridBuilder.gridBuilderAudioSource_Create.pitch = gridBuilder.pitchCreate_Curve.Evaluate(blockVolume);
            gridBuilder.gridBuilderAudioSource_Create.PlayOneShot(gridBuilder.gridBuierAudioClip_finalize1,1.0f);

            indexState++;
            GameObject newlyCreatedCube = GameObject.Instantiate(gridBuilder.customeCubePrefab, gridBuilder.EditorCube.transform.GetChild(0).position, gridBuilder.EditorCube.transform.rotation) as GameObject;

            SaveAndLoadLevel.Instance.allLoadedBlocks.Add(newlyCreatedCube);

            //a bit messy. this eseentially uses the editor cube block vertex positions to set the newly created cube vertex positions...
            newlyCreatedCube.GetComponent<BlockFaceTextureUVProperties>().corner_Y_X_Z.corner_Pos = 
            gridBuilder.EditorCube.transform.GetChild(0).TransformPoint(gridBuilder.EditorCube.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices[2]);
          
            newlyCreatedCube.GetComponent<BlockFaceTextureUVProperties>().corner_Y_X_Zneg.corner_Pos = 
            gridBuilder.EditorCube.transform.GetChild(0).TransformPoint(gridBuilder.EditorCube.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices[3]);

            newlyCreatedCube.GetComponent<BlockFaceTextureUVProperties>().corner_Y_Xneg_Z.corner_Pos = 
            gridBuilder.EditorCube.transform.GetChild(0).TransformPoint(gridBuilder.EditorCube.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices[1]);

            newlyCreatedCube.GetComponent<BlockFaceTextureUVProperties>().corner_Y_Xneg_Zneg.corner_Pos = 
            gridBuilder.EditorCube.transform.GetChild(0).TransformPoint(gridBuilder.EditorCube.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices[0]);


            newlyCreatedCube.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_X_Z.corner_Pos = 
            gridBuilder.EditorCube.transform.GetChild(0).TransformPoint(gridBuilder.EditorCube.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices[4]);

            newlyCreatedCube.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_X_Zneg.corner_Pos = 
            gridBuilder.EditorCube.transform.GetChild(0).TransformPoint(gridBuilder.EditorCube.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices[5]);

            newlyCreatedCube.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_Xneg_Z.corner_Pos = 
            gridBuilder.EditorCube.transform.GetChild(0).TransformPoint(gridBuilder.EditorCube.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices[10]);

            newlyCreatedCube.GetComponent<BlockFaceTextureUVProperties>().corner_Yneg_Xneg_Zneg.corner_Pos = 
            gridBuilder.EditorCube.transform.GetChild(0).TransformPoint(gridBuilder.EditorCube.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices[11]);


            newlyCreatedCube.GetComponent<BlockFaceTextureUVProperties>().UpdateCorners();



            newlyCreatedCube.name = System.Guid.NewGuid().ToString();
           // newlyCreatedCube.transform.localScale = gridBuilder.EditorCube.transform.localScale;
            newlyCreatedCube.layer = 14;
           // newlyCreatedCube.transform.DOShakeScale(1.5f, gridBuilder.createShakeAmount_Curve.Evaluate(blockVolume), 12, 70);
            totalBlockInScene++;
            gridBuilder.bluePrintTotalBlocks_Text.text = "Total: \n" + totalBlockInScene;
            indexState = 0;

            newlyCreatedCube.GetComponent<GeneralObjectInfo>().UpdatePosition();
            newlyCreatedCube.GetComponent<GeneralObjectInfo>().UpdatePosition();
            newlyCreatedCube.GetComponent<GeneralObjectInfo>().UpdateID();
            newlyCreatedCube.GetComponent<BlockFaceTextureUVProperties>().SetPivotToBlockCenter();
            newlyCreatedCube.GetComponent<BlockFaceTextureUVProperties>().ApplyCubeSpawnPivotOffset();

            }
               }
              else
               {
               gridBuilder.gridBuilderAudioSource_Build.pitch = 1f;
               gridBuilder.gridBuilderAudioSource_Build.clip = gridBuilder.gridBuierAudioClip_error;
               gridBuilder.gridBuilderAudioSource_Build.Play();
               }
        }


   if(Input.GetMouseButtonDown(1))
        {
            gridBuilder.gridBuilderAudioSource_StepBack.PlayOneShot(gridBuilder.gridBuilderErasePoint1_AudioClip,1.0f);
            indexState--;
        }


    //MOST COMPLICATED STATE, 
    //THE GRID GETS ORIENTED BASED ON THE NEAREST EDGE:       Quaternion.LookRotation(angleOfHitPointAndCloestLedge, hit.normal)
    //THEN GRID SNAPPING 

 if(indexState == 0)
        {
            canProgress = true;
            gridBuilder.grid.gameObject.SetActive(true);


        Ray ray = gridBuilder.mainCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridBuilder.collidableLayers_layerMask))
        {
           gridBuilder.wallHitnormal = new Vector3(hit.normal.x,hit.normal.y,hit.normal.z);

           gridBuilder.wallHitnormal_start = new Vector3(Mathf.Round(hit.normal.x),Mathf.Round(hit.normal.y),Mathf.Round(hit.normal.z));


            //these are used for visualization and debbuging, very useful... but they also dictate the correct positioning
            gridBuilder.closestCorner_Model.transform.position = hit.transform.GetComponent<BlockFaceTextureUVProperties>().GetClosestCornerFromClosestEdgeTwoVectors(hit.point, 1);
            gridBuilder.secondClosestCorner_Model.transform.position = hit.transform.GetComponent<BlockFaceTextureUVProperties>().GetClosestCornerFromClosestEdgeTwoVectors(hit.point, 2);
            gridBuilder.closestPointOnEdge_Model.transform.position = hit.transform.GetComponent<BlockFaceTextureUVProperties>().GetClosestPointOnEdgeFromRayHitAndTwoPositions(gridBuilder.closestCorner_Model.transform.position, gridBuilder.secondClosestCorner_Model.transform.position, hit.point);


            Vector3 angleOfHitPointAndCloestLedge = (hit.point - hit.transform.GetComponent<BlockFaceTextureUVProperties>().GetClosestPointOnEdgeFromRayHitAndTwoPositions(gridBuilder.closestCorner_Model.transform.position, gridBuilder.secondClosestCorner_Model.transform.position, hit.point)).normalized;



            Debug.Log("SquMgt of point to the line between to vertices" + hit.transform.GetComponent<BlockFaceTextureUVProperties>().GetSqrMagFromEdge(gridBuilder.closestCorner_Model.transform.position, gridBuilder.secondClosestCorner_Model.transform.position, hit.point) );


            hit.transform.GetComponent<BlockFaceTextureUVProperties>().NearestEdgeFromHitPoint(hit.triangleIndex, hit.point);



            //this part essentially uses the ray hit normal as the "up". And the angleOfHitPointAndClosestLedge as the forward direction
                                                                                                        //this part was key!
            gridBuilder.grid.transform.rotation = Quaternion.LookRotation(angleOfHitPointAndCloestLedge, hit.normal);

        
            //GRID POSITIONING BASED ON AN OBJECTS (CORNER OBJECT) ORIENTATION
            //--------------------------------------------------------------

            //orientate the first point, because this will be the origin the snapping is based around
            gridBuilder.closestCorner_Model.transform.rotation = Quaternion.LookRotation(angleOfHitPointAndCloestLedge, hit.normal);


            //set the grid position to the point being hit (so that later, it can get snapped based on the corner's orientation)
            gridBuilder.grid.transform.position = hit.point;

            //relative position of grid based on the corner orientation   (make sure the object isnt scaled or else it will cause offset issues)
            Vector3 relativePosition = gridBuilder.closestCorner_Model.transform.InverseTransformPoint(gridBuilder.grid.transform.position);
            
            //math rounding snapping
            Vector3 snappedPosition = new Vector3(
                SnapToGridValue(gridBuilder,relativePosition.x ),
                SnapToGridValue(gridBuilder,relativePosition.y ),
                SnapToGridValue(gridBuilder,relativePosition.z)
            );

            // Convert the snapped position back to world space
            Vector3 worldSnappedPosition = gridBuilder.closestCorner_Model.transform.TransformPoint(snappedPosition);

            //finally, set the grid to the snapped location based on the object's orientation
            gridBuilder.grid.transform.position = worldSnappedPosition;




            gridBuilder.EditorCube.transform.position = worldSnappedPosition;
            gridBuilder.EditorCube.transform.rotation = Quaternion.LookRotation(angleOfHitPointAndCloestLedge, hit.normal);
            startHitPointPos = gridBuilder.EditorCube.transform.position;
            gridBuilder.pointOnPlane.transform.position = worldSnappedPosition;





        }
        else //if nothing is being hit, project a plane onto the world y pos facing upward
        {

                    gridBuilder.m_Plane = new Plane(Vector3.up, new Vector3(0,0,0));

                    Ray worldPlaneray = gridBuilder.mainCam.ScreenPointToRay(Input.mousePosition);

                    //Initialise the enter variable
                    float enter = 0.0f;

                    if (gridBuilder.m_Plane.Raycast(worldPlaneray, out enter))
                    {
                        //Get the point that is clicked
                        Vector3 hitPoint = worldPlaneray.GetPoint(enter);
                        gridBuilder.pointOnPlane.transform.position = hitPoint ;



                        //use the grid(it will stay in place during this state) as the center position of the plane

                    
                    Vector3 snappedPosition = new Vector3(
                        SnapToGridValue(gridBuilder,hitPoint.x ),
                        SnapToGridValue(gridBuilder,hitPoint.y ),
                        SnapToGridValue(gridBuilder,hitPoint.z)
                    );
                    Vector3 worldSnappedPosition = gridBuilder.closestCorner_Model.transform.TransformPoint(snappedPosition);

                    gridBuilder.EditorCube.transform.position = snappedPosition;
                    gridBuilder.EditorCube.transform.rotation = Quaternion.identity;
                    startHitPointPos = gridBuilder.EditorCube.transform.position;
                    gridBuilder.pointOnPlane.transform.position = snappedPosition;
                    gridBuilder.grid.transform.position = snappedPosition;
                    gridBuilder.grid.transform.rotation = Quaternion.identity;
                    gridBuilder.wallHitnormal = Vector3.up;

                    }
        }



            gridBuilder.EditorCube.transform.localScale = new Vector3(0.0f,0.0f,0.0f);
        }

        //DURING THIS PHASE, THE GRID WILL STAY IN PLACE, AND THE X AND Z OF THE CUBE WILL BE SET DEPENDING ON THE OFFSET (THE Y OFFSET WILL ALWAYS BE 0)
 if(indexState == 1)
        {
            gridBuilder.EditorCube.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", gridBuilder.previewCube_correct);
            gridBuilder.grid.gameObject.SetActive(true);

            if(NumberOfAxisThatEqualZero(gridBuilder) <= 1)
            {
                canProgress = true;
            }
            else
            {
                canProgress = false;
            }

            //makes sure the positon goes back in case player goes back (happens due to negative dimesion and repositions code)
            gridBuilder.EditorCube.transform.position = startHitPointPos;

            gridBuilder.m_Plane = new Plane(gridBuilder.wallHitnormal, gridBuilder.EditorCube.transform.position);

            //Create a ray from the Mouse click position
            Ray ray = gridBuilder.mainCam.ScreenPointToRay(Input.mousePosition);

            //Initialise the enter variable
            float enter = 0.0f;

            if (gridBuilder.m_Plane.Raycast(ray, out enter))
            {
                //Get the point that is clicked
                Vector3 hitPoint = ray.GetPoint(enter);
                gridBuilder.pointOnPlane.transform.position = hitPoint ;
  
                //use the grid(it will stay in place during this state) as the center position of the plane
               gridBuilder.PosrelativeToCenterPlane = gridBuilder.grid.transform.InverseTransformPoint(hitPoint);
               //gridBuilder.PosrelativeToCenterPlane = OneAxisLockTwoAxisConform_PosrelativeToCenterPlane(gridBuilder);

            


            Vector3 relativePosition = gridBuilder.closestCorner_Model.transform.InverseTransformPoint(gridBuilder.grid.transform.position);
            Vector3 snappedPosition = new Vector3(
                SnapToGridValue(gridBuilder,gridBuilder.PosrelativeToCenterPlane.x ),
                SnapToGridValue(gridBuilder,gridBuilder.PosrelativeToCenterPlane.y ),
                SnapToGridValue(gridBuilder,gridBuilder.PosrelativeToCenterPlane.z)
            );
            Vector3 worldSnappedPosition = gridBuilder.closestCorner_Model.transform.TransformPoint(snappedPosition);


                //use scale the cube dependiding on the offset
                gridBuilder.EditorCube.transform.localScale = snappedPosition;

                //texture tiling
                gridBuilder.cubeMesh.GetComponent<Renderer>().material.mainTextureScale = new Vector2(gridBuilder.PosrelativeToCenterPlane.z, gridBuilder.PosrelativeToCenterPlane.y);
             }

        }


    //THIS IS THE HEIGHT (Y AXIS) PICKING PHASE OF THE CUBE... THIS STATE USES A PLANE THAT FACES PLAYER (ONE AXIS) AS OPPOSED TO THE PREVIOUS STATE THAT USES A SIMPLE PLANE
    //THIS IS BECAUSE OF THE "ARROW DRAGGING"-LIKE FUCKERY THAT NEEDS TO BE DONE.
 if(indexState == 2)
        {   
            gridBuilder.grid.gameObject.SetActive(true);

            if(NumberOfAxisThatEqualZero(gridBuilder) == 0)
            {
                gridBuilder.EditorCube.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", gridBuilder.previewCube_correct);
                canProgress = true;
            }
            else
            {
                gridBuilder.EditorCube.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", gridBuilder.previewCube_error);
                canProgress = false;
            }

        
            //makes sure the positon goes back in case player goes back (happens due to negative dimesion and repositions code)
            gridBuilder.EditorCube.transform.position = repositionedPreviewCubePosition;

            //plane that faces player...
            gridBuilder.planeThatFacesPlayerOneAxis.transform.position = gridBuilder.EditorCube.transform.position;

            gridBuilder.planeThatFacesPlayerOneAxis.transformUp = gridBuilder.grid.transform;

            gridBuilder.m_Plane = new Plane(gridBuilder.planeThatFacesPlayerOneAxis.transform.up, gridBuilder.EditorCube.transform.position);

             //Create a ray from the Mouse click position
            Ray ray = gridBuilder.mainCam.ScreenPointToRay(Input.mousePosition);

            //Initialise the enter variable
            float enter = 0.0f;

            if (gridBuilder.m_Plane.Raycast(ray, out enter))
            {
                //Get the point that is clicked
                Vector3 hitPoint = ray.GetPoint(enter);
                gridBuilder.pointOnPlane.transform.position = hitPoint;

                //Move your cube GameObject to the point where you clicked
             
               gridBuilder.PosrelativeToCenterPlane = gridBuilder.planeThatFacesPlayerOneAxis.transform.InverseTransformPoint(hitPoint);
               gridBuilder.PosrelativeToCenterPlane = new Vector3(Mathf.Round(gridBuilder.PosrelativeToCenterPlane.x * gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize, Mathf.Round(gridBuilder.PosrelativeToCenterPlane.y * gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize, Mathf.Round(gridBuilder.PosrelativeToCenterPlane.z*gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize);
              
              
               gridBuilder.EditorCube.transform.localScale = new Vector3(gridBuilder.EditorCube.transform.localScale.x, -gridBuilder.PosrelativeToCenterPlane.z, gridBuilder.EditorCube.transform.localScale.z);

             }
        }


 
 
        if(Input.GetKeyDown(KeyCode.C))
        {
                    Cursor.lockState = CursorLockMode.None;
                    gridBuilder.mouseLooker.enabled = false;
        }
        if(Input.GetKeyUp(KeyCode.C))
        {
                    Cursor.lockState = CursorLockMode.Locked;
                    gridBuilder.mouseLooker.enabled = true;
        }



        if (Input.GetAxis("Mouse ScrollWheel") > 0f && gridBuilder.gridIncrementIndex < gridBuilder.gridManager.gridIncrements.Length - 1) // forward
        {
            gridBuilder.gridIncrementIndex++;
            gridBuilder.gridManager.gridSize = gridBuilder.gridManager.gridIncrements[gridBuilder.gridIncrementIndex];

            gridBuilder.bluePrintGridSize_Text.text = 1 + "/" + gridBuilder.gridManager.gridIncrements[gridBuilder.gridIncrementIndex];
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && gridBuilder.gridIncrementIndex > 0) // backwards
        {
            gridBuilder.gridIncrementIndex--;
            gridBuilder.gridManager.gridSize = gridBuilder.gridManager.gridIncrements[gridBuilder.gridIncrementIndex];

            gridBuilder.bluePrintGridSize_Text.text = 1 + "/" + gridBuilder.gridManager.gridIncrements[gridBuilder.gridIncrementIndex];
        } 


    //draw plane
        Vector3 v3;
    if (gridBuilder.m_Plane.normal.normalized != Vector3.forward)
        v3 = Vector3.Cross(gridBuilder.m_Plane.normal, Vector3.forward).normalized * gridBuilder.m_Plane.normal.magnitude;
    else
        v3 = Vector3.Cross(gridBuilder.m_Plane.normal, Vector3.up).normalized * gridBuilder.m_Plane.normal.magnitude;;
        
    var corner0 = gridBuilder.pointOnPlane.transform.position + v3;
    var corner2 = gridBuilder.pointOnPlane.transform.position - v3;
    var q = Quaternion.AngleAxis(90.0f, gridBuilder.m_Plane.normal);
    v3 = q * v3;
    var corner1 = gridBuilder.pointOnPlane.transform.position + v3;
    var corner3 = gridBuilder.pointOnPlane.transform.position - v3;
    
    Debug.DrawLine(corner0, corner2, Color.green);
    Debug.DrawLine(corner1, corner3, Color.green);
    Debug.DrawLine(corner0, corner1, Color.green);
    Debug.DrawLine(corner1, corner2, Color.green);
    Debug.DrawLine(corner2, corner3, Color.green);
    Debug.DrawLine(corner3, corner0, Color.green);
    Debug.DrawRay(gridBuilder.pointOnPlane.transform.position, gridBuilder.m_Plane.normal * 10.0f, Color.red);


        }



        void OnExit(GridBuilderStateMachine gridBuilder)
        {
            gridBuilder.toolTipText.text = "";
            indexState = 0;
            gridBuilder.grid.gameObject.SetActive(false);
            gridBuilder.EditorCube.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            gridBuilder.mouseLooker.enabled = true;
            gridBuilder.playerObjectInteractionStateMachine.enabled = true;

            gridBuilder.mouseLooker.wheelPickerIsTurnedOn = false;

            gridBuilder.BluePrint_Model.SetActive(false);
        }


    //You have to use the preview blocks's forward, right, and up to re-align after the local scale has been set to positive. (Only when the axis is negative)
    void NegativeDimensionCubePreviewRepositioner(GridBuilderStateMachine gridBuilder)
    {
          //all if these if statements reposition the cube so that all the dimensional scales are always positive(by making the certain scale positive using Math Abs, and then substacting that from the position)
                if(gridBuilder.EditorCube.transform.localScale.x < 0)
                {
                    gridBuilder.EditorCube.transform.localScale = new Vector3(Mathf.Abs(gridBuilder.EditorCube.transform.localScale.x), gridBuilder.EditorCube.transform.localScale.y, gridBuilder.EditorCube.transform.localScale.z);
                    gridBuilder.EditorCube.transform.position = gridBuilder.EditorCube.transform.position + gridBuilder.EditorCube.transform.right * -gridBuilder.EditorCube.transform.localScale.x;
                }
                if(gridBuilder.EditorCube.transform.localScale.y < 0)
                {
                    gridBuilder.EditorCube.transform.localScale = new Vector3(gridBuilder.EditorCube.transform.localScale.x, Mathf.Abs(gridBuilder.EditorCube.transform.localScale.y), gridBuilder.EditorCube.transform.localScale.z);
                    gridBuilder.EditorCube.transform.position = gridBuilder.EditorCube.transform.position + gridBuilder.EditorCube.transform.up * -gridBuilder.EditorCube.transform.localScale.y;
                }
                if(gridBuilder.EditorCube.transform.localScale.z < 0)
                {
                    gridBuilder.EditorCube.transform.localScale = new Vector3(gridBuilder.EditorCube.transform.localScale.x,gridBuilder.EditorCube.transform.localScale.y, Mathf.Abs(gridBuilder.EditorCube.transform.localScale.z));
                    gridBuilder.EditorCube.transform.position = gridBuilder.EditorCube.transform.position + gridBuilder.EditorCube.transform.forward * -gridBuilder.EditorCube.transform.localScale.z;
                }
    }



    Vector3 OneAxisLockTwoAxisConform(GridBuilderStateMachine gridBuilder) //for making sure the player can build on top of cube out of the world axis
    {
        Vector3 vec;
        if(gridBuilder.wallHitnormal_start.y == 1 || gridBuilder.wallHitnormal_start.y == -1 && gridBuilder.wallHitnormal_start.x == 0 && gridBuilder.wallHitnormal_start.z == 0)
        {
          vec = new Vector3(Mathf.Round(hit.point.x * gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize,hit.point.y,Mathf.Round(hit.point.z * gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize) + gridBuilder.wallHitnormal * 0.01f; // prevent z fighting by adding the up normal to position(subtract it before the block gets placed)
          return vec;
        }

        if(gridBuilder.wallHitnormal_start.x == 1 || gridBuilder.wallHitnormal_start.x == -1 && gridBuilder.wallHitnormal_start.y == 0 && gridBuilder.wallHitnormal_start.z == 0)
        {
          vec = new Vector3(hit.point.x, Mathf.Round(hit.point.y*gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize,Mathf.Round(hit.point.z * gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize) + gridBuilder.wallHitnormal * 0.01f; // prevent z fighting by adding the up normal to position(subtract it before the block gets placed)
          return vec;
        }

        if(gridBuilder.wallHitnormal_start.z == 1 || gridBuilder.wallHitnormal_start.z == -1 && gridBuilder.wallHitnormal_start.x == 0 && gridBuilder.wallHitnormal_start.y == 0)
        {
          vec = new Vector3(Mathf.Round(hit.point.x * gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize, Mathf.Round(hit.point.y*gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize, hit.point.z) + gridBuilder.wallHitnormal * 0.01f; // prevent z fighting by adding the up normal to position(subtract it before the block gets placed)
          return vec;
        

        }

        return new Vector3(0,0,0);
    }

    Vector3 OneAxisLockTwoAxisConform_PosrelativeToCenterPlane(GridBuilderStateMachine gridBuilder) //same thing as above... except for the second phase(PosrelativeToCenterPlane)
    {
        Vector3 vec;
        if(gridBuilder.wallHitnormal_start.y == 1 || gridBuilder.wallHitnormal_start.y == -1 && gridBuilder.wallHitnormal_start.x == 0 && gridBuilder.wallHitnormal_start.z == 0)
        {
          vec = new Vector3(Mathf.Round(gridBuilder.PosrelativeToCenterPlane.x * gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize,gridBuilder.PosrelativeToCenterPlane.y,Mathf.Round(gridBuilder.PosrelativeToCenterPlane.z * gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize); 
          return vec;
        }

        if(gridBuilder.wallHitnormal_start.x == 1 || gridBuilder.wallHitnormal_start.x == -1 && gridBuilder.wallHitnormal_start.y == 0 && gridBuilder.wallHitnormal_start.z == 0)
        {
          vec = new Vector3(gridBuilder.PosrelativeToCenterPlane.x, Mathf.Round(gridBuilder.PosrelativeToCenterPlane.y*gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize,Mathf.Round(gridBuilder.PosrelativeToCenterPlane.z * gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize);
          return vec;
        }

        if(gridBuilder.wallHitnormal_start.z == 1 || gridBuilder.wallHitnormal_start.z == -1 && gridBuilder.wallHitnormal_start.x == 0 && gridBuilder.wallHitnormal_start.y == 0)
        {
          vec = new Vector3(Mathf.Round(gridBuilder.PosrelativeToCenterPlane.x * gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize, Mathf.Round(gridBuilder.PosrelativeToCenterPlane.y*gridBuilder.gridManager.gridSize)/gridBuilder.gridManager.gridSize, gridBuilder.PosrelativeToCenterPlane.z);
          return vec;
        

        }

        return new Vector3(0,0,0);
    }

    //snapping relative to object orientation functions


    float SnapToGridValue(GridBuilderStateMachine gridBuilder, float value)
    {
        return Mathf.Round(value*gridBuilder.gridManager.gridSize)/ gridBuilder.gridManager.gridSize;
    }



}
