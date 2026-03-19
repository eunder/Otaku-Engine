/*
UniGif
Copyright (c) 2015 WestHillApps (Hironari Nishioka)
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Texture Animation from GIF image
/// </summary>
public class PosterGifPlayer : MonoBehaviour
{
    /// <summary>
    /// This component state
    /// </summary>
    public enum State
    {
        None,
        Loading,
        Ready,
        Playing,
        Pause,
    }

    public Material posterMaterial_original;
    public Material posterMaterial;

    public byte[] gifbytes;

    // Textures filter mode
    [SerializeField]
    private FilterMode m_filterMode = FilterMode.Point;
    // Textures wrap mode
    [SerializeField]
    private TextureWrapMode m_wrapMode = TextureWrapMode.Repeat;
    // Load from url on start
    [SerializeField]
    private bool m_loadOnStart;
    // GIF image url (WEB or StreamingAssets path)
    [SerializeField]
    private string m_loadOnStartUrl;
    // Rotating on loading
    [SerializeField]
    private bool m_rotateOnLoading;
    // Debug log flag
    [SerializeField]
    private bool m_outputDebugLog;

    // Decoded GIF texture list
    public List<UniGif.GifTexture> m_gifTextureList;
    // Delay time
    private float m_delayTime;
    // Texture index
    private int m_gifTextureIndex;
    // loop counter
    private int m_nowLoopCount;

    /// <summary>
    /// Now state
    /// </summary>
    public State nowState
    {
        get;
        private set;
    }

    /// <summary>
    /// Animation loop count (0 is infinite)
    /// </summary>
    public int loopCount
    {
        get;
        private set;
    }

    /// <summary>
    /// Texture width (px)
    /// </summary>
    public int width
    {
        get;
        private set;
    }

    /// <summary>
    /// Texture height (px)
    /// </summary>
    public int height
    {
        get;
        private set;
    }

    private void Start()
    {

        if (m_loadOnStart)
        {
            SetGifFromUrl(m_loadOnStartUrl);
        }
    }

    private void OnDestroy()
    {
        Clear();
    }

    private void Update()
    {
        switch (nowState)
        {
            case State.None:
                break;

            case State.Loading:
                if (m_rotateOnLoading)
                {

                }
                break;

            case State.Ready:
                break;

            case State.Playing:
                if (posterMaterial == null || m_gifTextureList == null || m_gifTextureList.Count <= 0)
                {
                    return;
                }
                if (m_delayTime > Time.time)
                {
                    return;
                }
                // Change texture
                m_gifTextureIndex++;
                if (m_gifTextureIndex >= m_gifTextureList.Count)
                {
                    m_gifTextureIndex = 0;

                    //OnGifRestart Event
                    EventActionManager.Instance.TryPlayEvent(gameObject,"OnGifRestart");

                    if (loopCount > 0)
                    {
                        m_nowLoopCount++;
                        if (m_nowLoopCount >= loopCount)
                        {
                            Stop();
                            return;
                        }
                    }
                }
                posterMaterial.SetTexture("_MainTex", m_gifTextureList[m_gifTextureIndex].m_texture2d);
                m_delayTime = Time.time + m_gifTextureList[m_gifTextureIndex].m_delaySec;
                break;

            case State.Pause:
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Set GIF texture from url
    /// </summary>
    /// <param name="url">GIF image url (WEB or StreamingAssets path)</param>
    /// <param name="autoPlay">Auto play after decode</param>
    public void SetGifFromUrl(string url, bool autoPlay = true)
    {
        StartCoroutine(SetGifFromUrlCoroutine(url, autoPlay));
    }

    /// <summary>
    /// Set GIF texture from url
    /// </summary>
    /// <param name="url">GIF image url (WEB or StreamingAssets path)</param>
    /// <param name="autoPlay">Auto play after decode</param>
    /// <returns>IEnumerator</returns>
    public IEnumerator SetGifFromUrlCoroutine(string url, bool autoPlay = true)
    {
            posterMaterial_original = GetComponent<MeshRenderer>().material;
            posterMaterial = GetComponent<MeshRenderer>().material;

        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("URL is nothing.");
            yield break;
        }

        if (nowState == State.Loading)
        {
            Debug.LogWarning("Already loading.");
            yield break;
        }
        nowState = State.Loading;

        string path;
        if (url.StartsWith("http"))
        {
            // from WEB
            path = url;
        }
        else
        {
            // from StreamingAssets
            path = Path.Combine("file:///" + Application.streamingAssetsPath, url);
        }

        // Load file
        using (WWW www = new WWW(path))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error) == false)
            {
                Debug.LogError("File load error.\n" + www.error);
                nowState = State.None;
                yield break;
            }

            Clear();
            nowState = State.Loading;

            // Get GIF textures
            yield return StartCoroutine(UniGif.GetTextureListCoroutine(www.bytes, (gifTexList, loopCount, width, height) =>
            {
                if (gifTexList != null)
                {
                    m_gifTextureList = gifTexList;
                    this.loopCount = loopCount;


                    if(GameObject.Find("ItemEditStateMachine").GetComponent<ItemEditStateMachine>().currentObjectEditing != null)
                    {
                        GameObject.Find("ItemEditStateMachine").GetComponent<ItemEditStateMachine>().successfullImageUploadEventHandler.SuccessfulLocalImageUploadSoundEvent();
                    }

                    if(GetComponent<PosterMeshCreator>())
                    {
                    GetComponent<PosterMeshCreator>().width = width;
                    GetComponent<PosterMeshCreator>().height = height;
                    GetComponent<PosterMeshCreator>().image = new Vector2(width,height);

                    GetComponent<PosterFrameList>().AssignWhichFrameIsLast();
                    GetComponent<PosterMeshCreator>().BuildPoster();
                    }

                    if(GetComponent<LevelGlobalMediaManager>())
                    {
                    GetComponent<LevelGlobalMediaManager>().width = width;
                    GetComponent<LevelGlobalMediaManager>().height = height;
                    GetComponent<LevelGlobalMediaManager>().image = new Vector2(width,height);
                    GetComponent<LevelGlobalMediaManager>().BuildPoster();
                    }

                    gifbytes = www.bytes;

                    nowState = State.Ready;


                    if (m_rotateOnLoading)
                    {

                    }

                    if (autoPlay)
                    {
                        Play();
                    }
                }
                else
                {
                    Debug.LogError("Gif texture get error.");
                    nowState = State.None;
                }
            },
            m_filterMode, m_wrapMode, m_outputDebugLog));
        }
    }


    //mostly used for color picking
    public Texture2D GetCurrentTex2D()
    {
        if(m_gifTextureList != null && m_gifTextureList.Count >= 0 && m_gifTextureList[m_gifTextureIndex].m_texture2d != null)
        {
        return m_gifTextureList[m_gifTextureIndex].m_texture2d;
        }
        else
        {
        return null;
        }
    }


    /// <summary>
    /// Clear GIF texture
    /// </summary>
    public void Clear()
    {
        if (posterMaterial != null)
        {
            posterMaterial.SetTexture("_MainTex", null);
        }

        if (m_gifTextureList != null)
        {
            for (int i = 0; i < m_gifTextureList.Count; i++)
            {
                if (m_gifTextureList[i] != null)
                {
                    if (m_gifTextureList[i].m_texture2d != null)
                    {
                        Destroy(m_gifTextureList[i].m_texture2d);
                        m_gifTextureList[i].m_texture2d = null;
                    }
                    m_gifTextureList[i] = null;
                }
            }
            m_gifTextureList.Clear();
            m_gifTextureList = null;
        }

        nowState = State.None;
    }

    /// <summary>
    /// Play GIF animation
    /// </summary>
    public void Play()
    {
        if (nowState != State.Ready)
        {
            Debug.LogWarning("State is not READY.");
            return;
        }
        if (posterMaterial == null || m_gifTextureList == null || m_gifTextureList.Count <= 0)
        {
            Debug.LogError("Raw Image or GIF Texture is nothing.");
            return;
        }
        nowState = State.Playing;
        posterMaterial.SetTexture("_MainTex", m_gifTextureList[0].m_texture2d);
        m_delayTime = Time.time + m_gifTextureList[0].m_delaySec;
        m_gifTextureIndex = 0;
        m_nowLoopCount = 0;
    }

    /// <summary>
    /// Stop GIF animation
    /// </summary>
    public void Stop()
    {
        if (nowState != State.Playing && nowState != State.Pause)
        {
            Debug.LogWarning("State is not Playing and Pause.");
            return;
        }
        nowState = State.Ready;
    }

    /// <summary>
    /// Pause GIF animation
    /// </summary>
    public void Pause()
    {
        if (nowState != State.Playing)
        {
            Debug.LogWarning("State is not Playing.");
            return;
        }
        nowState = State.Pause;
    }

    /// <summary>
    /// Resume GIF animation
    /// </summary>
    public void Resume()
    {
        if (nowState != State.Pause)
        {
            Debug.LogWarning("State is not Pause.");
            return;
        }
        nowState = State.Playing;
    }
}