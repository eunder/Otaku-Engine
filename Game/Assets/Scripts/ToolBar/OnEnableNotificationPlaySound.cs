using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableNotificationPlaySound : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<AudioSource>().Play();
    }

}
