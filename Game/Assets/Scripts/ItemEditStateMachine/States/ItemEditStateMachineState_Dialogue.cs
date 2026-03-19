using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemEditStateMachineState_Dialogue : IItemEditStateMachine 
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
        return itemEditor.ItemEditStateMachineStateDialogue;
    }


    
        void OnEnter(ItemEditStateMachine itemEditor)
    {
                itemEditor.dialogueEditor_CANVAS.SetActive(true);

         
                  itemEditor.dialogue_dialogue_InputField.text = itemEditor.currentObjectEditing.GetComponent<DialogueContentObject>().dialogue;
                  itemEditor.dialogue_voicePath_InputField.text = itemEditor.currentObjectEditing.GetComponent<DialogueContentObject>().voicePath;
                  itemEditor.dialogue_pitch_InputField.text = itemEditor.currentObjectEditing.GetComponent<DialogueContentObject>().pitch.ToString();
                  itemEditor.dialogue_clearPreviousDialogue_Toggle.isOn = itemEditor.currentObjectEditing.GetComponent<DialogueContentObject>().clearPreviousDialogue;
     }

        void OnStay(ItemEditStateMachine itemEditor)
        {

                if(itemEditor.currentObjectEditing)
                {
                  itemEditor.currentObjectEditing.GetComponent<DialogueContentObject>().dialogue = itemEditor.dialogue_dialogue_InputField.text;
                  itemEditor.currentObjectEditing.GetComponent<DialogueContentObject>().voicePath = itemEditor.dialogue_voicePath_InputField.text;
                  itemEditor.currentObjectEditing.GetComponent<DialogueContentObject>().pitch = float.Parse(itemEditor.dialogue_pitch_InputField.text);
                  itemEditor.currentObjectEditing.GetComponent<DialogueContentObject>().clearPreviousDialogue = itemEditor.dialogue_clearPreviousDialogue_Toggle.isOn;
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
                itemEditor.dialogueEditor_CANVAS.SetActive(false);
        }



}
