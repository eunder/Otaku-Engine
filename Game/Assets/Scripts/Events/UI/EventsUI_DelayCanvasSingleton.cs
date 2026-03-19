using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventsUI_DelayCanvasSingleton : MonoBehaviour
{
    private static EventsUI_DelayCanvasSingleton _instance;
    public static EventsUI_DelayCanvasSingleton Instance { get { return _instance; } }

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
    public TMP_InputField inputField;

    public void OpenDelayWindow()
    {
        Cursor.lockState = CursorLockMode.None;
        SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = true;

        inputField.text = 0.ToString();
        windowAnimator.OpenWindow();
    }


    public void ConfirmDelayButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;

        PlayerObjectInteractionStateMachine.Instance.wiringToolState = 3;
        PlayerObjectInteractionStateMachine.Instance.delay = float.Parse(inputField.text);
        PlayerObjectInteractionStateMachine.Instance.AddEventToCurrentWiringObject();
        PlayerObjectInteractionStateMachine.Instance.canPickNextEventObject = true;

        windowAnimator.CloseWindow();
    }

}
