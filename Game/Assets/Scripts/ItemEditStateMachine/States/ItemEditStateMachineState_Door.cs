using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEditStateMachineState_Door : IItemEditStateMachine 
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

           if(itemEditor.currentItemType == CustomeItemType.TypeOfItem.empty)
            {
               OnExit(itemEditor);
               return itemEditor.ItemEditStateMachineStateIdle;
            }
        return itemEditor.ItemEditStateMachineStateDoor;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {
                itemEditor.doorEditor_CANVAS.SetActive(true);

            timePassed = 0.0f;

            if( itemEditor.currentObjectEditing.GetComponent<DoorInfo>().pathFileToLoad != null)
              {            
                  itemEditor.door_Path.text = itemEditor.currentObjectEditing.GetComponent<DoorInfo>().pathFileToLoad;
              }

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

            if(itemEditor.currentObjectEditing)
            {
                itemEditor.door_Path.text = itemEditor.currentObjectEditing.GetComponent<DoorInfo>().pathFileToLoad;
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
                itemEditor.doorEditor_CANVAS.SetActive(false);
        }



}
