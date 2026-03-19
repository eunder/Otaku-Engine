using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEditStateMachineState_Date : IItemEditStateMachine 
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
        return itemEditor.ItemEditStateMachineStateDate;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {
                itemEditor.DateEditor_CANVAS.SetActive(true);

                  itemEditor.DateEditor_date_InputField.text = itemEditor.currentObjectEditing.GetComponent<Date>().date.ToString();
     }

        void OnStay(ItemEditStateMachine itemEditor)
        {

            if(itemEditor.currentObjectEditing)
            {
                
            DateTime parsedValue;
            bool successfulParse = DateTime.TryParse(itemEditor.DateEditor_date_InputField.text, out parsedValue);
            
            if(successfulParse)
            {
                itemEditor.currentObjectEditing.GetComponent<Date>().date = parsedValue;
            }

                itemEditor.currentObjectEditing.GetComponent<WidgetInfo>().info = itemEditor.DateEditor_date_InputField.text;
            }
                
            //Notes
            if(itemEditor.currentObjectEditing && itemEditor.currentObjectEditing.GetComponent<Note>())
            {
                itemEditor.currentObjectEditing.GetComponent<Note>().note = itemEditor.noteInputField.text;
            }
        }

        void OnExit(ItemEditStateMachine itemEditor)
        {
                GlobalParameterManager.Instance.SaveGlobalEntities();

                entered = false; // reset
                itemEditor.DateEditor_CANVAS.SetActive(false);
        }



}
