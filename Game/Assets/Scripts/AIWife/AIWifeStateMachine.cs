using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;
using TMPro;
public class AIWifeStateMachine : MonoBehaviour
{
    [SerializeField]
    public string currentStateName;
    public IAIWifeState currentState;
    public AIWifeStateMachineState_Idle AIWifeStateMachineStateIdle = new AIWifeStateMachineState_Idle(); 
    public AIWifeStateMachineState_MainMenu AIWifeStateMachineStateMainMenu = new AIWifeStateMachineState_MainMenu(); 

    public AIWifeStateMachineState_TalkingToPlayer AIWifeStateMachineStateTalkingToPlayer = new AIWifeStateMachineState_TalkingToPlayer(); 

    public AIWifeStateMachineState_Trailer_TalkingBeforePickingMenu AIWifeStateMachineStateTrailerTalkingBeforePickingMenu = new AIWifeStateMachineState_Trailer_TalkingBeforePickingMenu(); 


    public bool newStateOnceEvent = false;
    public int i = 0;

    public Animator anim;
    public Animator Menu_anim;
    public DialogueBoxStateMachine dialogueBoxStateMachine;

    void OnEnable()
    {
        currentState = AIWifeStateMachineStateIdle;
    }
    void Update()
    {


        if(currentStateName != currentState.ToString()) // makes sure the enter event happens by checking if the state name has changed
        {
            newStateOnceEvent = false;
        }

        currentState = currentState.DoState(this); // put this AFTER the if statement or else the enter state machine state enter function wont run on first frame!


        currentStateName = currentState.ToString();

    }

    public void SetToTalk()
    {
        currentState = AIWifeStateMachineStateTalkingToPlayer;
 
    }
}
