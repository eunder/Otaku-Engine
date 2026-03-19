using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWifeStateMachineState_Idle : IAIWifeState 
{
    public IAIWifeState DoState(AIWifeStateMachine AIWife)
    {
        if(AIWife.newStateOnceEvent == false)
        {
            OnEnter(AIWife);
            AIWife.newStateOnceEvent = true;
        }


        OnStay(AIWife);

            if(AIWife.i == 1)
            {
                OnExit(AIWife);
                return AIWife.AIWifeStateMachineStateTalkingToPlayer;
            }
           
        return AIWife.AIWifeStateMachineStateIdle;
    }


    
        void OnEnter(AIWifeStateMachine AIWife)
    {
        AIWife.anim.CrossFade("New Animation", 0.3f);
    }

        void OnStay(AIWifeStateMachine AIWife)
        {

        }

        void OnExit(AIWifeStateMachine AIWife)
        {

        }



}
