using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEditStateMachineState_String : IItemEditStateMachine 
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
        return itemEditor.ItemEditStateMachineStateString;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {
                itemEditor.stringEditor_CANVAS.SetActive(true);

                  itemEditor.string_defaultString_InputField.text = itemEditor.currentObjectEditing.GetComponent<StringEntity>().currentString_default;
                  itemEditor.string_currentString_InputField.text = itemEditor.currentObjectEditing.GetComponent<StringEntity>().currentString;
     }

        void OnStay(ItemEditStateMachine itemEditor)
        {

                if(itemEditor.currentObjectEditing)
                {
                  itemEditor.currentObjectEditing.GetComponent<StringEntity>().currentString_default = itemEditor.string_defaultString_InputField.text;
                  itemEditor.currentObjectEditing.GetComponent<StringEntity>().currentString = itemEditor.string_currentString_InputField.text;
                  
                  itemEditor.currentObjectEditing.GetComponent<WidgetInfo>().info = itemEditor.string_currentString_InputField.text;
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
                itemEditor.stringEditor_CANVAS.SetActive(false);
        }



}
