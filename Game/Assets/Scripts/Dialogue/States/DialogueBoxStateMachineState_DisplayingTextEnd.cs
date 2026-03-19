using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class DialogueBoxStateMachineState_DisplayingTextEnd : IDialogueBoxStateMachine_Interface 
{
    public IDialogueBoxStateMachine_Interface DoState(DialogueBoxStateMachine DialogueBox)
    {
        if(DialogueBox.newStateOnceEvent == false)
        {
            OnEnter(DialogueBox);
            DialogueBox.newStateOnceEvent = true;
        }


        OnStay(DialogueBox);

            if(Input.GetMouseButtonDown(0)) //exit when all text has been displayed an player presses space  2 OR proceed to "endText" state
            {

                //dialogue end event
                if(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex])
                if(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex].gameObject.name != "PreviewDialogue")
                EventActionManager.Instance.TryPlayEvent(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex].gameObject, "OnDialogueExit");


                if(DialogueBox.currentSentenceIndex >= DialogueBox.dialogueObjectList.Count - 1)
                {
                    DialogueBox.dialogueEnd_audioSource_Last.Play();

                    OnExit(DialogueBox);
                    DialogueBox.currentSentenceIndex = 0;

                    DialogueBox.dialogueObjectList.Clear();
                    return DialogueBox.DialogueBoxStateMachineStateIdle;
                }
                else
                {
                    DialogueBox.dialogueEnd_audioSource.Play();
                DialogueBox.currentSentenceIndex += 1;
                    OnExit(DialogueBox);
                    return DialogueBox.DialogueBoxStateMachineStateDisplayingText;
                }

            }
           


           
            if(DialogueBox.addedImportantDialogueEvent)
            {
                DialogueBox.addedImportantDialogueEvent = false;
                return DialogueBox.DialogueBoxStateMachineStateIdle;
 
            }


        return DialogueBox.DialogueBoxStateMachineStateDisplayingTextEnd;
    }

        void OnEnter(DialogueBoxStateMachine DialogueBox)
    {
        DialogueBox.endSentenceTicker.SetActive(true);

        DialogueBox.endSentenceTicker.transform.localScale = new Vector3(0f,0f,0f);
        DialogueBox.endSentenceTicker.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f).OnComplete(() => DialogueBox.endSentenceTicker.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f));

        if(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex])
        if(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex].gameObject.name != "PreviewDialogue")
        EventActionManager.Instance.TryPlayEvent(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex].gameObject, "OnDialogueEnd");

        if(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex])
        if(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex].gameObject.name == "PreviewDialogue")
        GameObject.Destroy(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex].gameObject);
    }

        void OnStay(DialogueBoxStateMachine DialogueBox)
        {
          
        }

        void OnExit(DialogueBoxStateMachine DialogueBox)
        {
        DOTween.Kill(DialogueBox.endSentenceTicker.transform);
        DialogueBox.endSentenceTicker.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.1f).OnComplete(() => DialogueBox.endSentenceTicker.transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.1f));
        DialogueBox.dialogueBoxTextUI.text = ""; // prevents previous text from showing up for a moment
        }

}
