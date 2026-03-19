using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerRayHover : MonoBehaviour
{

    public bool onHover = false;

    public Vector3 startScale = new Vector3(1,1,1);
    public Vector3 endScale = new Vector3(2,2,2);

    void Start()
    {
        startScale = transform.localScale;
        endScale = startScale * 1.35f;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if(onHover)
        {
            transform.localScale = Vector3.Lerp (transform.localScale, endScale, Time.deltaTime * 15);
        }
        else
        {
            transform.localScale = Vector3.Lerp (transform.localScale, startScale, Time.deltaTime * 15);
        }
        onHover = false;
    }
}
