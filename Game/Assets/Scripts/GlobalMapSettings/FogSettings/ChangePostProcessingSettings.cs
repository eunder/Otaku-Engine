using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePostProcessingSettings : MonoBehaviour
{

    public Image buttonImage;
    public bool colorWheelisOpen = false;
    public UIColorPickerLogic colorpicker;



    public void Bloom_Toggle(bool value)
    {
        PostProcessingManager.Instance.bloom_isOn = value;
        PostProcessingManager.Instance.UpdatePostProcessingSettings();
    }

    public void Bloom_Intensity(string value)
    {
        PostProcessingManager.Instance.bloom_intensity = float.Parse(value);
        PostProcessingManager.Instance.UpdatePostProcessingSettings();
    }
    public void Bloom_Threshold(string value)
    {
        PostProcessingManager.Instance.bloom_threshold = float.Parse(value);
        PostProcessingManager.Instance.UpdatePostProcessingSettings();
    }


    //COLOR STUFF
    public void BloomColorButtonPressed(Image buttImage)
    {
        buttonImage = buttImage;
        OpenColorWheel();
    }

    public void OpenColorWheel()
    {
        colorpicker.InitialColor = PostProcessingManager.Instance.bloom_color;
        colorpicker.color = PostProcessingManager.Instance.bloom_color;

        colorpicker.ActivateColorPicker();
        colorWheelisOpen = true;
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
              PostProcessingManager.Instance.bloom_color = colorpicker.color;
              PostProcessingManager.Instance.UpdatePostProcessingSettings();
        }
    }

    public void Bloom_SoftKnee(string value)
    {
        PostProcessingManager.Instance.bloom_softKnee = float.Parse(value);
        PostProcessingManager.Instance.UpdatePostProcessingSettings();
    }


    public void Bloom_Diffusion(string value)
    {
        PostProcessingManager.Instance.bloom_diffusion = float.Parse(value);
        PostProcessingManager.Instance.UpdatePostProcessingSettings();
    }



    public void DepthOfField_Toggle(bool value)
    {
        PostProcessingManager.Instance.dof_isOn = value;
        PostProcessingManager.Instance.UpdatePostProcessingSettings();
    }
    public void DepthOfField_MaxFocusDistance(string value)
    {
        PostProcessingManager.Instance.dof_maxFocusDistance = float.Parse(value);
        PostProcessingManager.Instance.UpdatePostProcessingSettings();
    }

    public void AmbientOcclusion_Toggle(bool value)
    {
        PostProcessingManager.Instance.ao_isOn = value;
        PostProcessingManager.Instance.UpdatePostProcessingSettings();
    }

}
