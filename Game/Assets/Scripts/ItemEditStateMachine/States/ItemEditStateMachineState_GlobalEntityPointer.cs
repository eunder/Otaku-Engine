using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEditStateMachineState_GlobalEntityPointer : IItemEditStateMachine 
{
        bool entered = false;
        float timePassed = 0.0f;
        float timeEnd = 4.0f;

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
        return itemEditor.ItemEditStateMachineStateGlobalEntityPointer;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {
            timePassed = 0.0f;
            itemEditor.globalEntityEditor_CANVAS.SetActive(true);
            itemEditor.PopulateGlobalAndLocalEntityList();
     }

        void OnStay(ItemEditStateMachine itemEditor)
        {
               timePassed += Time.deltaTime;

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
                itemEditor.globalEntityEditor_CANVAS.SetActive(false);
        }



}
