using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerObjectInteractionState_ClickedOnDoor : IPlayerObjectInteractionState 
{
        bool entered = false;
    public IPlayerObjectInteractionState DoState(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(entered == false)
        {
            OnEnter(playerObjectInteraction);
            entered = true;
        }


        OnStay(playerObjectInteraction);

        return playerObjectInteraction.PlayerObjectInteractionStateClickedOnDoor;
    }


    
        void OnEnter(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.StartsWith("SA:"))
        {
            GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = Application.dataPath + "/StreamingAssets/" + GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp.Substring(3);
        }

        playerObjectInteraction.exitRoomEvent.exitRoomEvent();
    }

        void OnStay(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {


        }

        void OnExit(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
                entered = false; // reset
        }



}
