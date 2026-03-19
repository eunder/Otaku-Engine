using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPickerSoundEvents : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip_itemBought;

    public void AudioEvent_BoughtItem()
    {
        audioSource.PlayOneShot(audioClip_itemBought, 1.0f);
    }

}
