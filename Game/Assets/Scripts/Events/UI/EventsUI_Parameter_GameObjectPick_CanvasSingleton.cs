using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class EventsUI_Parameter_GameObjectPick_CanvasSingleton : MonoBehaviour
{
    private static EventsUI_Parameter_GameObjectPick_CanvasSingleton _instance;
    public static EventsUI_Parameter_GameObjectPick_CanvasSingleton Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public GameObject selectGameObjectCanvas;

    public GameObject currentGameObjectAssigned;

    public GameObject currentGameObjecterTicker;
    public TextMeshProUGUI parameterDesription_Text;

    bool loopEnabled = false; //HORRIBLE, FIX LATER THIS IS UNOPTIMZED!!!

    public bool isOnEventParameter = true;

    public void OpenParameter_ObjectPic_Window()
    {
        selectGameObjectCanvas.SetActive(true);
        loopEnabled = true;
    }

    public void CloseParameter_ObjectPic_Window()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;


        selectGameObjectCanvas.SetActive(false);
        loopEnabled = false;
        
        Invoke("CanPickDelay", 0.05f); 
    }

    //have a delay or else when you select the game object... the wheel picker will incorectly be brought up
    public void CanPickDelay()
    {
        PlayerObjectInteractionStateMachine.Instance.canPickNextEventObject = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(loopEnabled)
        {

          if(Input.GetMouseButtonDown(0))
            {

                Ray ray = PlayerObjectInteractionStateMachine.Instance.playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, PlayerObjectInteractionStateMachine.Instance.collidableLayers_layerMask))
                {  
                        currentGameObjectAssigned = hit.transform.gameObject;

                        CloseParameter_ObjectPic_Window();


                        if(isOnEventParameter)
                        {
                        PlayerObjectInteractionStateMachine.Instance.onParameter = currentGameObjectAssigned.name;
                        }
                        else
                        {
                        PlayerObjectInteractionStateMachine.Instance.doParameter = currentGameObjectAssigned.name;
                        EventsUI_DelayCanvasSingleton.Instance.OpenDelayWindow();
                        }             
                                   

                }
            }

            if(Input.GetMouseButtonDown(1))
            {
                        CloseParameter_ObjectPic_Window();
            }

        }
    }
}
