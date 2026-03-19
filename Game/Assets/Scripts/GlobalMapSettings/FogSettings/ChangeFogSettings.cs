using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChangeFogSettings : MonoBehaviour
{
    public Image buttonImage;
    public bool colorWheelisOpen = false;
    public UIColorPickerLogic colorpicker;


    public void fogisOnTogglePressed(bool toggleValue)
    {
        GlobalMapSettingsManager.Instance.fog_isOn = toggleValue;
        GlobalMapSettingsManager.Instance.UpdateFogProperties();
    }

    public void fogColorButtonPressed(Image buttImage)
    {
        buttonImage = buttImage;
        OpenColorWheel();
    }

    public void OpenColorWheel()
    {
        colorpicker.InitialColor = GlobalMapSettingsManager.Instance.fog_Color;
        colorpicker.color = GlobalMapSettingsManager.Instance.fog_Color;

        colorpicker.ActivateColorPicker();
        colorWheelisOpen = true;
    }

    public void SetFogStartOnSliderChange(float sliderValue)
    {
        GlobalMapSettingsManager.Instance.fog_Start = sliderValue;
        GlobalMapSettingsManager.Instance.UpdateFogProperties();
    }
    public void SetFogEndOnSliderChange(float sliderValue)
    {
        GlobalMapSettingsManager.Instance.fog_End = sliderValue;
        GlobalMapSettingsManager.Instance.UpdateFogProperties();
    }

    void Update()
    {
        if(Input.GetMouseButtonUp(0)) //bootleg soltion, find better logical way
        {
            buttonImage = null;
        }
        
        if(buttonImage)
        {
              buttonImage.color = colorpicker.color;
              GlobalMapSettingsManager.Instance.fog_Color = colorpicker.color;
              GlobalMapSettingsManager.Instance.UpdateFogProperties();
        }
    }
}
