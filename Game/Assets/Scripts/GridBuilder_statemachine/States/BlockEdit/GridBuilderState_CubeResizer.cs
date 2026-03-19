using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilderState_CubeResizer : IGridBuilderState 
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
                Cursor.lockState = CursorLockMode.Locked;
                gridBuilder.mouseLooker.enabled = true;

                OnExit(gridBuilder);
                return gridBuilder.GridBuilderStateIdle;
            }

            if(onExit_Selection)
            {
                OnExit(gridBuilder);
                return gridBuilder.GridBuilderStateSelection;
            }
        return gridBuilder.GridBuilderStateCubeResizer;
    }


    
        void OnEnter(GridBuilderStateMachine gridBuilder)
    {         
            if(gridBuilder.currentCubeToResize)
            {
            //update the corner positions... or else it will get set back!!!
            SaveAndLoadLevel.Corner[] currentCubeStartCornerData; //for resetting the corner position on cancel
            currentCubeStartCornerData = gridBuilder.currentCubeToResize.GetComponent<BlockFaceTextureUVProperties>().GetCurrentListOfUpdatedCorners();
            gridBuilder.currentCubeToResize.GetComponent<BlockFaceTextureUVProperties>().SetCornersFromCornerList(currentCubeStartCornerData);
            }
        
                gridBuilder.addingIcon.SetActive(false);
                gridBuilder.removingIcon.SetActive(false);


            gridBuilder.selectionInfo_Canvas.SetActive(true);
            RepositionArrowsAroundCube(gridBuilder);

            gridBuilder.selectedCornersLineRenderer_GameObject.SetActive(true);

            GridSizeManager.Instance.gridSize_Canvas.SetActive(true);                           
    }

        bool onExit_Selection = false;


        GameObject currentHoldingTransformArrow;

        Vector3 currentCubeStartScale; //for resetting the scale on cancel

        SaveAndLoadLevel.Corner[] currentCubeStartCornerData; //for resetting the corner position on cancel

        Vector3 currentCubeStartPos; //for resetting the position on cancel

        RaycastHit[] allHits;
        
        void OnStay(GridBuilderStateMachine gridBuilder)
        {
            //show/hide gimbal when a cube is selected
            if(gridBuilder.currentCubeToResize != null)
            {
                gridBuilder.rescaleEightArrowGimbal.SetActive(true);
            }
            else
            {
                gridBuilder.rescaleEightArrowGimbal.SetActive(false);
                onExit_Selection = true;
            }


        if(Input.GetMouseButtonDown(0))
        {

                                //prioritize arrow over selecting any other stuff
                                bool arrowWidgetTouchedFirst = false;
                                for (int i = 0; i < allHits.Length; i++)
                                {
                                    RaycastHit hit = allHits[i];

                                    if(hit.transform.GetComponent<TransformArrow_DragMovement>()) 
                                    {

                                        //important part, or else the gymbal parent holder will be offset on mouse hold down!
                                        //what this essentially does: it positions the parent holder at the exact position/rotation of the arrow.
                                        //make sure the arrow is set to local pos 0,0,0 and rotation 0,0,0... 
                                        gridBuilder.rescaleEightArrowGimbal.transform.position = hit.transform.gameObject.transform.position;
                                        gridBuilder.rescaleEightArrowGimbal.transform.rotation = hit.transform.gameObject.transform.rotation;

                                        hit.transform.gameObject.transform.localPosition = new Vector3(0,0,0);
                                        hit.transform.gameObject.transform.localRotation = Quaternion.identity;



                                        currentHoldingTransformArrow = hit.transform.gameObject;
                                        hit.transform.GetComponent<TransformArrow_DragMovement>().beingHeld = true;
                                        arrowWidgetTouchedFirst = true;

                                        //store start scale and pos for cancelling
                                        if(gridBuilder.currentCubeToResize != null)
                                        {
                                        currentCubeStartCornerData = gridBuilder.currentCubeToResize.GetComponent<BlockFaceTextureUVProperties>().GetCurrentListOfUpdatedCorners();
                                        currentCubeStartPos = gridBuilder.currentCubeToResize.transform.position;
                                        }

                                        SetSelectedCornersBasedOnCurrentlyHoldingResizeArrow(gridBuilder);
                                        CreateSelectedCornerList(gridBuilder);


                                        break;
                                    }
                                }

    
                if(arrowWidgetTouchedFirst == false)
                {
                //pick cube 

                  RaycastHit hit2;
                  Ray ray2 = gridBuilder.mainCam.ScreenPointToRay(Input.mousePosition);

                     if (Physics.Raycast(ray2, out hit2, Mathf.Infinity, gridBuilder.selectionTool_LayerMask))
                    {
                        //on select block
                         if(hit2.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>())
                        {
                            if(gridBuilder.currentCubeToResize)
                            {

                                                if(isAdding && !gridBuilder.selectedObjects.Contains(hit2.transform.gameObject))
                                                {
                                                }
                                                else if(isRemoving)
                                                {
                                                hit2.transform.SetParent(null);
                                                gridBuilder.selectedObjects.Remove(hit2.transform.gameObject);
                                                onExit_Selection = true;  
                                                }
                                                else if(!isAdding && !isRemoving) //if not adding or removeing, then you are resizing the cube... switch state...
                                                {
                                                    if(hit2.transform.gameObject == gridBuilder.currentCubeToResize)
                                                    {
                                                        
                                                        currentHoldingTransformArrow = null; //make sure to set it to null or else the block update functions will be called and cause errors in the code below
                                                        RemoveAllSelectedItems(gridBuilder);
                                                        gridBuilder.selectedObjects.Add(hit2.transform.gameObject);
                                                        hit2.transform.SetParent(gridBuilder.gymbalArrowsContainer);
                                                        onExit_Selection = true;
                                                    }
                                                    else
                                                    {
                                                        gridBuilder.currentCubeToResize = hit2.transform.gameObject;
                                                    }
                                                }
                            }
                            
                        }

                                            //on select poster or door
                                            if(hit2.transform.GetComponent<CustomeItemType>())
                                            {
                                                hit2.transform.SetParent(gridBuilder.gymbalArrowsContainer);

                                                 //Add, remove, or clear list and select the single object if modifers are all false
                                                if(isAdding && !gridBuilder.selectedObjects.Contains(hit2.transform.gameObject))
                                                {
                                                gridBuilder.selectedObjects.Add(hit2.transform.gameObject);
                                                onExit_Selection = true;
                                                }
                                                else if(isRemoving)
                                                {
                                                hit2.transform.SetParent(null);
                                                gridBuilder.selectedObjects.Remove(hit2.transform.gameObject);
                                                onExit_Selection = true;
                                                }
                                                else if(!isAdding && !isRemoving)
                                                {
                                                RemoveAllSelectedItems(gridBuilder);
                                                gridBuilder.selectedObjects.Add(hit2.transform.gameObject);
                                                onExit_Selection = true;
                                                }
                                            }

                    }

                    // position the arrows
                    RepositionArrowsAroundCube(gridBuilder);

        }
            
                    //if nothing was hit... then deselect everything
                    if(allHits.Length <= 0)
                    {
                        RemoveAllSelectedItems(gridBuilder);
                        onExit_Selection = true;
                    }

        }


                                Ray ray = gridBuilder.mainCam.ScreenPointToRay(Input.mousePosition);
                                allHits = Physics.RaycastAll(ray, Mathf.Infinity, gridBuilder.selectionTool_LayerMask);

                    //on mouse up... stop holding arrow
                    if(Input.GetMouseButtonUp(0))
                    {
                        //on let go of arrow, reposition them
                        if(gridBuilder.currentCubeToResize != null)
                        {
                              gridBuilder.currentCubeToResize.GetComponentInChildren<BlockFaceTextureUVProperties>().UpdateBlockUV();
                              gridBuilder.currentCubeToResize.GetComponentInChildren<BlockFaceTextureUVProperties>().SetPivotToBlockCenter();
                              gridBuilder.currentCubeToResize.GetComponent<GeneralObjectInfo>().UpdatePosition();

                              RepositionArrowsAroundCube(gridBuilder);
                        }



                        ClearSelectedCornerList(gridBuilder);


                        if(currentHoldingTransformArrow)
                        { 
                            currentHoldingTransformArrow.GetComponentInParent<TransformArrow_GimbalGroup>().UnhideAllArrows();

                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeld = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeldEvent = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().offsetStart = false;
                        }

                        currentHoldingTransformArrow = null;
                    }

                         //cancelling
                        if(Input.GetMouseButtonDown(1))
                        {

                            if(currentHoldingTransformArrow)
                            {
                            ClearSelectedCornerList(gridBuilder); //clear or else it will bounce back

                            currentHoldingTransformArrow.GetComponentInParent<TransformArrow_GimbalGroup>().UnhideAllArrows();

                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeld = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeldEvent = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().offsetStart = false;
                            currentHoldingTransformArrow = null;

                            if(gridBuilder.currentCubeToResize)
                            {
                                gridBuilder.currentCubeToResize.GetComponent<BlockFaceTextureUVProperties>().SetCornersFromCornerList(currentCubeStartCornerData);
                                gridBuilder.currentCubeToResize.transform.position= currentCubeStartPos;
                            }
                            RepositionArrowsAroundCube(gridBuilder);

                            }


                        }
        
                gridBuilder.selectionCount_Text.text = "Selected: " + gridBuilder.selectedObjects.Count;


            //exit on hold c AND shift OR alt
            if(Input.GetKey(KeyCode.C) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftAlt)))
            {
                onExit_Selection = true;
            }

            //freelook
            if(Input.GetKeyDown(KeyCode.C))
            {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        gridBuilder.mouseLooker.enabled = false;
            }
            if(Input.GetKeyUp(KeyCode.C))
            {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        gridBuilder.mouseLooker.enabled = true;
            }


            if(currentHoldingTransformArrow != null)
            {
                if(gridBuilder.currentCubeToResize)
                {
                gridBuilder.currentCubeToResize.GetComponent<BlockFaceTextureUVProperties>().UpdateCorners();
                gridBuilder.currentCubeToResize.GetComponent<BlockFaceTextureUVProperties>().UpdateBlockUV();
                }
            }

            if(Input.GetKeyDown(KeyCode.Q))
            {
              indexState = -1;
            }



        }


        //selection modifiers
        bool isAdding = false;
        bool isRemoving = false;


    public void RepositionArrowsAroundCube(GridBuilderStateMachine gridBuilder)
    {
                if(gridBuilder.currentCubeToResize)
                {
                    gridBuilder.cubeResizeArrow_Y_front.transform.position = gridBuilder.currentCubeToResize.GetComponentInChildren<BlockFaceTextureUVProperties>().GetFaceCenterPos_Top();
                    gridBuilder.cubeResizeArrow_Y_front.transform.rotation = Quaternion.FromToRotation(Vector3.up, gridBuilder.currentCubeToResize.transform.up);

                    gridBuilder.cubeResizeArrow_Y_back.transform.position = gridBuilder.currentCubeToResize.GetComponentInChildren<BlockFaceTextureUVProperties>().GetFaceCenterPos_Bot();
                    gridBuilder.cubeResizeArrow_Y_back.transform.rotation = Quaternion.FromToRotation(Vector3.up, -gridBuilder.currentCubeToResize.transform.up);

                    gridBuilder.cubeResizeArrow_X_front.transform.position = gridBuilder.currentCubeToResize.GetComponentInChildren<BlockFaceTextureUVProperties>().GetFaceCenterPos_X_Front();
                    gridBuilder.cubeResizeArrow_X_front.transform.rotation = Quaternion.FromToRotation(Vector3.up, gridBuilder.currentCubeToResize.transform.right);

                    gridBuilder.cubeResizeArrow_X_back.transform.position = gridBuilder.currentCubeToResize.GetComponentInChildren<BlockFaceTextureUVProperties>().GetFaceCenterPos_X_Back();
                    gridBuilder.cubeResizeArrow_X_back.transform.rotation = Quaternion.FromToRotation(Vector3.up, -gridBuilder.currentCubeToResize.transform.right);

                    gridBuilder.cubeResizeArrow_Z_front.transform.position = gridBuilder.currentCubeToResize.GetComponentInChildren<BlockFaceTextureUVProperties>().GetFaceCenterPos_Z_Front();
                    gridBuilder.cubeResizeArrow_Z_front.transform.rotation = Quaternion.FromToRotation(Vector3.up, gridBuilder.currentCubeToResize.transform.forward);

                    gridBuilder.cubeResizeArrow_Z_back.transform.position = gridBuilder.currentCubeToResize.GetComponentInChildren<BlockFaceTextureUVProperties>().GetFaceCenterPos_Z_Back();
                    gridBuilder.cubeResizeArrow_Z_back.transform.rotation = Quaternion.FromToRotation(Vector3.up, -gridBuilder.currentCubeToResize.transform.forward);
                }
    }

    public void SetSelectedCornersBasedOnCurrentlyHoldingResizeArrow(GridBuilderStateMachine gridBuilder)
    {
        if(gridBuilder.currentCubeToResize)
        {
            if(currentHoldingTransformArrow == gridBuilder.cubeResizeArrow_Y_front)
            {
            gridBuilder.selectedCorners = gridBuilder.currentCubeToResize.GetComponent<BlockFaceTextureUVProperties>().GetCornersFromFace_FromHitPoint(0);
            }
            if(currentHoldingTransformArrow == gridBuilder.cubeResizeArrow_Y_back)
            {
            gridBuilder.selectedCorners = gridBuilder.currentCubeToResize.GetComponent<BlockFaceTextureUVProperties>().GetCornersFromFace_FromHitPoint(4);
            }
            if(currentHoldingTransformArrow == gridBuilder.cubeResizeArrow_X_front)
            {
            gridBuilder.selectedCorners = gridBuilder.currentCubeToResize.GetComponent<BlockFaceTextureUVProperties>().GetCornersFromFace_FromHitPoint(2);
            }
            if(currentHoldingTransformArrow == gridBuilder.cubeResizeArrow_X_back)
            {
            gridBuilder.selectedCorners = gridBuilder.currentCubeToResize.GetComponent<BlockFaceTextureUVProperties>().GetCornersFromFace_FromHitPoint(6);
            }
            if(currentHoldingTransformArrow == gridBuilder.cubeResizeArrow_Z_front)
            {
            gridBuilder.selectedCorners = gridBuilder.currentCubeToResize.GetComponent<BlockFaceTextureUVProperties>().GetCornersFromFace_FromHitPoint(10);
            }
            if(currentHoldingTransformArrow == gridBuilder.cubeResizeArrow_Z_back)
            {
            gridBuilder.selectedCorners = gridBuilder.currentCubeToResize.GetComponent<BlockFaceTextureUVProperties>().GetCornersFromFace_FromHitPoint(8);
            }
        }
    }

        void CreateSelectedCornerList(GridBuilderStateMachine gridBuilder)
        {
                foreach(SaveAndLoadLevel.Corner corner in gridBuilder.selectedCorners)     
                {
                    Debug.Log(corner.corner_Pos);
                    GameObject obj = GameObject.Instantiate(gridBuilder.selectedCornerSetterPrefab,  corner.corner_Pos, Quaternion.identity);
                    obj.GetComponent<SelectedCornerObject>().corner = corner;
                    
                    gridBuilder.listOfSelectedCornerObjects.Add(obj);

                    obj.transform.SetParent(gridBuilder.cubeResizeGimbalContainer);
                }
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



        void RemoveAllSelectedItems(GridBuilderStateMachine gridBuilder)
        {
            //remove the items from the parent
            foreach(GameObject selectedItem in gridBuilder.selectedObjects)
            {
            selectedItem.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
            }
                gridBuilder.selectedObjects.Clear();
        }

        void OnExit(GridBuilderStateMachine gridBuilder)
            {

                onExit_Selection = false;
                gridBuilder.rescaleEightArrowGimbal.SetActive(false);
                gridBuilder.selectionInfo_Canvas.SetActive(false);

                gridBuilder.selectedCornersLineRenderer_GameObject.SetActive(false);

                indexState = 0;

                GridSizeManager.Instance.gridSize_Canvas.SetActive(false);                                                           
            }


}
