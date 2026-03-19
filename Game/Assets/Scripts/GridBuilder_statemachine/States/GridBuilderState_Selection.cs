using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class GridBuilderState_Selection : IGridBuilderState 
{
    public IGridBuilderState DoState(GridBuilderStateMachine gridBuilder)
    {
        if(gridBuilder.newStateOnceEvent == false)
        {
            OnEnter(gridBuilder);
            gridBuilder.newStateOnceEvent = true;
        }


        OnStay(gridBuilder);

            if(onExit)
            {
                Cursor.lockState = CursorLockMode.Locked;
                gridBuilder.mouseLooker.enabled = true;

                OnExit(gridBuilder);
                return gridBuilder.GridBuilderStateIdle;
            }

            if(onExit_Resizer)
            {
                OnExit(gridBuilder);
                return gridBuilder.GridBuilderStateCubeResizer;
            }
           
        return gridBuilder.GridBuilderStateSelection;
    }


    
        void OnEnter(GridBuilderStateMachine gridBuilder)
    {
        gridBuilder.toolTipText.text = "<color=green> Shift: Add <color=white> | <color=red> Alt: Remove <color=white>| Right Mouse: Options | H: Hide, Alt+H: Unhide | G + Shift: Parent, G + Alt: Unparent | C + Mouse Drag: Selection | X(hold): Block Corner Snap | Scroll wheel: grid size | Del: <color=red>Destroy Selected | <color =white>Q: go back";
        gridBuilder.selectionInfo_Canvas.SetActive(true);
  
  
        //makes sure the gymbal position is correct when switching from resizer state, to selection state...
        gridBuilder.ResetGimbalOrientation();


        GeneralHighLighting(gridBuilder);                

        GridSizeManager.Instance.gridSize_Canvas.SetActive(true);                           
    }

        bool onExit = false;
        bool onExit_Resizer = false;
        GameObject currentHoldingTransformArrow;


        Vector3 RepositionerStartPos; //for resetting the repositioner container position on cancel
        bool repositioner_Event = true;


        RaycastHit[] allHits;
        

        //for saving the position(rotation arrows)
        Vector3 localArrowPos;
        Quaternion localArrowRot;


        void OnStay(GridBuilderStateMachine gridBuilder)
        {


            //auto show gymbal based on selected object count
            if(gridBuilder.selectedObjects.Count >= 1)
            {
                gridBuilder.gymbalThreeArrowWidget.gameObject.SetActive(true);
            }
            else
            {
                gridBuilder.gymbalThreeArrowWidget.gameObject.SetActive(false);
            }





                                Ray ray = gridBuilder.mainCam.ScreenPointToRay(Input.mousePosition);
                                allHits = Physics.RaycastAll(ray, Mathf.Infinity, gridBuilder.selectionTool_LayerMask);
                                //sorts the allHits array in order based on distance
                                System.Array.Sort(allHits, (x,y) => x.distance.CompareTo(y.distance));


                //Single click selection and arrow dragging                         //this part makes sure the player dosnt select things in the game by clicking past UI elements
                    if(Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.X) && !EventSystem.current.IsPointerOverGameObject())
                    {

                                bool arrowWidgetTouchedFirst = false;


/*
                                //if nothing has hit, unselect and hide all
                                if(allHits.Length == 0)
                                {
                                    HideWidget(gridBuilder);
                                }
                                else
                                {
                                    ShowWidget(gridBuilder);
                                }
*/
                                for (int i = 0; i < allHits.Length; i++)
                                {
                                    RaycastHit hit = allHits[i];

                                    if(hit.transform.GetComponent<TransformArrow_DragMovement>()) //prioritize arrow over selecting any other stuff
                                    {
                                        currentHoldingTransformArrow = hit.transform.gameObject;
                                        localArrowPos = currentHoldingTransformArrow.transform.localPosition;
                                        localArrowRot = currentHoldingTransformArrow.transform.localRotation;
                                        hit.transform.GetComponent<TransformArrow_DragMovement>().beingHeld = true;
                                        arrowWidgetTouchedFirst = true;
                                        break;
                                    }
                                    if(hit.transform.GetComponent<TransformArrow_DragRotate>()) //prioritize arrow over selecting any other stuff
                                    {
                                        currentHoldingTransformArrow = hit.transform.gameObject;
                                        localArrowPos = currentHoldingTransformArrow.transform.localPosition;
                                        localArrowRot = currentHoldingTransformArrow.transform.localRotation;

                                        hit.transform.GetComponent<TransformArrow_DragRotate>().beingHeld = true;
                                        arrowWidgetTouchedFirst = true;
                                        break;
                                    }
                                }

  



                                 //if the widget was not touched, then proceed to loop trhough all hit objects and find the first one that is not in the selected objects list, break; from loop when object is found
                                if(arrowWidgetTouchedFirst == false) 
                                {

                                    for (int i = 0; i < allHits.Length; i++)
                                    {
                                        RaycastHit hit = allHits[i];

                                        RaycastHit closestUnselectedHit = ReturnClosestUnselectedHitObject(gridBuilder);
                                        RaycastHit closestSelectedHit = ReturnClosestSelectedHitObject(gridBuilder);

                                        //make sure its an entity that can be edited in the level editor...
                                        if(closestUnselectedHit.transform.GetComponent<GeneralObjectInfo>())
                                        {
                                            if(closestUnselectedHit.transform.GetComponent<GeneralObjectInfo>().CheckIfObjectIsBeingPlayedOnPath() == false)
                                            {
                                            if(isAdding)
                                            {
                                                        //make sure its not already selected
                                                        if(!gridBuilder.selectedObjects.Contains(closestUnselectedHit.transform.gameObject))
                                                        {
                                                            closestUnselectedHit.transform.SetParent(gridBuilder.gymbalArrowsContainer);
                                                            gridBuilder.selectedObjects.Add(closestUnselectedHit.transform.gameObject);
                                                            AddAllChildrenOfObjectToSelection(gridBuilder, closestUnselectedHit.transform);
                                                            gridBuilder.SetObjectPropertyGUIElements();
                                                        }

                                                GeneralHighLighting(gridBuilder);                                           
                                            }
                                            else if(isRemoving)
                                            {
                                                    gridBuilder.selectedObjects.Remove(closestSelectedHit.transform.gameObject);
                                                    closestSelectedHit.transform.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
                                                    closestSelectedHit.transform.GetComponent<GeneralObjectInfo>().UpdatePosition();
                                                GeneralHighLighting(gridBuilder);                                           
                                            }
                                            else if(!isAdding && !isRemoving)
                                            {
                                                //if it is a block...
                                                if(closestSelectedHit.transform.GetComponent<BlockFaceTextureUVProperties>())
                                                {
                                                    gridBuilder.currentCubeToResize = closestSelectedHit.transform.gameObject;
                                                    onExit_Resizer = true;
                                                }
                                                else
                                                {
                                                    RemoveAllSelectedItems(gridBuilder);
                                                    closestUnselectedHit.transform.SetParent(gridBuilder.gymbalArrowsContainer);
                                                    gridBuilder.selectedObjects.Add(closestUnselectedHit.transform.gameObject);
                                                    AddAllChildrenOfObjectToSelection(gridBuilder, closestUnselectedHit.transform);
                                                    gridBuilder.SetObjectPropertyGUIElements();
                                                }

                                                GeneralHighLighting(gridBuilder);                                           
                                            }

                                                gridBuilder.ResetGimbalOrientation();
                                                break;
                                            }
                                            else
                                            {
                                                UINotificationHandler.Instance.SpawnNotification("<color=red> Object is being played on a path.");
                                            }
                                        }
                                    }
                                        
                                    
                                }
                                else
                                {
                                            //if arrow touched first save the pos
                                                gridBuilder.gimbalParentStartPos = gridBuilder.gymbalThreeArrowWidget.transform.position; //save position in case of reset later
                                                gridBuilder.gimbalParentStartRot = gridBuilder.gymbalThreeArrowWidget.transform.rotation; //save position in case of reset later

                                        //unhighlight selected objects(preview)
                                        HighLightManager.Instance.UnHighLightObjects(gridBuilder.selectedObjects);
                                }


                    //if nothing was hit... then deselect everything
                    if(allHits.Length <= 0)
                    {
                        RemoveAllSelectedItems(gridBuilder);
                        GeneralHighLighting(gridBuilder);
                    }


                    }


                    //on mouse up... stop holding arrow
                    if(Input.GetMouseButtonUp(0))
                    {
                        if(currentHoldingTransformArrow)
                        {
                            ResetGimbal(gridBuilder);
                            
                            if(currentHoldingTransformArrow.GetComponentInParent<TransformArrow_GimbalGroup>())
                            currentHoldingTransformArrow.GetComponentInParent<TransformArrow_GimbalGroup>().UnhideAllArrows();

                            if(currentHoldingTransformArrow.GetComponent<TransformArrow_DragRotate>()) // it is this way because the rotation arrow gets unparented on hold
                            {
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragRotate>().transformGimbal.GetComponent<TransformArrow_GimbalGroup>().UnhideAllArrows();
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragRotate>().SwitchMeshToRotateArrow();
                            }
                            if(currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>())
                            {
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeld = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeldEvent = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().offsetStart = false;
                            }
                            if(currentHoldingTransformArrow.GetComponent<TransformArrow_DragRotate>())
                            {
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragRotate>().beingHeld = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragRotate>().beingHeldEvent = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragRotate>().offsetStart = false;
                            }

                            currentHoldingTransformArrow.transform.parent = gridBuilder.gymbalThreeArrowWidget;
                            currentHoldingTransformArrow.transform.localPosition = localArrowPos;
                            currentHoldingTransformArrow.transform.localRotation = localArrowRot;


                            gridBuilder.SetObjectPropertyGUIElements();

                            gridBuilder.ResetGimbalOrientation();
                        }

                        currentHoldingTransformArrow = null;

                        //re-highlight selected objects
                        GeneralHighLighting(gridBuilder);
                    }




                        //opening low level options window
                    if(Input.GetMouseButtonDown(1))
                    {
                        if(gridBuilder.selectionLowLevelPanelHolder_Canvas.activeSelf)
                        {
                            Cursor.lockState = CursorLockMode.Locked;
                            Cursor.visible = false;
                            SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;

                            gridBuilder.selectionLowLevelPanelHolder_Canvas.SetActive(false);
                        }
                        else
                        {
                            Cursor.lockState = CursorLockMode.None;
                            Cursor.visible = true;
                            SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = true;

                            gridBuilder.selectionLowLevelPanelHolder_Canvas.SetActive(true);

                            gridBuilder.SetObjectPropertyGUIElements();
                        }
                    }






                        //cancelling
                        if(Input.GetMouseButtonDown(1))
                        {

                            if(currentHoldingTransformArrow)
                            {
 

                            gridBuilder.gymbalThreeArrowWidget.GetComponent<TransformArrow_GimbalGroup>().UnhideAllArrows(); //have to call the group this way because the rotation arrow becomes unparented while dragging

                            if(currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>())
                            {
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeld = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().beingHeldEvent = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragMovement>().offsetStart = false;

                            gridBuilder.gymbalThreeArrowWidget.transform.position = gridBuilder.gimbalParentStartPos;
                            }

                            if(currentHoldingTransformArrow.GetComponent<TransformArrow_DragRotate>())
                            {
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragRotate>().transformGimbal.GetComponent<TransformArrow_GimbalGroup>().UnhideAllArrows();
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragRotate>().SwitchMeshToRotateArrow();
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragRotate>().beingHeld = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragRotate>().beingHeldEvent = false;
                            currentHoldingTransformArrow.GetComponent<TransformArrow_DragRotate>().offsetStart = false;

                            currentHoldingTransformArrow.transform.parent = gridBuilder.gymbalThreeArrowWidget;
                            currentHoldingTransformArrow.transform.localRotation = localArrowRot;
                            }


                            currentHoldingTransformArrow.transform.parent = gridBuilder.gymbalThreeArrowWidget;
                            currentHoldingTransformArrow.transform.localPosition = localArrowPos;
                            currentHoldingTransformArrow.transform.localRotation = localArrowRot;
                            gridBuilder.gymbalThreeArrowWidget.transform.rotation = Quaternion.identity;


                            currentHoldingTransformArrow = null;

                            }
                        }
    
                 
                //update paths linerenderers
                UpdatePathLineRenderersOfSelectedObjects(gridBuilder);



                //Duplicating

                if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D) && gridBuilder.selectedObjects.Count >= 1)
                {
                    if(GridBuilder_PersistantCopyPasteData.Instance.totalAmountOfCoroutinesBeingUsedForLoading >= 1)
                    {
                        UINotificationHandler.Instance.SpawnNotification("Please wait until the previous objects have loaded!");
                    }
                    else
                    {
                        DuplicateSelectedObjects(gridBuilder);
                    }
                }

                //Setting selected objects as children of current object looking at (object visiblity manager)
                if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.G) && gridBuilder.selectedObjects.Count >= 1)
                {
                    SetSelectedObjectAsChildrenOfObject(gridBuilder, ReturnClosestUnselectedHitObject(gridBuilder).transform.gameObject);
                }

                //clearing parent
                if(Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.G) && gridBuilder.selectedObjects.Count >= 1)
                {
                    ClearParentOfSelectedObjects(gridBuilder);
                }

                //Deleting
                if(Input.GetKeyDown(KeyCode.Delete) && gridBuilder.selectedObjects.Count >= 1)
                {
                    DeleteSelectedObjects(gridBuilder);
                }

                //Copy Selected Objects to clipboard
                if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C) && gridBuilder.selectedObjects.Count >= 1)
                {
                    GridBuilder_PersistantCopyPasteData.Instance.CopyListOfObjects(gridBuilder.selectedObjects);
                    UINotificationHandler.Instance.SpawnNotification("Copied " + (gridBuilder.selectedObjects.Count) +  " selected objects", UINotificationHandler.NotificationStateType.ping);
                }

                //Paste Selected Objects to map (world space)
                if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
                {
                    if(GridBuilder_PersistantCopyPasteData.Instance.totalAmountOfCoroutinesBeingUsedForLoading >= 1)
                    {
                        UINotificationHandler.Instance.SpawnNotification("Please wait until the previous objects have loaded!");
                    }
                    else
                    {
                        GridBuilder_PersistantCopyPasteData.Instance.PasteListOfObjects();
                        UINotificationHandler.Instance.SpawnNotification("Pasted " + SaveAndLoadLevel.Instance.GetAllObjectCount(GridBuilder_PersistantCopyPasteData.Instance.copiedData) +  " objects", UINotificationHandler.NotificationStateType.ping);
                    }

             
                  //  RemoveAllSelectedItems(gridBuilder);

                    
                  //  GeneralHighLighting(gridBuilder);
                }



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
                         if((vhit.transform.gameObject.layer == 14 || vhit.transform.gameObject.layer == 17) && vhit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>())
                        {
                            gridBuilder.positionerTicker.gameObject.SetActive(true);
                            vhit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().CornerClosestOffsetToEditToRayHit(vhit.point);

                            gridBuilder.positionerTicker.position = (Vector3)vhit.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().closestCornerPoint.corner_Pos;

                            //make the ticker match the current holding arrow rotation
                            gridBuilder.positionerTicker.rotation = currentHoldingTransformArrow.transform.rotation;
                        }

                        //get the position of the gymbal relative to the ticker's local axis.
                        Vector3 relativePosition = gridBuilder.positionerTicker.transform.InverseTransformPoint(gridBuilder.gymbalThreeArrowWidget.position);
                        
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

                if(Input.GetKey(KeyCode.X) && currentHoldingTransformArrow == null && gridBuilder.selectedObjects.Count >= 1)
                {
                    //hide arrows(for clarity)
                    gridBuilder.gymbalThreeArrowWidget.GetComponent<TransformArrow_GimbalGroup>().HideAllArrows();


                    RaycastHit hit;
                    Ray ray_PickCorner = gridBuilder.mainCam.ScreenPointToRay(Input.mousePosition);

                     if (Physics.Raycast(ray_PickCorner, out hit, Mathf.Infinity))
                    {
                        //on general object entity center
                        if(hit.transform.gameObject.GetComponent<GeneralObjectInfo>())
                        {
                            gridBuilder.positionerTicker.gameObject.SetActive(true);
                            gridBuilder.positionerTicker.position = hit.transform.position;
                        }
                        
                        //on block corner
                         if((hit.transform.gameObject.layer == 14 || hit.transform.gameObject.layer == 17) && hit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>())
                        {
                            gridBuilder.positionerTicker.gameObject.SetActive(true);
                            hit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().CornerClosestOffsetToEditToRayHit(hit.point);
                            gridBuilder.positionerTicker.position = (Vector3)hit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().closestCornerPoint.corner_Pos;

                        }
       
                    }
                    else
                    {
                        gridBuilder.positionerTicker.gameObject.SetActive(false);
                    }

                    //if positioner ticker is active and user presses mouse down... create the parent hierchy of selected objects
                    if(Input.GetMouseButton(0) && gridBuilder.positionerTicker.gameObject.activeSelf)
                    {
                        //1. create object on the corner position
                        gridBuilder.positionerContainer.transform.position = gridBuilder.positionerTicker.position;

                        //2. set the selected objects as children of this object(make sure not to reset position)
                        foreach(GameObject obj in gridBuilder.selectedObjects)
                        {
                            if(obj)
                            {
                                obj.transform.parent = gridBuilder.positionerContainer.transform;
                            }
                        }

                        //3. automatically place this object on the cloest corner of object hit by ray
                        if (Physics.Raycast(ray_PickCorner, out hit, Mathf.Infinity))
                        {
                            if((hit.transform.gameObject.layer == 14 || hit.transform.gameObject.layer == 17) && hit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>())
                            {
                                hit.transform.gameObject.GetComponent<BlockFaceTextureUVProperties>().CornerClosestOffsetToEditToRayHit(hit.point);
                                gridBuilder.positionerContainer.transform.position = (Vector3)hit.transform.gameObject.GetComponentInChildren<BlockFaceTextureUVProperties>().closestCornerPoint.corner_Pos;

                            }
                        }

                        //Save position in case of reset
                        if(repositioner_Event)
                        {
                            RepositionerStartPos = gridBuilder.positionerContainer.transform.position;
                            repositioner_Event = false;
                        }
                    }

                          if(Input.GetMouseButtonUp(0) && gridBuilder.positionerTicker.gameObject.activeSelf)
                    {
                        if(gridBuilder.selectedObjects.Count >= 1)
                        gridBuilder.gymbalThreeArrowWidget.transform.position = GlobalUtilityFunctions.CalculateAverageVectorPositionFromListOfGameObjects(gridBuilder.selectedObjects);

                         foreach(GameObject obj in gridBuilder.selectedObjects)
                        {
                            if(obj)
                            {
                                obj.transform.SetParent(gridBuilder.gymbalArrowsContainer);
                            }
                        }
                    }


                    //cancelling
                    if(Input.GetMouseButton(1))
                    {
                        gridBuilder.positionerContainer.transform.position = RepositionerStartPos;
                    }


                }
                else
                {
                    repositioner_Event = true;
                    gridBuilder.positionerTicker.gameObject.SetActive(false);

                    if(currentHoldingTransformArrow == null)
                    {
                    gridBuilder.gymbalThreeArrowWidget.GetComponent<TransformArrow_GimbalGroup>().UnhideAllArrows();
                    }
                }





            //mouse drag box (when arrow not being held and X key not being held)
            if(currentHoldingTransformArrow == null && !Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.C))
            {
                if(Input.GetMouseButtonDown(0))
                {
                    mousePositionInitial = Input.mousePosition;
                    isDragSelect = true;
                }

                if(Input.GetMouseButton(0))
                {

                        UpdateSelectionBox(gridBuilder);
                }

                if(Input.GetMouseButtonUp(0))
                {
                    if(isDragSelect)
                    {
                        isDragSelect = false;
                        UpdateSelectionBox(gridBuilder);
                    }
                }
            }




            //selection mofidiers
            if(Input.GetKey(KeyCode.LeftShift))
            {
                isAdding = true;
                isRemoving = false;
                gridBuilder.selectionBox.GetComponent<Image>().color = gridBuilder.selectionBoxAdd_Color;
                gridBuilder.addingIcon.SetActive(true);
                gridBuilder.removingIcon.SetActive(false);
            }
            else if(Input.GetKey(KeyCode.LeftAlt))
            {
                isRemoving = true;
                isAdding = false;
                gridBuilder.selectionBox.GetComponent<Image>().color = gridBuilder.selectionBoxRemove_Color;
                gridBuilder.addingIcon.SetActive(false);
                gridBuilder.removingIcon.SetActive(true);
            }
            else
            {
                isRemoving = false;
                isAdding = false;
                gridBuilder.selectionBox.GetComponent<Image>().color = new Color(0,0,0,0);
                gridBuilder.addingIcon.SetActive(false);
                gridBuilder.removingIcon.SetActive(false);

            }


            //selection count text updater
            gridBuilder.selectionCount_Text.text = "Selected (not including children): " + gridBuilder.selectedObjects.Count;



            if(Input.GetKeyDown(KeyCode.Q))
            {
                //if there are selected objects instead of exiting... clear all selection
                if(gridBuilder.selectedObjects.Count >= 1)
                {
                    RemoveAllSelectedItems(gridBuilder);
                    GeneralHighLighting(gridBuilder);
                    ObjectVisibilityViewManager.Instance.TriggerCurrentViewModeFunctions();
                }
                else
                {
                    onExit = true;
                }
            }

            //freelook

            //dont process these functions if the player is already in free mouse mode...
            if(gridBuilder.selectionLowLevelPanelHolder_Canvas.activeSelf == false)
            {
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
            }
           
            //Hiding Objects
            if(Input.GetKeyDown(KeyCode.H))
            {
                HideSelectedObjects(gridBuilder);
            }

            //UnHiding Objects
            if(Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.H)) 
            {
                UnHideSelectedObjects(gridBuilder);
            }

        }

        //selection modifiers
        bool isAdding = false;
        bool isRemoving = false;




        RaycastHit ReturnClosestUnselectedHitObject(GridBuilderStateMachine gridBuilder)
        {
            foreach(RaycastHit h in allHits)
            {

                    if(gridBuilder.selectedObjects.Contains(h.transform.gameObject) == false && h.transform.GetComponent<GeneralObjectInfo>())
                    {
                        return h;
                    }
            }
            return allHits[0];
        }

        RaycastHit ReturnClosestSelectedHitObject(GridBuilderStateMachine gridBuilder)
        {
            foreach(RaycastHit h in allHits)
            {
                    if(gridBuilder.selectedObjects.Contains(h.transform.gameObject) == true && h.transform.GetComponent<GeneralObjectInfo>())
                    {
                        return h;
                    }
            }
            return allHits[0];
        }





        //mouse drag box
        bool isDragSelect = false;
        Vector2 mousePositionInitial;
        Vector2 mousePositionEnd;


        void UpdateSelectionBox(GridBuilderStateMachine gridBuilder)
        {
            gridBuilder.selectionBox.gameObject.SetActive(isDragSelect);

            float width = Input.mousePosition.x - mousePositionInitial.x;
            float height = Input.mousePosition.y - mousePositionInitial.y;

            gridBuilder.selectionBox.anchoredPosition = mousePositionInitial + new Vector2(width/ 2, height/2 );
            gridBuilder.selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

            Bounds bounds = new Bounds(gridBuilder.selectionBox.anchoredPosition, gridBuilder.selectionBox.sizeDelta); 
 
            Vector2 minValue = gridBuilder.selectionBox.anchoredPosition - (gridBuilder.selectionBox.sizeDelta / 2);
            Vector2 maxValue = gridBuilder.selectionBox.anchoredPosition + (gridBuilder.selectionBox.sizeDelta / 2);


            //General Objects
            GeneralObjectInfo[] allCurrentObjectsInScene = Object.FindObjectsOfType(typeof(GeneralObjectInfo), true) as GeneralObjectInfo[];

            foreach(GeneralObjectInfo obj in allCurrentObjectsInScene)
            {
                    Vector3 objScreenPos = gridBuilder.mainCam.WorldToScreenPoint(obj.transform.GetComponent<Renderer>().bounds.center);

                    if(objScreenPos.x > minValue.x && objScreenPos.x < maxValue.x && objScreenPos.y > minValue.y && objScreenPos.y < maxValue.y  && objScreenPos.z > 0)
                    {
                        if(obj.transform.GetComponent<GeneralObjectInfo>().CheckIfObjectIsBeingPlayedOnPath() == false)
                        {   
                            //DO NOT select hidden gameobjects (local and global variables) 
                            if(obj.gameObject.layer != 21)
                            {
                                            //Add, remove, or clear list and select the single object if modifers are all false
                                                if(isAdding && !gridBuilder.selectedObjects.Contains(obj.gameObject))
                                                {
                                                obj.transform.SetParent(gridBuilder.gymbalArrowsContainer);
                                                gridBuilder.selectedObjects.Add(obj.gameObject);
                                                AddAllChildrenOfObjectToSelection(gridBuilder,obj.gameObject.transform);
                                                gridBuilder.SetObjectPropertyGUIElements();
                                                }
                                                else if(isRemoving)
                                                {
                                                obj.transform.SetParent(null);
                                                gridBuilder.selectedObjects.Remove(obj.gameObject);
                                                obj.gameObject.transform.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
                                                obj.gameObject.transform.GetComponent<GeneralObjectInfo>().UpdatePosition();
                                                }

                                                gridBuilder.ResetGimbalOrientation();
                            }
                        }
                    }
            }

        }



        void HideWidget(GridBuilderStateMachine gridBuilder)
        {
            RemoveAllSelectedItems(gridBuilder);
            gridBuilder.gymbalThreeArrowWidget.gameObject.SetActive(false);
        }

        void ShowWidget(GridBuilderStateMachine gridBuilder)
        {
            gridBuilder.gymbalThreeArrowWidget.gameObject.SetActive(true);
        }


        void ResetGimbal(GridBuilderStateMachine gridBuilder)
        {
                        foreach(GameObject selectedItem in gridBuilder.selectedObjects)
            {
                if(selectedItem)
                {
                    selectedItem.transform.parent = null;
                }
            }

            gridBuilder.gymbalThreeArrowWidget.eulerAngles = new Vector3(0f,0f,0f);


            foreach(GameObject selectedItem in gridBuilder.selectedObjects)
            {
                if(selectedItem)
                {
                    selectedItem.transform.parent = gridBuilder.gymbalArrowsContainer;
                }
            }
        }


        void DuplicateSelectedObjects(GridBuilderStateMachine gridBuilder)
        {
                    GridBuilder_PersistantCopyPasteData.Instance.CopyListOfObjects(gridBuilder.selectedObjects);
                    GridBuilder_PersistantCopyPasteData.Instance.PasteListOfObjects();

                    //show notification
                    GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("Duplicated " + gridBuilder.selectedObjects.Count + " objects", UINotificationHandler.NotificationStateType.ping);

        }

        void DeleteSelectedObjects(GridBuilderStateMachine gridBuilder)
        {
                    BlockBaseCubePositionFinder_Singleton.Instance.RemoveFromCube();

                    foreach(GameObject obj in gridBuilder.selectedObjects)
                    {
                        if(obj)
                        {
                            obj.GetComponent<GeneralObjectInfo>().RemoveThisChildFromParent();

                            //stop the video when the poster is deleted...
                            if(obj.GetComponent<PosterMeshCreator>())
                            {
                            obj.GetComponent<VLC_MediaPlayer>().StopVideoPlayerIfPosterWhereVideoIsPlayingWasChanged(obj.GetComponent<PosterMeshCreator>());
                            }

                            GameObject.Destroy(obj);
                            SaveAndLoadLevel.Instance.allLoadedBlocks.Remove(obj);
                            SaveAndLoadLevel.Instance.allLoadedGameObjects.Remove(obj);
                        }
                    }
                    gridBuilder.selectedObjects.Clear();



                    GlobalUtilityFunctions.UpdateChildrenObjectsOfAllObjectsInMap();

                    //update path renderers
                    ObjectVisibilityViewManager.Instance.UpdateAllPathLineRenderers();
        }



        void RemoveAllSelectedItems(GridBuilderStateMachine gridBuilder)
        {
            //remove the items from the parent
            foreach(GameObject selectedItem in gridBuilder.selectedObjects)
            {
                if(selectedItem)
                {
                    if(selectedItem.GetComponent<GeneralObjectInfo>())
                    {
                    selectedItem.GetComponent<GeneralObjectInfo>().SetParentAccordingToParentID();
                //    selectedItem.GetComponent<GeneralObjectInfo>().UpdateChildren();
                    }
                
                }
            }
                UpdatePositionsOfSelectedObjects(gridBuilder);

                gridBuilder.selectedObjects.Clear();
        }

        void HideSelectedObjects(GridBuilderStateMachine gridBuilder)
        {
            //foreach(GameObject selectedItem in gridBuilder.selectedObjects)  <--- this loops through "ALL" selected stuff! (legacy)
            
            //note! its important to loop through the container child list to add as parent... because if you loop through each selected object... the hierchy will be messed up.
            foreach(Transform selectedItem in gridBuilder.gymbalArrowsContainer)
            {
                if(selectedItem)
                {
                    if(selectedItem.GetComponent<GeneralObjectInfo>().canBeDeactivatedByEngine)
                    {
                                selectedItem.GetComponent<GeneralObjectInfo>().isActive_original = false;
                                selectedItem.GetComponent<GeneralObjectInfo>().ResetVisibility();
                    }
                }
            }   
        }

        void UnHideSelectedObjects(GridBuilderStateMachine gridBuilder)
        {
            //foreach(GameObject selectedItem in gridBuilder.selectedObjects)  <--- this loops through "ALL" selected stuff! (legacy)

            //note! its important to loop through the container child list to add as parent... because if you loop through each selected object... the hierchy will be messed up.
            foreach(Transform selectedItem in gridBuilder.gymbalArrowsContainer)
            {
                if(selectedItem)
                {
                    if(selectedItem.GetComponent<GeneralObjectInfo>().canBeDeactivatedByEngine)
                    {
                                selectedItem.GetComponent<GeneralObjectInfo>().isActive_original = true;
                                selectedItem.GetComponent<GeneralObjectInfo>().ResetVisibility();
                    }
                }
            }
        }

        void UpdatePathLineRenderersOfSelectedObjects(GridBuilderStateMachine gridBuilder)
        {
            foreach(GameObject selectedItem in gridBuilder.selectedObjects)
            {
                if(selectedItem)
                    {
                    if(selectedItem.GetComponent<GeneralObjectInfo>())
                    {
                        if(selectedItem.GetComponent<PathNode>())
                        {
                            selectedItem.GetComponent<PathNode_WaypointLineRenderer>().UpdateLineRenderer();
                        }


                        if(!string.IsNullOrEmpty(selectedItem.GetComponent<GeneralObjectInfo>().idOfParent))
                        {
                        //   if(GameObject.Find(selectedItem.GetComponent<GeneralObjectInfo>().idOfParent).GetComponent<PathNode>())
                        //   {
                        //   GameObject.Find(selectedItem.GetComponent<GeneralObjectInfo>().idOfParent).GetComponent<PathNode_WaypointLineRenderer>().UpdateLineRenderer();
                        //   }
                        }
                    }
                }
            }

  

        }


        void UpdatePositionsOfSelectedObjects(GridBuilderStateMachine gridBuilder)
        {



            foreach(GameObject selectedItem in gridBuilder.selectedObjects)
            {
                if(selectedItem)
                {
                    if(selectedItem.GetComponent<GeneralObjectInfo>())
                    {
                    selectedItem.GetComponent<GeneralObjectInfo>().UpdatePosition();
                    }

                    //update the corner positions! DO NOT update them on save... (because of auto save & path animation reasons)
                    if(selectedItem.GetComponent<BlockFaceTextureUVProperties>())
                    {
                    selectedItem.GetComponent<BlockFaceTextureUVProperties>().UpdateCornerPositions();
                    }
                }
            }
        }



        void SetSelectedObjectAsChildrenOfObject(GridBuilderStateMachine gridBuilder, GameObject objectToParentTo)
        {
        if(objectToParentTo.GetComponent<GeneralObjectInfo>())
        {
            //important! remove the highlighting or else...
            HighLightManager.Instance.UnHighLightObjects(gridBuilder.selectedObjects);



            //save the child list! DO NOT change hierchy(for example, "SetParent") while looping through children... it leads to messed up behavior
            List<Transform> childrenList = new List<Transform>();            
            //note! its important to loop through the container child list to add as parent... because if you loop through each selected object... the hierchy will be messed up.
            foreach(Transform selectedThing in gridBuilder.gymbalArrowsContainer)
            {
                childrenList.Add(selectedThing);
            }
            foreach(Transform child in childrenList)
            {
            child.SetParent(objectToParentTo.transform);
            }



            UINotificationHandler.Instance.SpawnNotification("Added " + (gridBuilder.selectedObjects.Count) +  " selected objects as children!!", UINotificationHandler.NotificationStateType.ping);

            //update hierchies
            foreach(GameObject selectedItem in gridBuilder.selectedObjects)
            {
                if(selectedItem)
                {
                    selectedItem.GetComponent<GeneralObjectInfo>().UpdateChildren();
                    selectedItem.GetComponent<GeneralObjectInfo>().UpdateParent();
                    selectedItem.GetComponent<GeneralObjectInfo>().UpdatePosition();
                }
            }

            RemoveAllSelectedItems(gridBuilder);
            GeneralHighLighting(gridBuilder);

            //do this so that the line renderers of the hierchy refresh
            ObjectVisibilityViewManager.Instance.currentGameObjectHovering = null;
        }
        }

        void ClearParentOfSelectedObjects(GridBuilderStateMachine gridBuilder)
        {

            //note! its important to loop through the contrainer child list to add as parent... because if you loop through each selected object... the hierchy will be messed up.
            foreach(Transform selectedThing in gridBuilder.gymbalArrowsContainer)
            {
                selectedThing.GetComponent<GeneralObjectInfo>().ClearParent();
            }



            foreach(GameObject selectedItem in gridBuilder.selectedObjects)
            {
                if(selectedItem)
                {
                    selectedItem.GetComponent<GeneralObjectInfo>().UpdateChildren();
                    selectedItem.GetComponent<GeneralObjectInfo>().UpdateParent();
                }
            }


            GlobalUtilityFunctions.UpdateChildrenObjectsOfAllObjectsInMap();

            UINotificationHandler.Instance.SpawnNotification("Cleard parent of selected objects!", UINotificationHandler.NotificationStateType.ping);
           
           
            RemoveAllSelectedItems(gridBuilder);
            GeneralHighLighting(gridBuilder);

        
        }

        //RECURSIVE FUNCTION TO ADD ALL CHILDREN OF SELECTED OBJECT
     private void AddAllChildrenOfObjectToSelection(GridBuilderStateMachine gridBuilder, Transform parent)
    {
        if(parent.GetComponent<GeneralObjectInfo>())
        {
            foreach (Transform child in parent)
            {
                //make sure its an entity and not just any object!!!
                if(child.GetComponent<GeneralObjectInfo>() && gridBuilder.selectedObjects.Contains(child.gameObject) == false) 
                gridBuilder.selectedObjects.Add(child.gameObject);

                // Recursively add children of this child
                AddAllChildrenOfObjectToSelection(gridBuilder, child);
            }
        }
    }

    //SELECTION PARENT/CHILD  MECHANIC

    public void GeneralHighLighting(GridBuilderStateMachine gridBuilder)
    {
        HighLightManager.Instance.UnHighLightObjects(gridBuilder.selectedObjects);
        
        if(gridBuilder.selectedObjects.Count >= 1)
        {
        HighLightManager.Instance.SetHighLightMaterial(HighLightManager.Instance.highlightMat_Basic);
        HighLightManager.Instance.HighLightObjects(gridBuilder.selectedObjects, Color.green);
        //HighLightCurrentSelectedObjectHierchy(gridBuilder);
        }
    }

    public void HighLightCurrentSelectedObjectHierchy(GridBuilderStateMachine gridBuilder)
    {
        HighLightManager.Instance.SetHighLightMaterial(HighLightManager.Instance.highlightMat_Basic);
        HighLightManager.Instance.HighLightObjects(gridBuilder.selectedObjects[gridBuilder.selectedObjects.Count - 1].transform.GetComponent<GeneralObjectInfo>().childrenObjects, Color.white);
       
        //if it has parent, highlight that red
        if(gridBuilder.selectedObjects[gridBuilder.selectedObjects.Count - 1].transform.GetComponent<GeneralObjectInfo>().parentObject)
        HighLightManager.Instance.HighLightObject(gridBuilder.selectedObjects[gridBuilder.selectedObjects.Count - 1].transform.GetComponent<GeneralObjectInfo>().parentObject, Color.red);
    }





        void OnExit(GridBuilderStateMachine gridBuilder)
            {
                gridBuilder.selectionLowLevelPanelHolder_Canvas.SetActive(false);
                RemoveAllSelectedItems(gridBuilder);
                gridBuilder.selectionInfo_Canvas.SetActive(false);
                onExit = false;
                onExit_Resizer = false;
                HideWidget(gridBuilder);

                GeneralHighLighting(gridBuilder);           

                GridSizeManager.Instance.gridSize_Canvas.SetActive(false);                                                           
            }


}
