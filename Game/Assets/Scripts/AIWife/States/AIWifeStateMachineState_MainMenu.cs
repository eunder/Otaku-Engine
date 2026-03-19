using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWifeStateMachineState_MainMenu : IAIWifeState 
{
    public IAIWifeState DoState(AIWifeStateMachine AIWife)
    {
        if(AIWife.newStateOnceEvent == false)
        {
            OnEnter(AIWife);
            AIWife.newStateOnceEvent = true;
        }


        OnStay(AIWife);
           
        return AIWife.AIWifeStateMachineStateMainMenu;
    }


    
        void OnEnter(AIWifeStateMachine AIWife)
    {
        AIWife.anim.CrossFade("menuReveal", 0.2f);
        AIWife.Menu_anim.Play("AIWifeMenu_Open");
        AIWife.dialogueBoxStateMachine.sentencesToDisplay.Add("Pick.");

    }

        void OnStay(AIWifeStateMachine AIWife)
        {

        }

        void OnExit(AIWifeStateMachine AIWife)
        {
        }



}
