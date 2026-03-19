using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class EscapeToggleToolBar : MonoBehaviour
{
    private static EscapeToggleToolBar _instance;
    public static EscapeToggleToolBar Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }



    public GameObject toolbar;
    ItemEditStateMachine itemEditStateMachine;
    WheelPickerHandler wheelPickerHandler;
    public static bool toolBarisOpened = false;

    public AudioMixer mainAudioMixer; //for dampening the media audio

    // Start is called before the first frame update
    void Start()
    {
                mainAudioMixer.SetFloat("LowPassCutOff", 22000f); //for making sure its not damped on scene load


                if(VLC_GlobalMediaPlayer.Instance != null)
                {
              //  if(VLC_MediaPlayer.Instance.currentPosterPlaying == null)
              //  VLC_GlobalMediaPlayer.Instance.SetVolume((int) (PlayerPrefs.GetFloat("mediaVolume", 0.90f) * 100));
                }


                if(GameObject.Find("ItemEditStateMachine"))
                {
                itemEditStateMachine = GameObject.Find("ItemEditStateMachine").GetComponent<ItemEditStateMachine>();
                }

                if(GameObject.Find("CANVAS_WHEELPICKER"))
                {
                wheelPickerHandler = GameObject.Find("CANVAS_WHEELPICKER").GetComponent<WheelPickerHandler>();
                }
    }


    
    //makes sure toolBarisOpened is set to false on scene load
    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        toolBarisOpened = false;
    }




    // Update is called once per frame
    void Update()
    {
                        if(Input.GetKeyDown(KeyCode.Escape))
                        {

                            if(SimpleSmoothMouseLook.Instance)
                            {
                                if(SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn == true)
                                {
                                    wheelPickerHandler.CloseWheelPicker();
                                }
                            }

                            if(toolbar.activeSelf)
                            {
                                //free the mouse if the player is viewing a poster or doing other stuff where the mouse should be free
                                if(PlayerObjectInteractionStateMachine.Instance && InputEventManager_String.Instance && InputEventManager_Counter.Instance

                                    &&
                                    (
                                        PlayerObjectInteractionStateMachine.Instance.currentState == PlayerObjectInteractionStateMachine.Instance.PlayerObjectInteractionStateViewingFrame
                                    || InputEventManager_String.Instance.InputEventManagerWindow.activeSelf == true
                                    || InputEventManager_Counter.Instance.InputEventManagerWindow.activeSelf == true)
                                    )
                                
                                {
                                Cursor.lockState = CursorLockMode.None;
                                Cursor.visible = true;
                                if(SimpleSmoothMouseLook.Instance)
                                {
                                    SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = true;
                                }

                                }
                                else
                                {
                                Cursor.lockState = CursorLockMode.Locked;
                                Cursor.visible = false;

                                //enable player interaction state machine 
                                if(SimpleSmoothMouseLook.Instance)
                                { 
                                    SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;
                                }
                               
                                }

                                toolbar.SetActive(false);
                                EscapeToggleToolBar.toolBarisOpened = false;


                                mainAudioMixer.SetFloat("LowPassCutOff", 22000f);

                                //set back volume of videos on tool bar closed

                                if(SaveAndLoadLevel.Instance)
                                {
                                    foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
                                    {
                                        if(poster != null)
                                        {
                                            poster.GetComponent<VLC_MediaPlayer>().ResetVolumeOriginal();
                                        } 
                                    }
                                }
                                if(VLC_GlobalMediaPlayer.Instance != null)
                                {
                                    VLC_GlobalMediaPlayer.Instance.ResetVolumeOriginal();
                                }

                            }
                            else
                            {
                                
                                Cursor.lockState = CursorLockMode.None;
                                Cursor.visible = true;
                                if(SimpleSmoothMouseLook.Instance)
                                {
                                    SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = true;
                                }
                                toolbar.SetActive(true);
                                EscapeToggleToolBar.toolBarisOpened = true;
                                mainAudioMixer.SetFloat("LowPassCutOff", 180f);

                                //disable player interaction

                                //lower volume of videos on tool bar opened
                                if(SaveAndLoadLevel.Instance)
                                {
                                    foreach(GameObject poster in SaveAndLoadLevel.Instance.allLoadedPosters)
                                    {
                                        if(poster)
                                        {
                                            poster.GetComponent<VLC_MediaPlayer>().mediaPlayer.SetVolume(poster.GetComponent<VLC_MediaPlayer>().volume_original / 2);
                                        } 
                                    }
                                }
                                if(VLC_GlobalMediaPlayer.Instance != null)
                                {
                                    VLC_GlobalMediaPlayer.Instance.mediaPlayer.SetVolume(VLC_GlobalMediaPlayer.Instance.volume_original /2);
                                }

                                if(GlobalParameterManager.Instance)
                                {
                                    GlobalParameterManager.Instance.LoadGlobalEntities();
                                    GlobalParameterManager.Instance.PopulateGlobalAndLocalEntityList();
                                }
                            }

                        }
        }


/*
        public void ToggleToolBarMenu()
        {
                ConfigFileHandler.Instance.SaveConfigFile();

                            if(toolbar.activeSelf)
                            {
                                Cursor.lockState = CursorLockMode.Locked;
                                SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;
                                toolbar.SetActive(false);
                                PlayerObjectInteractionStateMachine.Instance.enabled = true;
                                mainAudioMixer.SetFloat("LowPassCutOff", 22000f);


                                if(VLC_GlobalMediaPlayer.Instance != null)
                                {
                            //        if(VLC_MediaPlayer.Instance.currentPosterPlaying == null)
                            //    VLC_GlobalMediaPlayer.Instance.SetVolume((int) (PlayerPrefs.GetFloat("mediaVolume", 0.90f) * 100));
                                }
                            }
                            else
                            {
                                Cursor.lockState = CursorLockMode.None;
                                SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = true;
                                toolbar.SetActive(true);
                                PlayerObjectInteractionStateMachine.Instance.enabled = false;
                                mainAudioMixer.SetFloat("LowPassCutOff", 180f);


                                if(VLC_GlobalMediaPlayer.Instance != null)
                                {
        	                   	VLC_GlobalMediaPlayer.Instance.SetVolume((int) (PlayerPrefs.GetFloat("mediaVolume", 0.90f) * 40));
                                }
          
                                }

        }
*/

        public void OpenToolBarIfClosed()
        {
                            if(!toolbar.activeSelf)
                            {
                                Cursor.lockState = CursorLockMode.None;

                                if(SimpleSmoothMouseLook.Instance)
                                {
                                    SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = true;
                                } 
                                toolbar.SetActive(true);

                                if(PlayerObjectInteractionStateMachine.Instance)
                                {
                                    PlayerObjectInteractionStateMachine.Instance.enabled = false;
                                }
                                
                                mainAudioMixer.SetFloat("LowPassCutOff", 180f);


                                if(VLC_GlobalMediaPlayer.Instance != null)
                                {
        	                   	VLC_GlobalMediaPlayer.Instance.SetVolume((int) (PlayerPrefs.GetFloat("mediaVolume", 1f) * 40));
                                }
                            }
        }

        public void CloseToolBarIfOpen()
        {
                            if(toolbar.activeSelf)
                            {
                                Cursor.lockState = CursorLockMode.Locked;

                                if(SimpleSmoothMouseLook.Instance)
                                {
                                    SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;
                                }

                                toolbar.SetActive(false);

                                if(PlayerObjectInteractionStateMachine.Instance)
                                {
                                    PlayerObjectInteractionStateMachine.Instance.enabled = true;
                                }

                                mainAudioMixer.SetFloat("LowPassCutOff", 22000f);


                                if(VLC_GlobalMediaPlayer.Instance != null)
                                {
                               // if(VLC_MediaPlayer.Instance.currentPosterPlaying == null)
                               // VLC_GlobalMediaPlayer.Instance.SetVolume((int) (PlayerPrefs.GetFloat("mediaVolume", 0.90f) * 100));
                                }
                            }
        }
    }
