using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineScreenShaker : MonoBehaviour
{
    public ScreenShake2 screenShaker;
    public float shakeToadd = 0.5f;
    // Start is called before the first frame update
    void OnEnable()
    {
        screenShaker.Shake(shakeToadd);
    }
}
