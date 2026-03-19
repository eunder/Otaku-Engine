using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LibVLCSharp;
using TMPro;


///This script controls all the GUI for the VLC Unity Canvas Example
///It sets up event handlers and updates the GUI every frame
///This example shows how to safely set up LibVLC events and a simple way to call Unity functions from them
public class VLC_MediaPlayer_ControlsGUI : MonoBehaviour
{


	public VLC_MediaPlayer vlcPlayer;
	
	//GUI Elements
	public Slider seekBar;
	public Button playButton;
	public Button pauseButton;
	public Button stopButton;
	public Button tracksButton;
	public GameObject tracksButtonsGroup; //Group containing buttons to switch video, audio, and subtitle tracks
	public Slider volumeBar;
	public GameObject trackButtonPrefab;
	public GameObject trackLabelPrefab;
	public Color unselectedButtonColor; //Used for unselected track text
	public Color selectedButtonColor; //Used for selected track text



    public TextMeshProUGUI title_Text;
    public TextMeshProUGUI time_Text;
    public TextMeshProUGUI timeSecond_Text;






	//Configurable Options
	public int maxVolume = 100; //The highest volume the slider can reach. 100 is usually good but you can go higher.

	//State variables
	bool _isPlaying = false; //We use VLC events to track whether we are playing, rather than relying on IsPlaying 
	bool _isDraggingSeekBar = false; //We advance the seek bar every frame, unless the user is dragging it

	///Unity wants to do everything on the main thread, but VLC events use their own thread.
	///These variables can be set to true in a VLC event handler indicate that a function should be called next Update.
	///This is not actually thread safe and should be gone soon!
	bool _shouldUpdateTracks = false; //Set this to true and the Tracks menu will regenerate next frame
	bool _shouldClearTracks = false; //Set this to true and the Tracks menu will clear next frame

	List<Button> _videoTracksButtons = new List<Button>();
	List<Button> _audioTracksButtons = new List<Button>();
	List<Button> _textTracksButtons = new List<Button>();


	public void InitializeControlls()
	{
		//VLC Event Handlers
		vlcPlayer.mediaPlayer.Playing += (object sender, EventArgs e) => {
			//Always use Try/Catch for VLC Events
			try
			{
				//Because many Unity functions can only be used on the main thread, they will fail in VLC event handlers
				//A simple way around this is to set flag variables which cause functions to be called on the next Update
				_isPlaying = true;//Switch to the Pause button next update
				_shouldUpdateTracks = true;//Regenerate tracks next update


			}
			catch (Exception ex)
			{
				Debug.LogError("Exception caught in mediaPlayer.Play: \n" + ex.ToString());
			}
		};

		vlcPlayer.mediaPlayer.Paused += (object sender, EventArgs e) => {
			//Always use Try/Catch for VLC Events
			try
			{
				_isPlaying = false;//Switch to the Play button next update
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception caught in mediaPlayer.Paused: \n" + ex.ToString());
			}
		};

		vlcPlayer.mediaPlayer.Stopped += (object sender, EventArgs e) => {
			//Always use Try/Catch for VLC Events
			try
			{
				_isPlaying = false;//Switch to the Play button next update
				_shouldClearTracks = true;//Clear tracks next update
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception caught in mediaPlayer.Stopped: \n" + ex.ToString());
			}
		};

		//Buttons
		pauseButton.onClick.AddListener(() => { vlcPlayer.Pause(); });
		playButton.onClick.AddListener(() => 
		{
			 vlcPlayer.Pause(); 
		
			if(vlcPlayer.IsPlaying == false)
			{
				vlcPlayer.PlayVideo(vlcPlayer.transform.gameObject);
			}
		});
		//stopButton.onClick.AddListener(() => { vlcPlayer.Stop(); });
		tracksButton.onClick.AddListener(() => { ToggleElement(tracksButtonsGroup); SetupTrackButtons(); });

		UpdatePlayPauseButton(vlcPlayer.playOnAwake);

		//Seek Bar Events
		var seekBarEvents = seekBar.GetComponent<EventTrigger>();

		EventTrigger.Entry seekBarPointerDown = new EventTrigger.Entry();
		seekBarPointerDown.eventID = EventTriggerType.PointerDown;
		seekBarPointerDown.callback.AddListener((data) => { _isDraggingSeekBar = true; });
		seekBarEvents.triggers.Add(seekBarPointerDown);

		EventTrigger.Entry seekBarPointerUp = new EventTrigger.Entry();
		seekBarPointerUp.eventID = EventTriggerType.PointerUp;
		seekBarPointerUp.callback.AddListener((data) => { 
			_isDraggingSeekBar = false;
			vlcPlayer.SetTime((long)((double)vlcPlayer.Duration * seekBar.value));
		});
		seekBarEvents.triggers.Add(seekBarPointerUp);

		//Track Selection Buttons
		tracksButtonsGroup.SetActive(false);

		//Volume Bar
		volumeBar.wholeNumbers = true;
		volumeBar.maxValue = maxVolume; //You can go higher than 100 but you risk audio clipping
		volumeBar.value = vlcPlayer.Volume;
		volumeBar.onValueChanged.AddListener((data) => { vlcPlayer.SetVolume((int)volumeBar.value);	});







		//set title
		title_Text.text = GlobalUtilityFunctions.GetPathLastName(vlcPlayer.transform.GetComponent<PosterMeshCreator>().urlFilePath, false);



	}

	void Update()
	{
		if(vlcPlayer)
		{
		//Update screen aspect ratio. Doing this every frame is probably more than is necessary.
		UpdatePlayPauseButton(_isPlaying);

		UpdateSeekBar();

		UpdateTime();



		if (_shouldUpdateTracks)
		{
			SetupTrackButtons();
			_shouldUpdateTracks = false;
		}

		if (_shouldClearTracks)
		{
			ClearTrackButtons();
			_shouldClearTracks = false;
		}
		}
	}


	void UpdateTime()
	{
		// Get the time from VLC player in milliseconds
		long timeMilliseconds = vlcPlayer.Time;

		// Convert milliseconds to seconds
		double timeSeconds = timeMilliseconds / 1000.0; // Convert milliseconds to seconds
		
		int timeSecondsint = (int)timeMilliseconds / 1000; // Convert milliseconds to seconds
		timeSecond_Text.text = "sec. " + timeSecondsint.ToString();

		// Calculate hours, minutes, and seconds
		int hours = (int)(timeSeconds / 3600);
		int minutes = (int)((timeSeconds % 3600) / 60);
		int seconds = (int)(timeSeconds % 60);


		// Format the time string
		time_Text.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
	}



	//Show the Pause button if we are playing, or the Play button if we are paused or stopped
	void UpdatePlayPauseButton(bool playing)
	{
		pauseButton.gameObject.SetActive(playing);
		playButton.gameObject.SetActive(!playing);
	}

	//Update the position of the Seek slider to the match the VLC Player
	void UpdateSeekBar()
	{
		if(!_isDraggingSeekBar)
		{
			var duration = vlcPlayer.Duration;
			if (duration > 0)
				seekBar.value = (float)((double)vlcPlayer.Time / duration);
		}
	}

	//Enable a GameObject if it is disabled, or disable it if it is enabled
	bool ToggleElement(GameObject element)
	{
		bool toggled = !element.activeInHierarchy;
		element.SetActive(toggled);
		return toggled;
	}

	//Create Audio, Video, and Subtitles button groups
	void SetupTrackButtons()
	{
		Debug.Log("SetupTrackButtons");
		ClearTrackButtons();
		SetupTrackButtonsGroup(TrackType.Video, "Video Tracks", _videoTracksButtons);
		SetupTrackButtonsGroup(TrackType.Audio, "Audio Tracks", _audioTracksButtons);
		SetupTrackButtonsGroup(TrackType.Text, "Subtitle Tracks", _textTracksButtons, true);

	}

	//Clear the track buttons menu
	void ClearTrackButtons()
	{
		var childCount = tracksButtonsGroup.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Destroy(tracksButtonsGroup.transform.GetChild(i).gameObject);
		}
	}

	//Create Audio, Video, or Subtitle button groups
	void SetupTrackButtonsGroup(TrackType type, string label, List<Button> buttonList, bool includeNone = false)
	{
		buttonList.Clear();
		var tracks = vlcPlayer.Tracks(type);
		var selected = vlcPlayer.SelectedTrack(type);

		if (tracks.Count > 0)
		{
			var newLabel = Instantiate(trackLabelPrefab, tracksButtonsGroup.transform);
			newLabel.GetComponentInChildren<Text>().text = label;

			for (int i = 0; i < tracks.Count; i++)
			{
				var track = tracks[i];
				var newButton = Instantiate(trackButtonPrefab, tracksButtonsGroup.transform).GetComponent<Button>();
				var textMeshPro = newButton.GetComponentInChildren<Text>();
				textMeshPro.text = track.Name;
				if (selected != null && track.Id == selected.Id)
					textMeshPro.color = selectedButtonColor;
				else
					textMeshPro.color = unselectedButtonColor;

				buttonList.Add(newButton);
				newButton.onClick.AddListener(() => {
					foreach (var button in buttonList)
						button.GetComponentInChildren<Text>().color = unselectedButtonColor;
					textMeshPro.color = selectedButtonColor;
					vlcPlayer.Select(track);
				});
			}
			if (includeNone)
			{
				var newButton = Instantiate(trackButtonPrefab, tracksButtonsGroup.transform).GetComponent<Button>();
				var textMeshPro = newButton.GetComponentInChildren<Text>();
				textMeshPro.text = "None";
				if (selected == null)
					textMeshPro.color = selectedButtonColor;
				else
					textMeshPro.color = unselectedButtonColor;

				buttonList.Add(newButton); 
				newButton.onClick.AddListener(() => {
					foreach (var button in buttonList)
						button.GetComponentInChildren<Text>().color = unselectedButtonColor;
					textMeshPro.color = selectedButtonColor;
					vlcPlayer.Unselect(type);
				});
			}

		}

	}
}