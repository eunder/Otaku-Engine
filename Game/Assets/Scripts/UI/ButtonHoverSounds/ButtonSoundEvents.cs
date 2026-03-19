using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundEvents : MonoBehaviour
{
    public AudioClip pointerDown_AudioClip;
    public AudioClip pointerUp_AudioClip;

    public AudioClip hoverOver_AudioClip;

    public void PointerUp_Sound()
    {
            ToolBarStatic_UISFXAudioSource.Instance.ToolBar_UISFX_Play(pointerDown_AudioClip);
    }

    public void PointerDown_Sound()
    {
            ToolBarStatic_UISFXAudioSource.Instance.ToolBar_UISFX_Play(pointerUp_AudioClip);
    }


    public void PlayButtonMouseHoverOverSound()
    {
            ToolBarStatic_UISFXAudioSource.Instance.ToolBar_UISFX_Play(hoverOver_AudioClip);
    }

}
