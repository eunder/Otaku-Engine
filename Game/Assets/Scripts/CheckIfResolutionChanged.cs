using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckIfResolutionChanged : MonoBehaviour
{
public Camera mainCam;
public RawImage pixelOverlayImage;

public GameObject testQuad;
 private Vector2 resolution;
 
 private void Awake()
 {
     resolution = new Vector2(Screen.width, Screen.height);


         mainCam.targetTexture.Release( );
         RenderTexture renderTex = new RenderTexture( Screen.width/11, Screen.height/11, 24 ); 
         renderTex.filterMode = FilterMode.Point;
         mainCam.targetTexture = renderTex;
         pixelOverlayImage.texture = renderTex;

         testQuad.GetComponent<Renderer>().material.SetTexture("_MainTex", renderTex );
 }
 
 private void Update ()
 {
     if (resolution.x != Screen.width || resolution.y != Screen.height)
     {
         
         mainCam.targetTexture.Release( );
         RenderTexture renderTex = new RenderTexture( Screen.width/11, Screen.height/11, 24 ); 
         renderTex.filterMode = FilterMode.Point;
         mainCam.targetTexture = renderTex;
         pixelOverlayImage.texture = renderTex;

 
         resolution.x = Screen.width;
         resolution.y = Screen.height;
     }
 }

}
