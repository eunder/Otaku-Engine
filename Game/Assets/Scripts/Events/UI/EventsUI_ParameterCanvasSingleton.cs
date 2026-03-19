using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventsUI_ParameterCanvasSingleton : MonoBehaviour
{
    private static EventsUI_ParameterCanvasSingleton _instance;
    public static EventsUI_ParameterCanvasSingleton Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public ToolBarAnimation_WindowOpenAndClose windowAnimator;
    public TextMeshProUGUI parameterDesription_Text;
    public TMP_InputField inputField;


    public bool isOnEventParameter = true;

    WheelPickerReturnEventNameOnClick.TypeOfParameter parameterType;
    public void OpenParameterWindow(WheelPickerReturnEventNameOnClick.TypeOfParameter parameterType, string defaultValue = "0")
    {
        Cursor.lockState = CursorLockMode.None;
        SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = true;


        //change input field properties based on parameterType
        if(parameterType == WheelPickerReturnEventNameOnClick.TypeOfParameter._int)
        {
           // inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            inputField.GetComponent<MouseWheelIncrementElementField>().incrementAmount = 1f;
        }
        else if(parameterType == WheelPickerReturnEventNameOnClick.TypeOfParameter._float)
        {
           // inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
            inputField.GetComponent<MouseWheelIncrementElementField>().incrementAmount = 0.1f;
        }
        else
        {
           // inputField.contentType = TMP_InputField.ContentType.Standard;
            inputField.GetComponent<MouseWheelIncrementElementField>().incrementAmount = 0.1f;
        }



        inputField.text = defaultValue;
        windowAnimator.OpenWindow();
    }

    public void ConfirmParameterSet()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;


        windowAnimator.CloseWindow();

        //play sound
        PlayerObjectInteractionStateMachine.Instance.wiringToolSFX_soundType();

        //if this is for an on event parameter assign it to on, else, assign it to the do paramater
        //proceed to picking the delay if it is a do event
        if(isOnEventParameter)
        {
            PlayerObjectInteractionStateMachine.Instance.onParameter = inputField.text;

            //if the parameter type is none and we are on the ON event... then let the player proceed interacting with the next thing in the wiring state
            PlayerObjectInteractionStateMachine.Instance.canPickNextEventObject = true;

        }
        else
        {
            PlayerObjectInteractionStateMachine.Instance.doParameter = inputField.text;
            EventsUI_DelayCanvasSingleton.Instance.OpenDelayWindow();
        }

    }


}
