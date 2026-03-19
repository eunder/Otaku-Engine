using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VLC_MediaPlayer_ControlsGUI_ButtonToggleCanvas : MonoBehaviour
{
    private static VLC_MediaPlayer_ControlsGUI_ButtonToggleCanvas _instance;
    public static VLC_MediaPlayer_ControlsGUI_ButtonToggleCanvas Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public GameObject posterPlayerCanvasPrefab;
    public GameObject currentSpawnedCanvas;


    public GameObject canvas;
    public SimpleSmoothMouseLook mouseLooker;
    public PlayerObjectInteractionStateMachine playerInteractionStateMachine;
    public GridBuilderStateMachine gridBuilderStateMachine;
    public ItemEditStateMachine itemEditStateMachine;


    public bool isActive = false;

    RaycastHit hit;
    public LayerMask collidableLayers_layerMask;




    // Update is called once per frame

    //IMPORTANT NOTE: the GUI gets instantiated because of the way the template for video playing was made... (it subscribes to events) (If you delete the poster and try to play something else... it wont work)


    void Update()
    {
        if(playerInteractionStateMachine.currentState == playerInteractionStateMachine.PlayerObjectInteractionStateIdle && itemEditStateMachine.currentState == itemEditStateMachine.ItemEditStateMachineStateIdle && EscapeToggleToolBar.toolBarisOpened == false && gridBuilderStateMachine.currentState == gridBuilderStateMachine.GridBuilderStateIdle)
        {


        if(Input.GetKeyDown("c"))
        {
            if (Physics.Raycast(SimpleSmoothMouseLook.Instance.transform.position, SimpleSmoothMouseLook.Instance.transform.forward, out hit, Mathf.Infinity, collidableLayers_layerMask))
            {
                if(EditModeStaticParameter.isInEditMode)
                {
                        currentSpawnedCanvas = Instantiate(posterPlayerCanvasPrefab);
                        Cursor.lockState = CursorLockMode.None;
                        mouseLooker.wheelPickerIsTurnedOn = true;

                        currentSpawnedCanvas.GetComponent<VLC_MediaPlayer_ControlsGUI>().vlcPlayer = hit.transform.GetComponent<VLC_MediaPlayer>();
                        currentSpawnedCanvas.GetComponent<VLC_MediaPlayer_ControlsGUI>().InitializeControlls();

                        isActive = true;
                }
                else
                {
                if(hit.transform.GetComponent<PosterMeshCreator>().canVideoSeek)
                {
                    if(hit.transform.GetComponent<PosterMeshCreator>())
                    {
                        if(hit.transform.GetComponent<VLC_MediaPlayer>().IsPlaying)
                        {
                        Cursor.lockState = CursorLockMode.None;
                        mouseLooker.wheelPickerIsTurnedOn = true;

                        currentSpawnedCanvas.GetComponent<VLC_MediaPlayer_ControlsGUI>().vlcPlayer = hit.transform.GetComponent<VLC_MediaPlayer>();
                        currentSpawnedCanvas.GetComponent<VLC_MediaPlayer_ControlsGUI>().InitializeControlls();

                        isActive = true;
                        }
                    }
                }
                }
            }
        }


        
        if(Input.GetKeyUp("c"))
        {
            if(currentSpawnedCanvas)
            {
                Destroy(currentSpawnedCanvas);
            }

            Cursor.lockState = CursorLockMode.Locked;
            mouseLooker.wheelPickerIsTurnedOn = false;
            playerInteractionStateMachine.enabled = true;

            isActive = false;
        }

        }
    }
}
