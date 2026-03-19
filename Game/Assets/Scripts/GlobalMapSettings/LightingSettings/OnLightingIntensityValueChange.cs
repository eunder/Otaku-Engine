using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnLightingIntensityValueChange : MonoBehaviour
{
    public Color color;
    public Color InitialColor;
    public Image buttonImage;
    public UIColorPickerLogic colorpicker;


    public void SetAmbienceColoringOnIntensitySliderChange()
    {
                GlobalMapSettingsManager.Instance.lightingIntensity_shadow = GlobalMapSettingsManager.Instance.shadowIntensity_Slider.value;
                float factor = Mathf.Pow(2,GlobalMapSettingsManager.Instance.lightingIntensity_shadow);
                RenderSettings.ambientLight = GlobalMapSettingsManager.Instance.lightingColor_shadow * factor;
 
                 Shader.SetGlobalColor("_AmbientLightColor", RenderSettings.ambientLight);
                 Shader.SetGlobalFloat("_AmbientLightIntensity", GlobalMapSettingsManager.Instance.lightingIntensity_shadow);
    }

    public void shadColorButtonPressed(Image buttImage)
    {
        buttonImage = buttImage;
        OpenColorWheel();
    }
    public void OpenColorWheel()
    {
        colorpicker.InitialColor = GlobalMapSettingsManager.Instance.lightingColor_shadow; 
        colorpicker.color = GlobalMapSettingsManager.Instance.lightingColor_shadow;

        colorpicker.ActivateColorPicker();
    }

    void Update()
    {
 
        if(Input.GetMouseButtonUp(0)) //bootleg soltion, find better logical way
        {
            buttonImage = null;
        }

        if(buttonImage)
        {
            //this if else is simply used to make sure when opening the color wheel, the HDR colors dont get multiplied again...
            if(colorpicker.color == InitialColor)
            {
                            buttonImage.color = new Color(colorpicker.color.r, colorpicker.color.g, colorpicker.color.b, 1.0f);
                            RenderSettings.ambientLight = colorpicker.color;
         
                            Shader.SetGlobalColor("_AmbientLightColor", RenderSettings.ambientLight);
            }
            else
            {
                            buttonImage.color = new Color(colorpicker.color.r, colorpicker.color.g, colorpicker.color.b, 1.0f);
                            GlobalMapSettingsManager.Instance.lightingColor_shadow = colorpicker.color;
                            float factor = Mathf.Pow(2,GlobalMapSettingsManager.Instance.lightingIntensity_shadow);
                            RenderSettings.ambientLight = colorpicker.color * factor;
            
                            Shader.SetGlobalColor("_AmbientLightColor", RenderSettings.ambientLight);
            }

         }
             else
            {
                            GlobalMapSettingsManager.Instance.shadowColor_Image.color = new Color(GlobalMapSettingsManager.Instance.lightingColor_shadow.r, GlobalMapSettingsManager.Instance.lightingColor_shadow.g, GlobalMapSettingsManager.Instance.lightingColor_shadow.b, 1.0f);
            }
    }
}
