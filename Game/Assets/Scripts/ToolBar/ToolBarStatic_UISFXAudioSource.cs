using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBarStatic_UISFXAudioSource : MonoBehaviour
{
    private static ToolBarStatic_UISFXAudioSource _instance;
    public static ToolBarStatic_UISFXAudioSource Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }



    public AudioSource UISFX_AudioSource;
 public void ToolBar_UISFX_Play(AudioClip audioClip)
    {
        UISFX_AudioSource.PlayOneShot(audioClip, 1.0f);
    }
}
