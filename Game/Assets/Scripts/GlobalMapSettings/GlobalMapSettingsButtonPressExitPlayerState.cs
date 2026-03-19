using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMapSettingsButtonPressExitPlayerState : MonoBehaviour
{
    public PlayerObjectInteractionStateMachine playerObjectInteractionStateMachine;


    public void ExitGlobalSettingsState()
    {
        playerObjectInteractionStateMachine.pressedToExitGlobalMapSettings = true;
    }

}
