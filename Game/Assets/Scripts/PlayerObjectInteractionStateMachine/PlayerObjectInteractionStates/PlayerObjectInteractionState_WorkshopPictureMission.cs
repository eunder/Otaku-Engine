using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PlayerObjectInteractionState_WorkshopPictureMission : IPlayerObjectInteractionState 
{
        bool entered = false;
        bool clickedToExit = false;
        bool playedAIWifeCheeseAnimation = false;

    public IPlayerObjectInteractionState DoState(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
        if(entered == false)
        {
            OnEnter(playerObjectInteraction);
            entered = true;
        }


        OnStay(playerObjectInteraction);


            if(clickedToExit == true)
            {
                OnExit(playerObjectInteraction);
                return playerObjectInteraction.PlayerObjectInteractionStateIdle;
            }

        return playerObjectInteraction.PlayerObjectInteractionStateWorkshopPictureMission;
    }


        GameObject toolbar; //to hide
        Texture2D destinationTexture;
    
        void OnEnter(PlayerObjectInteractionStateMachine playerObjectInteraction)
    {
            if(playerObjectInteraction.toolTipText != null)
            {
            playerObjectInteraction.toolTipText.text = "Left Mouse:Take Picture | RIght Mouse: Cancel";
            }
            
            playerObjectInteraction.playerCamera.targetTexture = playerObjectInteraction.screenShot_RenderTexture;
            playerObjectInteraction.canvasRenderTexture_GameObject.SetActive(true);
            toolbar = GameObject.Find("CANVAS_TOOLBAR");
            toolbar.SetActive(false);
    }

        void OnStay(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {

          //AI wife detection
            RaycastHit objectHitInfo;

                   if (Physics.Raycast(playerObjectInteraction.gameObject.transform.position, playerObjectInteraction.gameObject.transform.forward, out objectHitInfo, Mathf.Infinity, playerObjectInteraction.collidableLayers_layerMask))
        {  
                if(objectHitInfo.transform.tag == "Wife" && playedAIWifeCheeseAnimation == false)
                {
                    playedAIWifeCheeseAnimation = true;
                  //  objectHitInfo.transform.parent.GetComponent<Animator>().CrossFade("cheesu_transition", 0.0f, 0);
                    playerObjectInteraction.aiWife_anim.CrossFade("emotion_camerashy1", 0.0f, 1);
                }

                            if(Input.GetMouseButtonDown(0) && objectHitInfo.transform.tag == "Wife")
                            {
                                    playerObjectInteraction.achievementManager.UnlockAchivement("ACH_CHEESE");
                            }
        }

            //the rest...
            if(Input.GetMouseButtonDown(0))
           {
                //take, process, and assign picture
                //save picture onto computer
                //assign to workshop canvas

                destinationTexture = new Texture2D(512, 512, TextureFormat.RGB24, false);
                RenderTexture.active = playerObjectInteraction.screenShot_RenderTexture;
                destinationTexture.ReadPixels(new Rect(0, 0, playerObjectInteraction.screenShot_RenderTexture.width, playerObjectInteraction.screenShot_RenderTexture.height), 0, 0);
                destinationTexture.Apply();

                byte[] bytes = destinationTexture.EncodeToPNG();
                System.IO.File.WriteAllBytes(Application.dataPath + "/StreamingAssets/" + "workshop_picture.png", bytes);

                playerObjectInteraction.screenShot_AudioSource.Play();

                playedAIWifeCheeseAnimation = false;
                playerObjectInteraction.aiWife_anim.CrossFade("emotion_theankzone", 0.0f, 1);

                clickedToExit = true;
           }


           if(Input.GetMouseButtonDown(1))
           {
                clickedToExit = true;
           }
           




        }

        void OnExit(PlayerObjectInteractionStateMachine playerObjectInteraction)
        {
            if(playerObjectInteraction.toolTipText != null)
            {
                playerObjectInteraction.toolTipText.text = "";
            }
                playerObjectInteraction.playerCamera.targetTexture = null;
                playerObjectInteraction.canvasRenderTexture_GameObject.SetActive(false);
                toolbar.SetActive(true);
                clickedToExit = false;
                playerObjectInteraction.enabled = false;
                playerObjectInteraction.mouseLooker.wheelPickerIsTurnedOn = true;

                GameObject.FindObjectOfType<AIWife_MissionSubmitUploadManager>().imageThunbmailPath = Application.dataPath + "/StreamingAssets/" + "workshop_picture.png";
                GameObject.FindObjectOfType<AIWife_MissionSubmitUploadManager>().LoadImage();

                entered = false; // reset
        }



}
