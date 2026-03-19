using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPickerButtonEvent_PlayerObjectInteractionSetState : MonoBehaviour
{
    PlayerObjectInteractionStateMachine playerObjectInteractionStateMachine;
     public enum playerObjectInteractionStateTypes{scanner,painter,globalsettings, gluegun, wiringgun};
     public playerObjectInteractionStateTypes playerInteractionStateType;

    void Start()
    {
        playerObjectInteractionStateMachine = GameObject.Find("MainPlayerCam").GetComponent<PlayerObjectInteractionStateMachine>();
    }

    public void SetPlayerInteractionState()
    {
        if(playerInteractionStateType == playerObjectInteractionStateTypes.scanner)
        {
        playerObjectInteractionStateMachine.enabled = true;
        playerObjectInteractionStateMachine.currentState = playerObjectInteractionStateMachine.PlayerObjectInteractionStateEquippedScanner;
        }

        if(playerInteractionStateType == playerObjectInteractionStateTypes.painter)
        {
        playerObjectInteractionStateMachine.enabled = true;
        playerObjectInteractionStateMachine.currentState = playerObjectInteractionStateMachine.PlayerObjectInteractionStateEquippedPainter;
        }
        if(playerInteractionStateType == playerObjectInteractionStateTypes.globalsettings)
        {
        playerObjectInteractionStateMachine.enabled = true;
        playerObjectInteractionStateMachine.currentState = playerObjectInteractionStateMachine.PlayerObjectInteractionStateGlobalSettings;
        }
        if(playerInteractionStateType == playerObjectInteractionStateTypes.gluegun)
        {
        playerObjectInteractionStateMachine.enabled = true;
        playerObjectInteractionStateMachine.currentState = playerObjectInteractionStateMachine.PlayerObjectInteractionStateEquippedGlueGun;
        }
        if(playerInteractionStateType == playerObjectInteractionStateTypes.wiringgun)
        {
        playerObjectInteractionStateMachine.enabled = true;
        playerObjectInteractionStateMachine.currentState = playerObjectInteractionStateMachine.PlayerObjectInteractionStateWiringTool;
        }
        //close wheel
        GameObject wheelPicker = GameObject.Find("CANVAS_WHEELPICKER");
        wheelPicker.GetComponent<WheelPickerHandler>().CloseWheelPicker();


    }

}
