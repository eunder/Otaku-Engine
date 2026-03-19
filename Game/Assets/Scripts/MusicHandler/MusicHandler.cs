using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MusicHandler : MonoBehaviour
{

    private static MusicHandler _instance;
    public static MusicHandler Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public AudioSource musicSource;
    public AudioClip musicTrack;
    public string musicTrackPath;
    public TMP_InputField musicUrl_InputField;

    void Start()
    {
        Random.seed = (int)System.DateTime.Now.Ticks;
    }

    public void FinishedEditingInputUrl()
    {
        if(GlobalUtilityFunctions.IsPathSafe(musicUrl_InputField.text))
        {
            //makes sure the song dosnt repeat if its the same link(aka, if user was checking the link)
            if(!musicUrl_InputField.text.Equals(musicTrackPath))
            {
            StartCoroutine(LoadMusic(musicUrl_InputField.text));
            }

            if(string.IsNullOrWhiteSpace(musicUrl_InputField.text))
            {
                musicSource.Stop();
            }
        }

    }

     public IEnumerator LoadMusic(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || path.Contains(".."))
        {
            yield break;
        }

        musicTrackPath = path;

        yield return AudioDownloaderManager.Instance.LoadAudioClip(path, (audioClip) =>
        {
            musicTrack = audioClip;
            PlayMusic();
        }); 
    }



    public void PlayMusic()
    {
        musicSource.clip = musicTrack;
        musicSource.Play();
    }

    }
