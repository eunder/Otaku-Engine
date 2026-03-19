using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using UnityEngine.Networking;

public class DialogueContentObject : MonoBehaviour
{
    public string dialogue;
    public string voicePath;
    public bool clearPreviousDialogue = false;
    public AudioClip voice_AudioClip;
    public float pitch = 1f;


    public UnityEvent startEvent;
    public UnityEvent endEvent;

    
    public IEnumerator LoadAudio(string path)
    {
        voicePath = path;

        yield return AudioDownloaderManager.Instance.LoadAudioClip(path, (audioClip) =>
        {
            voice_AudioClip = audioClip;
        });
    }
}
