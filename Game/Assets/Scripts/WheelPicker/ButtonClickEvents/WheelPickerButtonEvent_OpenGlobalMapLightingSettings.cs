using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelPickerButtonEvent_OpenGlobalMapLightingSettings : MonoBehaviour
{

    GameObject globalMapSettingsCanvas;
    void Start()
    {
        globalMapSettingsCanvas = GameObject.Find("CANVAS_GLOBALMAPSETTINGS");
    }

    public void OpenGlobalMapLightingSettings()
    {
        globalMapSettingsCanvas.transform.GetChild(0).gameObject.SetActive(true);

        Debug.Log(globalMapSettingsCanvas.transform.GetChild(0).GetChild(0).name);
        //apply global map settings to UI elements on open
        
        GlobalMapSettingsManager.Instance.shadowIntensity_Slider.value = GlobalMapSettingsManager.Instance.lightingIntensity_shadow;
        GlobalMapSettingsManager.Instance.lightingColor_shadow = RenderSettings.ambientLight;
        GlobalMapSettingsManager.Instance.shadowColor_Image.color = new Color(GlobalMapSettingsManager.Instance.lightingColor_shadow.r, GlobalMapSettingsManager.Instance.lightingColor_shadow.g, GlobalMapSettingsManager.Instance.lightingColor_shadow.b, 1.0f);


        globalMapSettingsCanvas.transform.GetChild(0).GetChild(0).Find("BUTTON_SKYBOX_TOPCOLOR").GetComponent<Image>().color = RenderSettings.skybox.GetColor("_TopColor");
        globalMapSettingsCanvas.transform.GetChild(0).GetChild(0).Find("BUTTON_SKYBOX_MIDCOLOR").GetComponent<Image>().color = RenderSettings.skybox.GetColor("_MiddleColor");
        globalMapSettingsCanvas.transform.GetChild(0).GetChild(0).Find("BUTTON_SKYBOX_BOTCOLOR").GetComponent<Image>().color = RenderSettings.skybox.GetColor("_BottomColor");
        globalMapSettingsCanvas.transform.GetChild(0).GetChild(0).Find("SLIDER_SKYBOXEXPO").GetComponent<Slider>().value = RenderSettings.skybox.GetFloat("_Exp");

        //close wheel
        GameObject wheelPicker = GameObject.Find("CANVAS_WHEELPICKER");
        wheelPicker.GetComponent<WheelPickerHandler>().CloseWheelPicker();
    }
}
