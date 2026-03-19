using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DialogueBoxStateMachineState_DisplayingText : IDialogueBoxStateMachine_Interface 
{
        float timeReseter = 0.0f;
        int totalNumberOfVisibleCharacters;
        int counter = 0;
                    
        //shitty duplicate prevention
        string audioVoiceClip_name = "";

    public IDialogueBoxStateMachine_Interface DoState(DialogueBoxStateMachine DialogueBox)
    {
        if(DialogueBox.newStateOnceEvent == false)
        {
            OnEnter(DialogueBox);
            DialogueBox.newStateOnceEvent = true;
        }


        OnStay(DialogueBox);

            if(counter >= totalNumberOfVisibleCharacters)
            {
               OnExit(DialogueBox);
               return DialogueBox.DialogueBoxStateMachineStateDisplayingTextEnd;
            }

            if(DialogueBox.addedImportantDialogueEvent)
            {
                DialogueBox.addedImportantDialogueEvent = false;
                return DialogueBox.DialogueBoxStateMachineStateIdle;
 
            }

           
        return DialogueBox.DialogueBoxStateMachineStateDisplayingText;
    }

        void OnEnter(DialogueBoxStateMachine DialogueBox)
    
    {   
        DialogueBox.endSentenceTicker.SetActive(false);



        //
        DialogueBox.dialogueBoxTextUI.text = GlobalUtilityFunctions.InsertVariableValuesInText(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex].dialogue);

        //Debug.Log("Sentence to display: " + DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex].dialogue );
        counter = 0;

        DialogueBox.dialogueBoxTextUI.ForceMeshUpdate(); // without this, the bottom line wont work because the text will need to wait to be rendered...
        totalNumberOfVisibleCharacters = DialogueBox.dialogueBoxTextUI.textInfo.characterCount;


        //dialogue start event (make sure its not a "previewdialogue" object)
        if(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex])
        if(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex].gameObject.name != "PreviewDialogue")
        EventActionManager.Instance.TryPlayEvent(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex].gameObject, "OnDialogueStart");


        DialogueBox.dialogueBox_anim.Play("DialogueBox_Open");

    }

        void OnStay(DialogueBoxStateMachine DialogueBox)
        {
            //Debug.Log("TotanuMberofBIs : " + totalNumberOfVisibleCharacters );

            if(Input.GetMouseButton(1)) //text space skipper
            {
                counter = totalNumberOfVisibleCharacters-1;
            }

            int visibleCount = counter % (totalNumberOfVisibleCharacters + 1);
            DialogueBox.dialogueBoxTextUI.maxVisibleCharacters = visibleCount + 1;

            if(Input.GetMouseButton(0)) //text space skipper
            {
            timeReseter += Time.deltaTime * 5.0f;
            }
            else
            {
            timeReseter += Time.deltaTime * 2.0f;
            }


            if(DialogueBox.dialogueBoxTextUI.textInfo.characterInfo[visibleCount].character == '.' || DialogueBox.dialogueBoxTextUI.textInfo.characterInfo[visibleCount].character == ',')  //if punctuation is found, check for period speed instead of textSpeed
            {
                if(timeReseter >= DialogueBox.textSpeed_period)
            {
                timeReseter = 0.0f;
                counter += 1;
                //Debug.Log("Counter : " + counter );

            }
            }
            else if(char.IsLetter(DialogueBox.dialogueBoxTextUI.textInfo.characterInfo[visibleCount].character))
            {
             if(timeReseter >= DialogueBox.textSpeed)
            {
                timeReseter = 0.0f;
                counter += 1;
                //Debug.Log("Counter : " + counter );

                if(!DialogueBox.audioSource_Voice.isPlaying)
                {
                    DialogueBox.audioSource_Voice.pitch = Random.Range(DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex].pitch - 0.2f, DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex].pitch);
                   
                    DialogueBox.audioSource_Voice.clip = DialogueBox.dialogueObjectList[DialogueBox.currentSentenceIndex].voice_AudioClip;

                    DialogueBox.audioSource_Voice.Play();

                    if(DialogueBox.anim)
                    DialogueBox.anim.Play("talk", 2, 0f);

                }

            }
            }
            else
            {
            if(timeReseter >= DialogueBox.textSpeed)
            {
                timeReseter = 0.0f;
                counter += 1;
                //Debug.Log("Counter : " + counter );
            } 
            }

          
        }


        void OnExit(DialogueBoxStateMachine DialogueBox)
        {

        }



}
