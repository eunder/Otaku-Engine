using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    private static PostProcessingManager _instance;
    public static PostProcessingManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    //bloom
    public bool bloom_isOn = false;
    public float bloom_intensity = 10.48f;
    public float bloom_threshold = 1.13f;
    public Color bloom_color = Color.white;
    public float bloom_softKnee = 0.5f;
    public float bloom_diffusion = 7;

    //Depth of Field
    public bool dof_isOn = false;
    public float dof_maxFocusDistance = 20f;

    //ambient occlusion
    public bool ao_isOn = false;



    public PostProcessVolume volume;
    AmbientOcclusion ambientOcclusion;
    Bloom bloom;
    ChromaticAberration chromaticAberration;

    public DepthOfField depthOfField;



    void start()
    {
        volume = SimpleSmoothMouseLook.Instance.transform.GetComponent<PostProcessVolume>(); 

        volume.profile.TryGetSettings(out bloom);
        volume.profile.TryGetSettings(out depthOfField);
        volume.profile.TryGetSettings(out ambientOcclusion);
        volume.profile.TryGetSettings(out chromaticAberration);
    }


    public void UpdatePostProcessingSettings()
    {
        if(volume == null)
        {
            volume = SimpleSmoothMouseLook.Instance.transform.GetComponent<PostProcessVolume>(); 
     
            volume.profile.TryGetSettings(out bloom);
            volume.profile.TryGetSettings(out depthOfField);
            volume.profile.TryGetSettings(out ambientOcclusion);
            volume.profile.TryGetSettings(out chromaticAberration);
        }

        if(volume)
        {
        bloom.enabled.value = bloom_isOn;
        bloom.intensity.value = bloom_intensity;
        bloom.threshold.value = bloom_threshold;
        bloom.color.value = bloom_color;
        bloom.softKnee.value = bloom_softKnee;
        bloom.diffusion.value = bloom_diffusion;

        depthOfField.enabled.value = dof_isOn;
        CameraDepthOfFieldAdjust.Instance.maxFocusDistance = dof_maxFocusDistance;

        ambientOcclusion.enabled.value = ao_isOn;
        }

        //for making sure player config settings are kept
        if(PlayerPrefs.GetInt("PostProccessingBloom", 1) == 0)
        {
            bloom.enabled.value = false;
        }
        if(PlayerPrefs.GetInt("AmbientOcclusion", 1) == 0)
        {
            ambientOcclusion.enabled.value = false;
        }
        if(PlayerPrefs.GetInt("PostProccessingChromaticAbberation", 1) == 0)
        {
            chromaticAberration.enabled.value = false;
        }
    }


}
