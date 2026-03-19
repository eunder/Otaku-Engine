using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosterFootstepSound : MonoBehaviour
{
    public string footstepSoundPath;
    public AudioClip footStepAudioClip;


    public IEnumerator LoadFootStepSound(string path)
    {
        footstepSoundPath = path;

        yield return AudioDownloaderManager.Instance.LoadAudioClip(path, (audioClip) =>
        {
            footStepAudioClip = audioClip;
        });

    }


}
