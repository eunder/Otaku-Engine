using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalMapSettingsManager : MonoBehaviour
{


    private static GlobalMapSettingsManager _instance;
    public static GlobalMapSettingsManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public Material linearColorskybox_Mat;

    public float lightingIntensity_light;
    public float lightingIntensity_shadow;

    public Color lightingColor_light;
    public Color lightingColor_shadow;

    public Color skybox_Color_Top;
    public Color skybox_Color_Top_original;
    public Color skybox_Color_Middle;
    public Color skybox_Color_Middle_original;
    public Color skybox_Color_Bottom;
    public Color skybox_Color_Bottom_original;
    public float skybox_Color_Exp;
    public float skybox_Color_Exp_original;

    //UI ELEMENTS
    public GameObject globalMapSettings_Canvas;
    public Slider shadowIntensity_Slider;
    public Image shadowColor_Image;
    public Image lightColor_Image;


    public Image skybox_TopColor_Image;
    public Image skybox_MiddleColor_Image;
    public Image skybox_BottomColor_Image;
    public Slider skybox_Exp_Slider;



    public TMP_InputField skyboxMedia_Path_InputFIeld;
    public Slider skyboxMedia_scaleX_Slider;
    public Slider skyboxMedia_scaleY_Slider;
    public Slider skyboxMedia_offsetX_Slider;
    public Slider skyboxMedia_offsetY_Slider;
    public TMP_InputField skyboxMedia_scrollX_InputField;
    public TMP_InputField skyboxMedia_scrollY_InputField;

    public TMP_InputField aiMonster_Path_InputField;




    public Toggle bloom_isOn_Toggle;
    public TMP_InputField bloom_intensity_InputField;
    public TMP_InputField bloom_threshold_InputField;
    public Image bloom_color_Image;
    public TMP_InputField bloom_softKnee_InputField;
    public TMP_InputField bloom_diffusion_InputField;

    public Toggle depthOfField_isOn_Toggle;
    public TMP_InputField depthOfField_maxFocusRange_InputField;

    public Toggle ambientOcclusion_isOn_Toggle;



    public Toggle fog_isOn_Toggle;
    public Image fog_color_Image;
    public Slider fog_Start_Slider;
    public Slider fog_End_Slider;


    public bool fog_isOn;
    public Color fog_Color;
    public float fog_Start;
    public float fog_End;




    public Toggle vertexSnapping_isOn_Toggle;
    public bool vertexSnapping = false;


    public void UpdateFogProperties()
    {
        RenderSettings.fog = fog_isOn;
        RenderSettings.fogColor = fog_Color;
        RenderSettings.fogStartDistance = fog_Start;
        RenderSettings.fogEndDistance = fog_End;

        Shader.SetGlobalFloat("_EnableFog", fog_isOn ? 2f:0f); //have to do this because of shadergraph not having an option to turn off fog...
    }

    public void RandomizeSkyboxColor()
    {
        skybox_Color_Top = Color.black;
        skybox_Color_Top_original = Color.black;

        Color randColor = Random.ColorHSV(0, 1, 0.8f, 1, 1,1, 1,1);

        skybox_Color_Middle = randColor;
        skybox_Color_Middle_original = randColor;


        skybox_Color_Bottom = Color.white;
        skybox_Color_Bottom_original = Color.white;

        UpdateSkyBoxColors();
        AutoLightingCorrect(skybox_Color_Middle);
    }

    public void UpdateSkyBoxColors()
    {
        linearColorskybox_Mat.SetColor("_TopColor", skybox_Color_Top);
        linearColorskybox_Mat.SetColor("_MiddleColor", skybox_Color_Middle);
        linearColorskybox_Mat.SetColor("_BottomColor", skybox_Color_Bottom);
    }


    //changes the level lighting colors and bases it all around the passed color
    public void AutoLightingCorrect(Color colorToBaseOff)
    {
            Shader.SetGlobalFloat("_LowIntensity", 1.5f);
            Shader.SetGlobalColor("_LowColor", colorToBaseOff * 0.7f);
   
            lightingColor_shadow = Shader.GetGlobalColor("_LowColor");
    }


    public void UpdateUIElements()
    {
              // the rest...
        shadowIntensity_Slider.value = lightingIntensity_shadow;
 
        skybox_TopColor_Image.color = skybox_Color_Top_original;
        skybox_MiddleColor_Image.color = skybox_Color_Middle_original;
        skybox_BottomColor_Image.color = skybox_Color_Bottom_original;
        skybox_Exp_Slider.value = skybox_Color_Exp_original;
       
        skyboxMedia_Path_InputFIeld.text = LevelGlobalMediaManager.Instance.urlFilePath;
        skyboxMedia_scaleX_Slider.value = LevelGlobalMediaManager.Instance.skyboxTextureScaleX;
        skyboxMedia_scaleY_Slider.value = LevelGlobalMediaManager.Instance.skyboxTextureScaleY;
        skyboxMedia_offsetX_Slider.value = LevelGlobalMediaManager.Instance.skyboxTextureOffsetX;
        skyboxMedia_offsetY_Slider.value = LevelGlobalMediaManager.Instance.skyboxTextureOffsetY;
        skyboxMedia_scrollX_InputField.text = LevelGlobalMediaManager.Instance.scrollSpeedX.ToString();
        skyboxMedia_scrollY_InputField.text = LevelGlobalMediaManager.Instance.scrollSpeedY.ToString();

        fog_isOn_Toggle.isOn = fog_isOn;
        fog_color_Image.color = fog_Color;
        fog_Start_Slider.value = fog_Start;
        fog_End_Slider.value = fog_End;


        bloom_isOn_Toggle.isOn = PostProcessingManager.Instance.bloom_isOn;
        bloom_intensity_InputField.text = PostProcessingManager.Instance.bloom_intensity.ToString();
        bloom_threshold_InputField.text = PostProcessingManager.Instance.bloom_threshold.ToString();
        bloom_color_Image.color = PostProcessingManager.Instance.bloom_color;
        bloom_softKnee_InputField.text = PostProcessingManager.Instance.bloom_softKnee.ToString();
        bloom_diffusion_InputField.text = PostProcessingManager.Instance.bloom_diffusion.ToString();
        depthOfField_isOn_Toggle.isOn = PostProcessingManager.Instance.dof_isOn;
        depthOfField_maxFocusRange_InputField.text = PostProcessingManager.Instance.dof_maxFocusDistance.ToString();
        ambientOcclusion_isOn_Toggle.isOn = PostProcessingManager.Instance.ao_isOn;






        vertexSnapping_isOn_Toggle.isOn = vertexSnapping;
    }

    public void UpdateUIAutomaticColorElements()
    {
              // the rest...

        skybox_TopColor_Image.color = linearColorskybox_Mat.GetColor("_TopColor");
        skybox_MiddleColor_Image.color = linearColorskybox_Mat.GetColor("_MiddleColor");
        skybox_BottomColor_Image.color = linearColorskybox_Mat.GetColor("_BottomColor");
        skybox_Exp_Slider.value = linearColorskybox_Mat.GetFloat("_Exp");

    }



    public void UpdateVertexSnappingOnToggle(bool value)
    {
        vertexSnapping = value;
        Shader.SetGlobalFloat("_EnableVertexSnapping", value ? 2f:0f);
    }



}
