using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class WheelPickerElementGrowOnHover : MonoBehaviour
{
    public AnimationCurve growCurve;
    public bool closetObj = false;
    public float time;

    public float minimumPitch = 0.5f;
    public float maximumPitch = 1.2f;

    //sound effects for closest object
    public AudioSource audioSource_closestObj;
    public AudioClip audioClip_closestObj;
    private bool playedClosestObjSound = false;
    // Update is called once per frame
    void Update()
    {

        if(closetObj == true)
        {
           time += Time.deltaTime;

        //makes sure to only play the sound once
         if(playedClosestObjSound == false)
         {
            audioSource_closestObj.pitch = Random.Range(minimumPitch, maximumPitch);
            audioSource_closestObj.PlayOneShot(audioClip_closestObj, 1.0f);
            playedClosestObjSound = true;
         }

        }
        else
        {
            time -= Time.deltaTime * 0.1f; //0.1f makes it go down slower
            playedClosestObjSound = false;
        }

        time = Mathf.Clamp(time, 0.0f, growCurve[growCurve.length - 1].time);

            gameObject.GetComponent<RectTransform>().localScale = new Vector3(growCurve.Evaluate(time), growCurve.Evaluate(time) , 0f);


    }
}
