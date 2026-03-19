using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class ItemEditStateMachineState_Path: IItemEditStateMachine 
{
        bool entered = false;

    public IItemEditStateMachine DoState(ItemEditStateMachine itemEditor)
    {
        if(entered == false)
        {
            OnEnter(itemEditor);
            entered = true;
        }


        OnStay(itemEditor);

           if(itemEditor.currentObjectEditing == null)
            {
               OnExit(itemEditor);
               return itemEditor.ItemEditStateMachineStateIdle;
            }
        return itemEditor.ItemEditStateMachineStatePath;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {
                itemEditor.pathEditor_CANVAS.SetActive(true);
         
                itemEditor.pathEditor_time_InputField.text = itemEditor.currentObjectEditing.GetComponent<PathNode>().time.ToString();

                itemEditor.pathEditor_loopType_Dropdown.value = GlobalUtilityFunctions.SetInputFieldValueFromString(itemEditor.pathEditor_loopType_Dropdown, itemEditor.currentObjectEditing.GetComponent<PathNode>().loopType);
                itemEditor.pathEditor_closeLoop_Toggle.isOn = itemEditor.currentObjectEditing.GetComponent<PathNode>().closeLoop;

                itemEditor.pathEditor_wayPointRotation_Toggle.isOn = itemEditor.currentObjectEditing.GetComponent<PathNode>().wayPointRotation;


                itemEditor.pathEditor_easeType_Dropdown.value = GlobalUtilityFunctions.SetInputFieldValueFromString(itemEditor.pathEditor_easeType_Dropdown, itemEditor.currentObjectEditing.GetComponent<PathNode>().easeType);
                itemEditor.pathEditor_pathType_Dropdown.value = GlobalUtilityFunctions.SetInputFieldValueFromString(itemEditor.pathEditor_pathType_Dropdown, itemEditor.currentObjectEditing.GetComponent<PathNode>().pathType);
         
                itemEditor.pathEditor_moveToPath_Toggle.isOn = itemEditor.currentObjectEditing.GetComponent<PathNode>().moveToPath;



     }

        void OnStay(ItemEditStateMachine itemEditor)
        {

            if(itemEditor.currentObjectEditing)
            {
                itemEditor.currentObjectEditing.GetComponent<PathNode>().time = float.Parse(itemEditor.pathEditor_time_InputField.text);

                itemEditor.currentObjectEditing.GetComponent<PathNode>().loopType = itemEditor.pathEditor_loopType_Dropdown.options[itemEditor.pathEditor_loopType_Dropdown.value].text;
                itemEditor.currentObjectEditing.GetComponent<PathNode>().closeLoop = itemEditor.pathEditor_closeLoop_Toggle.isOn; 

                itemEditor.currentObjectEditing.GetComponent<PathNode>().wayPointRotation = itemEditor.pathEditor_wayPointRotation_Toggle.isOn;

                itemEditor.currentObjectEditing.GetComponent<PathNode>().easeType = itemEditor.pathEditor_easeType_Dropdown.options[itemEditor.pathEditor_easeType_Dropdown.value].text;
                itemEditor.currentObjectEditing.GetComponent<PathNode>().pathType = itemEditor.pathEditor_pathType_Dropdown.options[itemEditor.pathEditor_pathType_Dropdown.value].text;
               
                itemEditor.currentObjectEditing.GetComponent<PathNode>().moveToPath = itemEditor.pathEditor_moveToPath_Toggle.isOn;

          
            }

            if(Input.GetMouseButtonDown(1))
            {
                itemEditor.mouseLooker.wheelPickerIsTurnedOn = false;
            }
            if(Input.GetMouseButtonUp(1))
            {
                itemEditor.mouseLooker.wheelPickerIsTurnedOn = true;
            }

            //Notes
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<Note>())
            {
                itemEditor.currentObjectEditing.GetComponent<Note>().note = itemEditor.noteInputField.text;
            }
        }

        void OnExit(ItemEditStateMachine itemEditor)
        {
                entered = false; // reset
                itemEditor.pathEditor_CANVAS.SetActive(false);
        }

        





}
