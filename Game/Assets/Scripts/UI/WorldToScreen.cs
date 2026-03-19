using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WorldToScreen : MonoBehaviour
{
     public Transform target;
    public Camera cam;

    void Start()
    {        
        Vector3 screenPos = cam.WorldToScreenPoint(target.position);
        transform.position = screenPos;
    }

    void Update()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(target.position);
        if(screenPos.z > 0)
        {
        transform.position = screenPos;
        }
         else
        {
            transform.position = new Vector3(0.0f,Screen.height + 100,0.0f); 
        }
    }


    }
