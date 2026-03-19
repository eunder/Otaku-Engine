using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using SFB;
using DG.Tweening;
using TMPro;

public class PlayerObjectInteractionState_Equipped_GlueGun : IPlayerObjectInteractionState 
{
        bool entered = false;
        bool clickedToExit = false;
        string filePathToSaveImages;

        float scannedAmount = 0.0f;
        GameObject PosterCurrentlyScanning;

        float distanceBetweenPlayerAndPoster = 0.0f;

    public IPlayerObjectInteractionState DoState(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(entered == false)
        {
            OnEnter(playerObjectInteraction);
            entered = true;
        }

        if(clickedToExit == true)
        {
            OnExit(playerObjectInteraction);
            return playerObjectInteraction.PlayerObjectInteractionStateIdle;
        }


        OnStay(playerObjectInteraction);

        return playerObjectInteraction.PlayerObjectInteractionStateEquippedGlueGun;
    }


    
        void OnEnter(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        //playerObjectInteraction.glueGun_GameObject.SetActive(true);

        playerObjectInteraction.toolTipText.text = "Left Mouse: <color=green>Glue <color=white>| RIght Mouse: <color=red>Cancel <color=white>";
    }

        void OnStay(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
 
             if(Input.GetMouseButtonDown(0) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
           {
                playerObjectInteraction.glueGun_ParticleSystem.Play();
                playerObjectInteraction.glueGun_ParticleSystem_Collision.Play();
                playerObjectInteraction.glueGun_AudioSource.Play();
           }
             if(Input.GetMouseButtonUp(0))
           {
                playerObjectInteraction.glueGun_ParticleSystem.Stop();
                playerObjectInteraction.glueGun_ParticleSystem_Collision.Stop();
                playerObjectInteraction.glueGun_AudioSource.Stop();
           }

            if(Input.GetMouseButtonDown(1) && SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == false)
           {
               clickedToExit = true;  
           }
        
        }

        void OnExit(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
                entered = false; // reset
                playerObjectInteraction.pickedUpObject = false;
                clickedToExit = false;
                //playerObjectInteraction.glueGun_GameObject.SetActive(false);
                playerObjectInteraction.glueGun_ParticleSystem.Stop();
                playerObjectInteraction.glueGun_ParticleSystem_Collision.Stop();
                playerObjectInteraction.glueGun_AudioSource.Stop();

        }



}
