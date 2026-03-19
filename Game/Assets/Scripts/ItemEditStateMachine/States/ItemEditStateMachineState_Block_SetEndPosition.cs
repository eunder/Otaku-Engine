using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEditStateMachineState_Block_SetEndPosition : IItemEditStateMachine 
{
        bool entered = false;

        bool exitEvent = false;

        bool exitEvent_idle = false;

    public IItemEditStateMachine DoState(ItemEditStateMachine itemEditor)
    {
        if(entered == false)
        {
            OnEnter(itemEditor);
            entered = true;
        }


        OnStay(itemEditor);

            if(exitEvent_idle)
            {
                OnExit(itemEditor);
                return itemEditor.ItemEditStateMachineStateIdle;
            }

           if(exitEvent)
            {
               OnExit(itemEditor);
               return itemEditor.ItemEditStateMachineStateBlock;
            }
        return itemEditor.ItemEditStateMachineStateBlockSetEndPosition;
    }


        Vector3 startPos_Gymbal; //for resting 
    
        void OnEnter(ItemEditStateMachine itemEditor)
    {


                PlayerObjectInteractionStateMachine.Instance.enabled = false;
                
                itemEditor.blockEditor_CANVAS.SetActive(false);


                Cursor.lockState = CursorLockMode.Locked;
                SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;

                PlayerMovementBasic.Instance.enabled = true;
                PlayerMovementTypeKeySwitcher.Instance.enabled = true;

                itemEditor.playerGameObject.SetActive(true);
    
                ObjectVisibilityViewManager.Instance.TurnOn_GizmoInteractionAndView();
                ObjectVisibilityViewManager.Instance.ShowHiddenGameObjects();




                                                itemEditor.currentObjectEditing.transform.SetParent(GridBuilderStateMachine.Instance.gymbalArrowsContainer);
                                                GridBuilderStateMachine.Instance.selectedObjects.Add(itemEditor.currentObjectEditing.transform.gameObject);

                                                GridBuilderStateMachine.Instance.gymbalArrowsContainer.parent = null;

                                                if(GridBuilderStateMachine.Instance.selectedObjects.Count >= 1)
                                                GridBuilderStateMachine.Instance.gymbalThreeArrowWidget.transform.position = GlobalUtilityFunctions.CalculateAverageVectorPositionFromListOfGameObjects(GridBuilderStateMachine.Instance.selectedObjects);
                                                
                                                GridBuilderStateMachine.Instance.gymbalArrowsContainer.parent = GridBuilderStateMachine.Instance.gymbalThreeArrowWidget.transform;
                                                                                  
                                                gimbalParentStartPos = GridBuilderStateMachine.Instance.gymbalThreeArrowWidget.transform.position; //save position in case of reset later
                                                gimbalParentStartRot = GridBuilderStateMachine.Instance.gymbalThreeArrowWidget.transform.rotation; //save rotation in case of reset later
               
               
                //store position after first item added, in case player resets 
                startPos_Gymbal = GridBuilderStateMachine.Instance.gymbalThreeArrowWidget.transform.position;

    }

        bool onExit = false;
        bool onExit_Resizer = false;
        GameObject currentHoldingTransformArrow;

        Vector3 gimbalParentStartPos; //for resetting the position on cancel
        Quaternion gimbalParentStartRot; //for resetting the rotation on cancel


        Vector3 RepositionerStartPos; //for resetting the repositioner container position on cancel
        bool repositioner_Event = true;


        RaycastHit[] allHits;
        

        //for saving the position(rotation arrows)
        Vector3 localArrowPos;
        Quaternion localArrowRot;


        void OnStay(ItemEditStateMachine itemEditor)
        {

            //auto show gymbal based on selected object count
            if(GridBuilderStateMachine.Instance.selectedObjects.Count >= 1)
            {
                GridBuilderStateMachine.Instance.gymbalThreeArrowWidget.gameObject.SetActive(true);
            }
            else
            {
                GridBuilderStateMachine.Instance.gymbalThreeArrowWidget.gameObject.SetActive(false);
            }





                                Ray ray = GridBuilderStateMachine.Instance.mainCam.ScreenPointToRay(Input.mousePosition);
                                allHits = Physics.RaycastAll(ray, Mathf.Infinity);

                //Single click selection and arrow dragging
                    if(Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.X))
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
   
                                }
                                else
                                {
                                            //if arrow touched first save the pos
                                                gimbalParentStartPos = GridBuilderStateMachine.Instance.gymbalThreeArrowWidget.transform.position; //save position in case of reset later
                                                gimbalParentStartRot = GridBuilderStateMachine.Instance.gymbalThreeArrowWidget.transform.rotation; //save position in case of reset later
                                }
                    }


                    //on mouse up... stop holding arrow
                    if(Input.GetMouseButtonUp(0))
                    {
                        if(currentHoldingTransformArrow)
                        {
                            ResetGimbal(itemEditor);
                            
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

                            currentHoldingTransformArrow.transform.parent = GridBuilderStateMachine.Instance.gymbalThreeArrowWidget;
                            currentHoldingTransformArrow.transform.localPosition = localArrowPos;
                            currentHoldingTransformArrow.transform.localRotation = localArrowRot;
                        }

                        currentHoldingTransformArrow = null;
                    }


            //freelook
            if(Input.GetKeyDown(KeyCode.C))
            {
                        Cursor.lockState = CursorLockMode.None;
                        GridBuilderStateMachine.Instance.mouseLooker.enabled = false;
            }
            if(Input.GetKeyUp(KeyCode.C))
            {
                        Cursor.lockState = CursorLockMode.Locked;
                        GridBuilderStateMachine.Instance.mouseLooker.enabled = true;
            }

                        if(Input.GetKeyUp(KeyCode.Q))
                        {
                            exitEvent = true;
                        }
            


            //cancel/reset
                if(Input.GetMouseButtonDown(1))
                {
                    GridBuilderStateMachine.Instance.gymbalThreeArrowWidget.transform.position = startPos_Gymbal;
                }


        }

        void OnExit(ItemEditStateMachine itemEditor)
        {

                entered = false; // reset
                exitEvent = false;
                exitEvent_idle = false;

                itemEditor.blockEditor_CANVAS.SetActive(true);

                HideWidget(itemEditor);


                Cursor.lockState = CursorLockMode.None;
                SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = true;

                PlayerMovementBasic.Instance.enabled = false;


                ObjectVisibilityViewManager.Instance.TurnOff_GizmoInteractionAndView();
                ObjectVisibilityViewManager.Instance.HideHiddenGameObjects();

        }


        void HideWidget(ItemEditStateMachine itemEditor)
        {
            RemoveAllSelectedItems(itemEditor);
            GridBuilderStateMachine.Instance.gymbalThreeArrowWidget.gameObject.SetActive(false);
        }

        void ResetGimbal(ItemEditStateMachine itemEditor)
        {
                        foreach(GameObject selectedItem in GridBuilderStateMachine.Instance.selectedObjects)
            {
                selectedItem.transform.parent = null;
            }

            GridBuilderStateMachine.Instance.gymbalThreeArrowWidget.eulerAngles = new Vector3(0f,0f,0f);


            foreach(GameObject selectedItem in GridBuilderStateMachine.Instance.selectedObjects)
            {
                selectedItem.transform.parent = GridBuilderStateMachine.Instance.gymbalArrowsContainer;
            }
        }

       void RemoveAllSelectedItems(ItemEditStateMachine itemEditor)
        {
            //remove the items from the parent
            foreach(GameObject selectedItem in GridBuilderStateMachine.Instance.selectedObjects)
            {
                selectedItem.transform.parent = null;
            }
                GridBuilderStateMachine.Instance.selectedObjects.Clear();
        }

}
