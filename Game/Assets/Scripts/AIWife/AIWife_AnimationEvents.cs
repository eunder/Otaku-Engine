using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWife_AnimationEvents : MonoBehaviour
{
    public AudioSource blink_AudioSource;

    int blinkInd = 0;
    public void BlinkAnimEvent()
    {
        if(blinkInd == 0)
        {
            blink_AudioSource.pitch = 1.0f;
            blinkInd = 1;
        }
        else
        {
            blink_AudioSource.pitch = 0.75f;
            blinkInd = 0;
        }
        blink_AudioSource.Play();
    }


}
