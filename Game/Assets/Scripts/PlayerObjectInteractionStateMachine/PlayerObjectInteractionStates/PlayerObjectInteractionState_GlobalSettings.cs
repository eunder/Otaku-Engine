using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerObjectInteractionState_GlobalSettings : IPlayerObjectInteractionState 
{
        bool entered = false;

    public IPlayerObjectInteractionState DoState(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(entered == false)
        {
            OnEnter(playerObjectInteraction);
            entered = true;
            playerObjectInteraction.pressedToExitGlobalMapSettings = false;
        }


        OnStay(playerObjectInteraction);

        if(playerObjectInteraction.pressedToExitGlobalMapSettings == true)
        {
            OnExit(playerObjectInteraction);
             return playerObjectInteraction.PlayerObjectInteractionStateIdle;

        }

        return playerObjectInteraction.PlayerObjectInteractionStateGlobalSettings;
    }


    
        void OnEnter(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        playerObjectInteraction.toolTipText.text = " | | RIght Mouse: <color=green>Move Camera <color=white>| |";
        Cursor.lockState = CursorLockMode.None;
        playerObjectInteraction.mouseLooker.wheelPickerIsTurnedOn = true;

        GlobalMapSettingsManager.Instance.globalMapSettings_Canvas.gameObject.SetActive(true);

        GlobalMapSettingsManager.Instance.UpdateUIElements();


        //close wheel
        WheelPickerHandler.Instance.CloseWheelPicker();
    }

        void OnStay(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
                    Cursor.lockState = CursorLockMode.None;

          if(Input.GetMouseButton(1))
        {
            playerObjectInteraction.mouseLooker.wheelPickerIsTurnedOn = false;
        }
        else
        {
            playerObjectInteraction.mouseLooker.wheelPickerIsTurnedOn = true;

        }
    

        }

        void OnExit(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
         Cursor.lockState = CursorLockMode.Locked;
        playerObjectInteraction.mouseLooker.wheelPickerIsTurnedOn = false;

        GlobalMapSettingsManager.Instance.globalMapSettings_Canvas.gameObject.SetActive(false);

                entered = false; // reset
        }



}
