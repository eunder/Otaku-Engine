using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeOnEnableObject : MonoBehaviour
{
    public ScreenShake2 screenshaker;
    public float shakeToAdd = 0.1f;
    void OnEnable()
    {
        screenshaker.Shake(shakeToAdd);
    }

}
