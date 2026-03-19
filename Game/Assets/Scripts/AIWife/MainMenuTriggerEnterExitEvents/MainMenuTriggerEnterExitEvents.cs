using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTriggerEnterExitEvents : MonoBehaviour
{
  public AIWifeStateMachine aiWifeStateMachine;
  public Animator Menu_anim;

  void OnTriggerEnter(Collider other)
    {
        if(other.transform.gameObject.tag == "Player")
        {
            aiWifeStateMachine.currentState = aiWifeStateMachine.AIWifeStateMachineStateMainMenu;
            Menu_anim.Play("AIWifeMenu_Open");
        }
    }

  void OnTriggerExit(Collider other)
    {
        if(other.transform.gameObject.tag == "Player")
        {
            aiWifeStateMachine.currentState = aiWifeStateMachine.AIWifeStateMachineStateIdle;
            Menu_anim.Play("AIWifeMenu_Close");

        }

    }
}
