using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementTypeKeySwitcher : MonoBehaviour
{
    
    private static PlayerMovementTypeKeySwitcher _instance;
    public static PlayerMovementTypeKeySwitcher Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }




    public AudioSource noclipSwitch_AudioSource;
    public AudioClip noclipSwitchOn_AudioClip;
    public AudioClip noclipSwitchOff_AudioClip;

    public bool isInFlyMode = false;
    public GameObject noClipIcon_Canvas;
    void Start()
    {
        noClipIcon_Canvas = GameObject.Find("CANVAS_NOCLIPMODEICON");

    }


    // Update is called once per frame
    void Update()
    {
        try
        {
        if(Input.GetKeyDown("v") && !Input.GetKey(KeyCode.LeftControl) && EditModeStaticParameter.isInEditMode == true
        && ItemEditStateMachine.Instance.currentState == ItemEditStateMachine.Instance.ItemEditStateMachineStateIdle
        && SaveAndLoadLevel.Instance.isLevelLoaded == true
        && ZipFileHandler.Instance.password_Window.activeSelf == false
        && InputEventManager_String.Instance.InputEventManagerWindow.activeSelf == false
        && InputEventManager_Counter.Instance.InputEventManagerWindow.activeSelf == false
        && GlobalUtilityFunctions.CheckIfPlayerIsEditingInputField() == false)
        {
 //           if(!GlobalUtilityFunctions.IsCurrentMapForeign())
  //          {
                
                if(isInFlyMode == false)
                {
                 EnableFlyMode();
                }
                else
                {
                  EnablePlayerController();
                }
  //          }
        }
        }
        catch (Exception e)
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>" + e.Message);
        }
    }


    //mostly used for item edit functions...
    public void EnableMovementBasedOnCurrentMovement()
    {
                if(isInFlyMode)
                {
                    EnableFlyMode_Raw();
                }
                else
                {
                    EnablePlayerController_Raw();
                }

    }



    //RAW = NO SFX (used for when play is done editing and the game has to enable the movement again)


    //NORMAL = for when the player press V to switch

    public void EnablePlayerController_Raw()
    {
        isInFlyMode = false;

                    //to make sure the restart level event happens when player leaves fly mode(if it hasnt happened yet)
                    if(ResetLevelParametersManager.Instance.levelRestartEventPopped == false)
                    {
                        ResetLevelParametersManager.Instance.PlayLevelStartEvents();
                    }

                    GetComponent<PlayerMovementBasic>().crushDetectionCapsule.gameObject.SetActive(true);
                    GetComponent<PlayerMovementBasic>().enabled = true;
                    GetComponent<PlayerMovementBasic>().RemovePlayerFromPlatform();
                    
                    GetComponent<Rigidbody>().isKinematic = true; //so that on player enter events happen while in normal walk mode (for debbuging)
                    GetComponent<Rigidbody>().isKinematic = false;
                    GetComponent<Collider>().enabled = true;

                    GetComponent<PlayerMovementNoClip>().enabled = false;

                    noClipIcon_Canvas.GetComponent<Canvas>().enabled = false;

    }




    public void EnablePlayerController()
    {
        isInFlyMode = false;

                    //to make sure the restart level event happens when player leaves fly mode(if it hasnt happened yet)
                    if(ResetLevelParametersManager.Instance.levelRestartEventPopped == false)
                    {
                        ResetLevelParametersManager.Instance.PlayLevelStartEvents();
                    }

                    GetComponent<PlayerMovementBasic>().crushDetectionCapsule.gameObject.SetActive(true);
                    GetComponent<PlayerMovementBasic>().enabled = true;
                    GetComponent<PlayerMovementBasic>().RemovePlayerFromPlatform();
                    GetComponent<Rigidbody>().isKinematic = false;
                    GetComponent<Collider>().enabled = true;

                    GetComponent<PlayerMovementNoClip>().enabled = false;

                    noClipIcon_Canvas.GetComponent<Canvas>().enabled = false;
                    GetComponent<PlayerMovementNoClip>().OnDisable_SFX();

                    noclipSwitch_AudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                    noclipSwitch_AudioSource.PlayOneShot(noclipSwitchOff_AudioClip, 1.0f);
    }



    //no effects
    public void EnableFlyMode_Raw()
    {
                GetComponent<PlayerMovementBasic>().crushDetectionCapsule.gameObject.SetActive(false);
                GetComponent<PlayerMovementBasic>().enabled = false;
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<Collider>().enabled = false; //disable collider so that player events dont happen while flying


                isInFlyMode = true;
                GetComponent<PlayerMovementNoClip>().enabled = true;

    }

    public void EnableFlyMode()
    {

        isInFlyMode = true;

                    GetComponent<PlayerMovementBasic>().crushDetectionCapsule.gameObject.SetActive(false);
                    GetComponent<PlayerMovementBasic>().enabled = false;
                    GetComponent<Rigidbody>().isKinematic = true;
                    GetComponent<Collider>().enabled = false; //disable collider so that player events dont happen while flying


                    GetComponent<PlayerMovementNoClip>().enabled = true;
                    GetComponent<PlayerMovementNoClip>().OnEnable_SFX();

                    noClipIcon_Canvas.GetComponent<Canvas>().enabled = true;
                    noclipSwitch_AudioSource.PlayOneShot(noclipSwitchOn_AudioClip, 1.0f);

    }



}
