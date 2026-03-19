using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputEventManager_String : MonoBehaviour
{
    private static InputEventManager_String _instance;
    public static InputEventManager_String Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public GameObject fromObj; //the pointer


    public GameObject InputEventManagerWindow;

    public TMP_InputField input_InputField;


    public void OpenInputWindow(GameObject obj)
    {
        InputEventManagerWindow.SetActive(true);
        ItemEditStateMachine.Instance.SetCurrentItemTypeToEmpty();

        //disable player
        Cursor.lockState = CursorLockMode.None;
        SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = true;
        PlayerMovementBasic.Instance.enabled = false;
        PlayerMovementNoClip.Instance.enabled = false;

        Cursor.visible = true;

 

        //select input field
        input_InputField.Select();
        input_InputField.ActivateInputField();



        fromObj = obj;

       //fill-in input field with current value
        input_InputField.text = fromObj.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<StringEntity>().currentString;



    }


    public void ButtonClickedToSubmit()
    {
        //enable player
        if(PlayerObjectInteractionStateMachine.Instance.currentState != PlayerObjectInteractionStateMachine.Instance.PlayerObjectInteractionStateViewingFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;
            PlayerMovementBasic.Instance.enabled = true;
            PlayerMovementTypeKeySwitcher.Instance.EnableMovementBasedOnCurrentMovement();
            Cursor.visible = false;
        }

        fromObj.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<StringEntity>().currentString = input_InputField.text;
        fromObj.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<WidgetInfo>().info = input_InputField.text;
        GlobalParameterManager.Instance.SaveGlobalEntities();

        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnStringInputWindow_Submit")
            {
                    EventActionManager.Instance.TryPlayEvent_Single(ev);
            }
        }

        InputEventManagerWindow.SetActive(false);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(InputEventManagerWindow.activeSelf)
            ButtonClickedToSubmit();
        }
    }

}
