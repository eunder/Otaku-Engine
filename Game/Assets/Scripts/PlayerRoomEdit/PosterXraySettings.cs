using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosterXraySettings : MonoBehaviour
{
    public float xray_Size;
    public float xray_Transparency;
    public float xray_Softness;


    public void UpdateShaderWithXraySettings()
    {
        GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial.SetFloat("_size", xray_Size);
        GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial.SetFloat("_transparency", xray_Transparency);
        GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial.SetFloat("_smoothness", xray_Softness);
    }
}
