using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2( 1.0f, 0.0f );
    public string textureName = "_MainTex";

     Vector2 uvOffset = Vector2.zero;
     void LateUpdate() 
     {
         uvOffset += ( uvAnimationRate * Time.deltaTime );
         if( gameObject.GetComponent<Renderer>() )
         {
             gameObject.GetComponent<Renderer>().materials[ materialIndex ].SetTextureOffset( textureName, uvOffset );
         }
     }


}
