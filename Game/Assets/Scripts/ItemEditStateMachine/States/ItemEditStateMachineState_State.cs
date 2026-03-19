using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEditStateMachineState_State : IItemEditStateMachine 
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
        return itemEditor.ItemEditStateMachineStateState;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {
                itemEditor.stateEditor_CANVAS.SetActive(true);

                itemEditor.PopulateStateList();

                itemEditor.stateEditor_timeUntilChoiceBoxCloses_InputField.text = itemEditor.currentObjectEditing.GetComponent<State>().timeUntilChoiceBoxCloses.ToString();
     }

        void OnStay(ItemEditStateMachine itemEditor)
        {
            if(itemEditor.currentObjectEditing)
            {          
                float parsedValue;
                bool successfulParse = float.TryParse(itemEditor.stateEditor_timeUntilChoiceBoxCloses_InputField.text, out parsedValue);
                
                if(successfulParse)
                {
                    itemEditor.currentObjectEditing.GetComponent<State>().timeUntilChoiceBoxCloses = parsedValue;
                }
                else
                {
                    itemEditor.currentObjectEditing.GetComponent<State>().timeUntilChoiceBoxCloses = 0;
                }
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
                itemEditor.stateEditor_CANVAS.SetActive(false);
        }

        





}
