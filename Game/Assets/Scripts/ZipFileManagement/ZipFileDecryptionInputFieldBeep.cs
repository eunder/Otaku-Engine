using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZipFileDecryptionInputFieldBeep : MonoBehaviour
{
    public TMP_InputField inputField;
    public AudioSource audioSource;

    public void PlayBeepSound()
    {
        // Play the beep sound
        audioSource.PlayOneShot(audioSource.clip, 1.0f);
    }
}