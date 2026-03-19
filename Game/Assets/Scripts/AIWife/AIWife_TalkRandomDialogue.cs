using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWife_TalkRandomDialogue : MonoBehaviour
{
    public DialogueContentObject[] dialogueContentObjList;
    int randomIndex;
    int lastIndex;
    public DialogueBoxStateMachine dialogueStateMachine;

    void Start()
    {
        dialogueContentObjList = GetComponentsInChildren<DialogueContentObject>();
        Random.seed = (int)System.DateTime.Now.Ticks; 
    }

    public void FetchRandomTalkDialogue()
    {
        
         randomIndex = Random.Range (0, dialogueContentObjList.Length);

         if(randomIndex == lastIndex)
         {
         randomIndex = Random.Range (0, dialogueContentObjList.Length);
         }
         lastIndex = randomIndex;
         dialogueStateMachine.AddDialogue(dialogueContentObjList[randomIndex]);
    }
}
