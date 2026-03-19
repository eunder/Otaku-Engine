using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioSourceEventComponent : MonoBehaviour
{

    public IEnumerator PlayAudio(float fadeInTime = 0f)
    {
        yield return new WaitForSeconds(0);
        

        //fade out if parameter is greater than 0
        if(fadeInTime >= 0.001f) //check it like this... due to floating point error-prone stuff
        {
            DOTween.Kill(GetComponent<AudioSource>());
            GetComponent<AudioSource>().volume = 0f;
        }

        GetComponent<AudioSource>().Play();


        //fade out if parameter is greater than 0
        if(fadeInTime >= 0.001f) //check it like this... due to floating point error-prone stuff
        {
        GetComponent<AudioSource>().DOFade(1f, fadeInTime);
        }
        else
        {
        GetComponent<AudioSource>().volume = 1f;
        }
        
        GetComponent<AudioSourceObject>().OnAudioPlay();
        
    }

    public IEnumerator PlayAudio_OneShot()
    {
        yield return new WaitForSeconds(0);
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, 1.0f);
    }


    public IEnumerator PauseAudio()
    {
        yield return new WaitForSeconds(0);
        GetComponent<AudioSource>().Pause();

        GetComponent<AudioSourceObject>().intentionallyPaused = true;
    }
    public IEnumerator ResumeAudio()
    {
        yield return new WaitForSeconds(0);
        GetComponent<AudioSource>().UnPause();
    }

    public IEnumerator StopAudio(float fadeOutTime = 0f)
    {
        yield return new WaitForSeconds(0);


        //fade out if parameter is greater than 0
        if(fadeOutTime >= 0)
        {
            DOTween.Kill(GetComponent<AudioSource>());

            GetComponent<AudioSource>().DOFade(0f, fadeOutTime).OnComplete(() =>
            {
            GetComponent<AudioSource>().Stop();
            });
        }
        else
        {
            GetComponent<AudioSource>().Stop();
        }


        GetComponent<AudioSourceObject>().OnAudioStop();
    }

    public IEnumerator SetPitch(float value = 0f)
    {
        yield return new WaitForSeconds(0);

        float val = value;
        val = Mathf.Clamp(val, 0.5f, 2.0f);
        GetComponent<AudioSource>().pitch = val;
    }

    public IEnumerator SetPitch_RandomRange(float minRange, float maxRange)
    {
        yield return new WaitForSeconds(0);

        float val = Random.Range(minRange, maxRange);
        val = Mathf.Clamp(val, 0.5f, 2.0f);
        GetComponent<AudioSource>().pitch = val;
    }
}
