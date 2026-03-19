using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWifeStateMachineState_TalkingToPlayer : IAIWifeState 
{
    
    public IAIWifeState DoState(AIWifeStateMachine AIWife)
    {
        if(AIWife.newStateOnceEvent == false)
        {
            OnEnter(AIWife);
            AIWife.newStateOnceEvent = true;
        }


        OnStay(AIWife);

            if(AIWife.dialogueBoxStateMachine.sentencesToDisplay.Count <= 0)
            {
                OnExit(AIWife);
                return AIWife.AIWifeStateMachineStateIdle;
            }
           
        return AIWife.AIWifeStateMachineStateTalkingToPlayer;
    }


    
        void OnEnter(AIWifeStateMachine AIWife)
    {
        //fetch the dialogue needed
        AIWife.dialogueBoxStateMachine.sentencesToDisplay.Add("wadasdad");
                AIWife.dialogueBoxStateMachine.sentencesToDisplay.Add("<#FFFFFF>Need some <#3DFF00>help <#FFFFFF>with that?");

        //play animation or whatever
        AIWife.anim.CrossFade("New Animation2", 0.3f);

    }

        void OnStay(AIWifeStateMachine AIWife)
        {

        }

        void OnExit(AIWifeStateMachine AIWife)
        {

        }



}
