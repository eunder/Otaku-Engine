using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBoxStateMachineState_Idle : IDialogueBoxStateMachine_Interface 
{
    public IDialogueBoxStateMachine_Interface DoState(DialogueBoxStateMachine DialogueBox)
    {
        if(DialogueBox.newStateOnceEvent == false)
        {
            OnEnter(DialogueBox);
            DialogueBox.newStateOnceEvent = true;
        }


        OnStay(DialogueBox);

            if(DialogueBox.dialogueObjectList.Count >= 1)
            {
                OnExit(DialogueBox);
                return DialogueBox.DialogueBoxStateMachineStateDisplayingText;
            }
           
        return DialogueBox.DialogueBoxStateMachineStateIdle;
    }


    
        void OnEnter(DialogueBoxStateMachine DialogueBox)
    {
        DialogueBox.dialogueBox_anim.Play("DialogueBox_Close");
    }

        void OnStay(DialogueBoxStateMachine DialogueBox)
        {

        }

        void OnExit(DialogueBoxStateMachine DialogueBox)
        {
        DialogueBox.dialogueBox_anim.Play("DialogueBox_Open");
        }



}
