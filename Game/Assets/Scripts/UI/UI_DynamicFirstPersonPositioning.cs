using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DynamicFirstPersonPositioning : MonoBehaviour
{    public Transform AI_Position;
    public Camera cam;
    public  RectTransform UI_Element;
    public GameObject canvas;
    RectTransform CanvasRect;

    public Transform player;

    float y_offset = 0.0f;

    void Start()
    {
         CanvasRect = canvas.GetComponent<RectTransform>();
    }

 public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue){
     
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
     
        return(NewValue);
    }




    void Update()
    {
        
        y_offset = scale(2.0f, 60.0f, 0.0f, 50.0f, Vector3.Distance( player.position, AI_Position.position));

        Vector2 ViewportPosition = cam.WorldToViewportPoint(AI_Position.transform.position);
        Vector2 WorldObject_ScreenPosition=new Vector2(
        ((ViewportPosition.x*CanvasRect.sizeDelta.x)-(CanvasRect.sizeDelta.x*0.5f)),
        ((ViewportPosition.y*CanvasRect.sizeDelta.y)-(CanvasRect.sizeDelta.y*0.5f)) + 0.0f);
 
            //now you can set the position of the ui element
            UI_Element.anchoredPosition=WorldObject_ScreenPosition;


        float f_scale = scale(2.0f, 40.0f, 0.6f, 0.2f, Vector3.Distance( player.position, AI_Position.position));
        UI_Element.localScale = new Vector2(f_scale, f_scale);




    }



}
