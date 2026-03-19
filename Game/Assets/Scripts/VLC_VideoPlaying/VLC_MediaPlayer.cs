using UnityEngine;
using System;
using LibVLCSharp;
using UnityEngine.UI;
using System.Collections;
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

public class VLC_MediaPlayer : MonoBehaviour
{
	public MediaPlayer mediaPlayer; //MediaPlayer is the main class we use to interact with VLC

	public int volume_original = 100;

	public Material postProcessMaterial;

	//Screens
	public Renderer screen; //Assign a mesh to render on a 3d object
	public RawImage canvasScreen; //Assign a Canvas RawImage to render on a GUI object

	Texture2D _vlcTexture = null; //This is the texture libVLC writes to directly. It's private.
	public RenderTexture texture = null; //We copy it into this texture which we actually use in unity.


	public string path = null; //Can be a web path or a local path

	public bool flipTextureX = false; //No particular reason you'd need this but it is sometimes useful
	public bool flipTextureY = true; //Set to false on Android, to true on Windows

	public bool automaticallyFlipOnAndroid = true; //Automatically invert Y on Android

	public bool playOnAwake = false; //Open path and Play during Awake

	public bool logToConsole = false; //Log function calls and LibVLC logs to Unity console


	//Unity Awake, OnDestroy, and Update functions
	#region unity
	void Awake()
	{
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
	
			//AND the poster's frames too!
			if(GetComponent<PosterFrameList>().posterFrameList.Count >= 1)
			{
				GetComponent<PosterFrameList>().posterFrameList[0].GetComponent<PosterMeshCreator_BorderFrame>().UpdateFrame();
			}
	
		}

		//when the video is playing update the poster dimensions
		if(IsPlaying)
		{
				var tracks = mediaPlayer?.Tracks(TrackType.Video);
				if(tracks != null)
				{
				GetComponent<PosterMeshCreator>().width = (float)tracks[0]?.Data.Video.Width;
				GetComponent<PosterMeshCreator>().height = (float)tracks[0]?.Data.Video.Height;
        		GetComponent<PosterMeshCreator>().image = new Vector2((float)tracks[0]?.Data.Video.Width,(float)tracks[0]?.Data.Video.Height);
				}
		}

		if (_vlcTexture != null)
		{
			//Update the vlc texture (tex)
			var texptr = mediaPlayer.GetTexture(width, height, out bool updated);
			if (updated)
			{
				_vlcTexture.UpdateExternalTexture(texptr);
				
					//MAKES SURE WHEN THE POSTER WIDTH AND SIZES ARE CHANGED, THE CORRECT ASPECT RATIO IS APPLIED
					 float aspect = (float)(_vlcTexture.width / GetComponent<PosterMeshCreator>().width ) / (float)_vlcTexture.height * GetComponent<PosterMeshCreator>().height;
       				 postProcessMaterial.SetFloat("_Aspect", aspect);

				//Copy the vlc texture into the output texture, flipped over
				var flip = new Vector2(flipTextureX ? -1 : 1, flipTextureY ? -1 : 1);
				Graphics.Blit(_vlcTexture, texture, postProcessMaterial); //If you wanted to do post processing outside of VLC you could use a shader here.
			}
		}
	}
	#endregion

    Coroutine co;



	//TIME LINE EVENTS MECHANIC
    private Coroutine CheckForTimelineEventCoroutine;
    public float timelineCheckInterval = 0.05f;
    public List<SaveAndLoadLevel.Event> timelineEvents = new List<SaveAndLoadLevel.Event>();

    public void UpdateTimelineEventList()
    {
        timelineEvents.Clear();

        foreach(SaveAndLoadLevel.Event e in transform.GetComponent<EventHolderList>().events)
        {
            if(e.onAction == "OnVideoReachTime")
            timelineEvents.Add(e);
        }
    }

    public void StartTimelineCheckingCoroutine()
    {
        if(CheckForTimelineEventCoroutine != null)
        StopCoroutine(CheckForTimelineEventCoroutine);

        CheckForTimelineEventCoroutine = StartCoroutine(CheckTimelineTriggerEvents());
    }

		public float previousTime;
		public float currentTime;
		public bool videoEnded = false;

        //This coroutine checks the timeline events by looping through all the timeline events at an interval
        IEnumerator CheckTimelineTriggerEvents()
        {
			previousTime = 0.0f;

            while (true) // Infinite loop
            {
				 	 currentTime = (float)mediaPlayer.Time / (float)1000;

				//ONLY USED FOR MAKING SURE THE DEPTH LAYER ASPECT RATIOS UPDATE! (not really part of event triggers but still needed for the depth layer mechanic)
				if(currentTime > 0)
				{
					if(GetComponent<PosterMeshCreator>().currentDepthLayerAssignedTo != null)
					GetComponent<PosterMeshCreator>().currentDepthLayerAssignedTo.GetComponent<Poster_DepthStencilFrame>().UpdateGeneralValues();
				}




                //for triggering timeline events
                List<SaveAndLoadLevel.Event> eventsToRemove = new List<SaveAndLoadLevel.Event>();

                foreach(SaveAndLoadLevel.Event timelineEvent in timelineEvents)
                {
                    if( float.Parse(timelineEvent.onParamater) <= currentTime)
                    {	 
                        // Trigger event here
                        EventActionManager.Instance.TryPlayEvent_Single(timelineEvent);

                        eventsToRemove.Add(timelineEvent);
                    }
                }



                //Remove it from the list so that it dosnt get played again until the audio restarts
                foreach(SaveAndLoadLevel.Event eventToRemove in eventsToRemove)
                {
                    timelineEvents.Remove(eventToRemove);
                }            


			//used to tell the game when a video ended
			if(currentTime < previousTime)	
			{
					videoEnded = true;
			}

            //video restarted

			//note: some weird logic is used here... the "currentTime < 0.5f" part when used at lower values didnt work with mp4 videos for some reason...
			if ((currentTime < 0.5f && previousTime > 0.0f) && videoEnded == true)
			{
                    UpdateTimelineEventList();
        			EventActionManager.Instance.TryPlayEvent(gameObject, "OnVideoEnd");
	        }

            previousTime = currentTime;

            yield return new WaitForSeconds(timelineCheckInterval);

			//so that timeline events are played again
			if(videoEnded = true)
			{
			videoEnded = false;
			}

            }
        }          




		


	public void PlayVideo(GameObject poster)
	{
		   //cancel if urls disabled
		  if(GlobalUtilityFunctions.IsURL(poster.GetComponent<PosterMeshCreator>().urlFilePath))
        {
            if(ConfigMenuUIEvents.Instance.allowURLmedia == false)
            {
				UINotificationHandler.Instance.SpawnNotification("<color=red>Url Media disabled!");
				return;
            }

        }
		
        if (string.IsNullOrWhiteSpace(poster.GetComponent<PosterMeshCreator>().urlFilePath) || poster.GetComponent<PosterMeshCreator>().urlFilePath.Contains(".."))
        {
            return;
        }

		if(!GlobalUtilityFunctions.IsPathSafe(poster.GetComponent<PosterMeshCreator>().urlFilePath))
		{
			return;
		}



                if(co != null)
                {
                StopCoroutine(co);
                }

					if(GetComponent<PosterMeshCreator>().meshRenderer)
               //     GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial = GetComponent<PosterMeshCreator>().videoStaticMat;
                                

                //asign the mesh
                screen = poster.GetComponent<Renderer>();

				//open

				//check if the file exists locally (for workshop)
				if(File.Exists(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + poster.GetComponent<PosterMeshCreator>().urlFilePath ))
				{
						Open(System.IO.Path.GetDirectoryName(GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp) + "/" + poster.GetComponent<PosterMeshCreator>().urlFilePath);
				}
				else if(poster.GetComponent<PosterMeshCreator>().urlFilePath.StartsWith("SA:"))
				{
						Open(Application.dataPath + "/StreamingAssets/" + poster.GetComponent<PosterMeshCreator>().urlFilePath.Substring(3));
				}
				else
				{
						Open(poster.GetComponent<PosterMeshCreator>().urlFilePath);
				}





                poster.GetComponent<PosterMeshCreator>().AssignVideoMaterial(texture);

				//update poster dimensions info


	}
	
	public void StopVideo()
	{
                mediaPlayer.Stop();

                if(GetComponent<PosterFrameList>().posterFrameList.Count >= 1)
                {
            		GetComponent<PosterFrameList>().posterFrameList[0].GetComponent<PosterMeshCreator_BorderFrame>().UpdateFrame();
                }

				if(GetComponent<PosterMeshCreator>().meshRenderer)
           //     GetComponent<PosterMeshCreator>().meshRenderer.sharedMaterial = GetComponent<PosterMeshCreator>().videoStaticMat;
              	GetComponent<PosterMeshCreator>().TriggerVideoPauseEvent();


	}


  public void StopVideoPlayerIfPosterWhereVideoIsPlayingWasChanged(PosterMeshCreator poster)
    {
                mediaPlayer.Stop();    
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

		Debug.Log("PLAY1: " + path);

		var trimmedPath = path.Trim(new char[]{'"'});//Windows likes to copy paths with quotes but Uri does not like to open them
			Debug.Log("TRIMPATH: " + trimmedPath);

		mediaPlayer.Media = new Media(new Uri(trimmedPath));
		Play();
	}

	public void Play()
	{
		Log("VLCPlayerExample Play");

		mediaPlayer.Play();

		volume_original = 100;
		mediaPlayer.SetVolume((int)((float)volume_original * ConfigMenuUIEvents.mapVolumeModifier)); //always set it to max volume by default


		//TIME LINE EVENT MECHANIC
		videoEnded = false;
        UpdateTimelineEventList();
        StartTimelineCheckingCoroutine();
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
			_vlcTexture.wrapMode = TextureWrapMode.Repeat;

            texture = new RenderTexture(_vlcTexture.width, _vlcTexture.height, 0, RenderTextureFormat.ARGB32); //Make a renderTexture the same size as vlctex
			texture.wrapMode = TextureWrapMode.Repeat;

			GetComponent<PosterMeshCreator>().UpdateTextureFiltering();

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