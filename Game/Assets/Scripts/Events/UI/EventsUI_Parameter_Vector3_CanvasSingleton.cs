using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventsUI_Parameter_Vector3_CanvasSingleton : MonoBehaviour
{
    private static EventsUI_Parameter_Vector3_CanvasSingleton _instance;
    public static EventsUI_Parameter_Vector3_CanvasSingleton Instance { get { return _instance; } }

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


    public void OpenParameterWindow()
    {
        Cursor.lockState = CursorLockMode.None;
        SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = true;

        inputField.text = 0.ToString();
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
        }
        else
        {
            PlayerObjectInteractionStateMachine.Instance.doParameter = inputField.text;
            EventsUI_DelayCanvasSingleton.Instance.OpenDelayWindow();
        }

    }


}
