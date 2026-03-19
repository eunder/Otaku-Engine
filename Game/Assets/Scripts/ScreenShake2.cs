using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake2 : MonoBehaviour
{
    private static ScreenShake2 _instance;
    public static ScreenShake2 Instance { get { return _instance; } }

        private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public AnimationCurve curve;
    public float duration = 1f;
    public float shakeCurrentTimer = 0.0f;
    public float minimum = 0.0f;
Vector3 startPosition;
public Vector3 posOffset;
    void Start()
    {
        startPosition = transform.localPosition;

    }


    public void Shake(float shakeToAdd)
    {
        shakeCurrentTimer += shakeToAdd;
    }


    // Update is called once per frame
    void Update()
    {
            shakeCurrentTimer -= Time.deltaTime;
            float strength = curve.Evaluate(shakeCurrentTimer/duration);
            transform.localPosition = startPosition + Random.insideUnitSphere * strength;
            
            posOffset = transform.localPosition - startPosition;

            if(shakeCurrentTimer < minimum)
            {
                shakeCurrentTimer = minimum;
            }


    }
}
