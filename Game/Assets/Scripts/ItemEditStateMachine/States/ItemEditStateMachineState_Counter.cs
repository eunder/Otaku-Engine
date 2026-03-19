using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEditStateMachineState_Counter : IItemEditStateMachine 
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
        return itemEditor.ItemEditStateMachineStateCounter;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {
                itemEditor.counterEditor_CANVAS.SetActive(true);

                  itemEditor.counter_defaultCount_InputField.text = itemEditor.currentObjectEditing.GetComponent<Counter>().currentCount_default.ToString();
                  itemEditor.counter_currentCount_InputField.text = itemEditor.currentObjectEditing.GetComponent<Counter>().currentCount.ToString();
     }

        void OnStay(ItemEditStateMachine itemEditor)
        {

                if(itemEditor.currentObjectEditing)
                {
                  itemEditor.currentObjectEditing.GetComponent<Counter>().currentCount_default = int.Parse(itemEditor.counter_defaultCount_InputField.text);
                  itemEditor.currentObjectEditing.GetComponent<Counter>().currentCount = int.Parse(itemEditor.counter_currentCount_InputField.text);
                  
                  itemEditor.currentObjectEditing.GetComponent<WidgetInfo>().info = itemEditor.counter_currentCount_InputField.text;
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
                itemEditor.counterEditor_CANVAS.SetActive(false);
        }



}
