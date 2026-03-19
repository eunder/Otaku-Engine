using UnityEngine;
using System;
using LibVLCSharp;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

///This is a basic implementation of a media player using VLC for Unity using LibVLCSharp
///It exposes some basic playback controls, you may wish to add more of these
///It outputs audio directly to speakers and video to a RenderTexture and a Renderer or RawImage screen
///This example also shows how to deal with several common problems including vertically flipped videos 
///
///libvlcsharp usage documentation: https://code.videolan.org/videolan/LibVLCSharp/-/blob/master/docs/home.md
///LibVLC parameters: https://wiki.videolan.org/VLC_command-line_help/
///Report a bug: https://code.videolan.org/videolan/vlc-unity/-/issues

public class VLC_GlobalMediaPlayer : MonoBehaviour
{

	private static VLC_GlobalMediaPlayer _instance;
    public static VLC_GlobalMediaPlayer Instance { get { return _instance; } }

	public bool dontPlayOnAwake_Gamemode = false;

	public MediaPlayer mediaPlayer; //MediaPlayer is the main class we use to interact with VLC

	public int volume_original = 100;

    public Material skyboxMat;

	//Screens
	public Renderer screen; //Assign a mesh to render on a 3d object
	public RawImage canvasScreen; //Assign a Canvas RawImage to render on a GUI object

	Texture2D _vlcTexture = null; //This is the texture libVLC writes to directly. It's private.
	public RenderTexture texture = null; //We copy it into this texture which we actually use in unity.


	public string path = null; //Can be a web path or a local path

	public bool flipTextureX = false; //No particular reason you'd need this but it is sometimes useful
	public bool flipTextureY = true; //Set to false on Android, to true on Windows

	public bool automaticallyFlipOnAndroid = true; //Automatically invert Y on Android

	public bool playOnAwake = true; //Open path and Play during Awake

	public bool logToConsole = false; //Log function calls and LibVLC logs to Unity console

	//Unity Awake, OnDestroy, and Update functions
	#region unity
	void Awake()
	{
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }


		//Setup Screen
		if (screen == null)
			screen = GetComponent<Renderer>();
		if (canvasScreen == null)
			canvasScreen = GetComponent<RawImage>();

		//Automatically flip on android
		if (automaticallyFlipOnAndroid && Application.platform == RuntimePlatform.Android)
			flipTextureY = !flipTextureY;

		//Setup Media Player
		CreateMediaPlayer();

		//Play On Start
		if (playOnAwake)
			Open();
	}

	void OnDestroy()
	{
		//Dispose of mediaPlayer, or it will stay in nemory and keep playing audio
		DestroyMediaPlayer();
	}

	//mostly used for the tool bar menu, recovering original audio set
	public void ResetVolumeOriginal()
	{
		mediaPlayer.SetVolume((int)((float)volume_original * ConfigMenuUIEvents.mapVolumeModifier));
	}




	void Update()
	{
		//Get size every frame
		uint height = 0;
		uint width = 0;
		mediaPlayer.Size(0, ref width, ref height);

		//Automatically resize output textures if size changes
		if (_vlcTexture == null || _vlcTexture.width != width || _vlcTexture.height != height)
		{
			ResizeOutputTextures(width, height);
		}

		if (_vlcTexture != null)
		{
			//Update the vlc texture (tex)
			var texptr = mediaPlayer.GetTexture(width, height, out bool updated);
			if (updated)
			{
				_vlcTexture.UpdateExternalTexture(texptr);
			
				//Copy the vlc texture into the output texture, flipped over
				var flip = new Vector2(flipTextureX ? -1 : 1, flipTextureY ? -1 : 1);
				Graphics.Blit(_vlcTexture, texture, flip, Vector2.zero); //If you wanted to do post processing outside of VLC you could use a shader here.
			}
		}
	}
	#endregion


    public LevelGlobalMediaManager currentPosterPlaying;

    Coroutine co;

 public void PlayVideoURL(LevelGlobalMediaManager poster)
    {
				//cancel if urls disabled
		  if(GlobalUtilityFunctions.IsURL(poster.urlFilePath))
        {
            if(ConfigMenuUIEvents.Instance.allowURLmedia == false)
            {
                UINotificationHandler.Instance.SpawnNotification("<color=red>Url Media disabled!");
				return;
            }

        }

        if (string.IsNullOrWhiteSpace(poster.urlFilePath) || poster.urlFilePath.Contains(".."))
        {
            return;
        }
		if(!GlobalUtilityFunctions.IsPathSafe(poster.urlFilePath))
		{
			return;
		}



            if(co != null)
            {
            StopCoroutine(co);
            }

            currentPosterPlaying = poster;


                    if(File.Exists(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + poster.urlFilePath ))
                    {
            			Open(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + poster.urlFilePath);
					}
					else if(File.Exists(poster.urlFilePath))
					{
            			Open(poster.urlFilePath);
					}
					else if(poster.urlFilePath.StartsWith("https"))
					{
            			Open(poster.urlFilePath);
					}

    }

  public void StopVideoPlayerIfPosterWhereVideoIsPlayingWasChanged(PosterMeshCreator poster)
    {
        if(currentPosterPlaying == poster.gameObject)
        {
                mediaPlayer.Stop();


             //   currentPosterPlaying.meshRenderer.sharedMaterial = currentPosterPlaying.videoStaticMat;
                currentPosterPlaying = null;
        }
    }
	//Public functions that expose VLC MediaPlayer functions in a Unity-friendly way. You may want to add more of these.
	#region vlc
	public void Open(string path)
	{
		Log("VLCPlayerExample Open " + path);
		this.path = path;
		Open();
	}

	public void Open()
	{
		if(!GlobalUtilityFunctions.IsPathSafe(path))
		return;

		if(mediaPlayer == null)
		return;

		if (mediaPlayer.Media != null)
			mediaPlayer.Media.Dispose();

		var trimmedPath = path.Trim(new char[]{'"'});//Windows likes to copy paths with quotes but Uri does not like to open them
		mediaPlayer.Media = new Media(new Uri(trimmedPath));
	//	mediaPlayer.Media.AddOption(":sout-schro-filtering=lowpass");
	//	mediaPlayer.Media.AddOption(":sout-schro-filter-value=10.0f");
		
		if(!dontPlayOnAwake_Gamemode)
		Play();

	}

	public void Play()
	{
		Log("VLCPlayerExample Play");

		volume_original = 100;
		mediaPlayer.SetVolume((int)((float)volume_original * ConfigMenuUIEvents.mapVolumeModifier)); //always set it to max volume by default

		mediaPlayer.Play();
	}

	public void Pause()
	{
		Log("VLCPlayerExample Pause");
		mediaPlayer.Pause();
	}

	public void Stop()
	{
		Log("VLCPlayerExample Stop");
		mediaPlayer?.Stop();

		_vlcTexture = null;
		texture = null;
	}

	public void Seek(long timeDelta)
	{
		Log("VLCPlayerExample Seek " + timeDelta);
		mediaPlayer.SetTime(mediaPlayer.Time + timeDelta);
	}

	public void SetTime(long time)
	{
		Log("VLCPlayerExample SetTime " + time);
		mediaPlayer.SetTime(time);
	}

	public void SetVolume(int volume = 100)
	{
		Log("VLCPlayerExample SetVolume " + volume);
		volume_original = volume;
		mediaPlayer.SetVolume((int)((float)volume_original * ConfigMenuUIEvents.mapVolumeModifier));
	}

	public int Volume
	{
		get
		{
			if (mediaPlayer == null)
				return 0;
			return mediaPlayer.Volume;
		}
	}

	public bool IsPlaying
	{
		get
		{
			if (mediaPlayer == null)
				return false;
			return mediaPlayer.IsPlaying;
		}
	}

	public long Duration
	{
		get
		{
			if (mediaPlayer == null || mediaPlayer.Media == null)
				return 0;
			return mediaPlayer.Media.Duration;
		}
	}

	public long Time
	{
		get
		{
			if (mediaPlayer == null)
				return 0;
			return mediaPlayer.Time;
		}
	}

	public List<MediaTrack> Tracks(TrackType type)
	{
		Log("VLCPlayerExample Tracks " + type);
		return ConvertMediaTrackList(mediaPlayer?.Tracks(type));
	}

	public MediaTrack SelectedTrack(TrackType type)
	{
		Log("VLCPlayerExample SelectedTrack " + type);
		return mediaPlayer?.SelectedTrack(type);
	}

	public void Select(MediaTrack track)
	{
		Log("VLCPlayerExample Select " + track.Name);
		mediaPlayer?.Select(track);
	}

	public void Unselect(TrackType type)
	{
		Log("VLCPlayerExample Unselect " + type);
		mediaPlayer?.Unselect(type);
	}

	//This returns the video orientation for the currently playing video, if there is one
	public VideoOrientation? GetVideoOrientation()
	{
		var tracks = mediaPlayer?.Tracks(TrackType.Video);

		if (tracks == null || tracks.Count == 0)
			return null;

		var orientation = tracks[0]?.Data.Video.Orientation; //At the moment we're assuming the track we're playing is the first track

		return orientation;
	}

	#endregion

	//Private functions create and destroy VLC objects and textures
	#region internal

	//Create a new MediaPlayer object and dispose of the old one. 
	void CreateMediaPlayer()
	{
		Log("VLCPlayerExample CreateMediaPlayer");
		if (mediaPlayer != null)
		{
			DestroyMediaPlayer();
		}
		mediaPlayer = new MediaPlayer(VLC_LibVLC_Singleton.Instance.libVLC);

	}

	//Dispose of the MediaPlayer object. 
	void DestroyMediaPlayer()
	{
		Log("VLCPlayerExample DestroyMediaPlayer");
		mediaPlayer?.Stop();
		mediaPlayer?.Dispose();
		mediaPlayer = null;
	}

	//Resize the output textures to the size of the video
	void ResizeOutputTextures(uint px, uint py)
	{
		var texptr = mediaPlayer.GetTexture(px, py, out bool updated);
		if (px != 0 && py != 0 && updated && texptr != IntPtr.Zero)
		{
			//If the currently playing video uses the Bottom Right orientation, we have to do this to avoid stretching it.
			if(GetVideoOrientation() == VideoOrientation.BottomRight)
			{
				uint swap = px;
				px = py;
				py = swap;
			}

			_vlcTexture = Texture2D.CreateExternalTexture((int)px, (int)py, TextureFormat.RGBA32, false, true, texptr); //Make a texture of the proper size for the video to output to

            texture = new RenderTexture(1000,1000, 0, RenderTextureFormat.ARGB32); //Make a renderTexture the same size as vlctex
            texture.wrapMode = TextureWrapMode.Repeat;

            currentPosterPlaying.AssignVideoMaterial(texture);

			if (screen != null)
				screen.material.mainTexture = texture;
			if (canvasScreen != null)
				canvasScreen.texture = texture;
		}
	}

	//Converts MediaTrackList objects to Unity-friendly generic lists. Might not be worth the trouble.
	List<MediaTrack> ConvertMediaTrackList(MediaTrackList tracklist)
	{
		if (tracklist == null)
			return new List<MediaTrack>(); //Return an empty list

		var tracks = new List<MediaTrack>((int)tracklist.Count);
		for (uint i = 0; i < tracklist.Count; i++)
		{
			tracks.Add(tracklist[i]);
		}
		return tracks;
	}

	void Log(object message)
	{
		if(logToConsole)
			Debug.Log(message);
	}
	#endregion





}