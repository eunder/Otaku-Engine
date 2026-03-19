using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Audio;

public class ConfigMenuUIEvents : MonoBehaviour
{
    private static ConfigMenuUIEvents _instance;
    public static ConfigMenuUIEvents Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    //global settings
    public static float playerPositioningOffset = 0.8f; //used to correctly position the player. theres really no good way to set this besides setting it manually...






    Resolution[] resolutions;
    public TMP_Dropdown resolutionDropDown;
    public Toggle isFullScreen_Toggle;
    public Toggle setPostProcessingBloom_Toggle;
    public Toggle setPostProcessingChromaticAberration_Toggle;
    public Toggle setPostProcessingAmbientOcclusion_Toggle;
    public Toggle setHeadBob_Toggle;
    public Toggle invertYAxis_Toggle;
    public Slider FoV_Slider;
    public Toggle setVSync_Toggle;
    public Toggle setShadows;

    public Slider mouseSensitivity_Slider; 
    public Slider mouseSmoothing_Slider;

    public Toggle setHeadTilt_Toggle;



    public Toggle setToolTips;
    GameObject toolTipGameObject;
    public Slider mediaVolume_Slider; //finish add movie player audiosource
    public Slider gameVolume_Slider; //finish

    public Toggle setStreamerMode;

    PostProcessVolume volume;
    AmbientOcclusion ambientOcclusion;
    Bloom bloom;
    ChromaticAberration chromaticAberration;

    //AUDIO
    public AudioMixer mainAudioMixer;


    Camera playerCamera;
    Camera playerPixelCamera;

    PlayerMovementBasic playerMovement;
    SimpleSmoothMouseLook mouseLooker;



    //network and media
    public bool allowURLmedia = false;
    public Toggle allowURLmediaToggle;
    public TMP_InputField autoBackUpInterval_InputField;



    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropDown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz"; 
            options.Add(option);

            if(resolutions[i].width == Screen.width &&
               resolutions[i].height  == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropDown.AddOptions(options);

        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();



        volume = GameObject.Find("MainPlayerCam").GetComponent<PostProcessVolume>();
        playerCamera = GameObject.Find("MainPlayerCam").GetComponent<Camera>();

        if(playerCamera.transform.Find("PixelCamera"))
        {
        playerPixelCamera = playerCamera.transform.GetChild(0).GetComponent<Camera>();
        }

        playerMovement = GameObject.Find("player(custom character controller)").GetComponent<PlayerMovementBasic>();
        mouseLooker = GameObject.Find("MainPlayerCam").GetComponent<SimpleSmoothMouseLook>();

        toolTipGameObject = GameObject.Find("CANVAS_TOOLTIPS");

        volume.profile.TryGetSettings(out bloom);
        volume.profile.TryGetSettings(out chromaticAberration);
        volume.profile.TryGetSettings(out ambientOcclusion);



        //apply config settings on scene start
        Screen.fullScreen = PlayerPrefs.GetInt("isFullScreen", 1) == 1 ? true : false;
        chromaticAberration.enabled.value = PlayerPrefs.GetInt("PostProccessingChromaticAbberation", 1) == 1 ? true : false;
        PlayerMovementBasic.Instance.headBob_enabled = PlayerPrefs.GetInt("headBobEnabled", 1) == 1 ? true : false;
        SimpleSmoothMouseLook.Instance.invertYAxis = PlayerPrefs.GetInt("invertYAxis", 0) == 1 ? true : false;
        playerCamera.fieldOfView = PlayerPrefs.GetFloat("fov", 60);
        mouseLooker.sensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 3.9f);
        mouseLooker.smoothing = PlayerPrefs.GetFloat("mouseSmoothing", 0.5f);
        if(PlayerPrefs.GetInt("camTilit", 0) == 1 ? true : false)
        {
        playerMovement.camRotZ_MaxTilt = 4;
        playerMovement.camRotX_MaxTilt = 1.5f;
        }
        else
        {
        playerMovement.camRotZ_MaxTilt = 0;
        playerMovement.camRotX_MaxTilt = 0;
        }
    

        QualitySettings.vSyncCount = PlayerPrefs.GetInt("vsync", 1);
        QualitySettings.shadows = PlayerPrefs.GetInt("shadows", 1) == 1 ? ShadowQuality.All : ShadowQuality.Disable;

        if(toolTipGameObject)
        {
        toolTipGameObject.SetActive(PlayerPrefs.GetInt("tooltipsenabled", 1) == 1 ? true : false);
        }

                if(playerCamera.transform.Find("PixelCamera"))
        {
        playerPixelCamera.fieldOfView = PlayerPrefs.GetFloat("fov", 60);
        playerPixelCamera.transform.gameObject.SetActive(PlayerPrefs.GetInt("streamermodeenabled", 0) == 1 ? true : false);
        }

        mainAudioMixer.SetFloat("Game",  Mathf.Log10(PlayerPrefs.GetFloat("gameVolume", 0.90f)) * 20);
        mainAudioMixer.SetFloat("Media",  Mathf.Log10(PlayerPrefs.GetFloat("mediaVolume", 1f)) * 20);
        mapVolumeModifier = PlayerPrefs.GetFloat("mediaVolume", 1f);

        //apply config settings to UI buttons
        resolutionDropDown.value = PlayerPrefs.GetInt("ResolutionIndex");
        isFullScreen_Toggle.isOn = PlayerPrefs.GetInt("isFullScreen", 1) == 1 ? true : false;
        setPostProcessingBloom_Toggle.isOn = PlayerPrefs.GetInt("PostProccessingBloom", 1) == 1 ? true : false;
        setPostProcessingChromaticAberration_Toggle.isOn = PlayerPrefs.GetInt("PostProccessingChromaticAbberation", 1) == 1 ? true : false;
        setPostProcessingAmbientOcclusion_Toggle.isOn = PlayerPrefs.GetInt("AmbientOcclusion", 1) == 1 ? true : false;
        setHeadBob_Toggle.isOn = PlayerPrefs.GetInt("headBobEnabled", 1) == 1 ? true : false;
        invertYAxis_Toggle.isOn = PlayerPrefs.GetInt("invertYAxis", 0) == 1 ? true : false;

        FoV_Slider.value = PlayerPrefs.GetFloat("fov", 60);
        setHeadTilt_Toggle.isOn = PlayerPrefs.GetInt("camTilit", 0) == 1 ? true : false;
        setVSync_Toggle.isOn = PlayerPrefs.GetInt("vsync", 1) == 1 ? true : false;
        setShadows.isOn = PlayerPrefs.GetInt("shadows", 1) == 1 ? true : false;
        gameVolume_Slider.value = PlayerPrefs.GetFloat("gameVolume", 0.90f);
        mediaVolume_Slider.value = PlayerPrefs.GetFloat("mediaVolume", 1f);
        setToolTips.isOn = PlayerPrefs.GetInt("tooltipsenabled", 1) == 1 ? true : false;
        setStreamerMode.isOn = PlayerPrefs.GetInt("streamermodeenabled", 0) == 1 ? true : false;
        mouseSensitivity_Slider.value = PlayerPrefs.GetFloat("mouseSensitivity", 3.9f);
        mouseSmoothing_Slider.value = PlayerPrefs.GetFloat("mouseSmoothing", 0.5f);


        allowURLmedia = PlayerPrefs.GetInt("allowURLmedia", 0) == 1 ? true : false;
        allowURLmediaToggle.isOn = PlayerPrefs.GetInt("allowURLmedia", 0) == 1 ? true : false;
        autoBackUpInterval_InputField.text = PlayerPrefs.GetInt("AutoBackupInterval", 2).ToString();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;

        PlayerPrefs.SetInt("isFullScreen", (isFullScreen ? 1 : 0));
    }

    public void SetPostProcessing_Bloom(bool isEnabled)
    {
        PlayerPrefs.SetInt("PostProccessingBloom", (isEnabled ? 1 : 0));
        PostProcessingManager.Instance.UpdatePostProcessingSettings();
    }

    public void SetPostProcessing_ChromaticAberration(bool isEnabled)
    {
        chromaticAberration.enabled.value = isEnabled;
        PlayerPrefs.SetInt("PostProccessingChromaticAbberation", (isEnabled ? 1 : 0));
    }

    public void SetPostProcessing_AmbientOcclusion(bool isEnabled)
    {
        PlayerPrefs.SetInt("AmbientOcclusion", (isEnabled ? 1 : 0));
        PostProcessingManager.Instance.UpdatePostProcessingSettings();
    }

    public void SetFoV(float sliderValue)
    {
        playerCamera.fieldOfView = sliderValue;

        if(playerPixelCamera)
        {
        playerPixelCamera.fieldOfView = sliderValue;
        }

        PlayerPrefs.SetFloat("fov", sliderValue);
    }

    public void SetHeadBob(bool isEnabled)
    {
        PlayerMovementBasic.Instance.headBob_enabled = isEnabled;
        PlayerPrefs.SetInt("headBobEnabled", (isEnabled ? 1 : 0));

    }
    public void SetInvertYAxis(bool isEnabled)
    {
        SimpleSmoothMouseLook.Instance.invertYAxis = isEnabled;
        PlayerPrefs.SetInt("invertYAxis", (isEnabled ? 1 : 0));

    }
    public void SetMouseSensitivity(float sliderValue)
    {
        mouseLooker.sensitivity = sliderValue;
        PlayerPrefs.SetFloat("mouseSensitivity", sliderValue);
    }
    public void SetMouseSmoothing(float sliderValue)
    {
        mouseLooker.smoothing = sliderValue;
        PlayerPrefs.SetFloat("mouseSmoothing", sliderValue);
    }
    public void SetVsync(bool isEnabled)
    {
        QualitySettings.vSyncCount = isEnabled ? 1 : 0;
        PlayerPrefs.SetInt("vsync", (isEnabled ? 1 : 0));
    }


    public void SetShadows(bool isEnabled)
    {
        QualitySettings.shadows = isEnabled ? ShadowQuality.All : ShadowQuality.Disable;
        PlayerPrefs.SetInt("shadows", (isEnabled ? 1 : 0));
    }

    public void SetToolTips(bool isEnabled)
    {
        if(toolTipGameObject)
        {
        toolTipGameObject.SetActive(isEnabled ? true : false);
        }
        PlayerPrefs.SetInt("tooltipsenabled", (isEnabled ? 1 : 0));
    }

    public void SetStreamerMode(bool isEnabled)
    {
        if(playerPixelCamera)
        {
        playerPixelCamera.transform.gameObject.SetActive(isEnabled ? true : false);
        }
        PlayerPrefs.SetInt("streamermodeenabled", (isEnabled ? 1 : 0));
    }

    public void SetCamTilt(bool isEnabled)
    {
        if(isEnabled)
        {
        playerMovement.camRotZ_MaxTilt = 4;
        playerMovement.camRotX_MaxTilt = 1;
        }
        else
        {
        playerMovement.camRotZ_MaxTilt = 0;
        playerMovement.camRotX_MaxTilt = 0;
        }

        PlayerPrefs.SetInt("camTilit", (isEnabled ? 1 : 0));
    }

    //AUDIO
    public void SetGameVolume(float sliderValue)
    {
        mainAudioMixer.SetFloat("Game", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("gameVolume", sliderValue);
    }


    
    public static float mapVolumeModifier = 1f;
    void Update()
    {
        if(mapVolumeModifier > 1f)
        {
            mapVolumeModifier = 1f;
        }
    }


    public void SetMediaVolume(float sliderValue)
    {
        mapVolumeModifier = sliderValue;

        mainAudioMixer.SetFloat("Media", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("mediaVolume", sliderValue);
    }


    
    public void SetURLmedia(bool isEnabled)
    {
        allowURLmedia = isEnabled;
        PlayerPrefs.SetInt("allowURLmedia", (isEnabled ? 1 : 0));
    }


    public void SetAutoBackupInterval(string value)
    {
        int number;
        bool success = int.TryParse(value, out number);
        if (!success)
        {
            number = 2;
        }

        AutoBackupManager.Instance.interval = number;
        PlayerPrefs.SetInt("AutoBackupInterval", number);
    }
}
