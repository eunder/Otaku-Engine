using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class AudioSourceObject : MonoBehaviour
{
    public AudioSource audioSource;
    public string audioPath;
    public float pitch = 1f;
    public float spatialBlend = 0;
    public float minDistance = 0;
    public float maxDistance = 10;
    public bool repeat = false;



    public void SetValuesOnAudioSource()
    {
                audioSource.pitch = pitch;
                audioSource.spatialBlend = spatialBlend;
                audioSource.minDistance = minDistance;
                audioSource.maxDistance = maxDistance;
                audioSource.loop = repeat;
    }



 public IEnumerator LoadAudio(string path)
    {
        audioPath = path;

        yield return AudioDownloaderManager.Instance.LoadAudioClip(path, (audioClip) =>
        {
            audioSource.clip = audioClip;
        });

    }

   

    //all of this just to detect audio source end... 
    //start coroutine to wait for audio source to end (so that you dont have to use an Update loop)

    private Coroutine waitToEndCoroutine;

    public void OnAudioPlay()
    {

        if(waitToEndCoroutine != null)
        {
        StopCoroutine(waitToEndCoroutine);
        waitToEndCoroutine = null;
        }

        StartCoroutine(CheckIsPlaying());

        UpdateTimelineEventList();
        StartTimelineCheckingCoroutine();
    }


    //for making sure events dont happen when the audio source is stopped...
    public void OnAudioStop()
    {
        StopAllCoroutines();
    }



    //used to prevent the end event from triggering because the user paused the audio
    public bool intentionallyPaused = false;

    //specific coroutine usage for audio end event
    IEnumerator CheckIsPlaying()
    {
        while (audioSource.isPlaying || intentionallyPaused)
        {
            intentionallyPaused = false;
            yield return null;
        }
        EventActionManager.Instance.TryPlayEvent(gameObject, "OnAudioSourceEnd");
    }



//TIMELINE EVENTS MECHANIC

    //isPlaying AND time == 0... THEN it has either not started or it ended

    public float previousTime = 0.0f;
    private Coroutine CheckForTimelineEventCoroutine;
    public float timelineCheckInterval = 1.0f;


    public List<SaveAndLoadLevel.Event> timelineEvents = new List<SaveAndLoadLevel.Event>();




    public void UpdateTimelineEventList()
    {
        timelineEvents.Clear();

        foreach(SaveAndLoadLevel.Event e in GetComponent<EventHolderList>().events)
        {
            if(e.onAction == "OnAudioSourceReachTime")
            timelineEvents.Add(e);
        }
    }

    public void StartTimelineCheckingCoroutine()
    {
        if(CheckForTimelineEventCoroutine != null)
        StopCoroutine(CheckForTimelineEventCoroutine);

        CheckForTimelineEventCoroutine = StartCoroutine(CheckTimelineTriggerEvents());
    }


        //This coroutine checks the timeline events by looping through all the timeline events at an interval
        IEnumerator CheckTimelineTriggerEvents()
        {
            while (true) // Infinite loop
            {
                //for triggering timeline events
                List<SaveAndLoadLevel.Event> eventsToRemove = new List<SaveAndLoadLevel.Event>();

                foreach(SaveAndLoadLevel.Event timelineEvent in timelineEvents)
                {
                    float currentTime = audioSource.time;
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




            //audio restarted
            if( audioSource.isPlaying && audioSource.time < previousTime)
            {
                    UpdateTimelineEventList();
                    Debug.Log("Audio Restartded...");
            }

            //audio ended (not restarted)
            if(audioSource.isPlaying == false && audioSource.time < previousTime)
            {
                Debug.Log("Audio Ended...");
                StopCoroutine(CheckForTimelineEventCoroutine);
            }


            previousTime = audioSource.time;

            Debug.Log("Checked timeline for event!");
            yield return new WaitForSeconds(timelineCheckInterval);
            }
        }          
    }
