using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWifeStateMachineState_Trailer_TalkingBeforePickingMenu : IAIWifeState 
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
                AIWife.newStateOnceEvent = false; //newStateOnceEvent Dosnt always work for some reason, so i have to add this in here!!!
                OnExit(AIWife);
                return AIWife.AIWifeStateMachineStateMainMenu;
            }
           
        return AIWife.AIWifeStateMachineStateTrailerTalkingBeforePickingMenu;
    }


    
        void OnEnter(AIWifeStateMachine AIWife)
    { 
        //fetch the dialogue needed
        //AIWife.dialogueBoxStateMachine.sentencesToDisplay.Add("<#FF1313>Bad<#FFFFFF> at mapping?");
        AIWife.dialogueBoxStateMachine.sentencesToDisplay.Add("<#FFFFFF>Not the <#FF0000>C<#FFA000>r<#F6FF00>e<#68FF00>a<#00FF7C>t<#00FFFF>i<#0046FF>v<#F400FF>e  <#FFFFFF>type?");
        AIWife.dialogueBoxStateMachine.sentencesToDisplay.Add("Thats ok...");
        AIWife.dialogueBoxStateMachine.sentencesToDisplay.Add("<#FFFFFF>I can <#3DFF00>help<#FFFFFF> you with that.");

        //play animation or whatever
        AIWife.anim.CrossFade("standpose1", 0.3f);

        if(AIWife.Menu_anim)
        {
        AIWife.Menu_anim.Play("AIWifeMenu_Close");
        }
    }

        void OnStay(AIWifeStateMachine AIWife)
        {

        }

        void OnExit(AIWifeStateMachine AIWife)
        {

        }



}
