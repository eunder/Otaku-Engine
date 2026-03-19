using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilderState_CubeEdgeEditor : IGridBuilderState 
{
    int indexState = 0;  
    //-1 = exit(mouse 2 on 0)
    // state 0: picking a cube to adjust
    // state 1: adjusting with arrows


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
           
        return gridBuilder.GridBuilderStateCubeEdgeEditor;
    }

    
        void OnEnter(GridBuilderStateMachine gridBuilder)
    {
            gridBuilder.selectedCornersLineRenderer_GameObject.SetActive(true);
            gridBuilder.edgeEditGimbal.gameObject.SetActive(true);
            gridBuilder.toolTipText.text = "Left Click: pick corner | C: UnlockMouse | Scroll wheel: change grid size | <color=red> Q: go back";

            gridBuilder.edgeEditModel_Head.SetActive(true);
            ToggleAppropriateModelDependingOnMode(gridBuilder);

            GridSizeManager.Instance.gridSize_Canvas.SetActive(true);            
    }
        
        GameObject currentCubeToEdit;

        GameObject currentHoldingTransformArrow;



        RaycastHit[] allHits;

        Vector3 currentEdgeModifyingValue; //to prevent mistakes by applying the offset

        bool isLocalMode = false;


        Vector3 RepositionerStartPos; //for resetting the repositioner container position on cancel
        bool repositioner_Event = true;


        Vector3 gimbalParentStartPos; //for resetting the position on cancel


        //for saving the position(rotation arrows)
        Vector3 localArrowPos;
        Quaternion localArrowRot;

        //used for snapping...
        public Vector3 posRelativeToLocalCornerPosition;


        void OnStay(GridBuilderStateMachine gridBuilder)
        {

                                 if(Input.GetMouseButtonDown(0))
        {


                                bool arrowWidgetTouchedFirst = false;



                                Ray ray2 = gridBuilder.mainCam.ScreenPointToRay(Input.mousePosition);
                                allHits = Physics.RaycastAll(ray2, Mathf.Infinity);

                                for (int i = 0; i < allHits.Length; i++)
                                {
                                    RaycastHit hit2 = allHits[i];

                                    if(hit2.transform.GetComponent<TransformArrow_DragMovement>()) //prioritize arrow over selecting any other stuff
                                    {
                                        currentHoldingTransformArrow = hit2.transform.gameObject;
                                        hit2.transform.GetComponent<TransformArrow_DragMovement>().beingHeld = true;
                                        arrowWidgetTouchedFirst = true;

                                        //turn on eyes
                                        gridBuilder.edgeEditModel_Eyes.SetActive(true);

                                        //Remove highlight on start arrow drag
                                        UnHighlightToCurrentlySelectedBlock(gridBuilder);


                                        break;
                                    }
                                }

                         if(arrowWidgetTouchedFirst == false)
                                {

                                    //sorts the allHits array in order based on distance
                                    System.Array.Sort(allHits, (x,y) => x.distance.CompareTo(y.distance));


                                    for (int i = 0; i < allHits.Length; i++)
                                    {
                                        RaycastHit hit3 = allHits[i];

                                            //on select block
                                            if(hit3.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>())
                                            {   
                                            
                                            //remove the highlight from the last selected block or else it will remain highlighted!
                                            UnHighlightToCurrentlySelectedBlock(gridBuilder);
  
                                            currentCubeToEdit = hit3.transform.gameObject;
                                            HighlightToCurrentlySelectedBlock(gridBuilder);

                                            hit3.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().CornerClosestOffsetToEditToRayHit(hit3.point);

                                            //some "magic" happens here... the player is able to pick which corner the entire group is based around (very convinient for position snapping)
                                            gridBuilder.edgeEditGimbal.position = currentCubeToEdit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().closestCornerPoint.corner_Pos;
                                            gridBuilder.edgeEditGimbal.rotation = currentCubeToEdit.transform.rotation;



                                            //set the orientation before adding the selected objects or else they will get completely messed up and ruin the positioning
                                             if(isLocalMode)
                                            {
                                                gridBuilder.edgeEditGimbal.rotation = Quaternion.identity;
                                                isLocalMode = false;
                                            }
                                            else
                                            {
                                                gridBuilder.edgeEditGimbal.rotation = currentCubeToEdit.transform.rotation;
                                                isLocalMode = true;
                                            }


                                            ClearSelectedCornerList(gridBuilder);


                                            //
                                            //add corners depending on what mode is on (corner, edge, face)
                                            //

                                            Debug.Log("triangle index: " + hit3.triangleIndex);

                                            //return which corners these positions belong to and add them to the list (depending on the mode)
                                            if(gridBuilder.editMode == GridBuilderStateMachine.EditMode.corner)
                                            {
                                                gridBuilder.selectedCorners = hit3.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().GetClosestCorner_FromHitPoint(hit3.point);
                                            }
                                            if(gridBuilder.editMode == GridBuilderStateMachine.EditMode.edge)
                                            {
                                                gridBuilder.selectedCorners = hit3.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().GetCornersFromClosestEdge_FromHitPoint(hit3.triangleIndex, hit3.point);
                                            }
                                            if(gridBuilder.editMode == GridBuilderStateMachine.EditMode.face)
                                            {
                                                gridBuilder.selectedCorners = hit3.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().GetCornersFromFace_FromHitPoint(hit3.triangleIndex);
                                            }                   
                                            




                                            foreach(SaveAndLoadLevel.Corner corner in gridBuilder.selectedCorners)
                                            {
                                                Debug.Log(corner.corner_Pos);
                                                GameObject obj = GameObject.Instantiate(gridBuilder.selectedCornerSetterPrefab,  corner.corner_Pos, Quaternion.identity);
                                                obj.GetComponent<SelectedCornerObject>().corner = corner;
                                                
                                                gridBuilder.listOfSelectedCornerObjects.Add(obj);
                                                obj.transform.SetParent(gridBuilder.edgeEditGimbalContainer);

                                                //obj.SetParent();
                                            }

         

                                            break;
                                            }
                                        
                                    }
                                }
                                else
                                {
                                            //if arrow touched first save the pos
                                            gimbalParentStartPos = gridBuilder.edgeEditGimbal.transform.position; //save position in case of reset later
                                }

        }



                    //on mouse up... stop holding arrow
                    if(Input.GetMouseButtonUp(0))
                    {
                        if(currentHoldingTransformArrow)
                        {
                            currentHoldingTransformArrow.GetComponentInParent<TransformArrow_GimbalGroup>().UnhideAllArrows();

                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeld = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeldEvent = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().offsetStart = false;

                            //turn off eyes
                            gridBuilder.edgeEditModel_Eyes.SetActive(false);
                            //highlight again on stop drag
                            HighlightToCurrentlySelectedBlock(gridBuilder);

                        }

                        currentHoldingTransformArrow = null;
                    
                        if(currentCubeToEdit)
                        {
                            currentCubeToEdit.GetComponent<BlockFaceTextureUVProperties>().SetPivotToBlockCenter();
                        }
                    }

                        //cancelling
                        if(Input.GetMouseButtonDown(1))
                        {

                            if(currentHoldingTransformArrow)
                            {
                            currentHoldingTransformArrow.GetComponentInParent<TransformArrow_GimbalGroup>().UnhideAllArrows();

                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeld = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeldEvent = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().offsetStart = false;
                          
                            currentHoldingTransformArrow = null;
                            gridBuilder.edgeEditGimbal.transform.position = gimbalParentStartPos;
                        //    currentCubeToEdit.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().RepositionCorner( gridBuilder.edgeEditGimbal.position);
                              
                                //turn off eyes on cancel
                                gridBuilder.edgeEditModel_Eyes.SetActive(false);
                                //highlight again on cancel
                                HighlightToCurrentlySelectedBlock(gridBuilder);


                            }


                            
                        }


            //constantly update vertices while in this state
            if(currentCubeToEdit != null)
            {

            } 


            /*
            //reset corners
            if(Input.GetKeyDown(KeyCode.R))
            {
                //clear lists or else reset wont work
                ClearSelectedCornerList(gridBuilder);


                currentCubeToEdit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().ResetCornerPositions();

                UnHighlightToCurrentlySelectedBlock(gridBuilder);
            }
*/
            //cycle through edit modes
            if(Input.GetKeyDown(KeyCode.LeftShift))
            {
                CycleThroughModes(gridBuilder);
            }



            //exit
            if(Input.GetKeyDown(KeyCode.Q))
            {
                indexState = -1;
            }

            //freelook
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

            //Constantly Update the corners and stuff of the block being edited
            currentCubeToEdit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().UpdateCorners();
            currentCubeToEdit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().UpdateBlockUV();



                //CONRNER ONE AXIS POSITIONER (while holding arrow)
                if(Input.GetKey(KeyCode.X) && currentHoldingTransformArrow != null)
                {
                    
                    //for preventing the arrow from moving by offset when the player is holding down the snap button
                    currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeld = false;

                    //Step 1. Find the Closest Corner hit by ray (so it can be used later to position the plane)
                    RaycastHit vhit;
                    Ray ray_PickCorner = gridBuilder.mainCam.ScreenPointToRay(Input.mousePosition);

                     if (Physics.Raycast(ray_PickCorner, out vhit, Mathf.Infinity))
                    {
                         if(vhit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>()
                         && vhit.transform.gameObject != currentCubeToEdit) // make sure it does not snap to corners of the current cube
                        {
                            gridBuilder.positionerTicker.gameObject.SetActive(true);
                            vhit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().CornerClosestOffsetToEditToRayHit(vhit.point);

                            gridBuilder.positionerTicker.position = (Vector3)vhit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().closestCornerPoint.corner_Pos;

                            //make the ticker match the current holding arrow rotation
                            gridBuilder.positionerTicker.rotation = currentHoldingTransformArrow.transform.rotation;
                        }

                        //get the position of the gymbal relative to the ticker's local axis.
                        Vector3 relativePosition = gridBuilder.positionerTicker.transform.InverseTransformPoint(gridBuilder.edgeEditGimbal.position);
                        
                        //keep the relative position but sent the y axis to zero... so that its on the same plane as the ticker
                        Vector3 modifiedRelativePosition = new Vector3(relativePosition.x, 0, relativePosition.z);

                        //finally, set the position back to world space                 
                        currentHoldingTransformArrow.transform.parent.position = gridBuilder.positionerTicker.transform.TransformPoint(modifiedRelativePosition);
                        }


                }
                else
                {
                    //for making sure the arrow goes back to being moved by offset dragging when the player is not holding the snap button
                    if(currentHoldingTransformArrow != null)
                    {
                        currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeld = true;
                    }
                }







                //Vertex Corner Repositioner

                if(Input.GetKey(KeyCode.X) && currentHoldingTransformArrow == null)
                {
                    //hide arrows(for clarity)
                    gridBuilder.edgeEditGimbal.GetComponent<TransformArrow_GimbalGroup>().HideAllArrows();


                    RaycastHit vhit;
                    Ray ray_PickCorner = gridBuilder.mainCam.ScreenPointToRay(Input.mousePosition);

                     if (Physics.Raycast(ray_PickCorner, out vhit, Mathf.Infinity))
                    {
                         if(vhit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>())
                        {
                            gridBuilder.positionerTicker.gameObject.SetActive(true);
                            vhit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().CornerClosestOffsetToEditToRayHit(vhit.point);
                            gridBuilder.positionerTicker.position = (Vector3)vhit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().closestCornerPoint.corner_Pos;

                        }
                    }
                    else
                    {
                        gridBuilder.positionerTicker.gameObject.SetActive(false);
                    }

                    //if positioner ticker is active and user presses mouse down... create the parent hierchy of selected objects
                    if(Input.GetMouseButton(0) && gridBuilder.positionerTicker.gameObject.activeSelf)
                    {
                        //3. automatically place this object on the cloest corner of object hit by ray
                        if (Physics.Raycast(ray_PickCorner, out vhit, Mathf.Infinity))
                        {
                            if(vhit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>())
                            {
                                vhit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().CornerClosestOffsetToEditToRayHit(vhit.point);
                            //    currentCubeToEdit.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().RepositionCorner( vhit.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().closestCornerPoint.corner_Pos);
                            }
                        }

                        //Save position in case of reset
                        if(repositioner_Event)
                        {
                            RepositionerStartPos = gridBuilder.edgeEditGimbal.transform.position;
                            repositioner_Event = false;
                        }
                    }

                          if(Input.GetMouseButtonUp(0) && gridBuilder.positionerTicker.gameObject.activeSelf)
                    {
                        if(gridBuilder.selectedObjects.Count >= 1)
                        gridBuilder.edgeEditGimbal.transform.position = GlobalUtilityFunctions.CalculateAverageVectorPositionFromListOfGameObjects(gridBuilder.selectedObjects);

                         foreach(GameObject obj in gridBuilder.selectedObjects)
                        {
                            obj.transform.SetParent(gridBuilder.gymbalArrowsContainer);
                        }
                    }


                    //cancelling
                    if(Input.GetMouseButton(1))
                    {
                        gridBuilder.edgeEditGimbal.transform.position = RepositionerStartPos;
                    }


                }
                else
                {
                    repositioner_Event = true;
                  //  gridBuilder.positionerTicker.gameObject.SetActive(false);

                    if(currentHoldingTransformArrow == null)
                    {
                    gridBuilder.edgeEditGimbal.GetComponent<TransformArrow_GimbalGroup>().UnhideAllArrows();
                    }
                }




            CheckForThresholdAndTrigger(gridBuilder);


   
         }

        void ResetState(GridBuilderStateMachine gridBuilder)
        {

        }


        void OnExit(GridBuilderStateMachine gridBuilder)
            {
                gridBuilder.edgeEditModel_Eyes.SetActive(false);
                UnHighlightToCurrentlySelectedBlock(gridBuilder);

                gridBuilder.selectedCornersLineRenderer_GameObject.SetActive(false);
                gridBuilder.edgeEditGimbal.gameObject.SetActive(false);
                gridBuilder.edgeEditModel_Head.SetActive(false);

                indexState = 0;

                GridSizeManager.Instance.gridSize_Canvas.SetActive(false);                                                           
            }


        void ClearSelectedCornerList(GridBuilderStateMachine gridBuilder)
        {
                gridBuilder.selectedCorners.Clear();
                foreach(GameObject obj in gridBuilder.listOfSelectedCornerObjects)
                {
                        GameObject.Destroy(obj);
                }
                gridBuilder.listOfSelectedCornerObjects.Clear();

        }

        void CycleThroughModes(GridBuilderStateMachine gridBuilder)
        {
                int currentStateIndex = (int)gridBuilder.editMode;
                currentStateIndex++;

                // Check if the new index exceeds the maximum enum value
                if (currentStateIndex >= System.Enum.GetValues(typeof(GridBuilderStateMachine.EditMode)).Length)
                {
                    currentStateIndex = 0; // Wrap around to the first state
                }

                // Convert the index back to the enum value
                gridBuilder.editMode = (GridBuilderStateMachine.EditMode)currentStateIndex;
    
                ToggleAppropriateModelDependingOnMode(gridBuilder);
        }


        void ToggleAppropriateModelDependingOnMode(GridBuilderStateMachine gridBuilder)
        {
            if(gridBuilder.editMode == GridBuilderStateMachine.EditMode.corner)
            {
                gridBuilder.edgeEditChangeMode_AudioSource.pitch = 0.8f;
                gridBuilder.edgeEditChangeMode_AudioSource.Play();

                gridBuilder.edgeEditModel_modeCorner.SetActive(true);
                gridBuilder.edgeEditModel_modeEdge.SetActive(false);
                gridBuilder.edgeEditModel_modeFace.SetActive(false);
            }
            if(gridBuilder.editMode == GridBuilderStateMachine.EditMode.edge)
            {
                gridBuilder.edgeEditChangeMode_AudioSource.pitch = 1f;
                gridBuilder.edgeEditChangeMode_AudioSource.Play();


                gridBuilder.edgeEditModel_modeCorner.SetActive(false);
                gridBuilder.edgeEditModel_modeEdge.SetActive(true);
                gridBuilder.edgeEditModel_modeFace.SetActive(false);
            }
                if(gridBuilder.editMode == GridBuilderStateMachine.EditMode.face)
            {
                gridBuilder.edgeEditChangeMode_AudioSource.pitch = 1.2f;
                gridBuilder.edgeEditChangeMode_AudioSource.Play();

                gridBuilder.edgeEditModel_modeCorner.SetActive(false);
                gridBuilder.edgeEditModel_modeEdge.SetActive(false);
                gridBuilder.edgeEditModel_modeFace.SetActive(true);
            }
        }






        //distance calculation
        float arrowMoveDistanceThreshold = 0.05f;
        Vector3 previousPosition;

         //used to prevent the function from being called too much
        private float maxCallsPerSecond = 7f;
        private float timeBetweenCalls;

        void CheckForThresholdAndTrigger(GridBuilderStateMachine gridBuilder)
        {
        if(currentHoldingTransformArrow)
        {
            float distanceMoved = Vector3.Distance(currentHoldingTransformArrow.transform.parent.position, previousPosition);

            //used to prevent the function from being called too much
            if (Time.time >= timeBetweenCalls)
            {

                    if (distanceMoved >= arrowMoveDistanceThreshold)
                        {

                            // Call your function here.
                            foreach(GameObject obj in gridBuilder.listOfSelectedCornerObjects)
                            {
                                GameObject particleObj = GameObject.Instantiate(gridBuilder.edgeEditParticle_Prefab, obj.transform.position, Quaternion.identity);
                                particleObj.GetComponent<ParticleSystem>().startSize  = gridBuilder.incrementSizeToParticleSize_AnimationCurve.Evaluate(distanceMoved);
                                particleObj.GetComponent<ParticleSystemRenderer>().material = currentCubeToEdit.GetComponent<Renderer>().sharedMaterial;
                            }

                            gridBuilder.edgeEditIncrement_AudioSource.transform.position = currentHoldingTransformArrow.transform.parent.position;
                            gridBuilder.edgeEditIncrement_AudioSource.pitch = gridBuilder.incrementSizeToPitch_AnimationCurve.Evaluate(distanceMoved);
                            gridBuilder.edgeEditIncrement_AudioSource.volume = gridBuilder.incrementSizeToVolume_AnimationCurve.Evaluate(distanceMoved);
                            gridBuilder.edgeEditIncrement_AudioSource.PlayOneShot(gridBuilder.edgeEditIncrement_AudioSource.clip, 1.0f);
                        }

            // Update the previous position to the current position for the next frame.
            previousPosition = currentHoldingTransformArrow.transform.parent.position;

            // Update the timer for the next allowed call
            timeBetweenCalls = Time.time + (1f / maxCallsPerSecond);
        }
       }

        }


        void HighlightToCurrentlySelectedBlock(GridBuilderStateMachine gridBuilder)
        {
            //set new material(s)
            currentCubeToEdit.GetComponent<BlockFaceTextureUVProperties>()._renderer.sharedMaterials = gridBuilder.edgeEditor_HighLightMatList;
            
        }

        void UnHighlightToCurrentlySelectedBlock(GridBuilderStateMachine gridBuilder)
        {
            //set back the older object's materials
            if(currentCubeToEdit)
            currentCubeToEdit.GetComponent<GeneralObjectInfo>().ResetMaterials();

        }

}
