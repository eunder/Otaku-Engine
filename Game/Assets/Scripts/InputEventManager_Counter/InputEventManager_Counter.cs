using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputEventManager_Counter : MonoBehaviour
{
    private static InputEventManager_Counter _instance;
    public static InputEventManager_Counter Instance { get { return _instance; } }

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
        ItemEditStateMachine.Instance.SetCurrentItemTypeToEmpty();
        InputEventManagerWindow.SetActive(true);


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
        input_InputField.text = fromObj.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<Counter>().currentCount.ToString();


    }


    public void ButtonClickedToSubmit()
    {
        //enable palyer
        Cursor.lockState = CursorLockMode.Locked;
        SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;
        PlayerMovementBasic.Instance.enabled = true;
        PlayerMovementTypeKeySwitcher.Instance.EnableMovementBasedOnCurrentMovement();

        Cursor.visible = false;

        int parsedValue;
        bool successfulParse = int.TryParse(input_InputField.text, out parsedValue);

        if(successfulParse)
        {
        fromObj.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<Counter>().currentCount = parsedValue;
        }
        else
        {
        fromObj.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<Counter>().currentCount = 0;
        }


        fromObj.GetComponent<GlobalParameterPointerEntity>().returnGameObjectEntity().GetComponent<WidgetInfo>().info = input_InputField.text;
        GlobalParameterManager.Instance.SaveGlobalEntities();

        foreach(SaveAndLoadLevel.Event ev in fromObj.GetComponentInChildren<EventHolderList>().events)
        {
            if(ev.onAction == "OnCounterInputWindow_Submit")
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
