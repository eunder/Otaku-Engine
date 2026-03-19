using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class AudioDownloaderManager : MonoBehaviour
{

    private static AudioDownloaderManager _instance;
    public static AudioDownloaderManager Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public IEnumerator LoadAudioClip(string path, System.Action<AudioClip> callback)
    {
        if(string.IsNullOrWhiteSpace(path) == false)
        {

            //check if the media is in the same folder
            if(File.Exists(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + path ))
            {
                path = System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + path;
            }

            //check if video is inside the current unzipped temp dir
            if(File.Exists(ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory + "/" + path))
            {
            path = ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory + "/" + path;
            }


        if(!File.Exists(path))
        {
            yield break;
        }


        if (string.IsNullOrWhiteSpace(path) || path.Contains(".."))
        {
            yield break;
        }
        if(!GlobalUtilityFunctions.IsSafeToReadBytes(path))
        {
            yield break;
        }
    
        bool cancel = false;

        if(GlobalUtilityFunctions.IsURL(path))
        {
        if(ConfigMenuUIEvents.Instance.allowURLmedia == false)
        {
            cancel = true;
            UINotificationHandler.Instance.SpawnNotification("<color=red>AuDwd: Url Media disabled!");
        }
        }

            if(cancel == false)
            {
                AudioType audioType = AudioType.MPEG;
                path = GlobalUtilityFunctions.UrlChecker_AudioFormat(path);

                if(path.EndsWith(".mp3") || path.EndsWith(".MP3"))
                {
                    audioType = AudioType.MPEG;
                }
                if(path.EndsWith(".wav") || path.EndsWith(".WAV"))
                {
                    audioType = AudioType.WAV;
                }
                if(path.EndsWith(".ogg") || path.EndsWith(".OGG"))
                {
                    audioType = AudioType.OGGVORBIS;
                }




                
                //prevents a weird delay
                if(ConfigMenuUIEvents.Instance.allowURLmedia == false)
                {
                    path = "file:///" + path;
                }

                //LOADING THE AUDIO
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, audioType))
                {
                    yield return www.SendWebRequest();

                    if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                    {
                        //prevents a weird message, "404 error" for local files
                        if(ConfigMenuUIEvents.Instance.allowURLmedia == true)
                        {
                            UINotificationHandler.Instance.SpawnNotification("<color=red>AuDwd ERROR:" + www.error);
                        }
                    }
                    else
                    {
                        try
                        {

                           byte[] audioBytes = www.downloadHandler.data;

                            if(!GlobalUtilityFunctions.IsFileDataSafe(audioBytes))
                            {
                                UINotificationHandler.Instance.SpawnNotification("<color=red>AuDwd ERROR: " + path);
                                yield break;
                            }

                            
                            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                            callback?.Invoke(audioClip);
                        }
                        catch (System.Exception e)
                        {
                                UINotificationHandler.Instance.SpawnNotification("<color=red>AuDwd ERROR: " + e.Message);
                        }
                    }
                }
            }
        }
    }
}
