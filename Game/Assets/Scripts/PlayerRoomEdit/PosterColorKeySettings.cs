using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosterColorKeySettings : MonoBehaviour
{
    public Color colorKey;
    public float threshold;
    public float transparencyThreshold;
    public float spillCorrection;

    public void ApplyColorKeySettingsToShader()
    {
        GetComponent<Renderer>().sharedMaterial.SetFloat("_Threshold", threshold);
        GetComponent<Renderer>().sharedMaterial.SetColor("_ColorKey", colorKey);
        GetComponent<Renderer>().sharedMaterial.SetFloat("_TransparencyThreshold", transparencyThreshold);
        GetComponent<Renderer>().sharedMaterial.SetFloat("_SpillCorrection", spillCorrection);
    }

}
