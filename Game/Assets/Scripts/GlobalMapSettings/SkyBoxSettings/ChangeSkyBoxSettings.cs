using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChangeSkyBoxSettings : MonoBehaviour
{
    public Image buttonImage;
        public Color color;
        public Color InitialColor;

    public bool colorWheelisOpen = false;
    public string shaderColorProperty = "";
    public UIColorPickerLogic colorpicker;

    public LevelGlobalMediaManager globalSkyboxMedia;


    // Start is called before the first frame update
        public void SetExpOnSkyBoxShader(float sliderValue)
    {
            GlobalMapSettingsManager.Instance.linearColorskybox_Mat.SetFloat("_Exp", sliderValue);
            GlobalMapSettingsManager.Instance.skybox_Color_Exp_original = sliderValue;
    }

    public void topColorButtonPressed(Image buttImage)
    {
        buttonImage = buttImage;
        shaderColorProperty = "_TopColor";
        OpenColorWheel();
    }
    public void midColorButtonPressed(Image buttImage)
    {
        buttonImage = buttImage;
        shaderColorProperty = "_MiddleColor";
        OpenColorWheel();
    }
    public void botColorButtonPressed(Image buttImage)
    {
        buttonImage = buttImage;
        shaderColorProperty = "_BottomColor";
        OpenColorWheel();
    }

    public void OpenColorWheel()
    {
        colorpicker.InitialColor = GlobalMapSettingsManager.Instance.linearColorskybox_Mat.GetColor(shaderColorProperty);
        colorpicker.color = GlobalMapSettingsManager.Instance.linearColorskybox_Mat.GetColor(shaderColorProperty);

        colorpicker.ActivateColorPicker();
        colorWheelisOpen = true;
    }

    public void SetSkyBoxMediaUVScaleXOnSliderChange(float sliderValue)
    {
        globalSkyboxMedia.skyboxTextureScaleX = sliderValue;
    }
    public void SetSkyBoxMediaUVScaleYOnSliderChange(float sliderValue)
    {
        globalSkyboxMedia.skyboxTextureScaleY = sliderValue;
    }
    public void SetSkyBoxMediaUVOffsetXOnSliderChange(float sliderValue)
    {
        globalSkyboxMedia.skyboxTextureOffsetX = sliderValue;
    }

    public void SetSkyBoxMediaUVOffsetYOnSliderChange(float sliderValue)
    {
        globalSkyboxMedia.skyboxTextureOffsetY = sliderValue;
    }


    public void SetSkyBoxMediaUVOffsetScrollXOnInputFieldChange(string value)
    {
        if(string.IsNullOrEmpty(value))
        {
            value = "0";
        }
        globalSkyboxMedia.scrollSpeedX = float.Parse(value);
    }
    public void SetSkyBoxMediaUVOffsetScrollYOnInputFieldChange(string value)
    {
        if(string.IsNullOrEmpty(value))
        {
            value = "0";
        }
        globalSkyboxMedia.scrollSpeedY = float.Parse(value);
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
              GlobalMapSettingsManager.Instance.linearColorskybox_Mat.SetColor(shaderColorProperty, colorpicker.color);

                //update "original" values based on ui fields. (this is how the global "original" paramters are set... not very clean)
                GlobalMapSettingsManager.Instance.skybox_Color_Top_original = GlobalMapSettingsManager.Instance.skybox_TopColor_Image.color;
                GlobalMapSettingsManager.Instance.skybox_Color_Middle_original = GlobalMapSettingsManager.Instance.skybox_MiddleColor_Image.color;
                GlobalMapSettingsManager.Instance.skybox_Color_Bottom_original = GlobalMapSettingsManager.Instance.skybox_BottomColor_Image.color;
                GlobalMapSettingsManager.Instance.skybox_Color_Exp_original = GlobalMapSettingsManager.Instance.skybox_Exp_Slider.value;

        }
    }
}
